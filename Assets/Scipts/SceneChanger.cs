using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    public void sceneChange() {

        SceneManager.LoadScene(1);
    }
    public void quitApp()
    {
        Application.Quit();
    }
}
