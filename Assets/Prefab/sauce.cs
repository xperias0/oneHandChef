using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sauce : MonoBehaviour
{
    public ParticleSystem system;
    // Start is called before the first frame update
    void Start()
    {
        system.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(gameObject.transform.rotation.z) >0.5|| Mathf.Abs(gameObject.transform.rotation.y) < 0.5)
        {

            system.Play(true);
            Debug.Log(gameObject.transform.rotation.z);
        }
        else
        {
            system.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Debug.Log(gameObject.transform.rotation.z);
        }
    }
}
