using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace VendersCloud.Common.Utils
{
    public class IpHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetCallerIp()
        {
            try
            {
                // Access the current HttpContext
                var context = _httpContextAccessor.HttpContext;

                // Get the IP address from the HttpContext
                string ip = context?.Request.Headers["CF-Connecting-IP"].ToString();

                if (string.IsNullOrEmpty(ip))
                {
                    return null;
                }

                // Handle local IP address
                if (ip == "::1")
                {
                    return "127.0.0.1"; // local IP in IPv4 format
                }

                // Remove any port information and validate the IP
                string iptext = Regex.Replace(ip, @"((\:.*)|(\,.*))", "", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
                var isValidIp = ValidateIP(iptext);
                return isValidIp ? iptext : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool ValidateIP(string ipAddress)
        {
            return System.Net.IPAddress.TryParse(ipAddress, out _);
        }
    }
}
