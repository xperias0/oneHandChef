using com.zibra.liquid.Manipulators;
using com.zibra.liquid.Solver;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

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
        if (!isTri && Vector3.Distance(hand.transform.position, transform.position) < 1f && hand.GetComponent<HandController>().isLeftGrab )
        {
            ButtonSwitch();

            isTri = true;
        }
        if (isTri) {
            timer += Time.deltaTime;
           
            if (timer>2f) {
                timer = 0;
                isTri = false;
            }
        }
       // Debug.Log("TImer  " + timer);
    }

    void ButtonSwitch() {
        if (!isOpen)
        {
            particle.GetComponent<ParticleSystem>().Play();
            particle.GetComponent<AudioSource>().Play();
            Debug.Log("yes");
            if (water) {
                waterOpen.GetComponent<ZibraLiquidEmitter>().enabled = true;
            }
            isOpen = true;
        }
        else {
            particle.GetComponent<ParticleSystem>().Stop();
            particle.GetComponent<AudioSource>().Pause();
            Debug.Log("yes:no");
            if (water)
            {
                waterOpen.GetComponent<ZibraLiquidEmitter>().enabled = false;
            }
            isOpen = false;
        }
    
    }
   





}
