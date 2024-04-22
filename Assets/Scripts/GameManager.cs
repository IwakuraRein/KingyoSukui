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
        private GameObject poi;
        [SerializeField]
        private GameObject bowl;
        [SerializeField]
        private GameObject player;
        [SerializeField]
        PoiGrabbableProxy[] grabbablePois;

        public bool hasPoiOnHand { get; private set; } = false;
        //public bool hasBowlOnHand { get; private set; } = false;
        //public bool rightHandOnUse { get; private set; } = false;
        //public bool leftHandOnUse{ get; private set; } = false;
        //public bool PoiOnLeft { get; private set; } = false;
        //public bool PoiOnRight { get; private set; } = false;
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

        public void setCurrentPoi(GameObject newPoi)
        {
            poi = newPoi;
        }

        public void destroyCurrentPoi()
        {
            hasPoiOnHand = false;
            poi.SetActive(false);
            //Destroy(poi);
            //if (PoiOnRight)
            //{
            //    rightHandOnUse = false;
            //    PoiOnRight = false;
            //} else if (PoiOnLeft)
            //{
            //    leftHandOnUse = false;
            //    PoiOnLeft = false;
            //}
        }
        public void OnPoiGetGrabbed(PoiGrabbableProxy p)
        {
            poi.SetActive(true);
            hasPoiOnHand = true;
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
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                if (poi.activeSelf)
                {
                    poi.SetActive(false);
                }
                else
                {
                    poi.SetActive(true);
                }
            }
        }
    }
}