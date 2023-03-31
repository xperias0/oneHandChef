using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSliceDetector : MonoBehaviour
{

    Vector3 lastFramePos = Vector3.zero;
    [SerializeField] Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update

    bool one = false;
    bool two = false;
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.GetComponent<Item>()!=null) {
            Item iComp = other.GetComponent<Item>();
            if (!iComp.isSliced) {
                GameObject.Find("Canvas").GetComponent<ScoreManager>().addScore(5);
                iComp.isSliced = true;
                listBoard.Instance.setToggleTrue(6);
                one = true;
            }
            if (iComp.isCooked) {
                GameObject.Find("Canvas").GetComponent<ScoreManager>().addScore(10);
                two = true;
            }
          //  other.transform.parent = transform;
        }

        if (one && two) {
            listBoard.Instance.setToggleTrue(7);
        }

        if (other.tag =="cookable") {
            Cookable c = other.GetComponent<Cookable>();
            GameObject.Find("Canvas").GetComponent<ScoreManager>().addScore(c.score);
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
