using ProjectDawn.Geometry3D;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProjectDawn.MeshSlicer
{
    /// <summary>
    /// Default implementation of <see cref="ICreatePiece"/> used by <see cref="MeshSlicerManager"/>.
    /// </summary>
    [RequireComponent(typeof(MeshSlicerManager))]
    [AddComponentMenu("Slicer/Modifier/Default CreatePiece")]
    public class DefaultCreatePiece : MonoBehaviour, ICreatePiece
    {
        static List<Mesh> s_CachedMeshes = new List<Mesh>();
        static List<Material> s_CachedMaterials = new List<Material>();
        static Dictionary<int, Material[]> s_CachedMaterialArrays = new Dictionary<int, Material[]>();

        /// <summary>
        /// Minimum allowed volume of slice piece.
        /// </summary>
        public float MinimumVolume = 0.01f;

        /// <summary>
        /// Mesh collider cooking options.
        /// </summary>
        public MeshColliderCookingOptions MeshColliderCookingOptions = MeshColliderCookingOptions.WeldColocatedVertices;

        public MeshSlicer CreatePiece(MeshSlicer source, Mesh.MeshDataArray meshDataArray, int index)
        {
            var meshData = meshDataArray[0];

            // Skip empty mesh
            if (meshData.vertexCount == 0 || meshData.subMeshCount == 0)
            {
                return null;
            }

            // Combine subMeshes bounds
            Box bounds = GetBounds(meshDataArray[0]);

            // Skip mesh that is below minimum volume
            Box volumeBounds = new Box(bounds.Position, bounds.Size * source.transform.localScale);
            float volume = volumeBounds.Volume;
            if (volume < MinimumVolume)
            {
                return null;
            }

            if (index == 0)
            {
                var meshes = s_CachedMeshes;
                s_CachedMeshes.Clear();

                // Update mesh filter
                if (source.TryGetComponent(out MeshFilter meshFilter))
                {
                    var mesh = new Mesh();
                    meshFilter.sharedMesh = mesh;
                    meshFilter.sharedMesh.bounds = bounds;
                    meshes.Add(mesh);

                    // Add fill material
                    if (source.TryGetComponent(out MeshRenderer meshRenderer))
                    {
                        meshRenderer.GetSharedMaterials(s_CachedMaterials);
                        if (s_CachedMaterials.Count <= source.Fill.SubMeshIndex && source.Fill.SubMeshIndex < meshData.subMeshCount)
                        {
                            s_CachedMaterials.Add(source.FillMaterial);
                            var array = GetMaterialArray(s_CachedMaterials.Count);
                            s_CachedMaterials.CopyTo(array);
                            meshRenderer.sharedMaterials = array;
                        }
                    }
                }

                // Update mesh collider
                if (source.TryGetComponent(out MeshCollider meshCollider))
                {
                    var mesh = new Mesh();
                    meshCollider.sharedMesh = null;
                    meshCollider.cookingOptions = MeshColliderCookingOptions;
                    meshes.Add(mesh);
                }
                else if (source.TryGetComponent(out BoxCollider boxCollider))
                {
                    meshCollider = source.gameObject.AddComponent<MeshCollider>();

                    var mesh = new Mesh();
                    meshCollider.sharedMesh = null;
                    meshCollider.convex = true;
                    meshCollider.sharedMaterial = boxCollider.sharedMaterial;
                    meshCollider.cookingOptions = MeshColliderCookingOptions;
                    meshes.Add(mesh);

                    Destroy(boxCollider);
                }
                else if (source.TryGetComponent(out SphereCollider sphereCollider))
                {
                    meshCollider = source.gameObject.AddComponent<MeshCollider>();

                    var mesh = new Mesh();
                    meshCollider.sharedMesh = null;
                    meshCollider.convex = true;
                    meshCollider.sharedMaterial = sphereCollider.sharedMaterial;
                    meshCollider.cookingOptions = MeshColliderCookingOptions;
                    meshes.Add(mesh);

                    Destroy(sphereCollider);
                }
                else if (source.TryGetComponent(out CapsuleCollider capsuleCollider))
                {
                    meshCollider = source.gameObject.AddComponent<MeshCollider>();

                    var mesh = new Mesh();
                    meshCollider.sharedMesh = null;
                    meshCollider.convex = true;
                    meshCollider.sharedMaterial = capsuleCollider.sharedMaterial;
                    meshCollider.cookingOptions = MeshColliderCookingOptions;
                    meshes.Add(mesh);

                    Destroy(capsuleCollider);
                }

                // Update mesh
                Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, meshes);

                if (meshCollider != null)
                    meshCollider.sharedMesh = meshes[1];

                return source;
            }
            else
            {
                // Create piece
                var copy = Instantiate(source);

                var meshes = s_CachedMeshes;
                s_CachedMeshes.Clear();

                // Update mesh filter
                if (copy.TryGetComponent(out MeshFilter meshFilter))
                {
                    var mesh = new Mesh();
                    meshFilter.sharedMesh = mesh;
                    meshFilter.sharedMesh.bounds = bounds;
                    meshes.Add(mesh);

                    // Add fill material
                    if (copy.TryGetComponent(out MeshRenderer meshRenderer))
                    {
                        meshRenderer.GetSharedMaterials(s_CachedMaterials);
                        if (s_CachedMaterials.Count <= source.Fill.SubMeshIndex && source.Fill.SubMeshIndex < meshData.subMeshCount)
                        {
                            s_CachedMaterials.Add(source.FillMaterial);
                            var array = GetMaterialArray(s_CachedMaterials.Count);
                            s_CachedMaterials.CopyTo(array);
                            meshRenderer.sharedMaterials = array;
                        }
                    }
                }

                // Update mesh collider
                if (copy.TryGetComponent(out MeshCollider meshCollider))
                {
                    var mesh = new Mesh();
                    meshCollider.sharedMesh = null;
                    meshCollider.cookingOptions = MeshColliderCookingOptions;
                    meshes.Add(mesh);
                }
                if (copy.TryGetComponent(out BoxCollider boxCollider))
                    Destroy(boxCollider);
                if (copy.TryGetComponent(out SphereCollider sphereCollider))
                    Destroy(sphereCollider);
                if (copy.TryGetComponent(out CapsuleCollider capsuleCollider))
                    Destroy(capsuleCollider);

                // Copy rigid body velocities
                if (copy.TryGetComponent(out Rigidbody copyRigidBody) && source.TryGetComponent(out Rigidbody rigidbody))
                {
                    copyRigidBody.velocity = rigidbody.velocity;
                    copyRigidBody.angularVelocity = rigidbody.angularVelocity;
                }

                // Update mesh
                Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, meshes);

                if (meshCollider != null)
                    meshCollider.sharedMesh = meshes[1];

                return copy;
            }
        }

        static Material[] GetMaterialArray(int length)
        {
            if (!s_CachedMaterialArrays.TryGetValue(length, out Material[] array))
            {
                array = new Material[length];
                s_CachedMaterialArrays.Add(length, array);
            }
            return array;
        }

        static Box GetBounds(in Mesh.MeshData meshData)
        {
            Box bounds = meshData.GetSubMesh(0).bounds;
            for (int subMeshIndex = 1; subMeshIndex < meshData.subMeshCount; ++subMeshIndex)
            {
                SubMeshDescriptor subMesh = meshData.GetSubMesh(subMeshIndex);
                Box subMeshBounds = subMesh.bounds;
                bounds = Box.Union(bounds, subMeshBounds);
            }
            return bounds;
        }
    }
}