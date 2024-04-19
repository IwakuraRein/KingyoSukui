using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

namespace Kingyo
{
    public class Poi : MonoBehaviour
    {
        public UltEvent<Poi, Fish> OnFishEnterPoi = new UltEvent<Poi, Fish>();
        public UltEvent<Poi, Fish> OnFishExitPoi = new UltEvent<Poi, Fish>();
        void Start()
        {

        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hands"))
            {
                if (!GameManager.Instance.hasPoiOnHand)
                {
                    if (other.gameObject.transform.name == "RightHandAnchor" && !GameManager.Instance.rightHandOnUse)
                    {
                        GameManager.Instance.rightHandOnUse = true;
                        GameManager.Instance.PoiOnRight = true;
                    }
                    else if (other.gameObject.transform.name == "LeftHandAnchor" && !GameManager.Instance.leftHandOnUse)
                    {
                        GameManager.Instance.leftHandOnUse = true;
                        GameManager.Instance.PoiOnLeft = true;
                    }
                    GameManager.Instance.setCurrentPoi(this.gameObject);
                    this.transform.parent = other.gameObject.transform;
                    GameManager.Instance.hasPoiOnHand = true;
                }
            }
            if (other.gameObject.tag == "Fish")
            {
                var fish = other.attachedRigidbody.gameObject.GetComponent<Fish>();
                if (fish != null)
                {
                    if (!fish.IsInPoi)
                    {
                        fish.IsInPoi = true;
                        OnFishEnterPoi?.Invoke(this, fish);
                        Debug.Log($"{fish} is on the poi!");
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Fish")
            {
                var fish = other.attachedRigidbody.gameObject.GetComponent<Fish>();
                if (fish != null)
                {
                    if (fish.IsInPoi)
                    {
                        fish.IsInPoi = false;
                        OnFishExitPoi?.Invoke(this, fish);
                        Debug.Log($"{fish} leaves the poi!");
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}