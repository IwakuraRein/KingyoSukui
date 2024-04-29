using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    public class PoiGrabbableProxy : MonoBehaviour
    {
        [SerializeField]
        HandGrabInteractable rightInteractable;
        [SerializeField]
        HandGrabInteractable leftInteractable;
        [SerializeField]
        Renderer[] renders;
        [SerializeField]
        GameObject poiSurface;
        public bool isBroken = false;
        public bool IsGrabbing { get; private set; }
        public void FixedUpdate()
        {
            foreach (var interactor in rightInteractable.Interactors)
            {
                if (interactor.IsGrabbing)
                {
                    if (
                    interactor.Hand.Handedness == Oculus.Interaction.Input.Handedness.Right && !GameManager.Instance.PoiOnRight)
                    {
                        GameManager.Instance.OnPoiGetGrabbed(this, false);
                        foreach (var r in renders)
                        {
                            r.enabled = false;
                        }
                        Debug.Log($"{this} is grabbed by right hand {interactor.Hand}!");
                    }
                }
            }
            foreach (var interactor in leftInteractable.Interactors)
            {
                if (interactor.IsGrabbing)
                {
                    if (
                    interactor.Hand.Handedness == Oculus.Interaction.Input.Handedness.Left && !GameManager.Instance.PoiOnLeft)
                    {
                        GameManager.Instance.OnPoiGetGrabbed(this, true);
                        foreach (var r in renders)
                        {
                            r.enabled = false;
                        }
                        Debug.Log($"{this} is grabbed by left hand {interactor.Hand}!");
                    }
                }
            }



            if (GameManager.Instance.currentRightGrabbing == this)
            {
                bool isGrabbing = false;
                foreach (var interactor in rightInteractable.Interactors)
                {
                    if (interactor.IsGrabbing)
                    {
                        isGrabbing = true;
                        break;
                    }
                }
                if (!isGrabbing)
                {
                    GameManager.Instance.OnPoiReleased(this);
                    foreach (var r in renders)
                    {
                        r.enabled = true;
                    }
                    Debug.Log($"{this} is released!");
                }
            }
            if (GameManager.Instance.currentLeftGrabbing == this)
            {
                bool isGrabbing = false;
                foreach (var interactor in leftInteractable.Interactors)
                {
                    if (interactor.IsGrabbing)
                    {
                        isGrabbing = true;
                        break;
                    }
                }
                if (!isGrabbing)
                {
                    GameManager.Instance.OnPoiReleased(this);
                    foreach (var r in renders)
                    {
                        r.enabled = true;
                    }
                    Debug.Log($"{this} is released!");
                }
            }
        }
        public void BreakNet()
        {
            poiSurface.SetActive(false);
            isBroken = true;
        }
    }
}