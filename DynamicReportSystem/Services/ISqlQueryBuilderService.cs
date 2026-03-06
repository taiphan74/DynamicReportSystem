using System.Collections.Generic;
using DynamicReportSystem.Models;

namespace DynamicReportSystem.Services
{
    public interface ISqlQueryBuilderService
    {
        QueryBuilderResult BuildSelectQuery(
            QueryBuilderOptions options,
            IReadOnlyCollection<string> allowedTables,
            IReadOnlyCollection<string> allowedColumns);
    }
}
