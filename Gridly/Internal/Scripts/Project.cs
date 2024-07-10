using System.Collections.Generic;
using Gridly.Internal;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using Grid = Gridly.Internal.Grid;
using LoggingMode = Gridly.Internal.GridlyLogging.LoggingMode;

namespace Gridly
{
    [System.Serializable]
    public class LangSupport
    {
        [FormerlySerializedAs("languagesSuport")] 
        public Languages languagesSupport;
        public string name;
        public string screenshotColumnId;

        public Font font;
        public TMP_FontAsset tmFont;
    }
    
    public class Project : ScriptableSingleton<Project>
    {
        private string chosenLangCodeName;
        private Languages chosenLanguageCode;
        
        public LangSupport TargetLanguage
        {
            set
            {
                SetChosenLanguageCode(value.languagesSupport);
            }
            get
            {
                if (languageLookup == null)
                    CreateLookups();
                
                return languageLookup.TryGetValue(chosenLanguageCode, out var lang) 
                    ? lang 
                    : langSupports[0];
            }
        }

        [SerializeField] private LoggingMode m_LoggingMode = LoggingMode.Verbose;
        public LoggingMode LoggingMode => m_LoggingMode;
        
        [HideInInspector]
        public List<Grid> grids;
        //[HideInInspector]
        public List<LangSupport> langSupports;
        [HideInInspector]
        public List<string> LangsToTakeScreenshotList;
        [HideInInspector]
        public int LastSelectedLangIndexToAdd = 0;
        [HideInInspector]
        public int LastSelectedLangIndexToRemove = 0;
        [HideInInspector]
        public List<string> DataToSend;
        [HideInInspector]
        public List<string> DataToSendSelectedItems;
        [HideInInspector]
        public bool SendIfChanged = false;

        private Dictionary<string, Grid> gridLookup;
        private Dictionary<Languages, LangSupport> languageLookup;

        public event System.Action OnLanguageChanged;

        private void CreateLookups()
        {
            languageLookup = new Dictionary<Languages, LangSupport>();
            foreach (var lang in langSupports)
            {
                languageLookup.Add(lang.languagesSupport, lang);
            }

            gridLookup = new Dictionary<string, Grid>();
            foreach (var grid in grids)
            {
                gridLookup.Add(grid.nameGrid, grid);
            }
        }

        public Grid GetGridByName(string gridName)
        {
            if (gridLookup == null) CreateLookups();
            
            return gridLookup.TryGetValue(gridName, out var grid) 
                ? grid 
                : null;
        }
        
        public Grid GetGridByIndex(int index)
        {
            if (index < 0 || index >= grids.Count) return null;
            return grids[index];
        }
        
        public int GetChosenLanguageIndex
        {
            get
            {
                for (var i = 0; i < langSupports.Count; i++)
                {
                    if (langSupports[i].languagesSupport == chosenLanguageCode)
                        return i;
                }
                return 0;
            }
        }

        public void SetChosenLanguageCode(Languages language)
        {
            chosenLanguageCode = language;
            chosenLangCodeName = language.ToString();
            OnLanguageChanged?.Invoke();
        }
        
        private void SetChosenLanguageCode(string langCode)
        {
            chosenLangCodeName = langCode;
            chosenLanguageCode = (Languages)System.Enum.Parse(typeof(Languages),chosenLangCodeName);
            OnLanguageChanged?.Invoke();
        }
    }
}