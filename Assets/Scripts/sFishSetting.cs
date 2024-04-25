using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    [System.Serializable]
    public struct FishSetting
    {
        public int MaxFishCount;
        public float WaterDepth;
        [Tooltip("The offset of the fish from the perimeter of the tank, to avoid the fish from spawning on the edge of the tank, used when the fish is created.")]
        public Vector3 OffsetPercentage;

        [SerializeField]
        private GameObject tank; // tank object
        public Bounds Bounds { get => tank.GetComponent<MeshRenderer>().bounds; }
        public Vector3 Center { get => Bounds.center; }

        [Header("bowl behaviors")]
        public float speedInBowl;
    }
}
