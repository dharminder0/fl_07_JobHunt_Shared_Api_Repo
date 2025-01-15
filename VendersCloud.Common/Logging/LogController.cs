
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace VendersCloud.Common.Logging
{
    public class LogController : ControllerBase {

        [Route("api/v1/logs/auth")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] AuthModel model) {
            if (model.Username != DbLogReader.LoggerProvider.Options.UserName || model.Password != DbLogReader.LoggerProvider.Options.Password)
                return Unauthorized();
            return Ok(new { success = true });
        }


        /// <summary>
        /// Gets all log entries given search criteria
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/logs")]
        [HttpPost]
        public object SearchLogs([FromBody] LogSearchRequestModel searchModel, int pageIndex = 1, int pageSize = 10) {
            if (searchModel.UserName != DbLogReader.LoggerProvider.Options.UserName || searchModel.Password != DbLogReader.LoggerProvider.Options.Password)
                return Unauthorized();

            var total = 0;
            var result = DbLogReader.SearchLogs(searchModel.Filters, pageIndex, pageSize, ref total);
            return new {
                total = total,
                data = result
            };
        }

        [Route("logs")]
        [HttpGet]
        public IActionResult Get() {
            return Content(GetHtml(), "text/html");
        }

        private string GetHtml() {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("Core.Infrastructure.Logging.logs.html");

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }

    }

    public class AuthModel {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LogSearchRequestModel {
        public List<LogSearchCondition> Filters { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}
