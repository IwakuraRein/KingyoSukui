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
        private GameObject leftPoi;
        [SerializeField]
        private GameObject rightPoi;
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
                leftPoi.SetActive(true);
                PoiOnLeft = true;
            }
            else
            {
                currentRightGrabbing = p;
                rightPoi.SetActive(true);
                PoiOnRight = true;
            }
        }
        public void OnPoiReleased(PoiGrabbableProxy p)
        {
            if (currentRightGrabbing == p)
            {
                currentRightGrabbing = null;
                rightPoi.SetActive(false);
                PoiOnRight = false;
            }
            if (currentLeftGrabbing == p)
            {
                currentLeftGrabbing = null;
                leftPoi.SetActive(false);
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
        }
    }
}