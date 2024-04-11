using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;



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
        public NativeArray<bool> useGravityFlags;

        public void Execute(int index)
        {
            useGravityFlags[index] = !IsUnderWater(positions[index], center, extents, waterDepth);
        }

        private bool IsUnderWater(Vector3 position, Vector3 center, Bounds extents, float waterDepth)
        {
            if (position.x < center.x - extents.extents.x || position.x > center.x + extents.extents.x)
            {
                return false;
            }
            if (position.y < center.y - extents.extents.y || position.y > center.y - extents.extents.y + waterDepth)

            {
                return false;
            }
            if (position.z < center.z - extents.extents.z || position.z > center.z + extents.extents.z)
            {
                return false;
            }
            return true;
        }
    }
    public class FishManager : MonoBehaviour
    {
        private static FishManager _instance;
        public static FishManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(FishManager)) as FishManager;
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        go.tag = "GameController";
                        _instance = go.AddComponent<FishManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        [SerializeField]
        GameObject[] fishPrefabs; // prefab of fish
        Fish[] fishes;
        private NativeArray<Vector3> FishPositions;
        private NativeArray<bool> useGravityFlags;
        [SerializeField]
        private FishSetting fishSetting;
        public FishSetting FishSetting { get => fishSetting; }
        void Start()
        {
            if (fishSetting.WaterDepth > fishSetting.Bounds.extents.y * 2)
            {
                Debug.LogError("WaterDepth is too deep. Please set the value less than " + fishSetting.Bounds.extents.y * 2);
            }
            CreateFish(fishSetting.Center, fishSetting.Bounds, fishSetting.Offset, fishSetting.MaxFishCount);
            FishPositions = new NativeArray<Vector3>(fishSetting.MaxFishCount, Allocator.Persistent);
            useGravityFlags = new NativeArray<bool>(fishSetting.MaxFishCount, Allocator.Persistent);
        }
        private void OnDestroy()
        {
            FishPositions.Dispose();
            useGravityFlags.Dispose();
        }
        void CreateFish(Vector3 center, Bounds bounds, Vector3 offset, int count)
        {
            fishes = new Fish[count];
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = center +
                new Vector3(Random.Range(-bounds.extents.x + offset.x, bounds.extents.x - offset.x),
                    Random.Range(-bounds.extents.y + offset.y, -bounds.extents.y + fishSetting.WaterDepth - offset.y),
                    Random.Range(-bounds.extents.z + offset.z, bounds.extents.z - offset.z));
                int index = Random.Range(0, fishPrefabs.Length);
                GameObject fish = Instantiate(fishPrefabs[index], pos, Quaternion.identity);
                fishes[i] = fish.GetComponent<Fish>();
            }
        }
        void Update()
        {
            for (int i = 0; i < fishSetting.MaxFishCount; i++)
            {
                FishPositions[i] = fishes[i].transform.position;
            }
            UpdateFishUseGravity(fishSetting.Center, fishSetting.Bounds);
        }
        void UpdateFishUseGravity(Vector3 center, Bounds extents)
        {
            var job = new UpdateFishGravityFlagsJob
            {
                center = center,
                extents = extents,
                positions = FishPositions,
                useGravityFlags = useGravityFlags,
                waterDepth = fishSetting.WaterDepth
            };

            JobHandle jobHandle = job.Schedule(FishPositions.Length, 64);
            jobHandle.Complete();

            for (int i = 0; i < fishSetting.MaxFishCount; i++)
            {
                fishes[i].GetComponent<Rigidbody>().useGravity = job.useGravityFlags[i];
            }
        }
    }
}
