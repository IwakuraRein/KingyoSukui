using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Kingyo
{
    public struct FishBehaviorJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeArray<Vector3> positions;
        public NativeArray<Vector3> velocities;
        public NativeArray<bool> isFishInBowl;
        [ReadOnly]
        public float avoidanceRadius;
        public float maxAvoidance;
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
            Vector3 boundaryAvoidance = Vector3.zero;

            Vector3 sdf = MyUtility.SDFUnderWater(transform.position, center, extents, waterDepth, perimeterThresholdPercentage);
            if (sdf.magnitude > 0.1f)
            {
                boundaryAvoidance = new Vector3(Mathf.Min(1 / sdf.x, maxAvoidance),
                Mathf.Min(1 / sdf.y, maxAvoidance),
                Mathf.Min(1 / sdf.z, maxAvoidance));
            }
            // avoid other fish
            Vector3 FishAvoidance = Vector3.zero;
            int count = 0;
            for (int i = 0; i < positions.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }
                float distance = Vector3.Distance(positions[i], transform.position);
                if (distance < avoidanceRadius && distance > 0.01f)
                {
                    Vector3 avoidVector = (transform.position - positions[i]);
                    // the closer the fish, the stronger the avoidance(inverse)
                    FishAvoidance += new Vector3(Mathf.Min(1 / distance, maxAvoidance) * avoidVector.x, 0, Mathf.Min(1 / distance, maxAvoidance) * avoidVector.z);
                    count++;
                }
            }
            Vector3 randomDirection = new Vector3(0, 0, 0);
            randomDirection.y = 0;
            if (count == 0)
            {
                velocities[index] += deltaTime * (boundaryAvoidance * boundaryAvoidanceWeight + randomDirection * 0.1f);
            }
            else
            {
                velocities[index] += deltaTime * (boundaryAvoidance * boundaryAvoidanceWeight + randomDirection * 0.1f + FishAvoidance * fishAvoidanceWeight / count);
            }
            if (velocities[index].magnitude > 1e-2f)
            {
                transform.rotation = Quaternion.LookRotation(velocities[index]);
            }
        }
    }
}
