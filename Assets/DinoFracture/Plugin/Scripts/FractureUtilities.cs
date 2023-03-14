using UnityEngine;

namespace DinoFracture
{
    static class FractureUtilities
    {
        public static float GetTotalMass(GameObject go)
        {
            FracturedObject fo = go.GetComponent<FracturedObject>();
            if (fo != null)
            {
                return fo.TotalMass;
            }

            Rigidbody rb = go.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                return rb.mass;
            }

            return 0.0f;
        }

        public static float GetThisMass(GameObject go)
        {
            FracturedObject fo = go.GetComponent<FracturedObject>();
            if (fo != null)
            {
                return fo.ThisMass;
            }

            float totalMass = GetTotalMass(go);
            float totalVolume = GetTotalVolume(go);
            float thisVolume = GetThisVolume(go);

            return CalculateMass(totalMass, totalVolume, thisVolume);
        }

        public static float GetThisMass(GameObject go, float totalMass, float totalVolume, float thisVolume)
        {
            FracturedObject fo = go.GetComponent<FracturedObject>();
            if (fo != null)
            {
                return fo.ThisMass;
            }

            return CalculateMass(totalMass, totalVolume, thisVolume);
        }

        private static float CalculateMass(float totalMass, float totalVolume, float thisVolume)
        {
            return totalMass * thisVolume / totalVolume;
        }

        public static float GetTotalVolume(GameObject go)
        {
            FracturedObject fo = go.GetComponent<FracturedObject>();
            if (fo != null)
            {
                return fo.TotalVolume;
            }

            float totalVolume = GetThisVolume(go);
            if (totalVolume == 0.0f)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    totalVolume += GetThisVolume(go.transform.GetChild(i).gameObject);
                }
            }

            return totalVolume;
        }

        public static float GetThisVolume(GameObject go)
        {
            if (go.TryGetComponent<Collider>(out var collider) && collider.enabled)
            {
                if (collider is MeshCollider meshCollider)
                {
                    if (meshCollider.sharedMesh == null)
                    {
                        // We might have converted our mesh collider into a sphere collider
                        if (go.TryGetComponent<BoxCollider>(out var replacedCollider))
                        {
                            var size = replacedCollider.size;
                            return size.x * size.y * size.z;
                        }
                    }
                    else
                    {
                        return CalculateVolume(meshCollider.sharedMesh.bounds);
                    }
                }
                else if (collider is BoxCollider boxCollider)
                {
                    var size = boxCollider.size;
                    return size.x * size.y * size.z;
                }
                else if (collider is SphereCollider sphereCollider)
                {
                    return (4.0f / 3.0f) * Mathf.PI * sphereCollider.radius * sphereCollider.radius * sphereCollider.radius;
                }
                else if (collider is CapsuleCollider capsuleCollider)
                {
                    float radiusSq = capsuleCollider.radius;
                    radiusSq *= radiusSq;

                    float topBottomVolume = (4.0f / 3.0f) * Mathf.PI * radiusSq * capsuleCollider.radius;
                    float middleVolume = Mathf.PI * radiusSq * capsuleCollider.height;

                    return topBottomVolume + middleVolume;
                }
            }
            else
            {
                if (go.TryGetComponent<Renderer>(out var r))
                {
                    return CalculateVolume(r.bounds);
                }
            }

            return 0.0f;
        }

        private static float CalculateVolume(in Bounds bounds)
        {
            Vector3 size = bounds.size;
            return size.x * size.y * size.z;
        }

        public static float GetFracturePieceRelativeSize(GameObject go)
        {
            if (go.TryGetComponent(out FracturedObject fo))
            {
                return fo.GetRelativeSize();
            }

            return GetThisVolume(go) / GetTotalVolume(go);
        }

        public static float GetFracturePieceRelativeMass(GameObject go)
        {
            if (go.TryGetComponent(out FracturedObject fo))
            {
                return fo.GetRelativeMass();
            }

            return GetThisMass(go) / GetTotalMass(go);
        }
    }
}