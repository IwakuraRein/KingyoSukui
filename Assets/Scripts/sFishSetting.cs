using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    [System.Serializable]
    public struct FishSetting
    {
        public int MaxFishCount;
        public Vector3 PerimeterThreshold;
        public float WaterDepth;
        public Vector3 Offset;
        public float avoidanceRadius;
        public float fishAvoidanceWeight;
        public float boundaryAvoidanceWeight;

        [SerializeField]
        private GameObject tank; // tank object
        public Bounds Bounds { get => tank.GetComponent<MeshRenderer>().bounds; }
        public Vector3 Center { get => Bounds.center; }
    }
}
