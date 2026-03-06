using System.Collections.Generic;

namespace DynamicReportSystem.Models
{
    public class QueryBuilderOptions
    {
        public string SelectedTable { get; set; } = string.Empty;

        public bool IncludeSelect { get; set; } = true;

        public bool UseWhere { get; set; }

        public bool UseOrderBy { get; set; }

        public bool UseGroupBy { get; set; }

        public int Top { get; set; } = 100;

        public string? OrderByColumn { get; set; }

        public string OrderDirection { get; set; } = "ASC";

        public string? GroupByColumn { get; set; }

        public List<string> SelectedColumns { get; set; } = new();
    }
}
