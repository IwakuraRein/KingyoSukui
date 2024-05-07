using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using AYellowpaper.SerializedCollections;

namespace Kingyo
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        [SerializeField]
        private TextMeshProUGUI scoreText;
        [SerializeField]
        private TextMeshProUGUI levelText;
        [SerializeField]
        private TextMeshProUGUI timeText;
        [SerializeField]
        private TextMeshProUGUI goalText;
        // Start is called before the first frame update
        private Bowl bowl;
        private float startingTime;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        void Start()
        {
            bowl = GameObject.Find("bowl").GetComponent<Bowl>();
            //level will equal to current scene number
            startLevel(GameManager.Instance.currentLevel);
        }

        // Update is called once per frame
        void Update()
        {
            if (bowl != null)
            {
                UpdateScore(bowl.GetComponent<Bowl>().GetScore());
            }
            else
            {
                Debug.Log("bool not found");//TODO: Continue Finding it?
            }

            //count the time and shown in the time text
            float elapsedTime = Time.time - startingTime;
            int remainingTime = Mathf.RoundToInt(startingTime - elapsedTime);
            timeText.text = "Time: " + remainingTime;
            if (remainingTime <= 0)
            {//if time is up, show up a message
                //SceneManager.LoadScene(0);
            }
        }

        void UpdateScore(int score)
        {
            scoreText.text = "Score: " + score;
            if (score >= ((GameManager.Instance.currentLevel == 0) ? 10 : GameManager.Instance.levelGoals[GameManager.Instance.currentLevel]))
            {
                GameManager.Instance.OnLoadNextLevel?.Invoke();
            }
        }

        void startLevel(int level)
        {
            levelText.text = "Level: " + level;
            goalText.text = level == 0 ? "Goal: " + GameManager.Instance.levelGoals[1] : "Goal: " + GameManager.Instance.levelGoals[level];
            startingTime = level == 0 ? GameManager.Instance.levelTimeLimits[1] : GameManager.Instance.levelTimeLimits[level];
        }


    }
}
