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
        [SerializeField]
        Transform[] snapPositions;
        [SerializeField]
        AudioSource audioSource;
        [SerializeField]
        AudioClip waterAudio;

        Dictionary<Fish, Transform> snapped = new Dictionary<Fish, Transform>(); // fish -> position

        public PoiGrabbableProxy proxy;

        bool isInWater = false;
        Material mat;
        public bool IsInWater { get => isInWater; set => isInWater = value; }

        public UltEvent<Poi, Fish> OnFishEnterPoi = new UltEvent<Poi, Fish>();
        public UltEvent<Poi, Fish> OnFishExitPoi = new UltEvent<Poi, Fish>();
        public UltEvent<Poi> OnPoiEnterWater = new UltEvent<Poi>();
        public UltEvent<Poi> OnPoiExitWater = new UltEvent<Poi>();
        public UltEvent OnPoiGetGrabbed = new UltEvent();
        public UltEvent OnPoiGetReleased = new UltEvent();
        public UltEvent OnPoiBreak = new UltEvent();

        public bool IsBroken { get => net.IsBroken; }

        private void Start()
        {
            mat = render.material;

            OnPoiBreak += () =>
            {
                proxy?.BreakNet();
                Fish[] l = new Fish[snapped.Count];
                int i = 0;
                foreach (var fish in snapped.Keys)
                {
                    //fish.rb.constraints = RigidbodyConstraints.None;
                    //fish.rb.useGravity = true;
                    //fish.rb.isKinematic = false;
                    l[i++] = fish;
                }
                foreach (var f in l)
                {
                    OnFishExitPoi?.Invoke(this, f);
                }
            };


            OnPoiEnterWater += (Poi _) =>
            {
                StopAllCoroutines();
                StartCoroutine(GetWet());
                audioSource.PlayOneShot(waterAudio);
                Logger.Log($"Poi {this} enters water!");
            };
            OnPoiExitWater += (Poi _) =>
            {
                StopAllCoroutines();
                StartCoroutine(GetDry());
                Logger.Log($"Poi {this} leaves water!");
            };
            OnFishEnterPoi += (Poi p, Fish f) =>
            {
                if (IsBroken) return;
                if (snapped.Count < snapPositions.Length)
                {
                    foreach (var pos in snapPositions)
                    {
                        if (snapped.ContainsValue(pos)) continue;
                        snapped.Add(f, pos);
                        f.transform.SetParent(pos, true);
                        f.transform.localPosition = Vector3.zero;
                        f.rb.velocity = Vector3.zero;
                        f.rb.angularVelocity = Vector3.zero;
                        //f.rb.constraints = RigidbodyConstraints.FreezeAll;
                        //f.rb.useGravity = true;
                        f.rb.isKinematic = true;
                        f.fishAttr.isInPoi = true;
                        Logger.Log($"Fish {f} is on the poi!");
                        break;
                    }
                }
            };
            OnFishExitPoi += (Poi p, Fish f) =>
            {
                f.rb.velocity = Vector3.zero;
                f.rb.angularVelocity = Vector3.zero;
                f.rb.constraints = RigidbodyConstraints.None;
                f.rb.isKinematic = false;
                if (snapped.ContainsKey(f))
                {
                    snapped.Remove(f);
                    f.transform.parent = null;
                }
                f.fishAttr.isInPoi = false;
                //if (!f.IsUnderWater) f.rb.useGravity = true;
                Logger.Log($"Fish {f} leaves the poi!");
            };
            OnPoiGetReleased += () =>
            {
                foreach((var f, var pos) in snapped)
                {
                    f.transform.parent = null;
                    f.rb.isKinematic = false;
                    f.rb.velocity = Vector3.zero;
                    f.rb.angularVelocity = Vector3.zero;
                    f.rb.constraints = RigidbodyConstraints.None;
                    f.fishAttr.isInPoi = false;
                    Logger.Log($"Fish {f} leaves the poi!");
                }
            };
        }
        private void FixedUpdate()
        {
            if (Vector3.Dot(transform.up, Vector3.up) < 0)
            {
                Fish[] l = new Fish[snapped.Count];
                int i = 0;
                foreach (var fish in snapped.Keys)
                {
                    //fish.rb.constraints = RigidbodyConstraints.None;
                    //fish.rb.useGravity = true;
                    //fish.rb.isKinematic = false;
                    l[i++] = fish;
                }
                foreach (var f in l)
                {
                    OnFishExitPoi?.Invoke(this, f);
                }
            }
            //else
                //foreach (var fish in snapped.Keys)
                //{
                //    fish.rb.constraints = RigidbodyConstraints.FreezeAll;
                //    fish.rb.useGravity = false;
                //}
        }
        private void OnEnable()
        {
            if (proxy && proxy.isBroken)
            {
                net.BreakNet();

            }
            else net.EnableNet();
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
                    if (!fish.fishAttr.isInPoi && Vector3.Dot(transform.up, Vector3.up) > Mathf.Epsilon)
                    {
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