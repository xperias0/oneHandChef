using UnityEngine;

namespace ProjectDawn.MeshSlicer
{
    /// <summary>
    /// Modifier interface used by <see cref="MeshSlicerManager"/>.
    /// Implement this interface on any <see cref="MonoBehaviour"/> and add it to <see cref="GameObject"/> that has <see cref="MeshSlicerManager"/> component.
    /// As result <see cref="MeshSlicerManager"/> will invoke function <see cref="CreatePiece(MeshSlicer, Mesh.MeshDataArray, int)"/> every time new piece will be created.
    /// Only one interface supported per <see cref="MeshSlicerManager"/>.
    /// </summary>
    public interface ICreatePiece
    {
        /// <summary>
        /// Callback to handle when the new piece is created after the slice.
        /// Can return null to skip this piece.
        /// </summary>
        /// <param name="source">Original mesh slicer.</param>
        /// <param name="meshDataArray">New piece mesh data.</param>
        /// <param name="index">Index of the slice starts from zero.</param>
        /// <returns>Returns newly created mesh slicer.</returns>
        MeshSlicer CreatePiece(MeshSlicer source, Mesh.MeshDataArray meshDataArray, int index);
    }
}