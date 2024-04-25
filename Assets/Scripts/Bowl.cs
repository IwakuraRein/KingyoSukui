using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

namespace Kingyo
{
    public class Bowl : MonoBehaviour
    {
        public UltEvent<Bowl, Fish> OnFishEnterBowl = new UltEvent<Bowl, Fish>();
        public UltEvent<Bowl, Fish> OnFishExitBowl = new UltEvent<Bowl, Fish>();
        private HashSet<Fish> fishesInBowl = new HashSet<Fish>();
        private int scoreTotal;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Fish")
            {
                var fish = other.attachedRigidbody.gameObject.GetComponent<Fish>();
                if (fish != null)
                {
                    if (!fish.fishAttr.isInBowl)
                    {
                        fish.fishAttr.isInBowl = true;
                        OnFishEnterBowl?.Invoke(this, fish);
                        Debug.Log($"{fish} is in the bowl!");
                        fishesInBowl.Add(fish);
                        // get its parent fish component and add its score to the total
                        scoreTotal += fish.fishAttr.score;
                    }
                }
            } 
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Fish")
            {
                var fish = other.attachedRigidbody.gameObject.GetComponent<Fish>();
                if (fish != null)
                {
                    if (fish.fishAttr.isInBowl)
                    {
                        fish.fishAttr.isInBowl = false;
                        OnFishExitBowl?.Invoke(this, fish);
                        Debug.Log($"{fish} leaves the bowl!");
                        fishesInBowl.Remove(fish);
                        // get its parent fish component and add its score to the total
                        scoreTotal -= fish.fishAttr.score;
                    }
                }
            }
        }

        public int GetFishCount()
        {
            return fishesInBowl.Count;
        }

        public int GetScore()
        {
            return scoreTotal;
        }
    }
}

