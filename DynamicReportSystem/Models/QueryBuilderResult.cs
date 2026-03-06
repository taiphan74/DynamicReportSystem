using System.Collections.Generic;

namespace DynamicReportSystem.Models
{
    public class QueryBuilderResult
    {
        public string Sql { get; set; } = string.Empty;

        public List<string> Warnings { get; set; } = new();
    }
}
