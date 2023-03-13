using System;

namespace ProjectDawn.MeshSlicer
{
    /// <summary>
    /// Slicer fill settings.
    /// Fill is cover surface on each slice piece.
    /// In some literature refered as cap.
    /// </summary>
    [Serializable]
    public struct SlicerFill
    {
        /// <summary>
        /// Is fill generated for each slice pieces.
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// Is fill surface convex shape.
        /// </summary>
        public bool Convex;

        /// <summary>
        /// Index of sub mesh that will be used for adding fill mesh.
        /// </summary>
        public int SubMeshIndex;

        public static SlicerFill Default => new SlicerFill
        {
            Enabled = true,
            Convex = true,
            SubMeshIndex = 0,
        };
    }
}