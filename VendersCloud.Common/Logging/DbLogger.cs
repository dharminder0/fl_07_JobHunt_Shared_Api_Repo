using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Reflection;

namespace VendersCloud.Common.Logging
{
    public class DbLogger : ILogger {
        /// <summary>
        /// Instance of <see cref="DbLoggerProvider" />.
        /// </summary>
        private readonly DbLoggerProvider _dbLoggerProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static bool _tableExists = false;

        /// <summary>
        /// Creates a new instance of <see cref="FileLogger" />.
        /// </summary>
        /// <param name="fileLoggerProvider">Instance of <see cref="FileLoggerProvider" />.</param>
        public DbLogger(DbLoggerProvider dbLoggerProvider, IHttpContextAccessor httpContextAccessor) {
            _dbLoggerProvider = dbLoggerProvider;
            _httpContextAccessor = httpContextAccessor;
            DbLogReader.LoggerProvider = dbLoggerProvider;
        }

        public IDisposable BeginScope<TState>(TState state) {
            return null;
        }

        /// <summary>
        /// Whether to log the entry.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel) {
            return logLevel != LogLevel.None;
        }        


        /// <summary>
        /// Used to log the entry.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel">An instance of <see cref="LogLevel"/>.</param>
        /// <param name="eventId">The event's ID. An instance of <see cref="EventId"/>.</param>
        /// <param name="state">The event's state.</param>
        /// <param name="exception">The event's exception. An instance of <see cref="Exception" /></param>
        /// <param name="formatter">A delegate that formats </param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            if (!IsEnabled(logLevel) || !_dbLoggerProvider.IsConfigured()) {
                // Don't log the entry if it's not enabled.
                return;
            }

            var threadId = Thread.CurrentThread.ManagedThreadId; // Get the current thread ID to use in the log file. 

            // Store record.
            using (var connection = GetDbConnection()) {
                connection.Open();
                // Add to database.

                // LogLevel
                // ThreadId
                // EventId
                // EventName
                // Message
                // Exception Message (use formatter)
                // Exception Stack Trace
                // Exception Source

                var values = new Dictionary<string, object>();
                var logEntry = new LogEntry();

                if (!string.IsNullOrWhiteSpace(_dbLoggerProvider?.Options?.LogFields)) {
                    foreach (var logField in _dbLoggerProvider.Options.LogFields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                        switch (logField) {
                            case "LogLevel":
                                if (!string.IsNullOrWhiteSpace(logLevel.ToString())) {
                                    logEntry.LogLevel = (short)logLevel;
                                }
                                break;
                            case "ThreadId":
                                logEntry.ThreadId = threadId;
                                break;
                            case "EventId":
                                logEntry.EventId = eventId.Id;
                                break;
                            case "EventName":
                                if (!string.IsNullOrWhiteSpace(eventId.Name)) {
                                    logEntry.EventName = eventId.Name;
                                }
                                break;
                            case "Message":
                                if (!string.IsNullOrWhiteSpace(formatter(state, exception))) {
                                    logEntry.Message = formatter(state, exception);
                                }
                                break;
                            case "ExceptionMessage":
                                if (exception != null && !string.IsNullOrWhiteSpace(exception.Message)) {
                                    logEntry.ExceptionMessage = exception?.Message;
                                }
                                break;
                            case "ExceptionStackTrace":
                                if (exception != null && !string.IsNullOrWhiteSpace(exception.StackTrace)) {
                                    logEntry.ExceptionStackTrace = exception?.StackTrace;
                                }
                                break;
                            case "ExceptionSource":
                                if (exception != null && !string.IsNullOrWhiteSpace(exception.Source)) {
                                    logEntry.ExceptionSource = exception?.Source;
                                }
                                break;
                        }
                    }

                }

                using (var command = new SqlCommand()) {
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    var populatedFields = GetPopulatedFields(logEntry);

                    command.CommandText = $@"
{(_tableExists ? string.Empty : GetCreateTableIfNotExistScript())}
INSERT INTO {_dbLoggerProvider.Options.LogTable} ({string.Join(",", populatedFields.Select(p => $"[{p.Name}]"))}, [Created]) 
VALUES ({string.Join(",", populatedFields.Select(p => $"@{p.Name}"))}, @Created)
";
                    foreach (var field in populatedFields) {
                        command.Parameters.Add(new SqlParameter($"{field.Name}", field.GetValue(logEntry)));
                    }
                    command.Parameters.Add(new SqlParameter("@Created", DateTimeOffset.Now));

                    command.ExecuteNonQuery();
                    _tableExists = true;
                }

                connection.Close();

            }
        }

        private string GetCreateTableIfNotExistScript() {
            return $@"
IF OBJECT_ID('{_dbLoggerProvider.Options.LogTable}') IS NULL
BEGIN
CREATE TABLE {_dbLoggerProvider.Options.LogTable} 
(
Id BIGINT IDENTITY PRIMARY KEY, 
LogLevel SMALLINT,
ThreadId INT,
EventId INT,
EventName NVARCHAR(200),
[Message] NVARCHAR(MAX),
[ExceptionMessage] NVARCHAR(MAX),
[ExceptionStackTrace] NVARCHAR(MAX),
[ExceptionSource] NVARCHAR(MAX), 
[Created] DATETIME
);
END
";
        }

        private List<PropertyInfo> GetPopulatedFields(LogEntry entry) {
            var props = typeof(LogEntry).GetProperties();
            return props.Where(p => 
                                p.GetValue(entry) != null && 
                                p.Name != nameof(LogEntry.Created) && 
                                p.Name != nameof(LogEntry.Id) && 
                                p.Name != nameof(LogEntry.LogLevelName)).ToList();
        }

        private SqlConnection GetDbConnection() {
            return new SqlConnection(_dbLoggerProvider.Options.ConnectionString);
        }
    }
}
