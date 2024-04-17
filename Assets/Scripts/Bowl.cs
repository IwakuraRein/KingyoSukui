using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    public class Bowl : MonoBehaviour
    {
        private HashSet<GameObject> fishesInBowl = new HashSet<GameObject>();
        private int scoreTotal;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Fish"))
            {
                fishesInBowl.Add(other.gameObject);
                scoreTotal += other.gameObject.GetComponent<Fish>().score;
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Fish"))
            {
                fishesInBowl.Remove(other.gameObject);
                scoreTotal -= other.gameObject.GetComponent<Fish>().score;
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

