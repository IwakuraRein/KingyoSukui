using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    public class Fish : MonoBehaviour
    {
        public float maxSpeed;
        public int score;
        private bool isInPoi = false;
        public bool IsInPoi { get => isInPoi; set => isInPoi = value; }
        private bool isInBowl = false;
        public bool IsInBowl { get => isInBowl; set => isInBowl = value; }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        //void OnTriggerEnter(Collider other)
        //{
        //    if (other.gameObject.tag == "poi" && !IsInPoi)
        //    {
        //        isInPoi = true;
        //        Debug.Log($"{this} is on the poi!");
        //    }
        //    if (other.gameObject.tag == "bowl" && !IsInBowl)
        //    {
        //        isInBowl = true;
        //        Debug.Log($"{this} is in the bowl!");
        //    }
        //}
        //void OnTriggerExit(Collider other)
        //{
        //    if (other.gameObject.tag == "poi" && IsInPoi)
        //    {
        //        isInPoi = false;

        //        Debug.Log($"{this} leaves the poi!");
        //    }
        //    if (other.gameObject.tag == "bowl" &&IsInBowl)
        //    {
        //        isInBowl = false;
        //    }
        //}
    }
}

