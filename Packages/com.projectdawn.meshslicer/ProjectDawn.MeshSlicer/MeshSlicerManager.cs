using ProjectDawn.Collections;
using ProjectDawn.Geometry3D;
using ProjectDawn.MeshSlicer.LowLevel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Plane = ProjectDawn.Geometry3D.Plane;

namespace ProjectDawn.MeshSlicer
{
    public struct SliceWithPlaneData
    {
        /// <summary>
        /// The original mesh slicer that is being sliced.
        /// </summary>
        public MeshSlicer Slicer;

        /// <summary>
        /// The side of the piece.
        /// </summary>
        public SlicerSide Side;

        /// <summary>
        /// Plane used for slicing.
        /// </summary>
        public Plane Plane;
    }

    public delegate void OnSliceWithPlane(SliceWithPlaneData data);

    /// <summary>
    /// Manages mesh slicers.
    /// </summary>
    [AddComponentMenu("Slicer/Mesh Slicer Manager")]
    public class MeshSlicerManager : MonoBehaviour
    {
        static class Markers
        {
            public static readonly ProfilerMarker Slice = new ProfilerMarker("Slice");
            public static readonly ProfilerMarker Setup = new ProfilerMarker("Setup");
            public static readonly ProfilerMarker Schedule = new ProfilerMarker("Schedule");
            public static readonly ProfilerMarker GetMeshes = new ProfilerMarker("GetMeshes");
        }

        /// <summary>
        /// Maximum number frames job can be executed.
        /// </summary>
        [Tooltip("Maximum number frames job can be executed.")]
        [Range(0, 4)]
        public long MaxJobAge = 4;

        List<Node> m_Nodes;
        List<Node> m_ActiveHeads;
        Stack<Node> m_FreeNodes;

        List<MeshSurface> m_CachedMeshSurfaces;

        List<IScheduleSliceJob> m_CachedScheduleSliceJobs;
        ICreatePiece m_CachedCreatePiece;
        IGetMeshSurfaces m_CachedGetMeshSurfaces;
        int m_CreatePieceCounter;

        long m_FrameCount;

        /// <summary>
        /// Returns existing MeshSlicerManager or creates new one.
        /// </summary>
        public static MeshSlicerManager GetOrCreateManager()
        {
            var manager = FindObjectOfType<MeshSlicerManager>();
            if (manager == null)
            {
                var gameObject = new GameObject("MeshSlicerManager");
                manager = gameObject.AddComponent<MeshSlicerManager>();
            }
            return manager;
        }

        /// <summary>
        /// Schedule job to slice mesh using plane.
        /// </summary>
        /// <param name="slicer">Mesh slicer that will be sliced.</param>
        /// <param name="plane">The plane will be used for slicing.</param>
        /// <param name="callback"></param>
        /// <returns>Returns handle to slice job.</returns>
        public MeshSliceHandle Slice(MeshSlicer slicer, Plane plane, OnSliceWithPlane callback)
        {
            using (Markers.Slice.Auto())
            {
                if (!IsValid(slicer.Handle))
                {
                    // Starting new mesh slice job tree

                    m_CachedMeshSurfaces.Clear();
                    var surfaces = m_CachedMeshSurfaces;
                    var handles = new NativeList<JobHandle>(1, Allocator.Temp);

                    Markers.GetMeshes.Begin();
                    Mesh.MeshDataArray meshDataArray = m_CachedGetMeshSurfaces.GetMeshSurfaces(slicer, surfaces, handles);
                    Markers.GetMeshes.End();

                    CheckSurfacesAndHandles(surfaces, handles);

                    Node node = Slice(slicer, surfaces, plane, handles);

                    handles.Dispose();

                    node.Slicer = slicer;
                    node.ReadMeshDataAllocated = true;
                    node.ReadMeshData = meshDataArray;
                    node.Callback += callback;
                    node.SliceCount = 1;

                    slicer.Handle = node.Handle;

                    m_ActiveHeads.Add(node);
                    return slicer.Handle;
                }
                else
                {
                    Node node = m_Nodes[slicer.Handle];
                    if (slicer.Additive.MaxSliceCount != node.SliceCount)
                    {
                        AdditiveSlice(slicer, node, plane, callback);
                        node.Callback += callback;
                        node.SliceCount++;
                    }
                    return slicer.Handle;
                }
            }
        }

        /// <summary>
        /// Returns true with mesh slice job finished.
        /// </summary>
        public bool IsCompleted(MeshSliceHandle handle)
        {
            return !IsValid(handle);
        }

        /// <summary>
        /// Waits until slice job is finished.
        /// </summary>
        public void Complete(MeshSliceHandle handle)
        {
            // Get tree node
            if (!IsValid(handle))
                return;
            var node = m_Nodes[handle];

            // Complete all nodes job
            CompleteRecursive(node);

            // Execute all node tree
            ExecuteNodeRecursive(node, node.Slicer);

            // Remove from active nodes
            // TODO: This is not very efficient, but it depends on project
            int index = m_ActiveHeads.FindIndex((n) => node == n);
            Assert.AreNotEqual(-1, index);
            m_ActiveHeads.RemoveAtSwapBack(index);
        }

        void CompleteRecursive(Node node)
        {
            node.JobHandle.Complete();
            if (node.NodeOutside != null)
                CompleteRecursive(node.NodeOutside);
            if (node.NodeInside != null)
                CompleteRecursive(node.NodeInside);
        }

        Node Slice(MeshSlicer slicer, List<MeshSurface> surfaces, Plane plane, NativeList<JobHandle> dependency)
        {
            using (Markers.Schedule.Auto())
            {
                Node node = Allocate();

                // Start counting node age
                node.FrameStart = m_FrameCount;

                Mesh.MeshDataArray meshDataArrayOutside = Mesh.AllocateWritableMeshData(surfaces.Count);
                Mesh.MeshDataArray meshDataArrayInside = Mesh.AllocateWritableMeshData(surfaces.Count);

                for (int i = 0; i < surfaces.Count; ++i)
                {
                    MeshSurface surface = surfaces[i];
                    MeshSurface surfaceOutside = new MeshSurface(1, surface.VertexData.GetVertexAttributes(), Allocator.TempJob);
                    MeshSurface surfaceInside = new MeshSurface(1, surface.VertexData.GetVertexAttributes(), Allocator.TempJob);

                    // Conver plane from world space to local space
                    float4x4 worldToLocal = slicer.transform.worldToLocalMatrix;
                    float3 normalOS = math.mul(worldToLocal, new float4(plane.Normal, 0)).xyz;
                    float3 pointOS = math.mul(worldToLocal, new float4(plane.Normal * -plane.Distance, 1)).xyz;
                    Plane planeOS = new Plane(normalOS, pointOS);

                    // Start slice job
                    JobHandle dependencyOutside = Slicer.SliceSurfaceWithPlane(surface, planeOS, surfaceOutside, slicer.Division, slicer.Fill, SlicerSide.Outside, dependency[i]);
                    JobHandle dependecyInside = Slicer.SliceSurfaceWithPlane(surface, planeOS, surfaceInside, slicer.Division, slicer.Fill, SlicerSide.Inside, dependency[i]);

                    // Recalculate bounds
                    dependencyOutside = surfaceOutside.RecalculateBounds(dependencyOutside);
                    dependecyInside = surfaceInside.RecalculateBounds(dependecyInside);

                    // Callback
                    foreach (IScheduleSliceJob sliceJob in m_CachedScheduleSliceJobs)
                    {
                        dependencyOutside = sliceJob.OnScheduleSliceJob(slicer, surfaceOutside, planeOS, dependencyOutside);
                        dependecyInside = sliceJob.OnScheduleSliceJob(slicer, surfaceInside, planeOS, dependecyInside);
                    }

                    var updateFlag = MeshUpdateFlags.DontRecalculateBounds;
#if !ENABLE_UNITY_COLLECTIONS_CHECKS
                    updateFlag |= MeshUpdateFlags.DontValidateIndices;
#endif

                    // Start mesh write jobs
                    dependencyOutside = surfaceOutside.Write(meshDataArrayOutside[i], updateFlag, dependencyOutside);
                    dependecyInside = surfaceInside.Write(meshDataArrayInside[i], updateFlag, dependecyInside);

                    dependency[i] = JobHandle.CombineDependencies(dependencyOutside, dependecyInside);

                    node.Surfaces.Add(surface);
                    node.SurfacesOutside.Add(surfaceOutside);
                    node.SurfacesInside.Add(surfaceInside);
                }

                node.Slicer = null;
                node.ReadMeshDataAllocated = false;
                node.WriteMeshDataOutside = meshDataArrayOutside;
                node.WriteMeshDataInside = meshDataArrayInside;
                node.JobHandle = JobHandle.CombineDependencies(dependency);
                node.NodeOutside = null;
                node.NodeInside = null;
                node.Callback = null;
                node.Plane = plane;

                return node;
            }
        }

        void AdditiveSlice(MeshSlicer slicer, Node node, Plane plane, OnSliceWithPlane callback)
        {
            if (node.IsLeaf)
            {
                var handles = new NativeList<JobHandle>(node.Surfaces.Count, Allocator.Temp);
                for (int i = 0; i < node.Surfaces.Count; ++i)
                    handles.Add(node.JobHandle);

                node.NodeOutside = Slice(slicer, node.SurfacesOutside, plane, handles);
                node.NodeInside = Slice(slicer, node.SurfacesInside, plane, handles);
                node.NodeOutside.Callback = node.Callback + callback;
                node.NodeInside.Callback = node.Callback + callback;

                handles.Dispose();
            }
            else
            {
                AdditiveSlice(slicer, node.NodeOutside, plane, callback);
                AdditiveSlice(slicer, node.NodeInside, plane, callback);
                node.NodeOutside.Callback += callback;
                node.NodeInside.Callback += callback;
            }
        }

        bool IsValid(MeshSliceHandle handle)
        {
            CollectionChecks.CheckIndexInRange(handle.m_Index, m_Nodes.Count);
            return m_Nodes[handle.m_Index].Version == handle.m_Version;
        }

        Node Allocate()
        {
            if (m_FreeNodes.Count != 0)
            {
                // Re-use existing nodes
                Node node = m_FreeNodes.Pop();
                node.Surfaces.Clear();
                node.SurfacesOutside.Clear();
                node.SurfacesInside.Clear();
                node.Version++;
                return node;
            }
            else
            {
                // Create new node
                int index = m_Nodes.Count;
                var node = new Node(index, 1);
                m_Nodes.Add(node);
                return node;
            }
        }

        void Deallocate(Node node)
        {
            node.Version++;
            m_FreeNodes.Push(node);
        }

        void ExecuteNodeRecursive(Node node, MeshSlicer slicer)
        {
            // Release node for re use
            Deallocate(node);

            Assert.IsTrue(node.JobHandle.IsCompleted);

            // Process outside surface side
            if (node.NodeOutside == null)
            {
                // Generate
                var newSlicer = m_CachedCreatePiece.CreatePiece(slicer, node.WriteMeshDataOutside, m_CreatePieceCounter);

                if (newSlicer != null)
                {
                    m_CreatePieceCounter++;

                    node.Callback?.Invoke(new SliceWithPlaneData
                    {
                        Slicer = newSlicer,
                        Side = SlicerSide.Outside,
                        Plane = node.Plane,
                    });
                }
                else
                {
                    node.WriteMeshDataOutside.Dispose();
                }

                //UnityEngine.Debug.Log(node.JobHandle.IsCompleted);
                foreach (var surface in node.SurfacesOutside)
                    surface.Dispose(node.JobHandle);
            }
            else if (!node.NodeOutside.JobHandle.IsCompleted)
            {
                // Generate
                var newSlicer = m_CachedCreatePiece.CreatePiece(slicer, node.WriteMeshDataOutside, m_CreatePieceCounter);

                // Transsition ownership to new slicer
                if (newSlicer != null)
                {
                    m_CreatePieceCounter++;

                    node.Callback?.Invoke(new SliceWithPlaneData
                    {
                        Slicer = newSlicer,
                        Side = SlicerSide.Outside,
                        Plane = node.Plane,
                    });

                    node.NodeOutside.Slicer = newSlicer;
                    newSlicer.Handle = node.NodeOutside.Handle;
                    m_ActiveHeads.Add(node.NodeOutside);
                }
                else
                {
                    // TODO: Check if there is point to add some early out logic as it will not generate new pieces
                    node.WriteMeshDataOutside.Dispose();
                    node.NodeOutside.Slicer = slicer;
                    m_ActiveHeads.Add(node.NodeOutside);
                }
            }
            else
            {
                ExecuteNodeRecursive(node.NodeOutside, slicer);
                node.WriteMeshDataOutside.Dispose();
            }

            // Process inside surface side
            if (node.NodeInside == null)
            {
                // Generate
                var newSlicer = m_CachedCreatePiece.CreatePiece(slicer, node.WriteMeshDataInside, m_CreatePieceCounter);

                if (newSlicer != null)
                {
                    m_CreatePieceCounter++;

                    node.Callback?.Invoke(new SliceWithPlaneData
                    {
                        Slicer = newSlicer,
                        Side = SlicerSide.Inside,
                        Plane = node.Plane,
                    });
                }
                else
                {
                    node.WriteMeshDataInside.Dispose();
                }

                foreach (var surface in node.SurfacesInside)
                    surface.Dispose(node.JobHandle);
            }
            else if (!node.NodeInside.JobHandle.IsCompleted)
            {
                // Generate
                var newSlicer = m_CachedCreatePiece.CreatePiece(slicer, node.WriteMeshDataInside, m_CreatePieceCounter);

                // Transsition ownership to new slicer
                if (newSlicer != null)
                {
                    m_CreatePieceCounter++;

                    node.Callback?.Invoke(new SliceWithPlaneData
                    {
                        Slicer = newSlicer,
                        Side = SlicerSide.Inside,
                        Plane = node.Plane,
                    });

                    node.NodeInside.Slicer = newSlicer;
                    newSlicer.Handle = node.NodeInside.Handle;
                    m_ActiveHeads.Add(node.NodeInside);
                }
                else
                {
                    // TODO: Check if there is point to add some early out logic as it will not generate new pieces
                    node.WriteMeshDataInside.Dispose();
                    node.NodeInside.Slicer = slicer;
                    m_ActiveHeads.Add(node.NodeInside);
                }
            }
            else
            {
                ExecuteNodeRecursive(node.NodeInside, slicer);
                node.WriteMeshDataInside.Dispose();
            }

            // Dispose surfaces
            foreach (var surface in node.Surfaces)
                surface.Dispose(node.JobHandle);
        }

        bool IsCompletedRecursive(Node node)
        {
            bool completed = node.JobHandle.IsCompleted;
            if (node.NodeOutside != null)
                completed &= IsCompletedRecursive(node.NodeOutside);
            if (node.NodeInside != null)
                completed &= IsCompletedRecursive(node.NodeInside);
            return completed;
        }

        void DestroyNodeRecursive(Node node)
        {
            // Wait for job so we could safely destroy it
            node.JobHandle.Complete();

            // Start recursive destroy for outside node
            if (node.NodeOutside != null)
            {
                DestroyNodeRecursive(node.NodeOutside);
            }
            else
            {
                foreach (var surface in node.SurfacesOutside)
                    surface.Dispose(node.JobHandle);
            }
            node.WriteMeshDataOutside.Dispose();

            // Start recursive destroy for inside node
            if (node.NodeInside != null)
            {
                DestroyNodeRecursive(node.NodeInside);
            }
            else
            {
                foreach (var surface in node.SurfacesInside)
                    surface.Dispose(node.JobHandle);
            }
            node.WriteMeshDataInside.Dispose();

            foreach (var surface in node.Surfaces)
                surface.Dispose(node.JobHandle);
        }

        void Awake()
        {
            m_Nodes = new List<Node>();
            m_ActiveHeads = new List<Node>();
            m_FreeNodes = new Stack<Node>();

            m_CachedMeshSurfaces = new List<MeshSurface>();
            m_CachedScheduleSliceJobs = new List<IScheduleSliceJob>();

            // Allocate first node this way empty handles will work
            var node = new Node(0, 1);
            m_Nodes.Add(node);
            m_FreeNodes.Push(node);

            UpdateCache();
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_ActiveHeads.Count; ++i)
            {
                Node node = m_ActiveHeads[i];
                DestroyNodeRecursive(node);

                if (node.ReadMeshDataAllocated)
                    node.ReadMeshData.Dispose();
            }
            m_CachedMeshSurfaces.Clear();
        }

        void Update()
        {
            m_FrameCount++;

            UpdateCache();

            // Executes active nodes
            for (int i = 0; i < m_ActiveHeads.Count; ++i)
            {
                Node node = m_ActiveHeads[i];

                // Skip if job is not finished
                if (!node.JobHandle.IsCompleted)
                {
                    // Age is the number of frames job is running
                    long age = m_FrameCount - node.FrameStart;

                    // If job exceeds the maximum age, wait for it to finish on main thread
                    // This is required in case MeshSurface is allocated with temp memory that cannot persist across many frames
                    if (age >= MaxJobAge)
                    {
                        // Complete whole tree, because its inside/outside MeshSurface will also be expired
                        CompleteRecursive(node);
                    }
                    else
                        continue;
                }

                // Skip if whole tree of nodes have not finished, if partial result is not allowed
                // TODO: Investigate if we should cached additive settings in node and use it there instead of referencing the slicer
                if (!node.Slicer.Additive.PartialResults && !IsCompletedRecursive(node))
                    continue;

                // Remove active node
                m_ActiveHeads.RemoveAtSwapBack(i);
                i--;

                m_CreatePieceCounter = 0; // TODO: We could pass this into ExecuteNodeRecursive, need to investigate performance
                ExecuteNodeRecursive(node, node.Slicer);

                // Usually the first slice job will read mesh data so here we dipose it
                if (node.ReadMeshDataAllocated)
                    node.ReadMeshData.Dispose();
            }  
        }

        void UpdateCache()
        {
            // Cache components for later usage
            GetComponents(m_CachedScheduleSliceJobs);

            // Find component that implements create piece
            if (TryGetComponent(out ICreatePiece createPiece))
            {
                m_CachedCreatePiece = createPiece;
            }
            else
            {
                // If not create the default one
                m_CachedCreatePiece = gameObject.AddComponent<DefaultCreatePiece>();
            }

            // Find component that implements get mesh surfaces
            if (TryGetComponent(out IGetMeshSurfaces getMeshSurfaces))
            {
                m_CachedGetMeshSurfaces = getMeshSurfaces;
            }
            else
            {
                // If not create the default one
                m_CachedGetMeshSurfaces = gameObject.AddComponent<DefaultGetMeshSurfaces>();
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void CheckSurfacesAndHandles(List<MeshSurface> surfaces, NativeList<JobHandle> handles)
        {
            if (surfaces.Count != handles.Length)
                throw new InvalidOperationException("Mesh surfaces and handles must be same size!");
        }

        /// <summary>
        /// Re-usable node for containing data for slice job.
        /// </summary>
        class Node
        {
            public MeshSlicer Slicer;
            public int SliceCount;

            public List<MeshSurface> Surfaces;
            public Mesh.MeshDataArray ReadMeshData;
            public bool ReadMeshDataAllocated;

            public List<MeshSurface> SurfacesOutside;
            public Mesh.MeshDataArray WriteMeshDataOutside;
            public Node NodeOutside;

            public List<MeshSurface> SurfacesInside;
            public Mesh.MeshDataArray WriteMeshDataInside;
            public Node NodeInside;

            public int Index;
            public int Version;

            public JobHandle JobHandle;
            public OnSliceWithPlane Callback;

            public Plane Plane;

            public long FrameStart;

            public MeshSliceHandle Handle => new MeshSliceHandle(Index, Version);

            public bool IsLeaf => NodeOutside == null && NodeInside == null;

            public Node(int index, int version)
            {
                Index = index;
                Version = version;
                Surfaces = new List<MeshSurface>();
                SurfacesOutside = new List<MeshSurface>();
                SurfacesInside = new List<MeshSurface>();
            }
        }
    }
}