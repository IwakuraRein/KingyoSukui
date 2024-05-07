using Kingyo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    [SerializeField] private GameObject menuCanvas;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    public void switchLevel(int level)
    {
        SceneManager.LoadScene(level,LoadSceneMode.Additive);
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
        if (GameManager.Instance.currentLevel == 0)
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                switchLevel(++GameManager.Instance.currentLevel);
                menuCanvas.SetActive(false);
            }
#if DEBUG
            else if (OVRInput.GetDown(OVRInput.Button.Four))
            {
                switchLevel(++GameManager.Instance.currentLevel); GameManager.Instance.currentLevel++;
                menuCanvas.SetActive(false);
            }
#endif
            else if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                quitGame();
            }
        }
        else
        {
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                ReturnToMenu();
                GameManager.Instance.currentLevel = 0;
                menuCanvas.SetActive(true);
            }
        }
    }
}
