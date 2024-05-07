//using AYellowpaper.SerializedCollections;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UltEvents;

namespace Kingyo
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField]
        internal Poi leftPoi;
        [SerializeField]
        internal Poi rightPoi;
        [SerializeField]
        internal GameObject bowl;
        [SerializeField]
        internal GameObject dplayer;
        [SerializeField]
        PoiGrabbableProxy[] grabbablePois;
        //public PoiGrabbableProxy currentGrabbingPoi { get; private set; }
        public PoiGrabbableProxy currentLeftGrabbing { get; private set; }
        public PoiGrabbableProxy currentRightGrabbing { get; private set; }

        public UltEvent OnLoadNextLevel = new UltEvent();


        //[SerializedDictionary("level", "time")]
        //public SerializableDictionary<int, float> levelTimeLimits = new SerializableDictionary<int, float>()
        public Dictionary<int, float> levelTimeLimits = new Dictionary<int, float>()
        {
            { 1, 200f }, // Level 1 has a time limit of 200 seconds
            { 2, 150f }, // Level 2 has a time limit of 150 seconds
            { 3, 100f } // Level 3 has a time limit of 100 seconds
            // Add more levels and their time limits as needed
        };

        //[SerializedDictionary("level", "goal")]
        //public SerializableDictionary<int, int> levelGoals = new SerializableDictionary<int, int>()
        public Dictionary<int, int> levelGoals = new Dictionary<int, int>()
        {
            { 1, 10 }, // Level 1 has a goal of 100 points
            { 2, 20 }, // Level 2 has a goal of 200 points
            { 3, 30 } // Level 3 has a goal of 300 points
            // Add more levels and their goals as needed
        };
        public Dictionary<int, float> levelSurfaceThreshold = new Dictionary<int, float>()
        {
            { 1, 999f }, // Level 1 has a goal of 100 points
            { 2, 2f }, // Level 2 has a goal of 200 points
            { 3, 2f } // Level 3 has a goal of 300 points
            // Add more levels and their goals as needed
        };

        public int currentLevel { get; set; }

        //public bool hasPoiOnHand { get; private set; } = false;
        //public bool hasBowlOnHand { get; private set; } = false;
        //public bool rightHandOnUse { get; private set; } = false;
        //public bool leftHandOnUse{ get; private set; } = false;
        public bool PoiOnLeft { get; private set; } = false;
        public bool PoiOnRight { get; private set; } = false;
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
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            OnLoadNextLevel += () =>
            {

                //SceneManager.LoadScene(GameManager.Instance.currentLevel + 1);
                //play some winning scenario
                if (currentLevel == SceneManager.sceneCountInBuildSettings - 1)
                {
                    // Load the menu scene if this is the last level
                    Time.timeScale = 0f;
                    SceneManager.UnloadSceneAsync(currentLevel).completed += (AsyncOperation _) => { /*SceneManager.LoadSceneAsync(0);*/ Time.timeScale = 1f; currentLevel = 0; };
                }
                else
                {
                    // Load the next level
                    Time.timeScale = 0f;
                    SceneManager.UnloadSceneAsync(currentLevel).completed += (AsyncOperation _) =>
                    {
                        Time.timeScale = 1f;
                        SceneManager.LoadSceneAsync(++currentLevel, LoadSceneMode.Additive);
                    };
                    leftPoi.net.threshold = levelSurfaceThreshold[currentLevel];
                    rightPoi.net.threshold = levelSurfaceThreshold[currentLevel];
                    leftPoi.gameObject.SetActive(false);
                    rightPoi.gameObject.SetActive(false);
                }
            };
        }
        public void OnPoiGetGrabbed(PoiGrabbableProxy p, bool isLeft)
        {
            if (isLeft && PoiOnLeft) return;
            if (!isLeft && PoiOnRight) return;
            if (isLeft)
            {
                currentLeftGrabbing = p;
                leftPoi.proxy = p;
                leftPoi.gameObject.SetActive(true);
                PoiOnLeft = true;
                leftPoi.OnPoiGetGrabbed?.Invoke();
            }
            else
            {
                currentRightGrabbing = p;
                rightPoi.proxy = p;
                rightPoi.gameObject.SetActive(true);
                PoiOnRight = true;
                rightPoi.OnPoiGetGrabbed?.Invoke();
            }
        }
        public void OnPoiReleased(PoiGrabbableProxy p)
        {
            if (currentRightGrabbing == p)
            {
                currentRightGrabbing = null;
                rightPoi.gameObject.SetActive(false);
                rightPoi.proxy = null;
                PoiOnRight = false;
                rightPoi.OnPoiGetReleased?.Invoke();
            }
            if (currentLeftGrabbing == p)
            {
                currentLeftGrabbing = null;
                leftPoi.gameObject.SetActive(false);
                leftPoi.proxy = null;
                PoiOnLeft = false;
                leftPoi.OnPoiGetReleased?.Invoke();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger));
            // if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5f)
            // {
            //     poi.SetActive(true);
            // } else
            // {
            //     poi.SetActive(false);
            // }
            //if (OVRInput.GetDown(OVRInput.Button.Two))
            //{
            //    if (poi.activeSelf)
            //    {
            //        poi.SetActive(false);
            //    }
            //    else
            //    {
            //        poi.SetActive(true);
            //    }
            //}

            if (currentLeftGrabbing != null && leftPoi.gameObject.activeSelf)
            {
                // Use left hand's joystick to adjust the rotation of the poi
                Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
                if (joystick.x != 0) {
                    float rotationSpeed = 10f;
                    float rotationAmount = joystick.x * rotationSpeed * Time.deltaTime;

                    // Get the current orientation of the controller
                    Quaternion controllerOrientation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);

                    // Convert joystick input into a world rotation
                    Vector3 rotationAxis = new Vector3(joystick.y, -joystick.x, 0); // Inverting x for intuitive horizontal rotation
                    rotationAxis = controllerOrientation * rotationAxis; // Transforming the axis to align with controller orientation

                    leftPoi.transform.Rotate(rotationAxis, rotationAmount, Space.World);
                }
            }
            if (currentRightGrabbing != null && rightPoi.gameObject.activeSelf)
            {
                // Use left hand's joystick to adjust the rotation of the poi
                Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.RTouch);
                if (joystick.x != 0) {
                    Debug.Log(joystick);
                    float rotationSpeed = 10f;
                    float rotationAmount = joystick.x * rotationSpeed * Time.deltaTime;

                    // Get the current orientation of the controller
                    Quaternion controllerOrientation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

                    // Convert joystick input into a world rotation
                    Vector3 rotationAxis = new Vector3(joystick.y, -joystick.x, 0); // Inverting x for intuitive horizontal rotation
                    rotationAxis = controllerOrientation * rotationAxis; // Transforming the axis to align with controller orientation

                    rightPoi.transform.Rotate(rotationAxis, rotationAmount, Space.World);
                }
            }
        }
    }
}