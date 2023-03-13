using ProjectDawn.MeshSlicer;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Plane = ProjectDawn.Geometry3D.Plane;

public class Blade : MonoBehaviour
{
    HashSet<MeshSlicer> m_Sliced = new HashSet<MeshSlicer>();
    MeshSlicerManager m_Manager;

    void Start()
    {
        m_Manager = MeshSlicerManager.GetOrCreateManager();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out MeshSlicer slicer) && !m_Sliced.Contains(slicer))
        {
            var position = transform.position;
            var normal = math.mul(transform.rotation, new float3(0, 0, 1));
            var plane = new Plane(normal, position);

            m_Manager.Slice(slicer, plane, (result) =>
            {
                m_Sliced.Add(result.Slicer);
            });
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out MeshSlicer slicer) && m_Sliced.Contains(slicer))
        {
            m_Sliced.Remove(slicer);
        }
    }
}
