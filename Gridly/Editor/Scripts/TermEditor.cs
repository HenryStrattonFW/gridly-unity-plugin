using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace Gridly.Internal
{
    public class TermEditor : GridlyEditor
    {
        public static TermEditor window;

        private Vector2 m_ScrollPosition;
        private static int gridSelectIndex;
        private static int pathSelectedIndex;

        private static Grid Grid
        {
            get
            {
                try
                {
                    return Project.Singleton.grids[gridSelectIndex];
                }
                catch
                {
                    gridSelectIndex = 0;
                    Refesh();
                    return Project.Singleton.grids[gridSelectIndex];
                }
            }
        }


        [MenuItem("Tools/Gridly/String Editor", false, 0)]
        private static void Init()
        {
            if (Project.Singleton.grids.Count == 0)
            {
                EditorUtility.DisplayDialog("No Data", "You need to setup data before using this feature", "Okay");
                return;
            }

            window = (TermEditor)GetWindow(typeof(TermEditor), false, $"String Editor - {GridlyInfo.ver}");
            window.minSize = new Vector2(400, 400);
            
            PopulateDataSendList();
            Refesh();
            
            window.Show();
        }

        public static void RepaintThis()
        {
            if (window != null)
            {
                window.Repaint();
            }
        }

        public static void PopulateDataSendList()
        {
            Project.Singleton.DataToSend.Clear();
            Project.Singleton.DataToSend.Add("Source text");
            Project.Singleton.DataToSend.Add("Source text and screenshots");

            foreach (LangSupport lang in Project.Singleton.langSupports)
            {
                Project.Singleton.DataToSend.Add(lang.name);
            }
        }


        static string gridSelect = "";
        static string pathSelect = "";
        static List<Record> records;
        static GridlyArrData popupData = new GridlyArrData();
        static string[] paths;
        public static void Refesh()
        {

            OnEnableEditor();
            init = true;
            popupData.RefreshAll(gridSelect, null);
            RefeshList();
            paths = getSceneList().ToArray();
            GridlyLogging.Log($"record count {records.Count}");
        }
        
        static void RefeshList()
        {
            if (popupData.indexGrid == -1)
                return;
            records = GetRecord(Grid);
            TermListLegth = records.Count;

        }

        private static List<string> getSceneList()
        {
            List<string> list = new List<string>();
            list.Add("");
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                list.Add(Path.GetFileNameWithoutExtension(scene.path));

            }
            return list;
        }

        static float mRowSize = 40;
        static int TermListLegth = 0;
        float scrollHeight;
        string selectRecordID;
        float YPos;
        float ScrolSizeY => 280 + position.height - 415;
        static string search;
        float res = 0;
        int countReset = 0;
        private static List<Record> GetRecord(Grid grid)
        {
            if (!string.IsNullOrEmpty(search))
            {
                return grid.records.FindAll(x => x.recordID.ToLower().Contains(search.ToLower()) |
                    x.columns.Find(x => x.columnID == UserData.Singleton.mainLangEditor.ToString() & x.text.ToLower().Contains(search.ToLower())) != null
                );
            }
            return grid.records;
        }
        static bool init;

        public void OnGUI()
        {

            if (!init)
            {
                Refesh();
            }

            GUILayout.Space(20);

            #region select data



            EditorGUI.BeginChangeCheck();
            gridSelectIndex = EditorGUILayout.Popup("Grid", popupData.indexGrid, popupData.gridArr);
            gridSelect = popupData.gridArr[gridSelectIndex];
            if (EditorGUI.EndChangeCheck())
            {
                m_ScrollPosition = Vector2.zero;
                UserData.Singleton.Save();
                Refesh();
                return;
            }


            #endregion
            GUILayout.Space(10);

            #region Search
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            search = GUILayout.TextField(search, GridlyUtilityEditor.SearchTextStyle);
            if (EditorGUI.EndChangeCheck())
            {
                RefeshList();
                //selectRecordID = "";
                m_ScrollPosition = Vector3.zero;
                //showCreateKey = false;
            }
            GUILayout.Label("Search", GUILayout.Width(60));

            if (GUILayout.Button(new GUIContent() { text = "▲", tooltip = "Scroll to the top" }, GUILayout.Width(35)))
            {
                m_ScrollPosition.y = 0;
            }

            if (GUILayout.Button(new GUIContent() { text = "▼", tooltip = "Scroll to the bottom" }, GUILayout.Width(35)))
            {
                m_ScrollPosition.y = TermListLegth * mRowSize;
                showCreateKey = true;
                OnGUI();
            }

            GUILayout.EndHorizontal();

            #endregion

            GUILayout.Space(5);


            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, TextStyle, GUILayout.MinHeight(ScrolSizeY), GUILayout.ExpandHeight(true));


            if (Event.current != null && Event.current.type == EventType.Layout)
                scrollHeight = m_ScrollPosition.y;

            #region draw record

            YPos = 0;

            int nDraw = 0;
            float nSkipStart = 0;
            bool spaceStart = false;

            for (int i = 0; i < TermListLegth; i++)
            {


                if (YPos < scrollHeight - mRowSize && !spaceStart)
                {
                    spaceStart = true;
                    nSkipStart = ((scrollHeight - mRowSize) / mRowSize);
                    YPos = scrollHeight - mRowSize;
                    i = (int)nSkipStart;
                    GUILayout.Space(nSkipStart * mRowSize);
                }

                GUILayout.Space(mRowSize);
                bool selected = false;
                float befforY = YPos;
                try
                {
                    DrawRecord(i + 1, records[i], ref selected);
                }
                catch
                {
                    break;
                }
                YPos += mRowSize;


                if (selected)
                {
                    if (countReset > 0)
                    {
                        res = GUILayoutUtility.GetLastRect().yMin - befforY;
                        countReset -= 1;
                    }

                    YPos += res;


                }

                nDraw++;

                //cut
                if (YPos > scrollHeight + ScrolSizeY)
                {
                    break;
                }


            }
            GUILayout.Space((TermListLegth + 1 - (nDraw + nSkipStart)) * (mRowSize));
            DrawAddRecord();

            #endregion
            GUILayout.EndScrollView();


            #region Button

            GUILayout.Space(5);
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));

            if (GUILayout.Button(new GUIContent() { text = "Pull selected Grid", tooltip = "Pull data of this grid from Gridly" }))
            {
                if (EditorUtility.DisplayDialog("Confirm Import Grid", "Are you sure you want to import Grid from Gridly?. It will overwrite the old data including translations.", "Yes", "Cancel"))
                {
                    popupData.Grid.records.Clear();
                    Refesh();
                    RepaintThis();

                    GridlyFunctionEditor.editor.doneOneProcess += Refesh;
                    GridlyFunctionEditor.editor.doneOneProcess += RepaintThis;

                    GridlyFunctionEditor.editor.RefreshDownloadTotal();
                    GridlyFunctionEditor.editor.SetupRecords(popupData.Grid, 0);
                }
            }

            if (GUILayout.Button(new GUIContent() { text = "Pull all Grids", tooltip = "Pull all data from Gridly to update grids you have in unity" }))
            {
                if (EditorUtility.DisplayDialog("Confirm Export", "Are you sure you want to import all data from Gridly?. It will overwrite the old data including translations.", "Yes", "Cancel"))
                {
                    GridlyFunctionEditor.editor.doneOneProcess += Refesh;
                    GridlyFunctionEditor.editor.doneOneProcess += RepaintThis;
                    GridlyFunctionEditor.editor.SetupDatabases();
                }
            }


            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Language screenshots/All target language screenshots"), Project.Singleton.DataToSendSelectedItems.Contains("All target language screenshots"), OnColorSelected, "All target language screenshots");

            foreach (string data in Project.Singleton.DataToSendSelectedItems.ToList())
            {
                if (!Project.Singleton.DataToSend.Contains(data))
                {
                    Project.Singleton.DataToSendSelectedItems.Remove(data);
                }
            }

            foreach (string item in Project.Singleton.DataToSend)
            {
                if (item.Length > 4)
                {
                    menu.AddItem(new GUIContent(item), Project.Singleton.DataToSendSelectedItems.Contains(item), OnColorSelected, item);
                }
                else
                {
                    if (item != UserData.Singleton.mainLangEditor.ToString())
                    {
                        menu.AddItem(new GUIContent("Language screenshots/" + item), Project.Singleton.DataToSendSelectedItems.Contains(item), OnColorSelected, item);
                    }
                }
            }




            if (EditorGUILayout.DropdownButton(new GUIContent() { text = "Push options", tooltip = "Select the languages to send their screenshots to gridly." }, 0, EditorStyles.miniButton))
            {
                menu.ShowAsContext();
            }


            if (GUILayout.Button(new GUIContent() { text = "Push data", tooltip = "Push source strings and screenshots with selected languages to Grdily" }))
            {
                string msg = "You are going to push data with these settings:\n";
                foreach (string data in Project.Singleton.DataToSendSelectedItems)
                {
                    if (data.Length == 4)
                    {
                        if (!Project.Singleton.DataToSendSelectedItems.Contains("All target language screenshots"))
                            msg += "- " + data + " screenshots" + "\n";
                    }
                    else
                    {
                        msg += "- " + data + "\n";
                    }
                }
                if (EditorUtility.DisplayDialog("Push data to Gridly", msg, "OK", "Cancel"))
                {
                    GridlyFunctionEditor.editor.AddUpdateRecordAll(popupData.Grid.records, popupData.Grid.choesenViewID, false, true);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUILayout.FlexibleSpace();
            Project.Singleton.SendIfChanged = GUILayout.Toggle(Project.Singleton.SendIfChanged, new GUIContent { text = "Push only changed records", tooltip = "Send records that has been changed in Unity to Gridly." });
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(Project.Singleton);
            GUILayout.EndHorizontal();


            #endregion
        }
        bool showCreateKey = true;
        string nameNewKey;
        void OnColorSelected(object selectedDataItem)
        {
            string selectedText = selectedDataItem.ToString();

            if (selectedText.Length == 4 && Project.Singleton.DataToSendSelectedItems.Contains("All target language screenshots"))
            {
                Project.Singleton.DataToSendSelectedItems.Remove("All target language screenshots");
            }

            if (selectedText == "All target language screenshots")
            {
                bool allLangAdded = true;
                foreach (LangSupport lang in Project.Singleton.langSupports)
                {

                    if (lang.name != UserData.Singleton.mainLangEditor.ToString())
                    {
                        if (!Project.Singleton.DataToSendSelectedItems.Contains(lang.name))
                        {
                            allLangAdded = false;
                        }
                    }
                }

                if (allLangAdded)
                {
                    foreach (LangSupport lang in Project.Singleton.langSupports)
                    {
                        if (Project.Singleton.DataToSendSelectedItems.Contains(lang.name))
                        {
                            Project.Singleton.DataToSendSelectedItems.Remove(lang.name);
                        }
                    }

                }
                else
                {
                    foreach (LangSupport lang in Project.Singleton.langSupports)
                    {
                        if (lang.name != UserData.Singleton.mainLangEditor.ToString())
                        {
                            if (!Project.Singleton.DataToSendSelectedItems.Contains(lang.name))
                            {
                                Project.Singleton.DataToSendSelectedItems.Add(lang.name);
                            }
                        }
                    }
                }

            }

            GridlyLogging.Log(selectedText);

            if (Project.Singleton.DataToSendSelectedItems.Contains(selectedText))
            {
                Project.Singleton.DataToSendSelectedItems.Remove(selectedText);
            }
            else
            {
                Project.Singleton.DataToSendSelectedItems.Add(selectedText);
            }

            if (!Project.Singleton.DataToSendSelectedItems.Where(d => d.Contains("Source")).Any())
            {
                Project.Singleton.DataToSendSelectedItems.Remove(UserData.Singleton.mainLangEditor.ToString());
            }

            EditorUtility.SetDirty(Project.Singleton);

        }
        void DrawAddRecord()
        {

            GUI.backgroundColor = new Color(1, 1, 1, 0.4f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(30)))
            {
                showCreateKey = !showCreateKey;
            }
            GUI.color = Color.white;
            if (showCreateKey) ShowCreateKey();
            GUILayout.EndHorizontal();

            void ShowCreateKey()
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                nameNewKey = EditorGUILayout.TextField(nameNewKey, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Create Record", "toolbarbutton", GUILayout.ExpandWidth(false)))
                {


                    Record record = new Record();
                    record.recordID = nameNewKey;
                    GridlyFunctionEditor.editor.AddRecord(record, Grid.choesenViewID);

                    foreach (var i in Project.Singleton.langSupports)
                    {
                        record.columns.Add(new Column(i.languagesSupport.ToString(), ""));
                    }

                    Grid.records.Add(record);
                    EditorUtility.SetDirty(Project.Singleton);

                    showCreateKey = false;
                    selectRecordID = nameNewKey;
                    nameNewKey = "";
                    RefeshList();
                }
            }

        }

        void DrawRecord(int i, Record record, ref bool selected)
        {
            selected = selectRecordID == record.recordID;
            if (selected)
            {
                Color color = Color.Lerp(Color.cyan, Color.white, 0.3f);
                color.a = 0.3f;
                GUI.backgroundColor = color;
                SelectStyle.normal.textColor = Color.white;
            }
            else
            {
                GUI.backgroundColor = darkLightColor;
                SelectStyle.normal.textColor = new Color(1, 1, 1, 0.2f);
            }


            Rect rect = new Rect(2, YPos, position.width, mRowSize);
            //draw record
            if (GUI.Button(rect, new GUIContent(i + ". " + record.recordID + " |  " + GetShowText(ref record).Replace('\n', ' ')), SelectStyle))
            //if(GUILayout.Button(new GUIContent(i + ". " + record.recordID + " |  " + GetShowText(ref record).Replace('\n', ' ')), GUILayout.Width(position.width-15), GUILayout.Height(mRowSize)))
            {
                countReset = 2;
                if (selectRecordID == record.recordID)
                    selectRecordID = "";
                else selectRecordID = record.recordID;

                //-turn off rename feature
                isRename = false;
                theNameToRename = "";

                //GUI.FocusControl(null);
            }


            if (selected)
            {
                DrawDetailRecord(record);
            }

        }

        string GetShowText(ref Record record)
        {

            foreach (var i in record.columns)
            {
                if (i.columnID == UserData.Singleton.mainLangEditor.ToString())
                    return LimitText(ref i.text);
            }
            return string.Empty;
        }

        string LimitText(ref string input)
        {
            string final = "";
            const int length = 50;
            for (int i = 0; i < input.Length; i++)
            {
                if (i >= length)
                {
                    final += "...";
                    break;
                }
                final += input[i];
            }
            return final;
        }

        bool isRename;
        string theNameToRename = "";
        void DrawDetailRecord(Record record)
        {
            //return;
            EditorGUI.BeginChangeCheck();
            int selectedIndex = 0;
            if (ArrayUtility.IndexOf(paths, record.pathTag) >= 0)
            {
                selectedIndex = ArrayUtility.IndexOf(paths, record.pathTag);
            }
            pathSelectedIndex = EditorGUILayout.Popup("Path", selectedIndex, paths);
            pathSelect = paths[pathSelectedIndex];
            if (EditorGUI.EndChangeCheck())
            {
                record.pathTag = pathSelect;
                return;
            }
            foreach (var i in Project.Singleton.langSupports)
            {
                //IDE
                GUILayout.BeginHorizontal(TextStyle);
                string name = i.name;

                GUIContent contenLabel;
                if (i.languagesSupport == UserData.Singleton.mainLangEditor)
                {
                    name = "*" + name;
                    contenLabel = new GUIContent() { text = name, tooltip = "This is the source language" };
                }
                else contenLabel = new GUIContent() { text = name };





                float widthNameLang = 70 + (position.width - 400) * 0.1f;
                GUILayout.Label(contenLabel, EditorStyles.boldLabel, GUILayout.Width(widthNameLang));

                Column col = record.columns.Find(x => x.columnID == i.languagesSupport.ToString());
                string text = "";
                if (col != null)
                    text = col.text;


                EditorGUI.BeginChangeCheck();
                //text = GUILayout.TextField(text, GUILayout.MaxWidth(position.width-80 - widthNameLang));
                text = GUILayout.TextArea(text, GUILayout.MaxWidth(position.width - 80 - widthNameLang));

                if (EditorGUI.EndChangeCheck())
                {
                    if (col != null)
                    {
                        col.text = text;
                        EditorUtility.SetDirty(Project.Singleton);
                    }
                    else
                    {
                        GridlyLogging.Log($"you cannot edit this field because there is no column {i.languagesSupport} on Gridly. To fix this please login to Gridly and add column with columnID as \"{i.languagesSupport}\"");
                    }
                }

                if (GUILayout.Button(new GUIContent() { text = "Export", tooltip = "Export this field to Gridly" }, GUILayout.Width(60)))
                {
                    GridlyFunctionEditor.editor.UpdateRecordLang(record, Grid.choesenViewID, i.languagesSupport);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(2);

            if (isRename)
            {
                ShowRename();
                void ShowRename()
                {
                    GUILayout.BeginHorizontal(TextStyle);
                    GUILayout.Label(record.recordID);


                    theNameToRename = GUILayout.TextField(theNameToRename, GUILayout.Width(position.width * 0.7f));
                    //theNameToRename = GUILayout.TextField(theNameToRename, GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("Save", GUILayout.Width(50)))
                    {
                        selectRecordID = theNameToRename;
                        GridlyFunctionEditor.editor.DeleteRecord(record.recordID, Grid.choesenViewID, null); //delete old record before rename
                        record.recordID = theNameToRename;
                        GridlyFunctionEditor.editor.AddRecord(record, Grid.choesenViewID);
                        EditorUtility.SetDirty(Project.Singleton);
                    }

                    GUILayout.EndHorizontal();
                }
            }
            #region delete, rename
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Confirm delete", "Are you sure you want to delete this record", "Yes", "Cancel"))
                {
                    EditorUtility.SetDirty(UserData.Singleton);
                    GridlyFunctionEditor.editor.DeleteRecord(record.recordID, Grid.choesenViewID, null);
                    Grid.records.Remove(record);
                    Refesh();
                    EditorUtility.SetDirty(Project.Singleton);
                }
            }


            if (GUILayout.Button("Rename"))
            {
                isRename = !isRename;
            }
            GUILayout.EndHorizontal();
            
            #endregion
            
            GUILayout.Space(mRowSize);
        }
    }

}
