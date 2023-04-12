
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Grab : MonoBehaviour
{
    // Start is called before the first frame update

    private static Grab m_instance = null;

    Vector3 lastHandPos;

    GameObject hand;

    public float min = 0.01f;

    public float max = 2f;

    float target = 0;

    public float turnSpeed;

    public float sphereRadius;

    bool isMatAdded = false;

    Dictionary<GameObject, List<Material>> objAndMats;

    bool isRecovered = false;

    GameObject touchedObj = null;

    Collider[] colliders;

    Vector3 sphereColPos;

    List<GameObject> touchedSphereObjs;

    HandController hc;


    [HideInInspector]
    public bool isGrabT = false;

    Material mat;

    GameObject firstObj;

    Vector3 RotPos = Vector3.zero;

    bool isGrabRot = false;

    float throwTime = 0;

    public float throwSpeed;

    float minDis = 0.2f;



    public static Grab Instance
    {
        get
        {
            return m_instance;
        }
    }
    private void Awake()
    {
        m_instance = this;
    }

    void Start()
    {
        hand = GameObject.Find("WhiteHand");

        mat = GameObject.Find("mat").GetComponent<Renderer>().material;

        firstObj = null;

        target = max;

        lastHandPos = transform.position;

        objAndMats = new Dictionary<GameObject, List<Material>>();

        touchedSphereObjs = new List<GameObject>();

        hc = GameObject.Find("WhiteHand").GetComponent<HandController>();
    }
    private void Update()
    {
        sphereColPos = transform.position;


        colliders = Physics.OverlapSphere(sphereColPos, sphereRadius, 1 << 7);

        //  overlapDetector();

        grabDetected();

        distanceDetector();
    }
    // Update is called once per frame

    void distanceDetector() {
        Vector3 direction = ( transform.position - lastHandPos).normalized;

        lastHandPos = transform.position;

        Ray ray = new Ray(transform.position,direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.6f, 1 << 0)) {

            float dis = Vector3.Distance(hit.point , transform.position);

            float num = (0.4f / dis) * Time.deltaTime * 0.7f;
            HandController.Instance.moveValue -= num;

      

          //  Debug.DrawLine(transform.position,hit.point,Color.blue);
        }
        Debug.DrawRay(transform.position,direction*2f,Color.red);
    }


    void grabDetected() {

        bool isLeft = HandController.Instance.isLeftGrab;
        bool isBoth = HandController.Instance.isGrab;

        if (colliders.Length != 0)
        {

            foreach (Collider c in colliders)
            {
                addMaterials(c.gameObject);

                if (!touchedSphereObjs.Contains(c.gameObject)) {
                    touchedSphereObjs.Add(c.gameObject);
                }
            }
            matGlow();

            if (isLeft)
            {

                GameObject obj = touchedSphereObjs[0];
                isGrabT = true;

                Vector3 dir = Camera.main.transform.forward;

                if (Input.GetButtonDown("ControllerA")) {
                
                
                }


            //    throwLine(obj,dir);

                setObject(obj);
                resetMats();

            }
            else if (isBoth)
            {

                foreach (GameObject obj in touchedSphereObjs)
                {
                    setObject(obj);
                }

                for (int i=0;i< touchedSphereObjs.Count;i++)
                {
                    GameObject b = touchedSphereObjs[i];
                    bool isIn = false;
                    for (int j = 0; j < colliders.Length; j++)
                    {
                        GameObject cur = colliders[j].gameObject;
                        if (b == cur)
                        {
                            isIn = true;
                        }

                    }

                    if (!isIn)
                    {
                        resetObject(b);
                        touchedSphereObjs.Remove(b);
                        isIn = false;
                    }
                }


                isGrabT = true;
                resetMats();
            }
            else {

                isGrabRot = false;
                foreach (GameObject obj in touchedSphereObjs)
                {
                    resetObject(obj);
                }
                isGrabT = false;
            
                touchedSphereObjs.Clear();
            }
          

        }
        else
        {
            resetMats();
            touchedSphereObjs.Clear();
        }


    }


    void setObject(GameObject obj) {
        string tag = obj.tag;

            switch (tag) {
            case "RotObj":
                if (!isGrabRot)
                {
                    RotPos = transform.position;
                    isGrabRot = true;
                }

                Vector3 direction = (transform.position - RotPos).normalized;
                float angleOfrot = direction.z + direction.x;
                angleOfrot = Mathf.Clamp(angleOfrot, -20f, 180f);
                obj.transform.RotateAround(obj.transform.position, obj.transform.up, angleOfrot);
                break;

            default:
                if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {

                    rb.isKinematic = true;
                    rb.freezeRotation = true;
                    rb.useGravity = false;
                    obj.transform.parent = transform;
                }
                   break;

               

            }

        
    
    }


    void resetObject(GameObject obj) {
        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.freezeRotation = false;
            rb.useGravity = true;
            obj.transform.parent = null; 
        }

    }

   


    void addMaterials(GameObject obj) {
        
        rebuildMaterialsArray(obj);
        for (int i=0;i<obj.transform.childCount;i++) {
            GameObject cur = obj.transform.GetChild(i).gameObject;
            string tag = obj.tag;

            if (!tag.Equals("particle")) {
                rebuildMaterialsArray(cur);
            }
           
        }
        isMatAdded = true;
    }

    void rebuildMaterialsArray(GameObject obj) {
        
        if (obj.GetComponent<Renderer>() && !objAndMats.ContainsKey(obj)) {
            List<Material> list = new List<Material>();
            list.Add(mat);
            foreach (Material m in obj.GetComponent<Renderer>().materials)
            {
                list.Add(m);
            }
            obj.GetComponent<Renderer>().materials = list.ToArray();
            objAndMats.Add(obj, list);
        }

        
    }

    void matGlow()
    {
        if (isMatAdded)
        {
            foreach (GameObject b in objAndMats.Keys)
            {

                Material curMat = b.GetComponent<Renderer>().material;

                string s = curMat.name.Substring(0, 4);
                if (s == "glow")
                {
                    float indes = Mathf.Lerp(curMat.GetFloat("_Emiss"), target, Time.deltaTime * turnSpeed);
                    if (Mathf.Abs(target - curMat.GetFloat("_Emiss")) < 0.02f)
                    {
                        target = target == max ? min : max;
                    }
                    curMat.SetFloat("_Emiss", indes);
                }
            }


        }


    }
    void resetMat(GameObject obj)
    {
        if (isMatAdded)
        {
            if (obj.transform.childCount == 0)
            {
                foreach (GameObject b in objAndMats.Keys)
                {
                    List<Material> curList = objAndMats[b];
                    if (curList.Count > 1)
                    {
                        objAndMats[b].RemoveAt(0);
                    }

                }

                obj.GetComponent<Renderer>().materials = objAndMats[obj].ToArray();
            }

            else
            {
                foreach (GameObject b in objAndMats.Keys)
                {
                    List<Material> curList = objAndMats[b];
                    if (curList.Count > 1)
                    {
                        objAndMats[b].RemoveAt(0);
                    }
                    b.GetComponent<Renderer>().materials = objAndMats[b].ToArray();
                }
            }

            //   objAndMats.Clear();
            isRecovered = true;
            isMatAdded = false;
            // Debug.Log("Clear");
        }


    }



    void resetMats()
    {
        foreach (GameObject b in objAndMats.Keys)
        {
            objAndMats[b].RemoveAt(0);
            b.GetComponent<Renderer>().materials = objAndMats[b].ToArray();
        }
        isRecovered = false;
        isMatAdded = false;
        isGrabRot = true;
        // touchedSphereObjs.Clear();
        objAndMats.Clear();

    }

    public void throwLine(GameObject b, Vector3 direction)
    {
        GetComponent<LineRenderer>().enabled = true;
        float timer = 0.1f;
        LineRenderer lr = GetComponent<LineRenderer>();

        for (int i = 0; i < lr.positionCount; i++)
        {
            float curTime = i * timer;
            Vector3 pos = transform.position + direction * curTime + 0.5f * -9.8f * Vector3.up * (curTime * curTime);
            lr.SetPosition(i, pos);

        }
        //   b.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);

    }
}
