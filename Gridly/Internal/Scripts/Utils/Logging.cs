using UnityEngine;

namespace Gridly.Internal
{
    public static class GridlyLogging
    {
        public enum LoggingMode
        {
            Verbose,
            ErrorsOnly,
            None
        }

        public static void Log(string msg)
        {
            if (Project.Singleton.LoggingMode != LoggingMode.Verbose)
            {
                return;
            }
            Debug.Log(msg);
        }
        
        public static void LogError(string msg)
        {
            if (Project.Singleton.LoggingMode != LoggingMode.ErrorsOnly &&
                Project.Singleton.LoggingMode != LoggingMode.Verbose)
            {
                return;
            }
            Debug.LogError(msg);
        }
    }
}
