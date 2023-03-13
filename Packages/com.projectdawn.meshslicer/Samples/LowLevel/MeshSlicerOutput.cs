using ProjectDawn.Geometry2D;
using ProjectDawn.Geometry2D.LowLevel.Unsafe;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using static ProjectDawn.Mathematics.math2;
using static Unity.Mathematics.math;
using static Unity.Mathematics.quaternion;
using Plane = ProjectDawn.Geometry3D.Plane;
using Debug = UnityEngine.Debug;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine.Rendering;
using UnityEngine.Profiling;
using ProjectDawn.Geometry3D;
using ProjectDawn.MeshSlicer;
using ProjectDawn.MeshSlicer.LowLevel;

public class MeshSlicerOutput : MonoBehaviour
{
    public Plane Plane;
    public MeshFilter OutputA;
    public MeshFilter OutputB;
    public SlicerDivision Division = SlicerDivision.Default;
    public SlicerFill Fill = SlicerFill.Default;
    public double SliceTime;
    public double MeshTime;

    unsafe void Update()
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;

        var sw = new Stopwatch();
        sw.Start();

        Profiler.BeginSample("Setup");

        var attributes = new NativeArray<VertexAttributeDescriptor>(mesh.vertexAttributeCount, Allocator.TempJob);
        for (int attributeIndex = 0; attributeIndex < mesh.vertexAttributeCount; ++attributeIndex)
            attributes[attributeIndex] = mesh.GetVertexAttribute(attributeIndex);

        var surface = new MeshSurface(mesh.vertexCount, attributes, Allocator.TempJob);

        // Read into the mesh surface
        var meshDataArray = Mesh.AcquireReadOnlyMeshData(mesh);
        var dependency = surface.Read(meshDataArray[0], default);

        // Slice
        var meshDataArrayNeg = Mesh.AllocateWritableMeshData(1);
        var meshDataArrayPos = Mesh.AllocateWritableMeshData(1);
        var meshSurfaceNeg = new MeshSurface(mesh.vertexCount, attributes, Allocator.TempJob);
        var meshSurfacePos = new MeshSurface(mesh.vertexCount, attributes, Allocator.TempJob);
        JobHandle dependencyPos = Slicer.SliceSurfaceWithPlane(surface, Plane.Normalize(Plane), meshSurfacePos, Division, Fill, SlicerSide.Outside, dependency);
        JobHandle dependencyNeg = Slicer.SliceSurfaceWithPlane(surface, Plane.Normalize(Plane), meshSurfaceNeg, Division, Fill, SlicerSide.Inside, dependency);

        // Write into the negative and positive mesh surfaces
        dependencyPos = meshSurfacePos.Write(meshDataArrayPos[0], MeshUpdateFlags.Default, dependencyPos);
        dependencyNeg = meshSurfaceNeg.Write(meshDataArrayNeg[0], MeshUpdateFlags.Default, dependencyNeg);

        dependency = JobHandle.CombineDependencies(dependencyPos, dependencyNeg);

        Profiler.EndSample();

        // Wait jobs to finish
        dependency.Complete();

        surface.Dispose(dependency);
        meshSurfaceNeg.Dispose(dependency);
        meshSurfacePos.Dispose(dependency);

        attributes.Dispose();

        sw.Stop();
        SliceTime = sw.Elapsed.TotalMilliseconds;

        sw.Restart();

        Profiler.BeginSample("Apply");

        // Update meshes
        if (OutputB.mesh == null)
            OutputB.mesh = new Mesh();
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArrayNeg, OutputB.mesh);
        if (OutputA.mesh == null)
            OutputA.mesh = new Mesh();
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArrayPos, OutputA.mesh);
        meshDataArray.Dispose();

        Profiler.EndSample();

        sw.Stop();
        MeshTime = sw.Elapsed.TotalMilliseconds;
    }
}
