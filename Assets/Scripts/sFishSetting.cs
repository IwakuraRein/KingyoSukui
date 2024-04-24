using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    [System.Serializable]
    public struct FishSetting
    {
        public int MaxFishCount;
        public Vector3 PerimeterThresholdPercentage;
        public float WaterDepth;
        [Tooltip("The offset of the fish from the perimeter of the tank, to avoid the fish from spawning on the edge of the tank, used when the fish is created.")]
        public Vector3 OffsetPercentage;
        public float avoidanceRadius;
        public float fishAvoidanceWeight;
        public float poiAvoidanceWeight;
        public float poiAvoidanceRadius;
        [Tooltip("The maximum magnitude of the avoidance vector. The avoidance vector is the inverse of the distance between the fish and the obstacle. The closer the fish, the stronger the avoidance.")]
        public float maxAvoidance;
        public float boundaryAvoidanceWeight;

        [SerializeField]
        private GameObject tank; // tank object
        public Bounds Bounds { get => tank.GetComponent<MeshRenderer>().bounds; }
        public Vector3 Center { get => Bounds.center; }

        [Header("bowl behaviors")]
        public float speedInBowl;
    }
}
