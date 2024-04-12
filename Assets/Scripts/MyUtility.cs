using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    class MyUtility
    {
        public static bool IsUnderWater(Vector3 position, Vector3 center, Bounds extents, float waterDepth, Vector3 threshold = default)
        {
            // threshold is the distance from the perimeter of the tank
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
    }
}