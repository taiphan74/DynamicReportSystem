using System.Collections.Generic;
using System.Data;
using DynamicReportSystem.Models;

namespace DynamicReportSystem.Services
{
    public interface IDatabaseExplorerService
    {
        List<string> LoadTableNames();

        List<ColumnInfo> LoadColumns(string tableName);

        DataTable GetPreviewData(string tableName);
    }
}
