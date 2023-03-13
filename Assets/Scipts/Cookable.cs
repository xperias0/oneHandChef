using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookable : MonoBehaviour
{
    // Start is called before the first frame update

    public float cookTime = 0;
    public float doneTime = 7f;
  //  GameObject beforeCooked;
    GameObject afterCooked;
    GameObject smokeEffect;
    void Start()
    {
        afterCooked  = transform.GetChild(0).gameObject;
        smokeEffect  = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    private void Update()
    {
        if (cookTime>7f) {
            transform.GetComponent<MeshRenderer>().enabled = false;

            afterCooked.SetActive(true);

            smokeEffect.SetActive(true);
        }
    }
}
