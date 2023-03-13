using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DrawSliceLine : MonoBehaviour
{
    public Color Color = Color.red;

    float3 m_Start;

    Material m_Material;

    void Start()
    {
        Camera.onPostRender += Draw;
    }

    void OnDestroy()
    {
        Camera.onPostRender -= Draw;
    }

    // Personally I do no recommend using this function as there is chance it wont work with SRP
    // I made it for this samples just have simple solution to show slice
    void Draw(Camera camera)
    {
        if (Input.GetMouseButton(0))
        {
            if (!m_Material)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things. In this case, we just want to use
                // a blend mode that inverts destination colors.
                var shader = Shader.Find("Hidden/Internal-Colored");
                m_Material = new Material(shader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
                // Turn off backface culling, depth writes, depth test.
                m_Material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                m_Material.SetInt("_ZWrite", 0);
                m_Material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                m_Material.SetColor("_Color", Color);
            }

            GL.PushMatrix();
            GL.LoadOrtho();

            float3 end = Camera.main.ScreenToViewportPoint(new float3(Input.mousePosition.x, Input.mousePosition.y, 15));
            float3 start = Camera.main.ScreenToViewportPoint(new float3(m_Start.x, m_Start.y, 15));

            // activate the first shader pass (in this case we know it is the only pass)
            m_Material.SetPass(0);
            // draw a quad over whole screen
            GL.Begin(GL.LINES);
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            GL.End();

            GL.PopMatrix();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_Start = Input.mousePosition;
        }
    }
}
