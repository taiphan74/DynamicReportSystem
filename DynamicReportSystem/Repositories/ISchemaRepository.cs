using System.Collections.Generic;
using DynamicReportSystem.Models;

namespace DynamicReportSystem.Repositories
{
    public interface ISchemaRepository
    {
        List<TableInfo> GetBaseTables();

        List<ColumnInfo> GetColumns(string tableName);
    }
}
