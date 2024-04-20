using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

namespace Kingyo
{
    public class Net : MonoBehaviour
    {
        public float threshold = 0.1f;

        void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal * 10, Color.red);
                // 检测撞击力度
                if (collision.relativeVelocity.magnitude > threshold || collision.impulse.magnitude > threshold)
                {
                    BreakNet(contact.point);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void BreakNet(Vector3 point)
        {
            // 破网
            Debug.Log("Break Net");
            // 破网后，网消失
            GameObject.Destroy(this.gameObject);
        }
    }
}