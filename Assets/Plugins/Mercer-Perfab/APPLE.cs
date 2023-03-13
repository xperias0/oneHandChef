using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zibra.liquid.Manipulators;
public class APPLE : MonoBehaviour
{
    float water;
    float waterdel;
    public GameObject water1;
    public GameObject water2;
    // Start is called before the first frame update
    void Start()
    {

      
    }

    // Update is called once per frame
    void Update()
    {
        water = water1.GetComponent<ZibraLiquidDetector>().particlesInside;//detect total number of water inside of a box 
        waterdel = water2.GetComponent<ZibraLiquidVoid>().deletedParticleCountTotal;//detect total number of water being absorb.
        if (waterdel>5000)//detect the number of water apple absorbs, to see if the apple is washed
        {
            Debug.Log("the apple is washed");
        }

    }
}
