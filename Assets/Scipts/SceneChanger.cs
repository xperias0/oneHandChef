using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update

    public Animator ani;
    // Update is called once per frame
    public void sceneChange() {

        StartCoroutine(loadScene());
    }
    public void quitApp()
    {
        Application.Quit();
    }


    IEnumerator loadScene() {
        ani.SetTrigger("nextScene");
        
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }



}
