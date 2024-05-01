using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    public void switchLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            // if (OVRInput.GetDown(OVRInput.Button.Three))
            // {
            //     switchLevel(1);
            // }
            // else if (OVRInput.GetDown(OVRInput.Button.Four))
            // {
            //     switchLevel(2);
            // }
            // else if (OVRInput.GetDown(OVRInput.Button.Two))
            // {
            //     quitGame();
            // }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                ReturnToMenu();
            }
        }
    }
}
