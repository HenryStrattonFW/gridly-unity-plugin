using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Gridly.Example
{
    public class GridlyPluginExample : MonoBehaviour
    {
        [SerializeField] private Text languageCodeLabel;
        
        private static LangSupport CurrentLanguage
        {
            get => Project.Singleton.TargetLanguage;
            set => Project.Singleton.TargetLanguage = value;
        }

        private int index = 0;
        private List<LangSupport> languagesSupport => Project.Singleton.langSupports;

        private void Start()
        {
            Project.Singleton.OnLanguageChanged += RefreshLanguageDisplay;
            RefreshLanguageDisplay();
        }

        public void NextLanguage()
        {
            if (++index == languagesSupport.Count)
                index = 0;
            
            CurrentLanguage = languagesSupport[index];
        }

        public void PreviousLanguage()
        {
            if (--index == -1)
                index = languagesSupport.Count-1;

            CurrentLanguage = languagesSupport[index];
        }

        private void RefreshLanguageDisplay()
        {
            languageCodeLabel.text = CurrentLanguage.languagesSupport.ToString();
        }

    }
}