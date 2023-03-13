using System;
using UnityEngine;

namespace ProjectDawn.MeshSlicer
{
    /// <summary>
    /// Slicer multiple slice settings.
    /// </summary>
    [Serializable]
    public struct SlicerAdditive
    {
        /// <summary>
        /// Maximum number of slice jobs one <see cref="MeshSlicer"/> can contain.
        /// </summary>
        [Range(1, 10)]
        public int MaxSliceCount;

        /// <summary>
        /// If false pieces will be created only once all scheduled jobs for <see cref="MeshSlicer"/> completed.
        /// Otherwise it will generated new pieces as soon as any job completes.
        /// </summary>
        public bool PartialResults;

        public static SlicerAdditive Default => new SlicerAdditive
        {
            MaxSliceCount = 3,
            PartialResults = true,
        };
    }
}