using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    public class Fish : MonoBehaviour
    {
        public float maxSpeed;
        private bool isCaught = false;
        public bool IsCaught { get => isCaught; }
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
                isCaught = true;
            }
        }
        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "poi")
            {
                isCaught = false;
            }
        }
    }
}

