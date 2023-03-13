using ProjectDawn.MeshSlicer;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Plane = ProjectDawn.Geometry3D.Plane;
using static ProjectDawn.Mathematics.math2;
using ProjectDawn.Geometry3D;

public class FruitSlicer : MonoBehaviour
{
    public float SeparationForce = 1.0f;
    public bool CheckSliceBounds = true;

    MeshSlicerManager m_MeshSlicerManager;
    float3 m_Start;

    void Awake()
    {
        m_MeshSlicerManager = MeshSlicerManager.GetOrCreateManager();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_Start = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            float3 end = Camera.main.ScreenToWorldPoint(new float3(Input.mousePosition.x, Input.mousePosition.y, 15));
            float3 start = Camera.main.ScreenToWorldPoint(new float3(m_Start.x, m_Start.y, 15));

            Box sliceBounds = new Box(math.min(start, end), math.abs(start - end));
            sliceBounds.Position.z -= 5;
            sliceBounds.Size.z += 5;

            float3 direction = math.normalize(end - start);
            float3 forward = math.normalize(start - (float3)Camera.main.transform.position);
            float3 normal = math.cross(direction, forward);

            Plane plane = new Plane(normal, start);

            // It is recommended that in game you would not use this function as it generates garbage and also quite slow
            // It is here just for simplicity reasons
            var fruits = FindObjectsOfType<Fruit>();

            foreach (var fruit in fruits)
            {
                Box bounds = fruit.MeshRenderer.bounds;

                // Check if slice bounds overlap with fruit bounds
                if (CheckSliceBounds && !bounds.Overlap(sliceBounds))
                    continue;

                // Check if slice plane overlap with fruit bounds
                if (!bounds.Overlap(plane))
                    continue;

                m_MeshSlicerManager.Slice(fruit.MeshSlicer, plane, (result) =>
                {
                    if (result.Slicer.TryGetComponent(out Rigidbody rigidBody))
                    {
                        float3 direction = result.Side == SlicerSide.Outside ? result.Plane.Normal : -result.Plane.Normal;
                        rigidBody.velocity += (Vector3)(direction * SeparationForce);
                    }
                });
            }
        }
    }
}
