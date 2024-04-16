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
        private NativeArray<bool> IsFishInBowl;
        private NativeArray<bool> useGravityFlags;
        private TransformAccessArray transformAccessArray;
        [SerializeField][Tooltip("Enable AI for fish. If false, fish will not move. AI is disabled for debugging.")]
        private bool IsAIEnabled = true;
        [SerializeField]
        private FishSetting fishSetting;
        public FishSetting FishSetting { get => fishSetting; }
        void Start()
        {
            if (fishSetting.WaterDepth > fishSetting.Bounds.extents.y * 2)
            {
                Debug.LogError("WaterDepth is too deep. Please set the value less than " + fishSetting.Bounds.extents.y * 2);
            }
            CreateFish(fishSetting.Center, fishSetting.Bounds, fishSetting.OffsetPercentage, fishSetting.MaxFishCount);
            transformAccessArray = new TransformAccessArray(fishes.Length);
            FishPositions = new NativeArray<Vector3>(fishes.Length, Allocator.Persistent);
            FishVelocities = new NativeArray<Vector3>(fishes.Length, Allocator.Persistent);
            IsFishInBowl = new NativeArray<bool>(fishes.Length, Allocator.Persistent);
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
            IsFishInBowl.Dispose();
        }
        void Update()
        {
            for (int i = 0; i < fishes.Length; i++)
            {
                FishPositions[i] = fishes[i].transform.position;
                FishVelocities[i] = fishes[i].GetComponent<Rigidbody>().velocity;
            }
            UpdateFishUseGravity(fishSetting.Center, fishSetting.Bounds);
            if (IsAIEnabled)
            {
                UpdateFishBehavior(fishSetting.Center, fishSetting.Bounds);
                UpdateFishRigidBody();
            }
            else
            {
                UpdateFishRigidBodyWOAI();
            }
        }
        void CreateFish(Vector3 center, Bounds bounds, Vector3 offsetPercentage, int count)
        {
            fishes = new Fish[count];
            for (int i = 0; i < count; i++)
            {
                Vector3 offset = new Vector3(bounds.extents.x * offsetPercentage.x, bounds.extents.y * offsetPercentage.y, bounds.extents.z * offsetPercentage.z);
                Vector3 pos = center +
                new Vector3(Random.Range(-bounds.extents.x + offset.x, bounds.extents.x - offset.x),
                    Random.Range(-bounds.extents.y + offset.y, -bounds.extents.y + fishSetting.WaterDepth - offset.y),
                    Random.Range(-bounds.extents.z + offset.z, bounds.extents.z - offset.z));
                int index = Random.Range(0, fishPrefabs.Length);
                GameObject fish = Instantiate(fishPrefabs[index], pos, Quaternion.identity);
                // randomize fish velocity and direction
                Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                fish.transform.forward = direction;
                if (IsAIEnabled)
                {
                    Vector3 velocity = direction * Random.Range(0, fish.GetComponent<Fish>().maxSpeed);
                    fish.GetComponent<Rigidbody>().velocity = velocity;
                }
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
                isFishInBowl = IsFishInBowl,
                avoidanceRadius = fishSetting.avoidanceRadius,
                maxAvoidance = fishSetting.maxAvoidance,
                boundaryAvoidanceWeight = fishSetting.boundaryAvoidanceWeight,
                fishAvoidanceWeight = fishSetting.fishAvoidanceWeight,
                center = center,
                extents = extents,
                perimeterThresholdPercentage = fishSetting.PerimeterThresholdPercentage,
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
                var fishRigidbody = fishes[i].GetComponent<Rigidbody>();
                fishRigidbody.useGravity = useGravityFlags[i];
                if (fishes[i].IsInBowl || fishRigidbody.useGravity)
                {
                    if (fishes[i].IsInBowl)
                    {
                        IsFishInBowl[i] = true;
                        fishRigidbody.useGravity = false;
                        fishRigidbody.velocity.Set(fishRigidbody.velocity.x, 0, fishRigidbody.velocity.z);
                        fishRigidbody.angularVelocity = Vector3.zero;
                    }
                    else
                    {
                        fishRigidbody.velocity *= 0.1f;
                        fishRigidbody.angularVelocity = Vector3.zero;
                    }
                }
                else // fish is in water
                {
                    Vector3 vel = FishVelocities[i];
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
                }
            }
        }
        void UpdateFishRigidBodyWOAI()
        {
            for (int i = 0; i < fishes.Length; i++)
            {
                var fishRigidbody = fishes[i].GetComponent<Rigidbody>();
                fishRigidbody.useGravity = useGravityFlags[i];
                fishRigidbody.velocity = new Vector3(0, 0, 0);
                fishRigidbody.angularVelocity = new Vector3(0, 0, 0);
                if (fishes[i].IsInBowl || fishRigidbody.useGravity)
                {
                    if (fishes[i].IsInBowl)
                    {
                        Debug.Log("Fish is in bowl");
                        IsFishInBowl[i] = true;
                        fishRigidbody.useGravity = false;
                        fishRigidbody.velocity.Set(fishRigidbody.velocity.x, 0, fishRigidbody.velocity.z);
                        fishRigidbody.angularVelocity = Vector3.zero;
                    }
                    else
                    {
                        fishRigidbody.velocity *= 0.1f;
                        fishRigidbody.angularVelocity = Vector3.zero;
                    }
                }
            }
        }
    }
}
