using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePasser : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public float score = 0;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
   
}
