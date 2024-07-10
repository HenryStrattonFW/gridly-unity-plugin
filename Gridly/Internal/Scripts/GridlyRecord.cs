using System.Collections.Generic;

public class GridlyRecord 
{
    public class Cell
    {
        public string ColumnId { get; set; }
        public string Value { get; set; }
        public string DependencyStatus { get; set; }
    }
    
    public class Records
    {
        public string Id { get; set; }
        public List<Cell> Cells { get; }
        public string Path { get; set; }

        public Records()
        {
            Cells = new List<Cell>();
        }

    }
}
