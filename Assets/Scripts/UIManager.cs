using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
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
        // Start is called before the first frame update
        private Bowl bowl;
        private float startingTime;

        private Dictionary<int, float> levelTimeLimits = new Dictionary<int, float>()
        {
            { 1, 200f }, // Level 1 has a time limit of 200 seconds
            { 2, 150f }, // Level 2 has a time limit of 150 seconds
            { 3, 100f } // Level 3 has a time limit of 100 seconds
            // Add more levels and their time limits as needed
        };
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
            startLevel(SceneManager.GetActiveScene().buildIndex);
        }

        // Update is called once per frame
        void Update()
        {
            if (bowl != null)
            {
                UpdateScore(bowl.GetComponent<Bowl>().GetScore());
            } else {
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
        }

        void startLevel(int level)
        {
            levelText.text = "Level: " + level;
            startingTime = level == 0? levelTimeLimits[1] : levelTimeLimits[level];
        }


    }
}
