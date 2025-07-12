using IgniteSecurityLib;
using Newtonsoft.Json.Serialization;
using Prompt.Engine.Business.Entities;

namespace VendersCloud.WebApi.Controllers
{
    public class BaseApiController : ControllerBase {
        protected string LanguageCode {
            get {
                var langCode = Request.Headers[DefaultSettings.LanguageHeader].ToString();
                return !string.IsNullOrWhiteSpace(langCode) ? langCode : DefaultSettings.LanguageCode;
            }
        }
        protected string UserToken {
            get {
                var token = Request.Headers["token"];
                return token;
            }
        }

        protected IActionResult Json<T>(T content) {
            var jsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings() {
                ContractResolver = new DefaultContractResolver {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            return new JsonResult(content, jsonSerializerSettings);
        }

        protected BadRequestErrorMessageResult BadRequest(Exception error) {
            string message = "";
            var ex = error;
            while (ex != null) {
                message += string.Format("{0}: {1}\r\n", ex.ToString(), ex.Message);
                ex = ex.InnerException;
            }
            return new BadRequestErrorMessageResult(message);
        }
        protected string GetUserTokenFromHeader(IConfiguration configuration) {
            var token = Request.Headers["token"];
            var isJwtToken = IsJwtToken(token);
            //var token = HttpContext.Current.Request.Headers["token"];
            var encryptionEnabled = Convert.ToBoolean(configuration["UserTokenEncryptionEnabled"]);
            if (encryptionEnabled && isJwtToken) {
                var issuer = configuration["JwtIssuer"];
                var audience = configuration["JwtAudience"];
                var symmetricSecretKey = configuration["SymmetricSecretKey"];
                var decryptedToken = JwtSecurityService.Decrypt(symmetricSecretKey, token);
                if (!string.IsNullOrWhiteSpace(decryptedToken)) {
                    var userToken = JwtSecurityService.Decode(decryptedToken);
                    return userToken;
                }
            }
            else {
                throw new Exception("Invalid user token");
            }

            return token;
        }

        protected string GetUserEncryptedTokenFromHeader() {
            var token = Request.Headers["token"];
            return token;
        }

        protected bool IsJwtToken(string token) {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            return IsBase64String(token);
        }

        protected bool IsBase64String(string str) {
            str = str.Trim();
            if (str.Length % 4 != 0)
                return false;

            int index = str.Length - 1;
            if (str[index] == '=')
                index--;

            if (str[index] == '=')
                index--;

            for (int i = 0; i <= index; i++) {
                char c = str[i];
                if (c != '+' && c != '/' && (c < '0' || c > '9') && (c < 'A' || c > 'Z') && (c < 'a' || c > 'z'))
                    return false;
            }
            return true;
        }
    }

    public class BadRequestErrorMessageResult : BadRequestResult {
        public BadRequestErrorMessageResult(string message) {
            Message = message;
        }
        public string Message { get; set; }
    }
}
