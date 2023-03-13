﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace ProjectDawn.Geometry3D
{
    /// <summary>
    /// A capsule is a sphere that is extended across the line.
    /// </summary>
    [DebuggerDisplay("Line = {Line}, Radius = {Radius}")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Capsule : IEquatable<Capsule>
    {
        /// <summary>
        /// Line through capsule extends.
        /// </summary>
        public Line Line;

        /// <summary>
        /// Radius of the capsule.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Center of the capsule.
        /// </summary>
        public float3 Center => Line.MidPoint;

        /// <summary>
        /// Height of the capsule;
        /// </summary>
        public float Height => Line.Length;

        /// <summary>
        /// Diameter of the capsule. Diameter = 2 * Radius. 
        /// </summary>
        public float Diameter
        {
            get => 2f * Radius;
            set => Radius = value * 0.5f;
        }

        public Capsule(float3 center, float height, float radius)
        {
            float3 extent = new float3(0, height, 0) * 0.5f;
            Line = new Line(center - extent, center + extent);
            Radius = radius;
        }

        public Capsule(Line line, float radius)
        {
            Line = line;
            Radius = radius;
        }

        /// <inheritdoc />
        public bool Equals(Capsule other) => Line == other.Line & Radius == other.Radius;

        /// <inheritdoc />
        public override bool Equals(object other) => throw new NotImplementedException();

        /// <inheritdoc />
        public override int GetHashCode() => base.GetHashCode();

        /// <inheritdoc />
        public static bool operator ==(Capsule lhs, Capsule rhs) => lhs.Line == rhs.Line & lhs.Radius == rhs.Radius;

        /// <inheritdoc />
        public static bool operator !=(Capsule lhs, Capsule rhs) => !(lhs == rhs);

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public bool Overlap(float3 point) => ShapeUtility.OverlapCapsuleAndPoint(this, point);

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public bool Overlap(Line line) => ShapeUtility.OverlapCapsuleAndLine(this, line);

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public bool Overlap(Sphere circle) => ShapeUtility.OverlapCapsuleAndSphere(this, circle);

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public bool Overlap(Box rectangle) => ShapeUtility.OverlapCapsuleAndBox(this, rectangle);

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public bool Overlap(Capsule capsule) => ShapeUtility.OverlapCapsuleAndCapsule(this, capsule);
    }
}
