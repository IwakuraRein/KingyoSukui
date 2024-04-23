using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    public class PoiGrabbableProxy : MonoBehaviour
    {
        [SerializeField]
        HandGrabInteractable interactable;
        [SerializeField]
        Renderer[] render;
        public bool IsGrabbing { get; private set; }
        public void Update()
        {
            if (!GameManager.Instance.hasPoiOnHand && interactable.Interactors.Count != 0)
            {
                GameManager.Instance.OnPoiGetGrabbed(this);
                foreach (var r in render)
                {
                    r.enabled = false;
                }
                Debug.Log($"{this} is grabbed!");
            };
            if (GameManager.Instance.currentGrabbingPoi == this && interactable.Interactors.Count == 0)
            {
                GameManager.Instance.OnPoiReleased(this);
                foreach (var r in render)
                {
                    r.enabled = true;
                }
                Debug.Log($"{this} is released!");
            };
        }
    }
}