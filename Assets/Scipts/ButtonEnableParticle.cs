    using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class ButtonEnableParticle : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public bool isOpen = false;
    public GameObject particle;
    GameObject b;
    bool isTri = false;

    public float maxX;
    public float minX;
    public float maxZ;
    public float minZ;
    // Update is called once per frame

    private void Start()
    {
        b = GameObject.Find("FirePlace")     ;
    }


    private void OnTriggerStay(Collider other)
    {
       // Debug.Log("isOpen: " + isOpen);

        if (GameObject.Find("WhiteHand").GetComponent<HandController>().isLeftGrab && !isTri)
        {
            if (!isOpen)
            {
                //  particle.AddComponent<ParticleSystem>();
                GameObject newPari = Instantiate(particle);
                newPari.transform.parent = b.transform;
                newPari.transform.position = b.transform.position;
                newPari.GetComponent<ParticleSystem>().Play();
                newPari.GetComponent<AudioSource>().Play();

                isOpen = true;
            }
            else
            {
                if (b.transform.childCount != 0)
                {
                    Destroy(b.transform.GetChild(0).gameObject);
                    isOpen = false;
                }

            }
            isTri = true;
            Debug.Log("isOpen");
        }


        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("HandEnter");
        }

    }


    private void OnTriggerExit(Collider other)
    {
        isTri = false;
    }




    void positionDetector() {
        float x = transform.position.x;
        float z = transform.position.z;
        if (transform.position.x>maxX || transform.position.x<minX) {

            x = Mathf.Clamp(x, minX, maxX);
        }

        if (transform.position.z > maxZ || transform.position.z < minZ)
        {

            z = Mathf.Clamp(x, minZ, maxZ);
        }  
        

        Vector3 newPos = new Vector3(x,transform.position.y,z);

        transform.position = newPos;
    
    }
}
