using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    [System.Serializable]
    public struct FishAttribute
    {
        // Removed the public declaration of isInPoi
        public bool isFishInBowl;
        public bool useGravity;
        public float poiAvoidanceRadius;
        public float avoidanceRadius;
        [Tooltip("The maximum magnitude of the avoidance vector. The avoidance vector is the inverse of the distance between the fish and the obstacle. The closer the fish, the stronger the avoidance.")]
        public float maxAvoidance;
        public float poiAvoidanceWeight;
        public float boundaryAvoidanceWeight;
        public float fishAvoidanceWeight;
        public float maxSpeed;
        public int score;

        public bool isInPoi;

        public bool isInBowl;

        public Vector3 perimeterThresholdPercentage;
    }
}
