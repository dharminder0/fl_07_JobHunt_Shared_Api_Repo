using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VendersCloud.Common.Logging
{
    [ProviderAlias("Database")]
    public class DbLoggerProvider : ILoggerProvider {
        public readonly DbLoggerOptions Options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbLoggerProvider(IOptions<DbLoggerOptions> _options, IHttpContextAccessor httpContextAccessor = null) {
            Options = _options.Value; // Stores all the options.
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Creates a new instance of the db logger.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName) {
            return new DbLogger(this, _httpContextAccessor);
        }

        public void Dispose() {
        }

        public bool IsConfigured() {
            return Options != null &&
                !string.IsNullOrWhiteSpace(Options.ConnectionStringName) &&
                !string.IsNullOrWhiteSpace(Options.LogTable) &&
                Options.LogFields != null &&
                Options.LogFields.Any();
        }
    }
}
