using com.zibra.liquid.Manipulators;
using com.zibra.liquid.Solver;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class ButtonEnableParticle : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public bool isOpen = false;
    public GameObject particle;
   // public GameObject b;
    bool isTri = false;
    GameObject hand;
    float timer = 0f;

    public bool water;
    public GameObject waterOpen;
    // Update is called once per frame
    private void Start()
    {
        hand = GameObject.Find("WhiteHand");
        particle.GetComponent<ParticleSystem>().Pause();
    }
    private void Update()
    {
        
        if (isTri)
        {
            timer += Time.deltaTime;

            if (timer > 2f)
            {
                timer = 0;
                isTri = false;
            }
        }
    }

    void ButtonSwitch() {
        if (!isOpen )
        {
            particle.GetComponent<ParticleSystem>().Play();
            particle.GetComponent<AudioSource>().Play();
           // Debug.Log("yes");
            if (water) {
                waterOpen.GetComponent<ZibraLiquidEmitter>().enabled = true;
            }
            isOpen = true;
        }
        else {
            particle.GetComponent<ParticleSystem>().Stop();
            particle.GetComponent<AudioSource>().Pause();
           // Debug.Log("yes:no");
            if (water)
            {
                waterOpen.GetComponent<ZibraLiquidEmitter>().enabled = false;
            }
            isOpen = false;
        }
    
    }


    private void OnTriggerStay(Collider other)
    {
       

        if (!isTri && other.tag == "Player" && HandController.Instance.isLeftGrab && Grab.Instance.isGrabT == false) {
           
            ButtonSwitch();
            isTri = true;
        }
    }


    


}
