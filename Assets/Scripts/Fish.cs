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
        [SerializeField]
        public Poi poi;
        // Start is called before the first frame update
        void Start()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(poi) // confine fish
            {
                var center = poi.transform.InverseTransformPoint(poi.bounds.bounds.center);
                var extents = poi.bounds.bounds.extents;
                //extents.x /= poi.transform.lossyScale.x;
                //extents.y /= poi.transform.lossyScale.y;
                //extents.z /= poi.transform.lossyScale.z;
                var pos = poi.transform.InverseTransformPoint(transform.position);
                pos.x = Mathf.Clamp(pos.x, center.x - extents.x, center.x + extents.x);
                pos.z = Mathf.Clamp(pos.z, center.z - extents.z, center.z + extents.z);
                pos.y = Mathf.Max(pos.y, center.y - extents.y);
                pos = poi.transform.TransformPoint(pos);
                rb.MovePosition(pos);
            }
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

