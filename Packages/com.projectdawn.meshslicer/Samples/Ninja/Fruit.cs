using ProjectDawn.Geometry3D;
using ProjectDawn.MeshSlicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshSlicer))]
[RequireComponent(typeof(MeshRenderer))]
public class Fruit : MonoBehaviour
{
    public float TexCoordScale = 1;

    Rigidbody m_CachedRigidbody;
    public Rigidbody Rigidbody
    {
        get
        {
            if (m_CachedRigidbody == null)
                m_CachedRigidbody = GetComponent<Rigidbody>();
            return m_CachedRigidbody;
        }
    }

    MeshSlicer m_CachedMeshSlicer;
    public MeshSlicer MeshSlicer
    {
        get
        {
            if (m_CachedMeshSlicer == null)
                m_CachedMeshSlicer = GetComponent<MeshSlicer>();
            return m_CachedMeshSlicer;
        }
    }

    MeshRenderer m_CachedMeshRenderer;
    public MeshRenderer MeshRenderer
    {
        get
        {
            if (m_CachedMeshRenderer == null)
                m_CachedMeshRenderer = GetComponent<MeshRenderer>();
            return m_CachedMeshRenderer;
        }
    }

    void OnDrawGizmosSelected()
    {
        ShapeGizmos.DrawWireBox(MeshRenderer.bounds, Color.white);
    }
}
