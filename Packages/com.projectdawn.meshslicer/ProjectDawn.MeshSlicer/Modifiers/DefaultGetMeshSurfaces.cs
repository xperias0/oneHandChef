using ProjectDawn.Geometry3D;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProjectDawn.MeshSlicer
{
    /// <summary>
    /// Detail level of collider quality. The better quality the more vertices it will have.
    /// </summary>
    public enum ColliderQuality
    {
        Low,
        Medium,
        High,
    }

    [AddComponentMenu("Slicer/Modifier/Default GetMeshSurfaces")]
    public class DefaultGetMeshSurfaces : MonoBehaviour, IGetMeshSurfaces
    {
        static List<Mesh> s_CachedMeshes = new List<Mesh>();
        
        /// <summary>
        /// Collider quality in terms of vertex count.
        /// The higher quality the more vertices generated shapes will have.
        /// </summary>
        public ColliderQuality ColliderQuality = ColliderQuality.High;

        public Mesh.MeshDataArray GetMeshSurfaces(MeshSlicer slicer, List<MeshSurface> surfaces, NativeList<JobHandle> handles)
        {
            bool hasMeshFilter = slicer.TryGetComponent(out MeshFilter meshFilter);
            bool hasMeshCollider = slicer.TryGetComponent(out MeshCollider meshCollider);

            // Gather meshes that will need reading
            s_CachedMeshes.Clear();
            if (hasMeshFilter)
                s_CachedMeshes.Add(meshFilter.sharedMesh);
            if (hasMeshCollider)
                s_CachedMeshes.Add(meshCollider.sharedMesh);
            var meshDataArray = Mesh.AcquireReadOnlyMeshData(s_CachedMeshes);

            // Create mesh surfaces
            int counter = 0;
            if (hasMeshFilter)
            {
                var mesh = meshFilter.sharedMesh;

                // Gathering attributes for mesh
                var attributes = new NativeArray<VertexAttributeDescriptor>(mesh.vertexAttributeCount, Allocator.Temp);
                for (int attributeIndex = 0; attributeIndex < mesh.vertexAttributeCount; ++attributeIndex)
                    attributes[attributeIndex] = mesh.GetVertexAttribute(attributeIndex);

                MeshSurface surface = new MeshSurface(mesh.vertexCount, attributes, Allocator.TempJob);
                JobHandle dependency = surface.Read(meshDataArray[counter++], default);

                surfaces.Add(surface);
                handles.Add(dependency);

                attributes.Dispose();
            }
            if (hasMeshCollider)
            {
                var mesh = meshCollider.sharedMesh;

                // Gathering attributes for mesh
                var attributes = new NativeArray<VertexAttributeDescriptor>(mesh.vertexAttributeCount, Allocator.Temp);
                for (int attributeIndex = 0; attributeIndex < mesh.vertexAttributeCount; ++attributeIndex)
                    attributes[attributeIndex] = mesh.GetVertexAttribute(attributeIndex);

                MeshSurface surface = new MeshSurface(mesh.vertexCount, attributes, Allocator.TempJob);
                JobHandle dependency = surface.Read(meshDataArray[counter++], default);

                surfaces.Add(surface);
                handles.Add(dependency);

                attributes.Dispose();
            }
            else if (slicer.TryGetComponent(out BoxCollider boxCollider))
            {
                var attributes = new NativeArray<VertexAttributeDescriptor>(1, Allocator.Temp);
                attributes[0] = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3);

                MeshSurface surface = new MeshSurface(1, attributes, Allocator.TempJob);
                JobHandle dependency = surface.Box(new Box(boxCollider.center - boxCollider.size * 0.5f, boxCollider.size), default);

                surfaces.Add(surface);
                handles.Add(dependency);

                attributes.Dispose();
            }
            else if (slicer.TryGetComponent(out SphereCollider sphereCollider))
            {
                var attributes = new NativeArray<VertexAttributeDescriptor>(1, Allocator.Temp);
                attributes[0] = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3);

                MeshSurface surface = new MeshSurface(1, attributes, Allocator.TempJob);
                JobHandle dependency = surface.Icosphere(new Sphere(sphereCollider.center, sphereCollider.radius), GetSubdivision(), default);

                surfaces.Add(surface);
                handles.Add(dependency);

                attributes.Dispose();
            }
            else if (slicer.TryGetComponent(out CapsuleCollider capsuleCollider))
            {
                var attributes = new NativeArray<VertexAttributeDescriptor>(1, Allocator.Temp);
                attributes[0] = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3);

                MeshSurface surface = new MeshSurface(1, attributes, Allocator.TempJob);
                JobHandle dependency = surface.Icocapsule(new Capsule(capsuleCollider.center, capsuleCollider.height, capsuleCollider.radius), GetSubdivision(), default);

                surfaces.Add(surface);
                handles.Add(dependency);

                attributes.Dispose();
            }

            return meshDataArray;
        }

        int GetSubdivision()
        {
            switch (ColliderQuality)
            {
                case ColliderQuality.Low:
                    return 1;
                case ColliderQuality.Medium:
                    return 2;
                case ColliderQuality.High:
                    return 3;
            }
            return 0;
        }
    }
}