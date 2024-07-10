using UnityEngine;

namespace Gridly.Internal
{
    [System.Serializable]
    public class RecordTemp
    {
        public string viewID;
        public ActionRecord actionRecord;
        public Record record;
    }

    public enum ActionRecord
    {
        Add,
        Delete
    }

    public class UserData : ScriptableSingleton<UserData>
    {
        public bool showServerMess;
        public Languages mainLangEditor = Languages.enUS;


        [SerializeField] private string _screenshotPath = "";
        public string ScreenshotPath
        {
            get => _screenshotPath.Decrypt();
            set => _screenshotPath = value.Enrypt();
        }


        [SerializeField] private bool _uploadImages = false;
        public bool UploadImages
        {
            get => _uploadImages;
            set => _uploadImages = value;
        }


        [SerializeField] private string _keyAPI = "";
        public string KeyAPI
        {
            get => _keyAPI.Decrypt();
            set => _keyAPI = value.Enrypt();
        }
    }
}