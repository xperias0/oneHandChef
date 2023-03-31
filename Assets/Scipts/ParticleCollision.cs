using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle on :"+other.name);
    }
}
