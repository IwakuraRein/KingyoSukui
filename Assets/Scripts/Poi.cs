using System.Collections;
using System.Collections.Generic;
using Kingyo;
using UnityEngine;

public class Poi : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hands"))
        {
            if (!GameManager.Instance.hasPoiOnHand)
            {
                if (other.gameObject.transform.name == "RightHandAnchor" && !GameManager.Instance.rightHandOnUse) {
                    GameManager.Instance.rightHandOnUse = true;
                    GameManager.Instance.PoiOnRight = true;
                } else if (other.gameObject.transform.name == "LeftHandAnchor" && !GameManager.Instance.leftHandOnUse) {
                    GameManager.Instance.leftHandOnUse = true;
                    GameManager.Instance.PoiOnLeft = true;
                }
                GameManager.Instance.setCurrentPoi(this.gameObject);
                this.transform.parent = other.gameObject.transform;
                GameManager.Instance.hasPoiOnHand = true;
            }
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
