using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    public class Bowl : MonoBehaviour
    {
        private HashSet<GameObject> fishesInBowl = new HashSet<GameObject>();

        private void OnTriggerEnter(Collider other)
        {
            // 确保进入的是鱼
            if (other.CompareTag("Fish"))
            {
                fishesInBowl.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // 确保离开的是鱼
            if (other.CompareTag("Fish"))
            {
                fishesInBowl.Remove(other.gameObject);
            }
        }

        // 公共方法，返回碗中鱼的数量
        public int GetFishCount()
        {
            return fishesInBowl.Count;
        }
    }
}

