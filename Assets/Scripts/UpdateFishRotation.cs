using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Kingyo
{
    public struct UpdateFishRotation : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeArray<Vector3> velocities;
        public NativeArray<bool> isFishInPoi;
        public void Execute(int index, TransformAccess transform)
        {
            if (isFishInPoi[index])
            {
                return;
            }
            if (velocities[index].magnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(velocities[index]);
                Vector3 forwardHorizontal = new Vector3(velocities[index].x, 0, velocities[index].z);
                if (forwardHorizontal != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(forwardHorizontal, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.5f);
                }
            }
        }
    }
}
