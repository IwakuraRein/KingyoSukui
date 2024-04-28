using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Kingyo
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField]
        private Poi leftPoi;
        [SerializeField]
        private Poi rightPoi;
        [SerializeField]
        private GameObject bowl;
        [SerializeField]
        private GameObject player;
        [SerializeField]
        PoiGrabbableProxy[] grabbablePois;
        //public PoiGrabbableProxy currentGrabbingPoi { get; private set; }
        public PoiGrabbableProxy currentLeftGrabbing { get; private set; }
        public PoiGrabbableProxy currentRightGrabbing { get; private set; }

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

        }

        public void onPoiNetBreak(Poi poi)
        {
            poi.proxy?.net.BreakNet();
            //if (poi == leftPoi)
            //{
            //    PoiOnLeft = false;
            //    leftPoi.SetActive(false);
            //}
            //if (poi == rightPoi)
            //{
            //    PoiOnRight = false;
            //    rightPoi.SetActive(false);
            //}
        }
        public void OnPoiGetGrabbed(PoiGrabbableProxy p, bool isLeft)
        {
            if (isLeft && PoiOnLeft) return;
            if (!isLeft && PoiOnRight) return;
            if (isLeft)
            {
                currentLeftGrabbing = p;
                leftPoi.gameObject.SetActive(true);
                leftPoi.proxy = p;
                PoiOnLeft = true;
            }
            else
            {
                currentRightGrabbing = p;
                rightPoi.gameObject.SetActive(true);
                rightPoi.proxy = p;
                PoiOnRight = true;
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
            }
            if (currentLeftGrabbing == p)
            {
                currentLeftGrabbing = null;
                leftPoi.gameObject.SetActive(false);
                leftPoi.proxy = null;
                PoiOnLeft = false;
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