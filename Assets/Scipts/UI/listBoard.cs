using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



    public class listBoard : MonoBehaviour
    {
        // Start is called before the first frame update
        private static listBoard m_instance = null;


        public static listBoard Instance
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
        public void setToggleTrue(int num)
        {
            Transform cur = transform.GetChild(num);

            Transform t = cur.GetChild(0);
            if (t.TryGetComponent<Toggle>(out Toggle toggle))
            {

                if (!toggle.isOn)
                {
                    toggle.isOn = true;
                }

            }

        }
    }


