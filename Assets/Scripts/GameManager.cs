using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Kingyo
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField]
        private GameObject poi;
        [SerializeField]
        private GameObject bowl;
        [SerializeField]
        private GameObject player;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger));
            // if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5f)
            // {
            //     poi.SetActive(true);
            // } else
            // {
            //     poi.SetActive(false);
            // }
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                if (poi.activeSelf)
                {
                    poi.SetActive(false);
                }
                else
                {
                    poi.SetActive(true);
                }
            }
        }
    }
}