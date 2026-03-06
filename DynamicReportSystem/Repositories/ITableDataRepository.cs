using System.Data;

namespace DynamicReportSystem.Repositories
{
    public interface ITableDataRepository
    {
        DataTable GetTopRows(string tableName, int top);
    }
}
