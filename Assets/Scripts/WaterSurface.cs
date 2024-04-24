using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    public class WaterSurface : MonoBehaviour
    {
        [SerializeField]
        BoxCollider volume;
        void Start()
        {
            var setting =
                FishManager.instance.FishSetting;
            this.transform.position = new Vector3(
                transform.position.x,
                setting.Bounds.center.y - setting.Bounds.extents.y + setting.WaterDepth,
                transform.position.z
                );
            Vector3 center = new Vector3(
                setting.Bounds.center.x,
                setting.Bounds.center.y - setting.Bounds.extents.y + setting.WaterDepth * 0.5f,
                setting.Bounds.center.z
                );
            Vector3 size = new Vector3(
                setting.Bounds.size.x,
                setting.WaterDepth,
                setting.Bounds.size.z
                );
            volume.center = volume.transform.InverseTransformPoint(center);
            volume.size = new Vector3(
                size.x * 1f / volume.transform.lossyScale.x,
                size.y * 1f / volume.transform.lossyScale.y,
                size.z * 1f / volume.transform.lossyScale.z
                );
        }
    }

}