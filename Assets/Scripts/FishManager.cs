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
        [SerializeField]
        Poi[] PoiObjects;
        Fish[] fishes;
        private int PoiInWaterCount = 0;
        private NativeArray<Vector3> PoiPositions;
        private NativeArray<Vector3> FishPositions;
        private NativeArray<Vector3> FishForces;
        private NativeArray<Vector3> FishVelocities;
        private NativeArray<bool> IsFishInBowl;
        private NativeArray<bool> IsFishInPoi;
        private NativeArray<bool> useGravityFlags;
        private TransformAccessArray transformAccessArray;
        [SerializeField]
        [Tooltip("Enable AI for fish. If false, fish will not move. AI is disabled for debugging.")]
        private bool isAIEnabled = true;
        [SerializeField]
        private FishSetting fishSetting;
        public FishSetting FishSetting { get => fishSetting; }

        public bool IsAIEnabled
        {
            get => isAIEnabled;
            set
            {
                isAIEnabled = value;
            }
        }

        void Start()
        {
            if (fishSetting.WaterDepth > fishSetting.Bounds.extents.y * 2)
            {
                Debug.LogError("WaterDepth is too deep. Please set the value less than " + fishSetting.Bounds.extents.y * 2);
            }
            CreateFish(fishSetting.Center, fishSetting.Bounds, fishSetting.OffsetPercentage, fishSetting.MaxFishCount);
            transformAccessArray = new TransformAccessArray(fishes.Length);
            PoiPositions = new NativeArray<Vector3>(PoiObjects.Length, Allocator.Persistent);
            FishPositions = new NativeArray<Vector3>(fishes.Length, Allocator.Persistent);
            FishForces = new NativeArray<Vector3>(fishes.Length, Allocator.Persistent);
            FishVelocities = new NativeArray<Vector3>(fishes.Length, Allocator.Persistent);
            IsFishInBowl = new NativeArray<bool>(fishes.Length, Allocator.Persistent);
            IsFishInPoi = new NativeArray<bool>(fishes.Length, Allocator.Persistent);
            for (int i = 0; i < fishes.Length; i++)
            {
                transformAccessArray.Add(fishes[i].transform);
            }
            useGravityFlags = new NativeArray<bool>(fishes.Length, Allocator.Persistent);
        }
        private void OnDestroy()
        {
            transformAccessArray.Dispose();
            PoiPositions.Dispose();
            FishPositions.Dispose();
            FishVelocities.Dispose();
            FishForces.Dispose();
            useGravityFlags.Dispose();
            IsFishInBowl.Dispose();
            IsFishInPoi.Dispose();
        }
        void FixedUpdate()
        {
            PoiInWaterCount = 0;
            for (int i = 0; i < PoiObjects.Length; i++)
            {
                if (PoiObjects[i].IsInWater)
                {
                    PoiPositions[PoiInWaterCount++] = PoiObjects[i].transform.position;
                }
            }

            for (int i = 0; i < fishes.Length; i++)
            {
                FishPositions[i] = fishes[i].transform.position;
                FishVelocities[i] = fishes[i].GetComponent<Rigidbody>().velocity;
            }
            UpdateFishUseGravity(fishSetting.Center, fishSetting.Bounds);
            if (isAIEnabled)
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
                Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                fish.transform.forward = direction;
                if (isAIEnabled)
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
        void UpdateFishRotation()
        {
            var job = new UpdateFishRotation
            {
                velocities = FishVelocities,
                isFishInPoi = IsFishInPoi
            };
            JobHandle jobHandle = job.Schedule(transformAccessArray);
            jobHandle.Complete();
        }
        void UpdateFishBehavior(Vector3 center, Bounds extents)
        {
            var job = new FishBehaviorJob
            {
                poiInWaterCount = PoiInWaterCount,
                poiPositions = PoiPositions,
                positions = FishPositions,
                velocities = FishVelocities,
                forces = FishForces,
                isFishInBowl = IsFishInBowl,
                useGravityFlags = useGravityFlags,
                avoidanceRadius = fishSetting.avoidanceRadius,
                maxAvoidance = fishSetting.maxAvoidance,
                poiAvoidanceRadius = fishSetting.poiAvoidanceRadius,
                poiAvoidanceWeight = fishSetting.poiAvoidanceWeight,
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
            // for (int i = 0; i < fishes.Length; i++)
            // {
            //    Debug.Log("Fish " + i + " force: " + FishForces[i] + ", poi in water count: " + PoiInWaterCount);
            // }
        }
        void UpdateFishRigidBody()
        {
            for (int i = 0; i < fishes.Length; i++)
            {
                var fishRigidbody = fishes[i].GetComponent<Rigidbody>();
                fishRigidbody.useGravity = useGravityFlags[i];
                if (fishes[i].IsInBowl)
                {
                    IsFishInBowl[i] = true;
                    fishRigidbody.useGravity = false;
                    if (fishRigidbody.velocity.magnitude > fishSetting.speedInBowl)
                        fishRigidbody.velocity *= 0.5f;
                    fishRigidbody.angularVelocity *= 0.5f;
                    UpdateFishRotation();
                }
                else if (fishRigidbody.useGravity)
                {
                    IsFishInBowl[i] = false;
                    fishRigidbody.angularVelocity = Vector3.zero;
                }
                else // fish is in water
                {
                    IsFishInBowl[i] = false;
                    Vector3 vel = fishRigidbody.velocity;
                    vel.y *= 0.8f;
                    Vector3 force = FishForces[i];
                    fishRigidbody.AddForce(force);
                    if (vel.magnitude > fishes[i].maxSpeed)
                        fishRigidbody.velocity = vel.normalized * fishes[i].maxSpeed;
                    else
                        fishRigidbody.velocity = vel;
                    if (vel.magnitude < 1e-2f)
                    {
                        Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                        fishRigidbody.AddForce(direction * Random.Range(0, fishes[i].maxSpeed) / Time.fixedDeltaTime);
                    }
                    UpdateFishRotation();
                }
                IsFishInPoi[i] = fishes[i].IsInPoi;
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
                if (fishes[i].IsInBowl)
                {
                    IsFishInBowl[i] = true;
                    fishRigidbody.useGravity = false;
                    if (fishRigidbody.velocity.magnitude > fishSetting.speedInBowl)
                        fishRigidbody.velocity *= 0.8f;
                    fishRigidbody.angularVelocity *= 0.5f;
                }
                else if (fishRigidbody.useGravity)
                {
                    IsFishInBowl[i] = false;
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
