using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float3 Velocity;
    public float Drag;

    [HideInInspector]
    public bool isWashed = false;
    [HideInInspector]
    public bool isSuaced = false;

    [HideInInspector]
    public bool isSliced = false;
    void Update()
    {
        float3 gravity = 10;

        // Add gravity
        Velocity += gravity * Time.deltaTime;


    }
}
