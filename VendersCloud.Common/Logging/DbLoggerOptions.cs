using Microsoft.Extensions.Configuration;

namespace VendersCloud.Common.Logging
{
    public class DbLoggerOptions {
        public string ConnectionStringName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }
        public string LogFields { get; set; }

        public string LogTable { get; set; }

        public void Bind(IConfiguration config) {
            config.GetSection("Logging").GetSection("Database").GetSection("Options").Bind(this);
            this.ConnectionString = config.GetSection("ConnectionStrings").GetValue<string>(this.ConnectionStringName);
        }

        public DbLoggerOptions() {
        }
    }
}
