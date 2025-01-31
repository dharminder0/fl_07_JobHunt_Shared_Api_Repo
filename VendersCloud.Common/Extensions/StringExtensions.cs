using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VendersCloud.Common.Extensions {
    public static class StringExtensions {
        private static object _locker = new object();
        public static string ConcatIf(this string str, bool condition, string text) {
            if (condition) {
                return str + text;
            }
            return str;
        }

        public static string Concat(this string str, string text) {
            return str + text;
        }

        public static bool HasValue(this string str) {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static bool EqualsCI(this string str, string text) {
            return str.Equals(text, StringComparison.OrdinalIgnoreCase);
        }


        public static string GenerateSlug(this string str) {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            var s = str.ToLower();
            s = Regex.Replace(s, @"[^a-z0-9\s-]", ""); // remove invalid characters
            s = Regex.Replace(s, @"\s+", " ").Trim(); // single space
            s = s.Substring(0, s.Length <= 45 ? s.Length : 45).Trim(); // cut and trim
            s = Regex.Replace(s, @"\s", "-"); // insert hyphens
            return s.ToLower();
        }
        public static string ReplacePlaceholders(this string str, Dictionary<string, object> objectData) {
  
            var objData = JObject.Parse(JsonConvert.SerializeObject(objectData));
            string pattern = @"\{\{([^{}]+)\}\}";
            var replacedInput = Regex.Replace(str, pattern, match => {
                if (objData.TryGetValue(match.Groups[1].Value, StringComparison.OrdinalIgnoreCase, out var propertyValue)) {
                    return propertyValue.ToString();
                }
                return match.Value; // Keep the placeholder if the property is not found
            });

            return replacedInput;
        }

        public static List<string> ExtractPropertiesFromInput(this string input) {
            string pattern = @"\{\{([^{}]+)\}\}";
            var matches = Regex.Matches(input, pattern).Cast<Match>().Select(x => x.Groups[1].Value).ToList();
            var properties = matches.Select(x => x.Split(new[] { '.' }).LastOrDefault()).ToList();
            return properties;
        }

        public static List<string> ExtractPlaceholdersFromInput(this string input) {
            string pattern = @"\{\{([^{}]+)\}\}";
            var matches = Regex.Matches(input, pattern).Cast<Match>().Select(x => x.Groups[1].Value).ToList();
            var properties = matches.Select(x => x.ToLower()).Distinct().ToList();
            return properties;
        }

        public static List<string> SplitByRegex(this string str, string pattern) {
            var matches = Regex.Matches(str, pattern).Cast<Match>().Select(x => x.Groups[1].Value).ToList();
            return matches;
        }

        public static string ReplaceByRegex(string input, string pattern, string newValue) {
            // Replace the matched number with the new number
            string result = Regex.Replace(input, pattern, m => $"{{{{trace({newValue}).Result}}}}");
            return result;
        }
        public static string ExtractJsonFromOutput(this string input) {
            Match match = Regex.Match(input, @"```json\s*(.+?)\s*```", RegexOptions.Singleline);
            return match.Success ? match.Groups[1].Value : input;
        }
    }
}
