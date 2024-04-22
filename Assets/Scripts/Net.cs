using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

namespace Kingyo
{
    public class Net : MonoBehaviour
    {
        [SerializeField]
        Renderer render;
        public float threshold = 0.1f;

        private void Start()
        {
            render = GetComponent<Renderer>();
        }
        void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal * 10, Color.red);
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
            Debug.Log("Break Net");
            //StartCoroutine(destroyParent(3f));
            //GameObject.Destroy(this.gameObject);
            render.enabled = false;
        }

        IEnumerator destroyParent(float time)
        {
            yield return new WaitForSeconds(time);
            GameObject.Destroy(this.transform.parent);
        }
    }
}