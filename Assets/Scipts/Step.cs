using UnityEngine;
using UnityEngine.UI;

public class Step : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject Guildlines;
    public float menuSpeed = 5.0f;
    public float menuTargetY = 3.0f;
    public float menuStartPosition = 1000.0f;



    void Update()
    {
        if (Input.GetKey(KeyCode.JoystickButton6))
        {
            // move pause menu towards target position
            float newY = Mathf.Lerp(pauseMenuPanel.transform.position.y, menuTargetY + 300, Time.deltaTime * menuSpeed);
            pauseMenuPanel.transform.position = new Vector3(pauseMenuPanel.transform.position.x, newY, pauseMenuPanel.transform.position.z);
            Guildlines.transform.position = new Vector3(pauseMenuPanel.transform.position.x, newY - 20, pauseMenuPanel.transform.position.z); ;

        }
        else
        {
            // move pause menu back to start position
            float newY = Mathf.Lerp(pauseMenuPanel.transform.position.y, menuStartPosition, Time.deltaTime * menuSpeed);
            pauseMenuPanel.transform.position = new Vector3(pauseMenuPanel.transform.position.x, newY, pauseMenuPanel.transform.position.z);
            Guildlines.transform.position = new Vector3(pauseMenuPanel.transform.position.x, newY - 20, pauseMenuPanel.transform.position.z); ;

        }
    }
}