using FatLib;
using UnityEngine;

namespace Gridly.Internal
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : Object
    {
        private static T singletonInstance;

        public static T Singleton 
        {
            get
            {
                if (singletonInstance == null)
                {
                    singletonInstance = Resources.Load<T>(typeof(T).Name);
                }
                return singletonInstance;
            }
        }
    }
}