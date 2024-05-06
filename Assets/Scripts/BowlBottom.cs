using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    public class BowlBottom : MonoBehaviour
    {
        [SerializeField] 
        Transform pivot;
        private void OnCollisionEnter(Collision collision)
        {
            var fish = collision.rigidbody.gameObject.GetComponent<Fish>();
            if (fish.transform.parent != pivot)
            {
                fish.transform.SetParent(pivot, true);
                //fish.transform.localPosition = Vector3.zero;
                //fish.transform.localRotation = Quaternion.identity;
                fish.rb.velocity = Vector3.zero;
                fish.rb.angularVelocity = Vector3.zero;
                fish.rb.isKinematic = true;
                fish.rb.constraints = RigidbodyConstraints.FreezeAll;
                Logger.Log($"{fish} sanpped to the bowl!");
            }
        }
    }
}