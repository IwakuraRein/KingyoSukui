using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kingyo
{
    public class Logger : MonoBehaviour
    {
        private static Logger instance;
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(Logger)) as Logger;
                    if (instance == null)
                    {
                        GameObject go = new GameObject("DebugLogger");
                        instance = go.AddComponent<Logger>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        [SerializeField]
        [Range(0, 50)]
        int qsize = 15;  // number of messages to keep
        [SerializeField]
        TextMeshProUGUI CustomLogText;
        [SerializeField]
        TextMeshProUGUI UnityLogText;
        Queue myLogQueue = new();
        Queue unityLogQueue = new();

        private void OnEnable()
        {
            Application.logMessageReceived += HandleUnityLog;
        }
        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleUnityLog;
        }
        public static void Log(object message, LogType type = LogType.Log)
        {
            if (!Application.isPlaying)
            {
                switch (type)
                {
                    default:
                    case LogType.Log:
                        Debug.Log(message); break;
                    case LogType.Warning:
                        Debug.LogWarning(message); break;
                    case LogType.Error:
                        Debug.LogError(message); break;
                }
            }
            else
            {
                Instance.myLogQueue.Enqueue("[" + type + "] : " + message.ToString());
                while (Instance.myLogQueue.Count > Instance.qsize)
                    Instance.myLogQueue.Dequeue();
            }
        }
        void HandleUnityLog(string logString, string stackTrace, LogType type)
        {
            unityLogQueue.Enqueue("[" + type + "] : " + logString);
            if (type == LogType.Exception)
                unityLogQueue.Enqueue(stackTrace);
            while (unityLogQueue.Count > qsize)
                unityLogQueue.Dequeue();
        }

        void Update()
        {
            //if (!Debug.isDebugBuild) return;
            if (CustomLogText) CustomLogText.text = string.Join("\n", myLogQueue.ToArray());
            if (UnityLogText) UnityLogText.text = string.Join("\n", unityLogQueue.ToArray());
        }
    }
}