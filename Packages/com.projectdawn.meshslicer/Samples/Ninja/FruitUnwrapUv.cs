using ProjectDawn.Geometry3D;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Plane = ProjectDawn.Geometry3D.Plane;

namespace ProjectDawn.MeshSlicer
{
    [AddComponentMenu("Slicer/Modifier/Fruit Unwrap UV")]
    public class FruitUnwrapUv : MonoBehaviour, IScheduleSliceJob
    {
        public JobHandle OnScheduleSliceJob(MeshSlicer slicer, in MeshSurface surface, Plane plane, JobHandle dependency)
        {
            if (!slicer.TryGetComponent(out Fruit fruit))
                return default;

            var job = new FruitUnwrapUvJob
            {
                Surface = surface,
                Plane = plane,
                TexCoordScale = fruit.TexCoordScale
            };
            return job.Schedule(dependency);
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    unsafe struct FruitUnwrapUvJob : IJob
    {
        public MeshSurface Surface;
        public Plane Plane;
        public float TexCoordScale;

        public void Execute()
        {
            if (Surface.VertexData.Length < 3 || Surface.SubMeshes.Length == 0)
                return;

            if (TexCoordScale == 0)
                return;

            VertexAttributes mask = Surface.VertexData.Flags;

            if (!mask.ContainsFlag(VertexAttributes.TexCoord0))
                return;

            float3 normal = Plane.Normal;

            quaternion rotation = Quaternion.FromToRotation(normal, new float3(0, 0, -1));

            var vertexData = Surface.VertexData.GetUnsafeVertexData();
            var indices = Surface.Indices;

            SubMeshDescriptor subMesh = Surface.SubMeshes[Surface.SubMeshes.Length - 1];
            int indexStart = subMesh.indexStart / 3;
            int indexEnd = (subMesh.indexStart + subMesh.indexCount) / 3;
            for (int i = indexStart; i < indexEnd; ++i)
            {
                int3 index = indices[i];

                byte* ptr0 = (byte*)vertexData->ElementPointerAt(index.x);
                byte* ptr1 = (byte*)vertexData->ElementPointerAt(index.y);
                byte* ptr2 = (byte*)vertexData->ElementPointerAt(index.z);

                // Positions
                float3* p0 = (float3*)ptr0;
                float3* p1 = (float3*)ptr1;
                float3* p2 = (float3*)ptr2;
                ptr0 += sizeof(float3);
                ptr1 += sizeof(float3);
                ptr2 += sizeof(float3);

                // Normals
                if (mask.ContainsFlag(VertexAttributes.Normal))
                {
                    ptr0 += sizeof(float3);
                    ptr1 += sizeof(float3);
                    ptr2 += sizeof(float3);
                }

                // Early Out
                if (mask.ContainsFlag(VertexAttributes.Tangent))
                {
                    float4* t0 = (float4*)ptr0;
                    float4* t1 = (float4*)ptr1;
                    float4* t2 = (float4*)ptr2;

                    // All new fill vertices will have tangent set to 0.
                    if (t0->w != 0)
                        continue;
                }

                // Project positions to plane
                float2 projectedPosition0 = math.mul(rotation, *p0).xy * TexCoordScale;
                float2 projectedPosition1 = math.mul(rotation, *p1).xy * TexCoordScale;
                float2 projectedPosition2 = math.mul(rotation, *p2).xy * TexCoordScale;

                // Calculate tex coords
                float2 uv0 = projectedPosition0 * 0.5f + 0.5f;
                float2 uv1 = projectedPosition1 * 0.5f + 0.5f;
                float2 uv2 = projectedPosition2 * 0.5f + 0.5f;

                // Tangents
                if (mask.ContainsFlag(VertexAttributes.Tangent))
                {
                    float4* t0 = (float4*)ptr0;
                    float4* t1 = (float4*)ptr1;
                    float4* t2 = (float4*)ptr2;

                    // Calculate tangent and binormal
                    float3 dv1 = p1 - p0;
                    float3 dv2 = p2 - p0;
                    float2 duv1 = uv1 - uv0;
                    float2 duv2 = uv2 - uv0;
                    float r = 1.0f / (duv1.x * duv2.y - duv1.y * duv2.x);
                    float3 tangent = (dv1 * duv2.y - dv2 * duv1.y) * r;
                    float3 binormal = (dv2 * duv1.x - dv1 * duv2.x) * r;

                    // Packed tangent and binormal into float4
                    float sign = math.sign(math.dot(math.cross(normal, tangent), binormal));
                    float4 packedTangent = new float4(tangent, sign);

                    *t0 = packedTangent;
                    *t1 = packedTangent;
                    *t2 = packedTangent;

                    ptr0 += sizeof(float4);
                    ptr1 += sizeof(float4);
                    ptr2 += sizeof(float4);
                }

                // Color
                if (mask.ContainsFlag(VertexAttributes.Color))
                {
                    ptr0 += sizeof(Color32);
                    ptr1 += sizeof(Color32);
                    ptr2 += sizeof(Color32);
                }

                // Write uv
                float2* u0 = (float2*)ptr0;
                float2* u1 = (float2*)ptr1;
                float2* u2 = (float2*)ptr2;
                *u0 = uv0;
                *u1 = uv1;
                *u2 = uv2;
            }
        }
    }
}