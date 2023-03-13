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
using Plane = ProjectDawn.Geometry3D.Plane;
using Debug = UnityEngine.Debug;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine.Rendering;
using ProjectDawn.Geometry3D;
using ProjectDawn.Geometry3D.LowLevel.Unsafe;
using System;
using ProjectDawn.Mathematics;

namespace ProjectDawn.MeshSlicer.LowLevel
{
    /// <summary>
    /// Helper class for slice functions.
    /// </summary>
    public static class Slicer
    {
        /// <summary>
        /// Schedules job that slices surface with the plane.
        /// This job only produces single piece of slice. In order to produce two pieces shedule two jobs with <see cref="SlicerSide.Outside"/> and <see cref="SlicerSide.Inside"/>.
        /// </summary>
        /// <param name="source">Surface that will be sliced.</param>
        /// <param name="plane">Plane used for slicing surface.</param>
        /// <param name="destination">Resulting surface after the slice.</param>
        /// <param name="division">Division settings.</param>
        /// <param name="fill">Fill settings.</param>
        /// <param name="side">Which side will be output.</param>
        /// <param name="dependency">Dependency job.</param>
        /// <returns>Returns handle to job.</returns>
        public static JobHandle SliceSurfaceWithPlane(
            in MeshSurface source,
            in Plane plane,
            in MeshSurface destination,
            in SlicerDivision division,
            in SlicerFill fill,
            SlicerSide side,
            JobHandle dependency = default)
        {
            CheckMask(source, destination);
            CheckAttributes(source.VertexData.GetVertexAttributes());
            CheckAttributes(destination.VertexData.GetVertexAttributes());

            var job = new SliceJob
            {
                VertexData = source.VertexData,
                Indices = source.Indices,
                SubMeshes = source.SubMeshes,

                NewVertexData = destination.VertexData,
                NewIndices = destination.Indices,
                NewSubMeshes = destination.SubMeshes,

                Plane = plane,

                Division = division,
                Fill = fill,
                Side = side,

                Mask = source.VertexData.Flags,
            };

            return job.Schedule(dependency);
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS"), Conditional("UNITY_DOTS_DEBUG")]
        static void CheckMask(in MeshSurface source, in MeshSurface destination)
        {
            if (!source.VertexData.Flags.HasFlag(VertexAttributes.Position))
                throw new InvalidOperationException("Mesh surface must contain positions!");
            if (source.VertexData.Flags != destination.VertexData.Flags)
                throw new InvalidOperationException("Source and destination vertex data mask must be same!");
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS"), Conditional("UNITY_DOTS_DEBUG")]
        static void CheckAttributes(NativeArray<VertexAttributeDescriptor> attributes)
        {
            for (int attributeIndex = 0; attributeIndex < attributes.Length; ++attributeIndex)
            {
                VertexAttributeDescriptor attribute = attributes[attributeIndex];
                switch (attribute.attribute)
                {
                    case VertexAttribute.Position:
                        if (attribute.format != VertexAttributeFormat.Float32 || attribute.dimension != 3)
                            throw new InvalidOperationException($"Position must be compatible with type float3!");
                        break;
                    case VertexAttribute.Normal:
                        if (attribute.format != VertexAttributeFormat.Float32 || attribute.dimension != 3)
                            throw new InvalidOperationException($"Normal must be compatible with type float3!");
                        break;
                    case VertexAttribute.Tangent:
                        if (attribute.format != VertexAttributeFormat.Float32 || attribute.dimension != 4)
                            throw new InvalidOperationException($"Tangent must be compatible with type float4!");
                        break;
                    case VertexAttribute.Color:
                        if (attribute.format != VertexAttributeFormat.UNorm8 || attribute.dimension != 4)
                            throw new InvalidOperationException($"Color must be compatible with type Color32!");
                        break;
                    case VertexAttribute.TexCoord0:
                    case VertexAttribute.TexCoord1:
                        if (attribute.format != VertexAttributeFormat.Float32 || attribute.dimension != 2)
                            throw new InvalidOperationException($"TexCoord must be compatible with type float4!");
                        break;
                    default:
                        throw new NotImplementedException($"Currently attribute {attribute.attribute} is not supported for slicing.");
                }
            }
        }
    }

    /// <summary>
    /// Job for slicing surface.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    struct SliceJob : IJob
    {
        [ReadOnly]
        public VertexData VertexData;
        [ReadOnly]
        public NativeList<int3> Indices;
        [ReadOnly]
        public NativeList<SubMeshDescriptor> SubMeshes;

        public VertexData NewVertexData;
        public NativeList<int3> NewIndices;
        public NativeList<SubMeshDescriptor> NewSubMeshes;

        public Plane Plane;
        public SlicerDivision Division;
        public SlicerFill Fill;
        public SlicerSide Side;
        public VertexAttributes Mask;

        bool Outside => Side == SlicerSide.Outside;
        bool Inside => Side == SlicerSide.Inside;

        public unsafe void Execute()
        {
            // Early out
            if (VertexData.IsEmpty || Indices.IsEmpty || SubMeshes.IsEmpty)
                return;

            var indices = Indices;
            var subMeshes = SubMeshes;

            var remmap = new NativeParallelHashMap<int, int>(VertexData.Length, Allocator.Temp);

            var newIndices = NewIndices;
            var newSubMeshes = NewSubMeshes;

            // Convert plane to discrete space
            // This helps if multiple slices executed with similar planes
            Plane plane = new Plane
            {
                NormalAndDistance = round(Plane.NormalAndDistance * Division.Precision) / Division.Precision
            };

            // In most cases new mesh capacity should not exceed the original one
            NewVertexData.Capacity = VertexData.Capacity;

            var fillIndices = new NativeList<int>(Allocator.Temp);

            for (int subMeshIndex = 0; subMeshIndex < subMeshes.Length; ++subMeshIndex)
            {
                SubMeshDescriptor subMesh = subMeshes[subMeshIndex];
                int indexStart = subMesh.indexStart / 3;
                int indexEnd = indexStart + subMesh.indexCount / 3;

                CheckSubMesh(subMesh);

                int newIndexStart = newIndices.Length * 3;
                for (int i = indexStart; i < indexEnd; ++i)
                {
                    int3 index = indices[i];

                    float3 a = VertexData.GetVertexAt(index.x);
                    float3 b = VertexData.GetVertexAt(index.y);
                    float3 c = VertexData.GetVertexAt(index.z);

                    float3 distances = new float3(
                        plane.SignedDistanceToPoint(a),
                        plane.SignedDistanceToPoint(b),
                        plane.SignedDistanceToPoint(c));

                    distances = round(distances * Division.Precision) / Division.Precision;

                    // Idea of slicing triangles is actually very simple.
                    // We have triangle and plane:
                    //       b
                    //     /  \
                    //    /    \
                    //---/------\-----Plane
                    //  /        \
                    // a -------- c
                    // Firstly, we find each vertex distance from plane.
                    // Then we only need signs form distance so we get bool3.
                    // Finally, we need to handle all possible value signs has.

                    bool3 signs = distances >= 0;

                    // All vertices on the plane
                    bool3 isZero = distances == 0;
                    if (all(isZero))
                    {
                        continue;
                    }

                    // 111: All vertices above plane
                    //       y
                    //     /  \
                    //    /    \
                    //   /      \
                    //  /        \
                    // x -------- z
                    //-----------------Plane
                    if (all(signs))
                    {
                        if (Outside)
                        {
                            int3 newIndex = new int3(RemmapIndex(remmap, index.x), RemmapIndex(remmap, index.y), RemmapIndex(remmap, index.z));

                            newIndices.Add(newIndex);

                            // If two vertices are exectly on the plane use them as fill vertices
                            // This will avoid creating new vertices that are exectly on the plane
                            if (isZero.x && isZero.y)
                            {
                                fillIndices.Add(newIndex.x);
                                fillIndices.Add(newIndex.y);
                            }
                            else if (isZero.z && isZero.x)
                            {
                                fillIndices.Add(newIndex.z);
                                fillIndices.Add(newIndex.x);
                            }
                            else if (isZero.y && isZero.z)
                            {
                                fillIndices.Add(newIndex.y);
                                fillIndices.Add(newIndex.z);
                            }
                        }

                        continue;
                    }

                    // 000: All vertices below plane
                    //-----------------Plane
                    //       y
                    //     /  \
                    //    /    \
                    //   /      \
                    //  /        \
                    // x -------- z
                    if (all(distances <= 0))
                    {
                        if (Inside)
                        {
                            int3 newIndex = new int3(RemmapIndex(remmap, index.x), RemmapIndex(remmap, index.y), RemmapIndex(remmap, index.z));

                            newIndices.Add(newIndex);

                            // If two vertices are exectly on the plane use them as fill vertices
                            // This will avoid creating new vertices that are exectly on the plane
                            if (isZero.x && isZero.y)
                            {
                                fillIndices.Add(newIndex.x);
                                fillIndices.Add(newIndex.y);
                            }
                            else if (isZero.z && isZero.x)
                            {
                                fillIndices.Add(newIndex.z);
                                fillIndices.Add(newIndex.x);
                            }
                            else if (isZero.y && isZero.z)
                            {
                                fillIndices.Add(newIndex.y);
                                fillIndices.Add(newIndex.z);
                            }
                        }

                        continue;
                    }

                    // Discrete division does not slice triangles
                    if (Division.Type == DivionType.Discrete)
                        continue;

                    // Triangle and plane intersect
                    //-----------------Plane
                    //       z
                    //     /  \
                    //    /    \
                    //---/------\-----Plane
                    //  /        \
                    // x -------- y
                    if (signs.x == signs.y)
                    {
                        float blend0 = distances.x / (distances.x - distances.z);
                        float blend1 = distances.y / (distances.y - distances.z);

                        int2 newIndex = CreateSectionVertices(index, blend0, blend1);

                        if (Fill.Enabled)
                        {
                            fillIndices.Add(newIndex.y);
                            fillIndices.Add(newIndex.x);
                        }

                        if (signs.z)
                        {
                            if (Inside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.x), RemmapIndex(remmap, index.y), newIndex.y));
                                newIndices.Add(new int3(RemmapIndex(remmap, index.x), newIndex.y, newIndex.x));
                            }
                            else // if (Outside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.z), newIndex.x, newIndex.y));
                            }
                        }
                        else
                        {
                            if (Outside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.x), RemmapIndex(remmap, index.y), newIndex.y));
                                newIndices.Add(new int3(RemmapIndex(remmap, index.x), newIndex.y, newIndex.x));
                            }
                            else // if (Inside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.z), newIndex.x, newIndex.y));
                            }
                        }
                    }
                    // Triangle and plane intersect
                    //-----------------Plane
                    //       y
                    //     /  \
                    //    /    \
                    //---/------\-----Plane
                    //  /        \
                    // z -------- x
                    else if (signs.z == signs.x)
                    {
                        float blend0 = distances.z / (distances.z - distances.y);
                        float blend1 = distances.x / (distances.x - distances.y);

                        int2 newIndex = CreateSectionVertices(index.zxy, blend0, blend1);

                        if (Fill.Enabled)
                        {
                            fillIndices.Add(newIndex.y);
                            fillIndices.Add(newIndex.x);
                        }

                        if (signs.y)
                        {
                            if (Inside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.z), RemmapIndex(remmap, index.x), newIndex.y));
                                newIndices.Add(new int3(RemmapIndex(remmap, index.z), newIndex.y, newIndex.x));
                            }
                            else // if (Outside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.y), newIndex.x, newIndex.y));
                            }

                        }
                        else
                        {
                            if (Outside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.z), RemmapIndex(remmap, index.x), newIndex.y));
                                newIndices.Add(new int3(RemmapIndex(remmap, index.z), newIndex.y, newIndex.x));
                            }
                            else // if (Inside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.y), newIndex.x, newIndex.y));
                            }
                        }
                    }
                    // Triangle and plane intersect
                    //-----------------Plane
                    //       x
                    //     /  \
                    //    /    \
                    //---/------\-----Plane
                    //  /        \
                    // y -------- z
                    else // if (signs.y == signs.z)
                    {
                        float blend0 = distances.y / (distances.y - distances.x);
                        float blend1 = distances.z / (distances.z - distances.x);

                        int2 newIndex = CreateSectionVertices(index.yzx, blend0, blend1);

                        if (Fill.Enabled)
                        {
                            fillIndices.Add(newIndex.y);
                            fillIndices.Add(newIndex.x);
                        }

                        if (signs.x)
                        {
                            if (Inside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.y), RemmapIndex(remmap, index.z), newIndex.y));
                                newIndices.Add(new int3(RemmapIndex(remmap, index.y), newIndex.y, newIndex.x));
                            }
                            else // if (Outside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.x), newIndex.x, newIndex.y));
                            }
                        }
                        else
                        {
                            if (Outside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.y), RemmapIndex(remmap, index.z), newIndex.y));
                                newIndices.Add(new int3(RemmapIndex(remmap, index.y), newIndex.y, newIndex.x));
                            }
                            else // if (Inside)
                            {
                                newIndices.Add(new int3(RemmapIndex(remmap, index.x), newIndex.x, newIndex.y));
                            }
                        }
                    }
                }
                int newIndexEnd = newIndices.Length * 3;

                newSubMeshes.Add(new SubMeshDescriptor(newIndexStart, newIndexEnd - newIndexStart));
            }

            // Create fill triangles
            if (Fill.Enabled && Division.Type == DivionType.Linear)
            {
                var builder = new UnsafeVoronoiBuilder(fillIndices.Length, Allocator.Temp);
                {
                    float3 normal = Outside ? -plane.Normal : plane.Normal;

                    quaternion rotation = Quaternion.FromToRotation(normal, new float3(0, 0, -1));

                    int fillVerticesStart = NewVertexData.Length;

                    // Adding fill vertices to vornoi builder
                    for (int fillIndex = 0; fillIndex < fillIndices.Length; ++fillIndex)
                    {
                        int index = fillIndices[fillIndex];
                        float3 vertex = (int3)round(NewVertexData.GetVertexAt(index) * Division.Precision);
                        float2 projectedVertex = mul(rotation, vertex).xy;

                        if (builder.Add(projectedVertex))
                        {
                            CreateFillVertex(index, vertex, normal);
                        }
                    }

                    int fillIndexStart = newIndices.Length * 3;

                    // Triangulate fill vertices
                    var delaunay = new DelaunayTriangulation(builder.NumSites, Allocator.Temp);
                    builder.Construct(ref delaunay);
                    for (int i = 0; i < delaunay.Indices.Length; ++i)
                    {
                        int3 index = delaunay.Indices[i];
                        newIndices.Add(index + fillVerticesStart);
                    }
                    delaunay.Dispose();

                    // Update submesh with new index count
                    int fillIndexEnd = newIndices.Length * 3;
                    int fillIndexCount = fillIndexEnd - fillIndexStart;
                    if (fillIndexCount != 0)
                    {
                        if (Fill.SubMeshIndex == newSubMeshes.Length) // Add new
                        {
                            newSubMeshes.Add(new SubMeshDescriptor(fillIndexStart, fillIndexEnd - fillIndexStart));
                        }
                        else if (Fill.SubMeshIndex == newSubMeshes.Length - 1) // Last
                        {
                            newSubMeshes.ElementAt(newSubMeshes.Length - 1).indexCount += fillIndexEnd - fillIndexStart;
                        }
                        else
                            newSubMeshes.ElementAt(newSubMeshes.Length - 1).indexCount += fillIndexEnd - fillIndexStart;
                        //throw new InvalidOperationException("Invalid sub mesh index used!");
                    }
                }
                builder.Dispose();
            }

            fillIndices.Dispose();
        }

        unsafe int RemmapIndex(in NativeParallelHashMap<int, int> remmap, int index)
        {
            if (remmap.TryGetValue(index, out int newIndex))
            {
                return newIndex;
            }
            else
            {
                // Create new vertex and return new index to it
                newIndex = CreateSideVertex(index);
                remmap.Add(index, newIndex);

                return newIndex;
            }
        }

        unsafe int CreateSideVertex(int index)
        {
            int newIndex = NewVertexData.Length;

            NewVertexData.ResizeUninitialized(NewVertexData.Length + 1);

            UnsafeVertexData* vertexData = VertexData.GetUnsafeVertexData();
            UnsafeVertexData* newVertexData = NewVertexData.GetUnsafeVertexData();
            void* vertex = vertexData->ElementPointerAt(index);
            void* newVertex = newVertexData->ElementPointerAt(newIndex);

            UnsafeUtility.MemCpy(newVertex, vertex, vertexData->Size);

            return newIndex;
        }

        unsafe int2 CreateSectionVertices(int3 index, float t0, float t1)
        {
            int2 newIndex = new int2(NewVertexData.Length, NewVertexData.Length + 1);

            NewVertexData.ResizeUninitialized(NewVertexData.Length + 2);

            UnsafeVertexData* vertexData = VertexData.GetUnsafeVertexData();
            UnsafeVertexData* newVertexData = NewVertexData.GetUnsafeVertexData();
            byte* vertex0 = (byte*)vertexData->ElementPointerAt(index.x);
            byte* vertex1 = (byte*)vertexData->ElementPointerAt(index.y);
            byte* vertex2 = (byte*)vertexData->ElementPointerAt(index.z);
            byte* newVertex0 = (byte*)newVertexData->ElementPointerAt(newIndex.x);
            byte* newVertex1 = (byte*)newVertexData->ElementPointerAt(newIndex.y);

            //if (Mask.Contains(VertexAttributes.Position))
            {
                float3* value0 = (float3*)vertex0;
                float3* value1 = (float3*)vertex1;
                float3* value2 = (float3*)vertex2;
                float3* newValue0 = (float3*)newVertex0;
                float3* newValue1 = (float3*)newVertex1;

                *newValue0 = lerp(*value0, *value2, t0);
                *newValue1 = lerp(*value1, *value2, t1);

                vertex0 += sizeof(float3);
                vertex1 += sizeof(float3);
                vertex2 += sizeof(float3);
                newVertex0 += sizeof(float3);
                newVertex1 += sizeof(float3);
            }
            if (Mask.ContainsFlag(VertexAttributes.Normal))
            {
                float3* value0 = (float3*)vertex0;
                float3* value1 = (float3*)vertex1;
                float3* value2 = (float3*)vertex2;
                float3* newValue0 = (float3*)newVertex0;
                float3* newValue1 = (float3*)newVertex1;

                *newValue0 = lerp(*value0, *value2, t0);
                *newValue1 = lerp(*value1, *value2, t1);

                vertex0 += sizeof(float3);
                vertex1 += sizeof(float3);
                vertex2 += sizeof(float3);
                newVertex0 += sizeof(float3);
                newVertex1 += sizeof(float3);
            }
            if (Mask.ContainsFlag(VertexAttributes.Tangent))
            {
                float4* value0 = (float4*)vertex0;
                float4* value1 = (float4*)vertex1;
                float4* value2 = (float4*)vertex2;
                float4* newValue0 = (float4*)newVertex0;
                float4* newValue1 = (float4*)newVertex1;

                *newValue0 = lerp(*value0, *value2, t0);
                *newValue1 = lerp(*value1, *value2, t1);

                vertex0 += sizeof(float4);
                vertex1 += sizeof(float4);
                vertex2 += sizeof(float4);
                newVertex0 += sizeof(float4);
                newVertex1 += sizeof(float4);
            }
            if (Mask.ContainsFlag(VertexAttributes.Color))
            {
                Color32* value0 = (Color32*)vertex0;
                Color32* value1 = (Color32*)vertex1;
                Color32* value2 = (Color32*)vertex2;
                Color32* newValue0 = (Color32*)newVertex0;
                Color32* newValue1 = (Color32*)newVertex1;

                *newValue0 = Color32.Lerp(*value0, *value2, t0);
                *newValue1 = Color32.Lerp(*value1, *value2, t1);

                vertex0 += sizeof(Color32);
                vertex1 += sizeof(Color32);
                vertex2 += sizeof(Color32);
                newVertex0 += sizeof(Color32);
                newVertex1 += sizeof(Color32);
            }
            if (Mask.ContainsFlag(VertexAttributes.TexCoord0))
            {
                float2* value0 = (float2*)vertex0;
                float2* value1 = (float2*)vertex1;
                float2* value2 = (float2*)vertex2;
                float2* newValue0 = (float2*)newVertex0;
                float2* newValue1 = (float2*)newVertex1;

                *newValue0 = lerp(*value0, *value2, t0);
                *newValue1 = lerp(*value1, *value2, t1);

                vertex0 += sizeof(float2);
                vertex1 += sizeof(float2);
                vertex2 += sizeof(float2);
                newVertex0 += sizeof(float2);
                newVertex1 += sizeof(float2);
            }
            if (Mask.ContainsFlag(VertexAttributes.TexCoord1))
            {
                float2* value0 = (float2*)vertex0;
                float2* value1 = (float2*)vertex1;
                float2* value2 = (float2*)vertex2;
                float2* newValue0 = (float2*)newVertex0;
                float2* newValue1 = (float2*)newVertex1;

                *newValue0 = lerp(*value0, *value2, t0);
                *newValue1 = lerp(*value1, *value2, t1);

                vertex0 += sizeof(float2);
                vertex1 += sizeof(float2);
                vertex2 += sizeof(float2);
                newVertex0 += sizeof(float2);
                newVertex1 += sizeof(float2);
            }

            return newIndex;
        }

        unsafe void CreateFillVertex(int index, float3 position, float3 normal)
        {
            int newIndex = NewVertexData.Length;

            NewVertexData.ResizeUninitialized(NewVertexData.Length + 1);

            UnsafeVertexData* newVertexData = NewVertexData.GetUnsafeVertexData();
            byte* vertex = (byte*)newVertexData->ElementPointerAt(index);
            byte* newVertex = (byte*)newVertexData->ElementPointerAt(newIndex);

            UnsafeUtility.MemCpy(newVertex, vertex, newVertexData->Size);

            //if (Mask.ContainsFlag(VertexAttributes.Position))
            {
                float3* value = (float3*)newVertex;
                *value = position / Division.Precision;
                newVertex += sizeof(float3);
            }
            if (Mask.ContainsFlag(VertexAttributes.Normal))
            {
                float3* value = (float3*)newVertex;
                *value = normal;
                newVertex += sizeof(float3);
            }
            if (Mask.ContainsFlag(VertexAttributes.Tangent))
            {
                float4* value = (float4*)newVertex;
                *value = 0;
                newVertex += sizeof(float4);
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS"), Conditional("UNITY_DOTS_DEBUG")]
        static void CheckSubMesh(SubMeshDescriptor subMeshDescriptor)
        {
            if (subMeshDescriptor.topology != MeshTopology.Triangles)
                throw new InvalidOperationException("Slicer only works with triangles!");
        }
    }
}