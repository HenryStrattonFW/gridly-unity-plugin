using UnityEngine;
using UnityEditor;

namespace Gridly.Internal
{
    public static class GridlyUtilityEditor
    {
        public static GUIStyle SearchTextStyle 
        {
#if UNITY_2022_3_OR_NEWER
            get => GUI.skin.GetStyle("ToolbarSearchTextField");
#else
            get => GUI.skin.GetStyle("ToolbarSeachTextField");
#endif
        }

        public static void Save(this Object i)
        {
            EditorUtility.SetDirty(i);
            AssetDatabase.SaveAssets();
        }

        public static void SetDirty(this Object i)
        {
            EditorUtility.SetDirty(i);
        }
    }

}