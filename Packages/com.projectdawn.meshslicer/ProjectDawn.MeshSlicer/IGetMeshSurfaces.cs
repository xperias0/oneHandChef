using ProjectDawn.Geometry3D;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace ProjectDawn.MeshSlicer
{
    /// <summary>
    /// Modifier interface used by <see cref="MeshSlicerManager"/>.
    /// Implement this interface on any <see cref="MonoBehaviour"/> and add it to <see cref="GameObject"/> that has <see cref="MeshSlicerManager"/> component.
    /// As result <see cref="MeshSlicerManager"/> will invoke function <see cref="GetMeshSurfaces(MeshSlicer, List{MeshSurface}, NativeList{JobHandle})"/>.
    /// Only one interface supported per <see cref="MeshSlicerManager"/>.
    /// </summary>
    public interface IGetMeshSurfaces
    {
        /// <summary>
        /// Callback to collect mesh surfaces from the mesh slicer.
        /// </summary>
        /// <param name="slicer">The mesh slicer.</param>
        /// <param name="surfaces">Output surfaces.</param>
        /// <param name="handles">Output handles.</param>
        /// <returns>Returns newly created mesh data array.</returns>
        Mesh.MeshDataArray GetMeshSurfaces(MeshSlicer slicer, List<MeshSurface> surfaces, NativeList<JobHandle> handles);
    }
}