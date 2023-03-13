using ProjectDawn.Geometry3D;
using Unity.Jobs;

namespace ProjectDawn.MeshSlicer
{
    /// <summary>
    /// Modifier interface used by <see cref="MeshSlicerManager"/>.
    /// Implement this interface on any <see cref="MonoBehaviour"/> and add it to <see cref="GameObject"/> that has <see cref="MeshSlicerManager"/> component.
    /// As result <see cref="MeshSlicerManager"/> will invoke function <see cref="OnScheduleSliceJob(in MeshSurface, Plane, JobHandle)"/>.
    /// </summary>
    public interface IScheduleSliceJob
    {
        /// <summary>
        /// Callback that allows adding additional jobs after the slice job.
        /// </summary>
        /// <param name="surface">Mesh surface of piece.</param>
        /// <param name="plane">Slicing plane.</param>
        /// <param name="dependency">Job handle that currently uses surface.</param>
        /// <returns>Returns new job handle.</returns>
        JobHandle OnScheduleSliceJob(MeshSlicer slicer, in MeshSurface surface, Plane plane, JobHandle dependency);
    }
}