using System;
using UnityEngine;

namespace ProjectDawn.MeshSlicer
{
    /// <summary>
    /// Slice piece side.
    /// </summary>
    public enum SlicerSide
    {
        /// <summary>
        /// Plane divides the surface into two volume pieces.
        /// Outside is refered to part which plane normal is facing.
        /// Also known as Outer, Left and Negative side.
        /// </summary>
        Outside,
        /// <summary>
        /// Plane divides the surface into two volume pieces.
        /// Inside is refered to part which plane normal is back facing.
        /// Also known as Inner, Right and Positive side.
        /// </summary>
        Inside,
    }

    /// <summary>
    /// Controls how mesh will be divided.
    /// </summary>
    public enum DivionType
    {
        /// <summary>
        /// Triangles will not be sliced.
        /// </summary>
        Discrete,
        /// <summary>
        /// Triangles will be sliced.
        /// </summary>
        Linear,
    }

    /// <summary>
    /// Slicer division settings.
    /// </summary>
    [Serializable]
    public struct SlicerDivision
    {
        /// <summary>
        /// Controls how mesh will be divided.
        /// </summary>
        public DivionType Type;

        /// <summary>
        /// To minimze floating point errors slices uses fixed precision.
        /// </summary>
        [Range(1000, 20000)]
        public int Precision;

        public static SlicerDivision Default => new SlicerDivision
        {
            Type = DivionType.Linear,
            Precision = 10000,
        };
    }
}