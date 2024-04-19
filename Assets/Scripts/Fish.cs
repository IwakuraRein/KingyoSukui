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
    }
}

