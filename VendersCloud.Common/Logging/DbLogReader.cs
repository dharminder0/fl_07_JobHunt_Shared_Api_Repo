using System.Data.SqlClient;

namespace VendersCloud.Common.Logging
{

    public class LogSearchCondition {
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public object Terms { get; set; }
    }

    public class DbLogReader {
        public static DbLoggerProvider LoggerProvider { get; set; }

        public static IEnumerable<LogEntry> SearchLogs(List<LogSearchCondition> filters, int pageIndex, int pageSize, ref int total) {

            var result = new List<LogEntry>();
            if (LoggerProvider == null || !LoggerProvider.IsConfigured())
                return result;

            var filterGroups = filters.GroupBy(f => f.FieldName);
            var conditions = new List<Tuple<string, string, string, object>>();
            foreach (var filterGroup in filterGroups) {
                var counter = 1;
                foreach (var filter in filterGroup.Where(g => g.FieldName != nameof(LogEntry.LogLevelName))) {
                    conditions.Add(new Tuple<string, string, string, object>(filterGroup.Key, filter.Operator, $"{filter.FieldName}{counter++}", filter.Terms));

                }
            }

            var fieldNames = typeof(LogEntry).GetProperties().Where(p => p.Name != nameof(LogEntry.LogLevelName)).Select(p => p.Name).ToList();

            var sql = $@"
SELECT {string.Join(",", fieldNames)}
FROM {LoggerProvider.Options.LogTable}
";
            var where = string.Empty;
            if (filters != null && filters.Any()) {
                where = $@"
WHERE {string.Join(" AND ", conditions.Select(f => $"{f.Item1} {f.Item2} {((f.Item2.ToLower().Equals("like") || f.Item2.ToLower().Equals("not like")) ? $"'%' + @{f.Item3} + '%'" : $"@{f.Item3}")}"))} 
";
            }
            sql += $@"
{where}
ORDER BY Id DESC
OFFSET {(pageIndex - 1) * pageSize} ROWS
FETCH NEXT {pageSize} ROWS ONLY;
";

            using (var connection = GetDbConnection()) {
                connection.Open();
                using (var command = new SqlCommand()) {
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;

                    command.CommandText = $"SELECT COUNT(*) FROM {LoggerProvider.Options.LogTable} {where}";

                    if (filters != null && filters.Any()) {
                        foreach (var condition in conditions) {
                            command.Parameters.Add(new SqlParameter($"{condition.Item3}", condition.Item4));
                        }
                    }
                    total = int.Parse(command.ExecuteScalar().ToString());
                    
                }
                connection.Close();
            }

            using (var connection = GetDbConnection()) {
                connection.Open();
                using (var command = new SqlCommand()) {
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;

                    command.CommandText = sql;

                    if(filters != null && filters.Any()) {
                        foreach (var condition in conditions) {
                            command.Parameters.Add(new SqlParameter($"{condition.Item3}", condition.Item4));
                        }
                    }

                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var entry = new LogEntry {
                                Id = reader.GetInt64(fieldNames.IndexOf(nameof(LogEntry.Id))),
                                EventId = (int?)reader.GetValue(fieldNames.IndexOf(nameof(LogEntry.EventId))),
                                EventName = reader.GetValue(fieldNames.IndexOf(nameof(LogEntry.EventName)))?.ToString(),
                                LogLevel =  (short?)reader.GetValue(fieldNames.IndexOf(nameof(LogEntry.LogLevel))),
                                ThreadId = (int?)reader.GetValue(fieldNames.IndexOf(nameof(LogEntry.ThreadId))),
                                Message = reader.GetValue(fieldNames.IndexOf(nameof(LogEntry.Message)))?.ToString(),
                                ExceptionMessage = reader.GetValue(fieldNames.IndexOf(nameof(LogEntry.ExceptionMessage)))?.ToString(),
                                ExceptionStackTrace = reader.GetValue(fieldNames.IndexOf(nameof(LogEntry.ExceptionStackTrace)))?.ToString(),
                                ExceptionSource = reader.GetValue(fieldNames.IndexOf(nameof(LogEntry.ExceptionSource)))?.ToString(),
                                Created = (DateTime)reader.GetValue(fieldNames.IndexOf(nameof(LogEntry.Created)))
                            };
                            result.Add(entry);
                        }
                    }
                }
                connection.Close();
            }
            return result;
        }

        private static SqlConnection GetDbConnection() {
            return new SqlConnection(LoggerProvider.Options.ConnectionString);
        }
    }
}
