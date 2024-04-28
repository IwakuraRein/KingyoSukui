using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

namespace Kingyo
{
    public class Poi : MonoBehaviour
    {
        [SerializeField]
        Renderer render;
        [SerializeField]
        float timeGetWet = 1f;
        [SerializeField]
        float timeGetDry = 3f;
        [SerializeField]
        Net net;

        public PoiGrabbableProxy proxy;

        bool isInWater = false;
        Material mat;
        public bool IsInWater { get => isInWater; set => isInWater = value; }

        public UltEvent<Poi, Fish> OnFishEnterPoi = new UltEvent<Poi, Fish>();
        public UltEvent<Poi, Fish> OnFishExitPoi = new UltEvent<Poi, Fish>();
        public UltEvent<Poi> OnPoiEnterWater = new UltEvent<Poi>();
        public UltEvent<Poi> OnPoiExitWater = new UltEvent<Poi>();

        private void Start()
        {
            mat = render.material;
            OnPoiEnterWater += (Poi _) =>
            {
                StopAllCoroutines();
                StartCoroutine(GetWet());
            };
            OnPoiExitWater += (Poi _) =>
            {
                StopAllCoroutines();
                StartCoroutine(GetDry());
            };
        }
        private void OnEnable()
        {
            net.EnableNet();
        }
        IEnumerator GetWet()
        {
            float wetness = mat.GetFloat("_Wetness");
            float elapsedTime = wetness * timeGetWet;

            while (wetness < 0.99999f)
            {
                float t = Mathf.Clamp01(elapsedTime / timeGetWet);
                mat.SetFloat("_Wetness", t);

                elapsedTime += 0.025f;
                yield return new WaitForSeconds(0.025f);
            }
        }
        IEnumerator GetDry()
        {
            float wetness = mat.GetFloat("_Wetness");
            float elapsedTime = (1f - wetness) * timeGetDry;

            while (wetness > Mathf.Epsilon)
            {
                float t = 1f - Mathf.Clamp01(elapsedTime / timeGetDry);
                mat.SetFloat("_Wetness", t);

                elapsedTime += 0.025f;
                yield return new WaitForSeconds(0.025f);
            }
        }
        void OnTriggerEnter(Collider other)
        {
            //if (other.CompareTag("Hands"))
            //{
            //    if (!GameManager.Instance.hasPoiOnHand)
            //    {
            //        if (other.gameObject.transform.name == "RightHandAnchor" && !GameManager.Instance.rightHandOnUse)
            //        {
            //            GameManager.Instance.rightHandOnUse = true;
            //            GameManager.Instance.PoiOnRight = true;
            //        }
            //        else if (other.gameObject.transform.name == "LeftHandAnchor" && !GameManager.Instance.leftHandOnUse)
            //        {
            //            GameManager.Instance.leftHandOnUse = true;
            //            GameManager.Instance.PoiOnLeft = true;
            //        }
            //        GameManager.Instance.setCurrentPoi(this.gameObject);
            //        this.transform.parent = other.gameObject.transform;
            //        GameManager.Instance.hasPoiOnHand = true;
            //    }
            //}
            if (other.CompareTag("Fish"))
            {
                var fish = other.attachedRigidbody.gameObject.GetComponent<Fish>();
                if (fish != null)
                {
                    if (!fish.fishAttr.isInPoi)
                    {
                        fish.fishAttr.isInPoi = true;
                        OnFishEnterPoi?.Invoke(this, fish);
                        Debug.Log($"{fish} is on the poi!");
                    }
                }
            }
            if (other.CompareTag("Water"))
            {
                if (!IsInWater)
                {
                    isInWater = true;
                    OnPoiEnterWater?.Invoke(this);
                    Debug.Log($"{this} enters water!");
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Fish"))
            {
                var fish = other.attachedRigidbody.gameObject.GetComponent<Fish>();
                if (fish != null)
                {
                    if (fish.fishAttr.isInPoi)
                    {
                        fish.fishAttr.isInPoi = false;
                        OnFishExitPoi?.Invoke(this, fish);
                        Debug.Log($"{fish} leaves the poi!");
                    }
                }
            }
            if (other.CompareTag("Water"))
            {
                if (IsInWater)
                {
                    isInWater = false;
                    OnPoiExitWater?.Invoke(this);
                    Debug.Log($"{this} leaves water!");
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}