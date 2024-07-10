using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Gridly
{
    public class Translator : MonoBehaviour
    {
        public TextMeshProUGUI textMeshPro;
        public Text text;

        [HideInInspector]
        public string grid;

        [HideInInspector]
        public string key;
     
        private void OnEnable()
        {
            Refresh();
            Project.Singleton.OnLanguageChanged += Refresh;
        }

        private void OnDisable()
        {
            Project.Singleton.OnLanguageChanged -= Refresh;
        }

        public void Refresh()
        {
            if (textMeshPro != null)
            {
                textMeshPro.text = GridlyLocal.GetStringData(grid, key);
                textMeshPro.font = Project.Singleton.TargetLanguage.tmFont;
            }

            if (text != null)
            {
                text.text = GridlyLocal.GetStringData(grid, key);
                text.font = Project.Singleton.TargetLanguage.font;
            }
        }
    }
}