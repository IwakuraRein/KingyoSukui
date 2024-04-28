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
        [SerializeField]
        Poi poi;
        [SerializeField]
        Collider[] colliders;
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
                    BreakNet();
                }
            }
        }

        // Update is called once per frame
        public void EnableNet()
        {
            render.enabled = true;
            foreach (var collider in colliders)
            {
                collider.enabled = true;
            }
        }

        public void BreakNet()
        {
            Debug.Log("Break Net");
            //StartCoroutine(destroyParent(3f));
            //GameObject.Destroy(this.gameObject);
            render.enabled = false;
            foreach(var collider in colliders)
            {
                collider.enabled = false;
            }
            if (poi) GameManager.Instance.onPoiNetBreak(poi);
        }

        IEnumerator destroyParent(float time)
        {
            yield return new WaitForSeconds(time);
            GameObject.Destroy(this.transform.parent);
        }
    }
}