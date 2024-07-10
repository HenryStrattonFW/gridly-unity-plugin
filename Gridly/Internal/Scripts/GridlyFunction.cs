using UnityEngine.Networking;
using System;
using System.Threading.Tasks;

namespace Gridly.Internal
{
    public static class GridlyUtility
    {
        public static bool CheckOutput(this string output)
        {
            if (output.Length != 0) return true;

            GridlyLogging.LogError("Something is wrong. Please check your key again");
            return false;
        }
    }

    public class GridlyFunction
    {
        private const int StepSize = 1000;


        public static int Download { get; private set; }
        public static int DownloadedTotal { get; private set; }
        public static bool IsDownloading => Download < DownloadedTotal;


        #region Setup

        public void SetupDatabases()
        {
            RefreshDownloadTotal();

            foreach (var i in Project.Singleton.grids)
            {
                if (i.choesenViewID == "###")
                    continue;

                i.records.Clear(); //COMMENTED
                SetupRecords(i, 0);
            }
        }

        public static string GetRecordPage(string viewID, int i)
        {
            GridlyLogging.Log($"get record from {(i * StepSize)} to: {(i + 1) * StepSize}");
            return $"https://api.gridly.com/v1/views/{viewID}/records?page=%7B%22offset%22%3A+{i * StepSize}%2C+%22limit%22%3A+{StepSize}%7D";
        }

        public async void SetupRecords(Grid grid, int page)
        {
            int limit = 2 + page; //download 2k record
            for (int i = 0 + page; i < limit; i++)
            {
                await SetupRecords(grid, i, i == limit - 1);
            }
        }

        public void RefreshDownloadTotal()
        {
            Download = 0;
            DownloadedTotal = 0;
        }

        public async Task SetupRecords(Grid grid, int page, bool autoDownload)
        {
            if (grid == null)
                return;
            
            GridlyLogging.Log($"Setting up record for {grid.nameGrid}");
            DownloadedTotal += 1;

            UnityWebRequest unityWeb = UnityWebRequest.Get(GetRecordPage(grid.choesenViewID, page));
            unityWeb.SetRequestHeader("Authorization", $"ApiKey {UserData.Singleton.KeyAPI}");
            unityWeb.SendWebRequest();

            while (!unityWeb.isDone)
                await Task.Yield();

            if (unityWeb.isDone)
            {
                Download += 1;
                var json = unityWeb.downloadHandler.text;
                if (string.IsNullOrEmpty(json))
                {
                    var N = JSON.Parse(json);
                    int index = 0;

                    while (N[index].Count != 0)
                    {
                        Record record = new Record();

                        record.recordID = N[index]["id"];
                        record.pathTag = N[index]["path"];
                        int index1 = 0;
                        while (N[index]["cells"][index1].Count != 0)
                        {
                            string value = "";


                            int lengthVal = N[index]["cells"][index1]["value"].Count;
                            for (int indexValue = 0; indexValue <= lengthVal; indexValue++)
                            {
                                if (value != "")
                                    value += ";";
                                if (indexValue == 0)
                                    value += N[index]["cells"][index1]["value"];
                                else
                                    value += N[index]["cells"][index1]["value"][indexValue];
                            }

                            record.columns.Add(new Column(N[index]["cells"][index1]["columnId"], value));

                            index1++;
                        }

                        grid.records.Add(record);

                        //done 1 process check
                        if (index == (StepSize - 1))
                        {
                            doneOneProcess?.Invoke();
                        }

                        if (index == (StepSize - 1) && autoDownload)
                        {
                            SetupRecords(grid, page + 1);
                            return;
                        }

                        index++;
                    }


                    if (Download == DownloadedTotal)
                    {
                        GridlyLogging.Log("Finished downloading");
                        doneOneProcess?.Invoke();
                        finishAction?.Invoke();
                        SaveProject();
                    }

                    //Done
                    SetDirty();
                }
            }
        }


        public void MergeRecord(Grid grid, int page)
        {
            if (grid == null)
                return;
            
            GridlyLogging.Log($"Setting up record for {grid.nameGrid}");

            UnityWebRequest unityWeb = UnityWebRequest.Get(GetRecordPage(grid.choesenViewID, page));
            unityWeb.SetRequestHeader("Authorization", $"ApiKey {UserData.Singleton.KeyAPI}");
            unityWeb.SendWebRequest();

            void action()
            {
                if (unityWeb.isDone)
                {
                    var i = unityWeb.downloadHandler.text;
                    if (i.CheckOutput())
                    {
                        var N = JSON.Parse(i);
                        int index = 0;
                        //string _path = "";
                        while (N[index].Count != 0)
                        {
                            Record record = new Record();

                            record.recordID = N[index]["id"];
                            record.pathTag = N[index]["path"];
                            int index1 = 0;
                            while (N[index]["cells"][index1].Count != 0)
                            {
                                string value = "";

                                int lengthVal = N[index]["cells"][index1]["value"].Count;
                                for (int indexValue = 0; indexValue <= lengthVal; indexValue++)
                                {
                                    if (value != "")
                                        value += ";";
                                    if (indexValue == 0)
                                        value += N[index]["cells"][index1]["value"];
                                    else
                                        value += N[index]["cells"][index1]["value"][indexValue];
                                }

                                record.columns.Add(new Column(N[index]["cells"][index1]["columnId"], value));

                                index1++;
                            }

                            Record _tempRecord = grid.records.Find(x => x.recordID == record.recordID);
                            if (_tempRecord != null)
                            {
                                grid.records.Remove(_tempRecord);
                            }

                            grid.records.Add(record);
                            index++;
                        }


                        //Project.singleton.Save();
                    }
                }
            }

            CancelWhenDone(action, unityWeb);
        }

        #endregion

        public virtual void SetDirty()
        {
        }

        public virtual void SaveProject()
        {
        }

        public static Action process;
        public Action finishAction;
        public Action doneOneProcess { get; set; }


        public void CancelWhenDone(Action action, UnityWebRequest unityWebRequest)
        {
            CancelWhenDone(action, unityWebRequest, UserData.Singleton.showServerMess);
        }

        public virtual void CancelWhenDone(Action action, UnityWebRequest unityWebRequest, bool printServerMes)
        {
            action += () =>
            {
                if (unityWebRequest.isDone)
                {
                    process -= action.Invoke;
                    if (printServerMes)
                        GridlyLogging.Log($"Server Message: {unityWebRequest.downloadHandler.text}");
                }
            };

            process += action.Invoke;
        }
    }
}