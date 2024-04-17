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
        public bool IsInPoi { get => isInPoi; }
        private bool isInBowl = false;
        public bool IsInBowl { get => isInBowl; }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "poi")
            {
                if (collision.contacts[0].point.y < transform.position.y)
                {
                    return;
                }
                isInPoi = true;
            }
            if (collision.gameObject.tag == "bowl")
            {
                isInBowl = true;
            }
        }
        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "poi")
            {
                isInPoi = false;
            }
            if (collision.gameObject.tag == "bowl")
            {
                isInBowl = false;
            }
        }
    }
}

