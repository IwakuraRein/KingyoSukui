using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    public class Fish : MonoBehaviour
    {
        [SerializeField]
        public FishAttribute fishAttr;
        public Rigidbody rb;
        // Start is called before the first frame update
        void Start()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        public bool IsUnderWater
        {
            get
            {
                return MyUtility.IsUnderWater(transform.position, FishManager.instance.FishSetting.Bounds.center, FishManager.instance.FishSetting.Bounds, FishManager.instance.FishSetting.WaterDepth);
            }
        }
    }
}

