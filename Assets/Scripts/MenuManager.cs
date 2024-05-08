using Kingyo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    [SerializeField] internal GameObject menuCanvas;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        //SceneManager.LoadScene(0);
        if (GameManager.Instance.currentLevel != 0)
        {
            Time.timeScale = 0;
            SceneManager.UnloadSceneAsync(GameManager.Instance.currentLevel).completed += (AsyncOperation _) =>
            {
                Time.timeScale = 1f;
                GameManager.Instance.currentLevel = 0;
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(GameManager.Instance.currentLevel));
                menuCanvas.SetActive(true);
            };
        }
    }

    private void Update()
    {
        if (GameManager.Instance.currentLevel == 0)
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                SceneManager.LoadSceneAsync(++GameManager.Instance.currentLevel, LoadSceneMode.Additive).completed += (AsyncOperation _) =>
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(GameManager.Instance.currentLevel));
                    menuCanvas.SetActive(false);
                };
            }
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
            }
        }
    }
}
