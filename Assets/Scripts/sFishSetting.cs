using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    [System.Serializable]
    public struct FishSetting
    {
        public int MaxFishCount;
        [SerializeField]
        private GameObject tank; // tank object
        public GameObject Tank { get => tank; }
        public Bounds bounds { get => tank.GetComponent<MeshRenderer>().bounds; }
    }
}
