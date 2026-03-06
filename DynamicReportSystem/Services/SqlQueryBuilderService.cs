using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicReportSystem.Models;

namespace DynamicReportSystem.Services
{
    public class SqlQueryBuilderService : ISqlQueryBuilderService
    {
        public QueryBuilderResult BuildSelectQuery(
            QueryBuilderOptions options,
            IReadOnlyCollection<string> allowedTables,
            IReadOnlyCollection<string> allowedColumns)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (allowedTables is null || allowedTables.Count == 0)
            {
                throw new InvalidOperationException("No allowed tables available for SQL generation.");
            }

            if (string.IsNullOrWhiteSpace(options.SelectedTable))
            {
                throw new ArgumentException("Selected table is required.", nameof(options.SelectedTable));
            }

            var tableSet = new HashSet<string>(allowedTables, StringComparer.OrdinalIgnoreCase);
            if (!tableSet.Contains(options.SelectedTable))
            {
                throw new InvalidOperationException("Selected table is not in the allowed schema list.");
            }

            if (!options.IncludeSelect)
            {
                throw new InvalidOperationException("SELECT option must be enabled for this builder.");
            }

            var columnSet = new HashSet<string>(allowedColumns ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            var result = new QueryBuilderResult();
            var sql = new StringBuilder();

            sql.Append("SELECT");
            if (options.Top > 0)
            {
                sql.Append($" TOP {options.Top}");
            }

            sql.Append(" *");
            sql.AppendLine();
            sql.Append($"FROM [{EscapeIdentifier(options.SelectedTable)}]");

            if (options.UseOrderBy)
            {
                if (string.IsNullOrWhiteSpace(options.OrderByColumn))
                {
                    throw new InvalidOperationException("ORDER BY is enabled but no ORDER BY column is selected.");
                }

                if (!columnSet.Contains(options.OrderByColumn))
                {
                    throw new InvalidOperationException("ORDER BY column is not valid for the selected table.");
                }

                var direction = string.Equals(options.OrderDirection, "DESC", StringComparison.OrdinalIgnoreCase)
                    ? "DESC"
                    : "ASC";

                sql.AppendLine();
                sql.Append($"ORDER BY [{EscapeIdentifier(options.OrderByColumn)}] {direction}");
            }

            if (options.UseWhere)
            {
                result.Warnings.Add("WHERE is checked but condition builder is not implemented yet.");
            }

            if (options.UseGroupBy)
            {
                if (string.IsNullOrWhiteSpace(options.GroupByColumn))
                {
                    result.Warnings.Add("GROUP BY is checked but no GROUP BY column is selected.");
                }
                else if (!columnSet.Contains(options.GroupByColumn))
                {
                    result.Warnings.Add("GROUP BY column is not valid for the selected table.");
                }
                else
                {
                    sql.AppendLine();
                    sql.Append($"GROUP BY [{EscapeIdentifier(options.GroupByColumn)}]");
                }
            }

            result.Sql = sql.ToString();
            return result;
        }

        private static string EscapeIdentifier(string value)
        {
            return value.Replace("]", "]]", StringComparison.Ordinal);
        }
    }
}
