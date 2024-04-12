using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;

namespace Kingyo
{
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
        private NativeArray<Vector3> FishVelocities;
        private NativeArray<bool> useGravityFlags;
        private TransformAccessArray transformAccessArray;
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
            transformAccessArray = new TransformAccessArray(fishes.Length);
            FishPositions = new NativeArray<Vector3>(fishes.Length, Allocator.Persistent);
            FishVelocities = new NativeArray<Vector3>(fishes.Length, Allocator.Persistent);

            for (int i = 0; i < fishes.Length; i++)
            {
                transformAccessArray.Add(fishes[i].transform);
            }
            useGravityFlags = new NativeArray<bool>(fishes.Length, Allocator.Persistent);
        }
        private void OnDestroy()
        {
            transformAccessArray.Dispose();
            FishPositions.Dispose();
            FishVelocities.Dispose();
            useGravityFlags.Dispose();
        }
        void Update()
        {
            for (int i = 0; i < fishes.Length; i++)
            {
                FishPositions[i] = fishes[i].transform.position;
                FishVelocities[i] = fishes[i].GetComponent<Rigidbody>().velocity;
            }
            UpdateFishUseGravity(fishSetting.Center, fishSetting.Bounds);
            UpdateFishBehavior(fishSetting.Center, fishSetting.Bounds);
            UpdateFishRigidBody();
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
                // randomize fish velocity and direction
                Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                fish.transform.forward = direction;
                Vector3 velocity = direction * Random.Range(0, fish.GetComponent<Fish>().maxSpeed);
                fish.GetComponent<Rigidbody>().velocity = velocity;
                fishes[i] = fish.GetComponent<Fish>();
            }
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
        }
        void UpdateFishBehavior(Vector3 center, Bounds extents)
        {
            var job = new FishBehaviorJob
            {
                positions = FishPositions,
                velocities = FishVelocities,
                avoidanceRadius = fishSetting.avoidanceRadius,
                boundaryAvoidanceWeight = fishSetting.boundaryAvoidanceWeight,
                fishAvoidanceWeight = fishSetting.fishAvoidanceWeight,
                center = center,
                extents = extents,
                perimeterThreshold = fishSetting.PerimeterThreshold,
                waterDepth = fishSetting.WaterDepth,
                deltaTime = Time.deltaTime
            };
            JobHandle jobHandle = job.Schedule(transformAccessArray);
            jobHandle.Complete();
        }
        void UpdateFishRigidBody()
        {
            for (int i = 0; i < fishes.Length; i++)
            {
                Vector3 vel = FishVelocities[i];
                var fishRigidbody = fishes[i].GetComponent<Rigidbody>();
                fishRigidbody.MoveRotation(fishes[i].transform.rotation);
                if (vel.magnitude > fishes[i].maxSpeed)
                {
                    vel = vel.normalized * fishes[i].maxSpeed;
                }
                vel.y *= 0.8f;
                if (vel.magnitude > 1e-2f)
                {
                    fishRigidbody.velocity = vel;
                }
                else
                {
                    Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                    fishRigidbody.velocity = direction * Random.Range(0, fishes[i].maxSpeed);
                }
                fishRigidbody.useGravity = useGravityFlags[i];
            }
        }
    }
}
