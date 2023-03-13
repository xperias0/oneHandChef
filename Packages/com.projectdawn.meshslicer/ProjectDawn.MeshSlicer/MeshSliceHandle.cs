using System;

namespace ProjectDawn.MeshSlicer
{
    /// <summary>
    /// Mesh slice job handle.
    /// Used by <see cref="MeshSlicerManager"/> to identify mesh slice job.
    /// </summary>
    public struct MeshSliceHandle
    {
        internal int m_Index;
        internal int m_Version;

        /// <summary>
        /// Returns handle to null mesh slice job.
        /// Can be used as already completed mesh slice job.
        /// </summary>
        public static MeshSliceHandle Null => new MeshSliceHandle();

        internal MeshSliceHandle(int index, int version)
        {
            m_Index = index;
            m_Version = version;
        }

        public override bool Equals(object obj) => throw new NotImplementedException();
        public override int GetHashCode() => m_Index;
        public static implicit operator int(MeshSliceHandle handled) => handled.m_Index;
        public static bool operator ==(MeshSliceHandle lhs, MeshSliceHandle rhs) => lhs.m_Index == rhs.m_Index && lhs.m_Version == rhs.m_Version;
        public static bool operator !=(MeshSliceHandle lhs, MeshSliceHandle rhs) => lhs.m_Index != rhs.m_Index || lhs.m_Version != rhs.m_Version;
    }
}