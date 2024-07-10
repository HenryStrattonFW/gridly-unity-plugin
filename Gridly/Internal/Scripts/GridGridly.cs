using System.Collections.Generic;

namespace Gridly.Internal
{
    [System.Serializable]
    public class ViewID
    {
        public string viewName;
        public string viewID;

        public ViewID(string viewName, string viewID)
        {
            this.viewName = viewName;
            this.viewID = viewID;
        }
    }

    [System.Serializable]
    public class Grid
    {
        public string databaseID;
        public string nameGrid;
        public string gridID;

        public string choesenViewID;
        public List<Record> records = new List<Record>();

        private Dictionary<string, Record> recordLookup;

        public void CopyThisBlankGridTo(ref Grid grid)
        {
            grid.databaseID = databaseID;
            grid.nameGrid = nameGrid;
            grid.gridID = gridID;
        }

        public Grid()
        {
        }

        public Record FindRecord(string recordId)
        {
            if (recordLookup == null)
                CreateRecordLookup();

            return recordLookup.TryGetValue(recordId, out var record)
                ? record
                : null;
        }

        private void CreateRecordLookup()
        {
            recordLookup = new Dictionary<string, Record>();
            foreach (var rec in records)
            {
                recordLookup.Add(rec.recordID, rec);
            }
        }
    }
}