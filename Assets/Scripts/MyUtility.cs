using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    class MyUtility
    {
        public static bool IsUnderWater(Vector3 position, Vector3 center, Bounds extents, float waterDepth, Vector3 thresholdPercentage = default)
        {
            // threshold is the distance from the perimeter of the tank
            Vector3 threshold = new Vector3(extents.extents.x * thresholdPercentage.x, extents.extents.y * thresholdPercentage.y, extents.extents.z * thresholdPercentage.z);
            if (position.x < center.x - extents.extents.x + threshold.x || position.x > center.x + extents.extents.x - threshold.x)
            {
                return false;
            }
            if (position.y < center.y - extents.extents.y + threshold.y || position.y > center.y - extents.extents.y + waterDepth - threshold.y)
            {
                return false;
            }
            if (position.z < center.z - extents.extents.z + threshold.z || position.z > center.z + extents.extents.z - threshold.z)
            {
                return false;
            }
            return true;
        }
        public static Vector3 SDFUnderWater(Vector3 position, Vector3 center, Bounds extents, float waterDepth, Vector3 thresholdPercentage = default)
        {
            // threshold is the distance from the perimeter of the tank
            Vector3 threshold = new Vector3(extents.extents.x * thresholdPercentage.x, extents.extents.y * thresholdPercentage.y, extents.extents.z * thresholdPercentage.z);
            Vector3 sdf = new Vector3(0, 0, 0);
            if (position.x < center.x - extents.extents.x + threshold.x)
            {
                sdf.x = position.x - (center.x - extents.extents.x);
            }
            if (position.x > center.x + extents.extents.x - threshold.x)
            {
                sdf.x = position.x - (center.x + extents.extents.x);
            }
            if (position.y < center.y - extents.extents.y + threshold.y)
            {
                sdf.y = position.y - (center.y - extents.extents.y);
            }
            if (position.y > center.y - extents.extents.y + waterDepth - threshold.y)
            {
                sdf.y = position.y - (center.y - extents.extents.y + waterDepth);
            }
            if (position.z < center.z - extents.extents.z + threshold.z)
            {
                sdf.z = position.z - (center.z - extents.extents.z);
            }
            if (position.z > center.z + extents.extents.z - threshold.z)
            {
                sdf.z = position.z - (center.z + extents.extents.z);
            }
            return sdf;
        }
    }
}