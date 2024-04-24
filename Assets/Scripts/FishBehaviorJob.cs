using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Kingyo
{
    public struct FishBehaviorJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeArray<Vector3> poiPositions;
        [ReadOnly]
        public NativeArray<Vector3> positions;
        [ReadOnly]
        public NativeArray<Vector3> velocities;
        public NativeArray<Vector3> forces;
        [ReadOnly]
        public NativeArray<bool> isFishInBowl;
        [ReadOnly]
        public NativeArray<bool> useGravityFlags;
        public int poiInWaterCount;
        public float poiAvoidanceRadius;
        public float avoidanceRadius;
        public float maxAvoidance;
        public float poiAvoidanceWeight;
        public float boundaryAvoidanceWeight;
        public float fishAvoidanceWeight;
        public float waterDepth;
        public Vector3 perimeterThresholdPercentage;
        public Vector3 center;
        public Bounds extents;
        public float deltaTime;
        public void Execute(int index, TransformAccess transform)
        {
            if (isFishInBowl[index])
            {
                return;
            }
            Vector3 poiAvoidance = ComputePoiAvoidance(transform.position);

            Vector3 boundaryAvoidance = ComputeBoundaryAvoidance(transform.position);

            Vector3 fishAvoidance = ComputeFishAvoidance(transform.position, index);

            forces[index] = (poiAvoidance * poiAvoidanceWeight + boundaryAvoidance * boundaryAvoidanceWeight + fishAvoidance * fishAvoidanceWeight) / deltaTime;
        }
        private Vector3 ComputePoiAvoidance(Vector3 position)
        {
            Vector3 PoiAvoidance = Vector3.zero;
            for (int i = 0; i < poiInWaterCount; i++)
            {
                float distance = Vector3.Distance(poiPositions[i], position);
                if (distance < poiAvoidanceRadius && distance > 0.01f)
                {
                    Vector3 avoidVector = (position - poiPositions[i]);
                    PoiAvoidance += new Vector3(Mathf.Min(1 / distance, maxAvoidance) * avoidVector.x, 0, Mathf.Min(1 / distance, maxAvoidance) * avoidVector.z);
                }
            }
            return PoiAvoidance;
        }

    private Vector3 ComputeBoundaryAvoidance(Vector3 position)
    {
        Vector3 BoundaryAvoidance = Vector3.zero;
        Vector3 sdf = MyUtility.SDFUnderWater(position, center, extents, waterDepth, perimeterThresholdPercentage);
        if (sdf.magnitude > 0.1f)
        {
            BoundaryAvoidance = new Vector3(Mathf.Min(1 / sdf.x, maxAvoidance),
            Mathf.Min(1 / sdf.y, maxAvoidance),
            Mathf.Min(1 / sdf.z, maxAvoidance));
        }
        return BoundaryAvoidance;
    }

    private Vector3 ComputeFishAvoidance(Vector3 position, int index)
    {
        Vector3 FishAvoidance = Vector3.zero;
        int count = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            if (i == index)
            {
                continue;
            }
            float distance = Vector3.Distance(positions[i], position);
            if (distance < avoidanceRadius && distance > 0.01f)
            {
                Vector3 avoidVector = (position - positions[i]);
                // the closer the fish, the stronger the avoidance(inverse)
                FishAvoidance += new Vector3(Mathf.Min(1 / distance, maxAvoidance) * avoidVector.x, 0, Mathf.Min(1 / distance, maxAvoidance) * avoidVector.z);
                count++;
            }
        }
        if (count == 0)
        {
            return Vector3.zero;
        }
        else
        {
            return FishAvoidance / count;
        }
    }
}
}
