using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.VFX;

public class Cookable : MonoBehaviour
{
    // Start is called before the first frame update

    public float cookTime = 0;
    public float doneTime = 7f;

    [HideInInspector]
    public int score ;
    bool isCooked = false;
  //  GameObject beforeCooked;
    GameObject afterCooked;
    GameObject smokeEffect;
    GameObject overCooked;
    GameObject overSmoke;


    float cookedTime = 8f;
    float overCookedTime = 18f;
    void Start()
    {
        afterCooked  = transform.GetChild(0).gameObject;
        smokeEffect  = transform.GetChild(1).gameObject;
        overCooked   = transform.GetChild(2).gameObject;
        overSmoke    = transform.GetChild(3).gameObject;
        score = 5;
    }

    // Update is called once per frame
    private void Update()
    {
        if (cookTime> cookedTime && cookTime<=overCookedTime) {
            GameObject.Find("FirePlace").GetComponent<AudioSource>().Play() ;

            transform.GetComponent<MeshRenderer>().enabled = false;

            afterCooked.SetActive(true);

            smokeEffect.SetActive(true);

            isCooked = true;

            score = 20;
        }
        if (cookTime > overCookedTime) {
            afterCooked.SetActive(false);

            overCooked.SetActive(true);

            overSmoke.SetActive(true);

            score = 5;

            listBoard.Instance.setToggleTrue(5);
        }

        if (cookTime > overCookedTime + 4f)
        {
            overSmoke.SetActive(false);

        }
        else {
            GameObject.Find("FirePlace").GetComponent<AudioSource>().Pause();


        }
    }



}
