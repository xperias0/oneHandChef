using ProjectDawn.MeshSlicer;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;

using Plane = ProjectDawn.Geometry3D.Plane;
using UnityEngine.UI;

public class Blade : MonoBehaviour
{
    HashSet<MeshSlicer> m_Sliced = new HashSet<MeshSlicer>();
    MeshSlicerManager m_Manager;
    float timer;
    bool isSliced = false;
    void Start()
    {
        m_Manager = MeshSlicerManager.GetOrCreateManager();
    }


    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0.2f)
        {
            isSliced = false;
        }
        else {
            isSliced = true;
        }
     
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


            if (other.name.Length >= 6 && other.name.Substring(0,6).Equals("Potato")) {
                GameObject.Find("third").transform.GetChild(0).GetComponent<Toggle>().isOn = true;

            }
           

            if (isSliced) {
                GetComponent<AudioSource>().Play();
                isSliced = true;
                timer = 0.5f;
            }
            
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
