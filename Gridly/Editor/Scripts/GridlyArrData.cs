using System.Collections.Generic;
using System.Linq;

namespace Gridly.Internal
{
    public class GridlyArrData
    {
        public bool IsInitialized { get; private set; }

        public string[] gridArr;
        public string[] keyArr;
        public int indexGrid;
        public int indexKey;
        public string keyID;
        public string searchKey;
        

        public Grid Grid => Project.Singleton.GetGridByIndex(indexGrid);
        public Record ChosenRecord => Grid?.FindRecord(keyID);
        

        public void RefreshAll(string gridName, string keyID) 
        {
            IsInitialized = true;

            this.keyID = keyID;

            gridArr = Project.Singleton.grids
                .Select(x => x.nameGrid)
                .ToArray();
            
            if (!string.IsNullOrEmpty(gridName))
            {
                indexGrid = GetIndex(gridName, gridArr);
            }

            if (Grid == null) return;
            
            List<string> nameKey = new List<string>();
            foreach(var record in Grid.records)
            {
                nameKey.Add(record.recordID);
            }

            if (!string.IsNullOrEmpty(searchKey))
            {
                nameKey = nameKey.FindAll(x => x.Contains(searchKey));
            }

            keyArr = nameKey.ToArray();

            if (!string.IsNullOrEmpty(keyID))
                indexKey = GetIndex(keyID, keyArr);
        }
       
        private static int GetIndex(string select, params string[] arr)
        {
            if (arr == null)
                return -1;
            
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] == select)
                    return i;
            }
            return 0;
        }
    }

}