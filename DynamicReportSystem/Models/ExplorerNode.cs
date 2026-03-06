using System.Collections.Generic;

namespace DynamicReportSystem.Models
{
    public enum ExplorerNodeType
    {
        Root,
        Table,
        Column
    }

    public class ExplorerNode
    {
        public string Name { get; set; } = string.Empty;

        public ExplorerNodeType NodeType { get; set; }

        public string? TableName { get; set; }

        public string? DataType { get; set; }

        public List<ExplorerNode> Children { get; set; } = new();

        public string DisplayText
        {
            get
            {
                if (NodeType == ExplorerNodeType.Column && !string.IsNullOrWhiteSpace(DataType))
                {
                    return $"{Name} ({DataType})";
                }

                return Name;
            }
        }
    }
}
