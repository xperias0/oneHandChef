using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Radio : MonoBehaviour
{
    public List<AudioClip> sources;

    public AudioSource AudioSource;
    public AudioSource sound_button;
    
    private bool power;

    public GameObject buttonPower;
    public GameObject buttonChannel;
    


    public void changeChannel(int num)
    {
        GetComponent<AudioSource>().clip = sources[num];
        GetComponent<AudioSource>().Play();
        sound_button.Play();

        if (num == 0)
        {
            buttonChannel.transform.localPosition = new Vector3(0, 0.015f, 0);
            Debug.Log("Current Channel: " + sources[num]);
        }

        if (num == 1)
        {
            buttonChannel.transform.localPosition = new Vector3(0, 0.015f, 0);
            Debug.Log("Current Channel: " + sources[num]);
        }

        if (num == 2)
        {
            buttonChannel.transform.localPosition = new Vector3(0, 0.015f, 0);
            Debug.Log("Current Channel: " + sources[num]);
        }

    }
    
    public void Power()
    {
        if (GetComponent<AudioSource>().isPlaying)
        {

            GetComponent<AudioSource>().Stop();
            sound_button.Play();
            buttonPower.transform.localPosition = new Vector3(0, 0.015f, 0);
            Debug.Log("poweroff");
        }
        else
        {

            GetComponent<AudioSource>().Play();
            sound_button.Play();
            buttonPower.transform.localPosition = new Vector3(0f, 0.015f, 0);
            Debug.Log("poweron");
        }
    }

    public void Start()
    {
        GetComponent<AudioSource>().clip = sources[0];
        AudioSource.Play();

    }
}
