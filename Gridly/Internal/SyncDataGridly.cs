using UnityEngine;
using Gridly.Internal;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gridly
{
    public class SyncDataGridly : MonoBehaviour
    {
        public static SyncDataGridly Singleton { get; private set; }
        
        private static GridlyFunction m_GridlyFunction = new GridlyFunction();

        
        public static int ProgressNumberTotal => GridlyFunction.DownloadedTotal;
        public static int ProgressDone => GridlyFunction.Download;
        public static float Progress
        {
            get
            {
                if (ProgressNumberTotal == 0) return 0;
                return (float)ProgressDone / ProgressNumberTotal;
            }
        }
        
        
        [FormerlySerializedAs("syncOnAwake")]
        [SerializeField] private bool m_SyncOnAwake = true;
        
        [FormerlySerializedAs("onDowloadComplete")]
        [SerializeField] private UnityEvent m_OnDownloadComplete;
        

        private void Awake()
        {
            if (Singleton != null && Singleton != this)
            {
                Destroy(gameObject);
                return; 
            }
            
            Singleton = this;

            if (m_SyncOnAwake)
                StartSync();
        }

        public void StartSync()
        {
            if (string.IsNullOrEmpty(UserData.Singleton.KeyAPI))
            {
                GridlyLogging.Log("Please enter your api key to use this feature");
                return;
            }

            // Apply new data when finish setup userlocal
            m_GridlyFunction.finishAction = Finish;
        }

        private void Finish()
        {
            m_OnDownloadComplete.Invoke();
        }

        private void Update()
        {
            GridlyFunction.process?.Invoke();
        }
    }
}