using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMaterial : MonoBehaviour
{
    private Material material;
    private ParticleSystem particleSystem;
    
    public GameObject particleObject;
    public float rate = 1;
    
    // private float startTime;
    // public float duration = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // Init particle system
        GameObject obj = Instantiate(particleObject, transform.position, Quaternion.identity);
        obj.transform.SetParent(transform);
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        particleSystem = obj.GetComponent<ParticleSystem>();
        
        // Init Material
        material = GetComponent<Renderer>().material;
        
        // startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // float t = (Time.time - startTime) / duration;
        // float value = Mathf.Lerp(1f, 0f, t);

        var particleSystemEmission = particleSystem.emission;
        var rateOverTime = particleSystemEmission.rateOverTime;
        rateOverTime.constant = rate * 5;
        particleSystemEmission.rateOverTime = rateOverTime;
        material.SetFloat("_Smoothness", rate);
    }
}
