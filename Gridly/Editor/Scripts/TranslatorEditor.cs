using UnityEngine;
using UnityEditor;

namespace Gridly.Internal
{
    [CustomEditor(typeof(Translator))]
    public class TranslatorEditor : Editor
    {
        private GridlyArrData popupData = new GridlyArrData();
        private Column chosenColum;
        private string search = "";
        
        private void OnEnable()
        {
            search = "";
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Translator translator = (Translator)target;

            if (!popupData.IsInitialized)
            {
                Refresh();
            }

            if (Project.Singleton.grids.Count == 0)
                return;

            //Db
            GUILayout.Space(10);


            //Grid
            if (popupData.indexGrid == -1)
                return;
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Grid");
            EditorGUI.BeginChangeCheck();
            try
            {
                translator.grid = popupData.gridArr[EditorGUILayout.Popup(popupData.indexGrid, popupData.gridArr)];
            }
            catch
            {
                popupData.indexGrid = 0;
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(translator);
                Refresh();
            }
            GUILayout.EndHorizontal();
            

            GUILayout.Space(10);


            //Key


            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            
            search = GUILayout.TextField(search, GridlyUtilityEditor.SearchTextStyle);
            
            if (EditorGUI.EndChangeCheck())
            {
                popupData.searchKey = search;
                Refresh();
            }

            if (popupData.keyArr == null)
                return;

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Key");
            EditorGUI.BeginChangeCheck();
            try
            {
                translator.key = popupData.keyArr[EditorGUILayout.Popup(popupData.indexKey, popupData.keyArr)];
            }
            catch 
            { 
                GUILayout.Label("can't find key"); 
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(translator);
                Refresh();

            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            try
            {
                Languages main = UserData.Singleton.mainLangEditor;
                GUILayout.BeginHorizontal();
                GUILayout.Label(main.ToString() + ": ");
                if (chosenColum != null)
                {
                    chosenColum.text = GUILayout.TextArea(chosenColum.text);
                    if(GUILayout.Button(new GUIContent() {text = "Export" , tooltip = "Export text to Girdly" }, GUILayout.MinWidth(60)))
                    {
                        GridlyFunctionEditor.editor.UpdateRecordLang(popupData.ChosenRecord, popupData.Grid.choesenViewID, UserData.Singleton.mainLangEditor);
                    }
                }
    
                GUILayout.EndHorizontal();
            }
            catch { }
        }
        
        private void Refresh()
        {
            Translator translator = (Translator)target;
            popupData.RefreshAll(translator.grid, translator.key);

            try
            {
                Languages main = UserData.Singleton.mainLangEditor;
                chosenColum = popupData.ChosenRecord.columns.Find(x => x.columnID == main.ToString());
            }
            catch { }
        }
    }
}