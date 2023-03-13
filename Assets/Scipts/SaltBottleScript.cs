using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltBottleScript : MonoBehaviour
{
    public GameObject saltPrefab;
    public GameObject socketObject;
    public float dropAmount = 10f;
    public float dropSpeed = 2f;
    public float particleSpeed = 1f;
    public float particleVariance = 0.1f;

    private bool isDropping = false;
    private float currentTilt = 0.0f;
    private bool isStarted = false;

    private Rigidbody rigid;
    
    public float rotationSpeed = 10.0f;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // float horizontalInput = Input.GetAxis("Horizontal");
        // rigidbody.AddTorque(transform.forward * horizontalInput * rotationSpeed);
        
        currentTilt = Mathf.Abs(transform.rotation.eulerAngles.z);

        if (currentTilt > 100.0f && currentTilt < 250.0f)
        {
            isDropping = true;
        }
        else
        {
            isDropping = false;
        }

        if (isDropping && !isStarted)
        {
            StartCoroutine(DropSalt());
        }
    }
    
    IEnumerator DropSalt()
    {
        isStarted = true;
        
        float angle;// = currentTilt * Mathf.Deg2Rad;
        // // calculate angle
        // print(currentTilt);
        if (currentTilt > 180.0f)
        {
            angle = (currentTilt - 180f);// * Mathf.Deg2Rad;
        }
        else
        {
            angle = (180f - currentTilt);// * Mathf.Deg2Rad;
        }
        print(angle);
        float drop = Mathf.Floor(Mathf.Lerp(dropAmount, 0.0f,angle / 90.0f));
        print(drop);
        // float particleSpeed = Mathf.Sqrt(Mathf.Sin(angle) * dropSpeed);
        
        // calculate x and y velocity
        // float xVelocity = Mathf.Sin(angle) * particleSpeed;
        // float yVelocity = Mathf.Cos(angle) * particleSpeed;
        
        Vector3 dir = (socketObject.transform.position - transform.position).normalized;
        Vector3 v = dir * particleSpeed;
        v.y /= 2;

        for (int i = 0; i < drop; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-particleVariance, particleVariance), Random.Range(-particleVariance, particleVariance), Random.Range(-particleVariance, particleVariance));
            GameObject salt = Instantiate(saltPrefab, socketObject.transform.position + offset, Quaternion.identity);
            salt.transform.parent = GameObject.Find("SaltParent").transform;
            salt.GetComponent<Rigidbody>().velocity = v;

            yield return new WaitForSeconds(0.1f);
        }

        isStarted = false;
    }
}
