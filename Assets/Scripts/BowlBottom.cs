using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kingyo
{
    public class BowlBottom : MonoBehaviour
    {
        [SerializeField]
        Transform pivot;

        List<Fish> snapped = new List<Fish>();
        void FixedUpdate()
        {
            //if (Vector3.Dot(transform.up, Vector3.up) < 0)
            //{
            //    foreach (var f in snapped)
            //    {
            //        f.transform.parent = null;
            //        f.rb.isKinematic = false;
            //        f.rb.constraints = RigidbodyConstraints.None;
            //    }
            //}
        }

        //private void OnDestroy()
        //{
        //    foreach (var f in snapped)
        //    {
        //        DestroyImmediate(f);
        //        Debug.Log($"{f} destroy.");
        //    }
        //}

        private void OnCollisionEnter(Collision collision)
        {
            var fish = collision.rigidbody.gameObject.GetComponent<Fish>();
            if (fish != null && fish.transform.parent != pivot)
            {
                fish.transform.SetParent(pivot, true);
                //fish.transform.localPosition = Vector3.zero;
                //fish.transform.localRotation = Quaternion.identity;
                fish.rb.velocity = Vector3.zero;
                fish.rb.angularVelocity = Vector3.zero;
                fish.rb.isKinematic = true;
                fish.rb.constraints = RigidbodyConstraints.FreezeAll;
                fish.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                snapped.Add(fish);
                Logger.Log($"{fish} sanpped to the bowl!");
            }
        }
    }
}