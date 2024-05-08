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
                        //DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        [SerializeField]
        GameObject[] fishPrefabs; // prefab of fish
        Fish[] fishes;
        private int PoiInWaterCount = 0;
        private NativeArray<Vector3> PoiPositions;
        private NativeArray<Vector3> FishPositions;
        private NativeArray<Vector3> FishForces;
        private NativeArray<Vector3> FishVelocities;
        private NativeArray<FishAttribute> FishAttributes;
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
            PoiPositions = new NativeArray<Vector3>(2, Allocator.Persistent);
            FishPositions = new NativeArray<Vector3>(fishes.Length, Allocator.Persistent);
            FishForces = new NativeArray<Vector3>(fishes.Length, Allocator.Persistent);
            FishVelocities = new NativeArray<Vector3>(fishes.Length, Allocator.Persistent);
            FishAttributes = new NativeArray<FishAttribute>(fishes.Length, Allocator.Persistent);
            for (int i = 0; i < fishes.Length; i++)
            {
                FishAttributes[i] = fishes[i].fishAttr;
            }
            for (int i = 0; i < fishes.Length; i++)
            {
                transformAccessArray.Add(fishes[i].transform);
            }
        }
        private void OnDestroy()
        {
            transformAccessArray.Dispose();
            PoiPositions.Dispose();
            FishPositions.Dispose();
            FishVelocities.Dispose();
            FishForces.Dispose();
            FishAttributes.Dispose();
            //foreach (var f in fishes)
            //{
            //    Debug.Log($"{f} destroy");
            //    DestroyImmediate(f);
            //}
        }
        void FixedUpdate()
        {
            PoiInWaterCount = 0;
            //for (int i = 0; i < PoiObjects.Length; i++)
            //{
            if (GameManager.Instance.leftPoi.IsInWater)
            {
                PoiPositions[PoiInWaterCount++] = GameManager.Instance.leftPoi.transform.position;
            }
            if (GameManager.Instance.rightPoi.IsInWater)
            {
                PoiPositions[PoiInWaterCount++] = GameManager.Instance.rightPoi.transform.position;
            }
            //}

            for (int i = 0; i < fishes.Length; i++)
            {
                FishPositions[i] = fishes[i].transform.position;
                FishVelocities[i] = fishes[i].rb.velocity;
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
                GameObject fish = Instantiate(fishPrefabs[index], pos, Quaternion.identity, this.transform);
                // randomize fish velocity and direction
                Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                fish.transform.forward = direction;
                if (isAIEnabled)
                {
                    Vector3 velocity = direction * Random.Range(0, fish.GetComponent<Fish>().fishAttr.maxSpeed);
                    fish.GetComponent<Fish>().rb.velocity = velocity;
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
                fishAttributes = FishAttributes,
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
                fishAttributes = FishAttributes
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
                fishAttributes = FishAttributes,
                positions = FishPositions,
                velocities = FishVelocities,
                forces = FishForces,
                center = center,
                extents = extents,
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
                var fishRigidbody = fishes[i].rb;
                FishAttribute fishAttr = FishAttributes[i];
                if (fishes[i].fishAttr.isInPoi)
                {
                    //fishRigidbody.useGravity = fishAttr.useGravity;
                    //fishRigidbody.velocity = new Vector3(0, 0, 0);
                    //fishRigidbody.angularVelocity = new Vector3(0, 0, 0);
                    //if (fishes[i].fishAttr.isInBowl)
                    //{
                    //    fishAttr.isFishInBowl = true;
                    //    fishRigidbody.useGravity = false;
                    //    if (fishRigidbody.velocity.magnitude > fishSetting.speedInBowl)
                    //        fishRigidbody.velocity *= 0.5f;
                    //    fishRigidbody.angularVelocity *= 0.5f;
                    //    UpdateFishRotation();
                    //}
                    //else if (fishRigidbody.useGravity)
                    //{
                    //    fishAttr.isFishInBowl = false;
                    //    fishRigidbody.angularVelocity = Vector3.zero;
                    //}
                    //else
                    //{
                    //    fishRigidbody.velocity *= 0.1f;
                    //    fishRigidbody.angularVelocity = Vector3.zero;
                    //}
                }
                else
                {
                    fishRigidbody.useGravity = fishAttr.useGravity;
                    if (fishes[i].fishAttr.isInBowl)
                    {
                        fishAttr.isFishInBowl = true;
                        fishRigidbody.useGravity = true;
                        if (fishRigidbody.velocity.magnitude > fishSetting.speedInBowl)
                            fishRigidbody.velocity *= 0.5f;
                        fishRigidbody.angularVelocity *= 0.5f;
                        UpdateFishRotation();
                    }
                    else if (fishRigidbody.useGravity)
                    {
                        fishAttr.isFishInBowl = false;
                        fishRigidbody.angularVelocity = Vector3.zero;
                    }
                    else // fish is in water
                    {
                        fishAttr.isFishInBowl = false;
                        Vector3 vel = fishRigidbody.velocity;
                        vel.y *= 0.8f;
                        Vector3 force = FishForces[i];
                        fishRigidbody.AddForce(force);
                        if (vel.magnitude > fishes[i].fishAttr.maxSpeed)
                            fishRigidbody.velocity = vel.normalized * fishes[i].fishAttr.maxSpeed;
                        else
                            fishRigidbody.velocity = vel;
                        if (vel.magnitude < 1e-2f)
                        {
                            Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                            fishRigidbody.AddForce(direction * Random.Range(0, fishes[i].fishAttr.maxSpeed) / Time.fixedDeltaTime);
                        }
                        UpdateFishRotation();
                    }
                }
                fishAttr.isInPoi = fishes[i].fishAttr.isInPoi;
                FishAttributes[i] = fishAttr;
            }
        }
        void UpdateFishRigidBodyWOAI()
        {
            for (int i = 0; i < fishes.Length; i++)
            {
                var fishRigidbody = fishes[i].rb;
                FishAttribute fishAttr = FishAttributes[i];
                fishRigidbody.useGravity = fishAttr.useGravity;
                fishRigidbody.velocity = new Vector3(0, 0, 0);
                fishRigidbody.angularVelocity = new Vector3(0, 0, 0);
                if (fishes[i].fishAttr.isInBowl)
                {
                    fishAttr.isFishInBowl = true;
                    fishRigidbody.useGravity = false;
                    if (fishRigidbody.velocity.magnitude > fishSetting.speedInBowl)
                        fishRigidbody.velocity *= 0.5f;
                    fishRigidbody.angularVelocity *= 0.5f;
                    UpdateFishRotation();
                }
                else if (fishRigidbody.useGravity)
                {
                    fishAttr.isFishInBowl = false;
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
