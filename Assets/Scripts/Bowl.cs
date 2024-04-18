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
                // get its parent fish component and add its score to the total
                scoreTotal += other.gameObject.transform.parent.gameObject.GetComponent<Fish>().score;
            }
            // if (other.CompareTag("Hands"))
            // {
            //     if (!GameManager.Instance.hasBowlOnHand )
            //     {
            //         if (other.gameObject.transform.name == "RightHandAnchor" && !GameManager.Instance.rightHandOnUse) {
            //             GameManager.Instance.rightHandOnUse = true;
            //         } else if (other.gameObject.transform.name == "LeftHandAnchor" && !GameManager.Instance.leftHandOnUse) {
            //             GameManager.Instance.leftHandOnUse = true;
            //         }
            //         GameManager.Instance.setCurrentPoi(this.gameObject);
            //         this.transform.parent = other.gameObject.transform;
            //         GameManager.Instance.hasPoiOnHand = true;
            //     }
            // }   

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Fish"))
            {
                fishesInBowl.Remove(other.gameObject);
                scoreTotal -= other.gameObject.transform.parent.gameObject.GetComponent<Fish>().score;
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

