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
            if (interactable.Interactors.Count != 0)
            {
                GameManager.Instance.OnPoiGetGrabbed(this);
                foreach (var r in render)
                {
                    r.enabled = false;
                }
                Debug.Log($"{this} is grabbed!");
            };
        }
    }
}