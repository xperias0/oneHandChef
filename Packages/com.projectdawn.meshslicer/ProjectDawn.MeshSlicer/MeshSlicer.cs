using UnityEngine;

namespace ProjectDawn.MeshSlicer
{
    /// <summary>
    /// Component for slicing mesh game objects.
    /// </summary>
    [AddComponentMenu("Mesh/Mesh Slicer")]
    public class MeshSlicer : MonoBehaviour
    {
        /// <summary>
        /// Mesh slicer division settings.
        /// </summary>
        public SlicerDivision Division = SlicerDivision.Default;

        /// <summary>
        /// Mesh slicer fill settings.
        /// </summary>
        public SlicerFill Fill = SlicerFill.Default;

        /// <summary>
        /// Mesh slicer additive slice settings.
        /// </summary>
        public SlicerAdditive Additive = SlicerAdditive.Default;

        /// <summary>
        /// Mesh slicer material used for fill.
        /// </summary>
        public Material FillMaterial;

        /// <summary>
        /// Version of the component.
        /// Currently not used, but required for the future to easily indentify older version components.
        /// </summary>
        internal int m_Version;

        /// <summary>
        /// Current handle to mesh slice job.
        /// </summary>
        public MeshSliceHandle Handle { get; internal set; }

        // Required for component to show enable property
        void OnEnable() { }
    }
}