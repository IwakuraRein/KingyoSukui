using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField]
        private FishSetting fishSetting;
        public FishSetting FishSetting { get => fishSetting; }
        void Start()
        {
            CreateFish(fishSetting.Tank.transform.position, fishSetting.bounds, fishSetting.MaxFishCount);
        }
        void CreateFish(Vector3 center, Bounds bounds, int count)
        {
            fishes = new Fish[count];
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = center + new Vector3(Random.Range(-bounds.extents.x, bounds.extents.x),
                    Random.Range(-bounds.extents.y, bounds.extents.y),
                    Random.Range(-bounds.extents.z, bounds.extents.z));
                int index = Random.Range(0, fishPrefabs.Length);
                GameObject fish = Instantiate(fishPrefabs[index], pos, Quaternion.identity);
                fishes[i] = fish.GetComponent<Fish>();
            }
        }
        public void UpdateFish()
        {

        }
    }
}
