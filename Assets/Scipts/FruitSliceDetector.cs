using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSliceDetector : MonoBehaviour
{

    Vector3 lastFramePos = Vector3.zero;
    [SerializeField] Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        Item  iComp = other.GetComponent<Item>();
        if (other.gameObject.layer==7&&! iComp.isSliced) {
            GameObject.Find("Canvas").GetComponent<ScoreManager>().addScore(5);
            iComp.isSliced = true;
          //  other.transform.parent = transform;
        }
    }

    private void FixedUpdate()
    {
        velocity = (transform.position - lastFramePos) / Time.fixedDeltaTime;
        lastFramePos = transform.position;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Grable")
        {
          //  collision.gameObject.GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().velocity / Time.fixedDeltaTime);

            collision.gameObject.GetComponent<Rigidbody>().mass = 3f;
            Debug.Log("plate: " + collision.gameObject.name);
        }

    }
}
