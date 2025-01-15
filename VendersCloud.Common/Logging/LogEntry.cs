namespace VendersCloud.Common.Logging
{
    public class LogEntry {
        public long Id { get; set; }
        public short? LogLevel { get; set; }
        public string LogLevelName { 
            get {
                return ((LogLevel != null) ? ((Microsoft.Extensions.Logging.LogLevel)this.LogLevel.Value).ToString() : "");
            }
        }
        public int? ThreadId { get; set; }
        public int? EventId { get; set; }
        public string EventName { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string ExceptionSource { get; set; }
        public DateTime Created { get; set; }        
    }
}
