using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using Gridly.Internal;
using System;
using Gridly;
using System.Linq;
using UnityEngine.UI;

public class initiateCaptures : MonoBehaviour
{

    public int WaitBetweenScenes = 5;

    private string screenshotPath;

    private int index = 0;

    private List<LangSupport> finishedLangs = new List<LangSupport>();



    LangSupport currentLanguage
    {
        get => Project.Singleton.TargetLanguage;
        set => Project.Singleton.TargetLanguage = value;
    }

    public static List<LangSupport> languagesSupport => Project.Singleton.langSupports;

    void Awake()
    {

        DontDestroyOnLoad(transform.gameObject);
        screenshotPath = UserData.Singleton.ScreenshotPath.ToString();


    }

    IEnumerator Start()
    {
        GridlyLogging.Log($"Loaded Lang: {Project.Singleton.TargetLanguage.name}");

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {

            foreach (string lang in Project.Singleton.LangsToTakeScreenshotList)
            {
                NextLanguage(lang);

                string sceneName = Path.GetFileNameWithoutExtension(scene.path);

                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

                while (!operation.isDone)
                {
                    yield return null;
                }


                takeScreenshot(Project.Singleton.TargetLanguage.name);
                yield return new WaitForSeconds((float)1);



            }


            yield return new WaitForSeconds((float)WaitBetweenScenes);

            //UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();


    }

    private void takeScreenshot(string lang)
    {

        int superSize = 3;

        string filepath = EditorSceneManager.GetActiveScene().path;
        string levelname = SceneManager.GetActiveScene().name;

        filepath = Path.GetDirectoryName(filepath);
        string screenshotDirPath = screenshotPath + "\\" + lang;

        if (!Directory.Exists(screenshotDirPath))
        {
            Directory.CreateDirectory(screenshotDirPath);
        }
        string filename = screenshotDirPath + "\\" + levelname + ".png";
        ScreenCapture.CaptureScreenshot(filename, superSize);

    }

    private static List<string> getSceneNames()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            names.Add(Path.GetFileNameWithoutExtension(scene.path));
        }
        return names;
    }

    private void Refresh()
    {
        Translator[] translareText = FindObjectsOfType<Translator>();
        foreach (var i in translareText)
        {
            i.Refresh();
        }

    }
    public void NextLanguage(string lang)
    {
        index++;
        if (index == languagesSupport.Count)
            index = 0;

        currentLanguage = languagesSupport.Where(ls => ls.name == lang).FirstOrDefault();
        Refresh();


    }


}


[CustomEditor(typeof(initiateCaptures))]

public class LookAtPointEditor : Editor
{
    SerializedProperty WaitBetweenScenes;


    void OnEnable()
    {
        WaitBetweenScenes = serializedObject.FindProperty("WaitBetweenScenes");



    }
    public static bool toggle = false;
    public override void OnInspectorGUI()
    {
        List<string> langs = new List<string>();

        foreach (LangSupport lang in Project.Singleton.langSupports)
        {
            if (!Project.Singleton.LangsToTakeScreenshotList.Contains(lang.name))
            {
                langs.Add(lang.name);
            }
        }

        serializedObject.Update();
        EditorGUILayout.PropertyField(WaitBetweenScenes);
        EditorGUILayout.BeginHorizontal();
        Project.Singleton.LastSelectedLangIndexToAdd = EditorGUILayout.Popup(Project.Singleton.LastSelectedLangIndexToAdd, langs.ToArray());
        if (GUILayout.Button("Add", GUILayout.MinWidth(100), GUILayout.MaxWidth(200)))
        {
            Project.Singleton.LangsToTakeScreenshotList.Add(langs[Project.Singleton.LastSelectedLangIndexToAdd]);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        Project.Singleton.LastSelectedLangIndexToRemove = EditorGUILayout.Popup(Project.Singleton.LastSelectedLangIndexToRemove, Project.Singleton.LangsToTakeScreenshotList.ToArray());
        if (GUILayout.Button("Remove", GUILayout.MinWidth(100), GUILayout.MaxWidth(200)))
        {
            Project.Singleton.LangsToTakeScreenshotList.Remove(Project.Singleton.LangsToTakeScreenshotList[Project.Singleton.LastSelectedLangIndexToRemove]);
        }
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.LabelField("Added languages");
        foreach (string lang in Project.Singleton.LangsToTakeScreenshotList)
        {
            EditorGUILayout.LabelField(lang);
        }



        serializedObject.ApplyModifiedProperties();
    }




}