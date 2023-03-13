using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zibra.liquid.Solver;
namespace com.zibra.liquid.Manipulators
{

    public class sacue : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject emitter;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(gameObject.transform.rotation.x);
            if (Mathf.Abs(gameObject.transform.rotation.x) > 0.7 || Mathf.Abs(gameObject.transform.rotation.z) > 0.7)// detect the rotation to see if the sauce bottle is face down
            {
                emitter.GetComponent<ZibraLiquidEmitter>().VolumePerSimTime = 0.125f;//adjuest the volume come out from the emitter
                
            }
            else
            {
                emitter.GetComponent<ZibraLiquidEmitter>().VolumePerSimTime = 0f;
            }
        }
    }

}

