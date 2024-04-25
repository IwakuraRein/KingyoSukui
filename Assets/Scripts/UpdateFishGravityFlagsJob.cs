using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Kingyo
{
    public struct UpdateFishGravityFlagsJob : IJobParallelFor
    {
        [ReadOnly]
        public Vector3 center;
        [ReadOnly]
        public Bounds extents;
        public float waterDepth;
        public NativeArray<Vector3> positions;
        public NativeArray<FishAttribute> fishAttributes;

        public void Execute(int index)
        {
            FishAttribute fishAttr = fishAttributes[index];
            fishAttr.useGravity = !MyUtility.IsUnderWater(positions[index], center, extents, waterDepth, new Vector3(0, 0, 0));
            fishAttributes[index] = fishAttr;
        }
    }
}
