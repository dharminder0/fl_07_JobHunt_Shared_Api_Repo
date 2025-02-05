using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace VendersCloud.Business.Service.Concrete
{
    public class HttpService
    {
        public HttpContext CurrentHttpContext;
        private string _userName;
        private string _password;
        private Dictionary<string, string> _headers = new Dictionary<string, string>();
        private JsonSerializerSettings _jsonSerializerSettings;

        public string RootUrl { get; set; }

        public HttpService(string url, string userName = null, string password = null)
        {
            RootUrl = url;
            _userName = userName;
            _password = password;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
        public object Get(string route)
        {
            var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
            var client = GetClient();
            var response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                try
                {
                    return JsonConvert.DeserializeObject(data, _jsonSerializerSettings);
                }
                catch
                {
                    return data;
                }
            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }

        public async Task<object> GetAsync(string route)
        {
            var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
            var client = GetClient();
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                try
                {
                    return JsonConvert.DeserializeObject(data, _jsonSerializerSettings);
                }
                catch
                {
                    return data;
                }
            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }
        public T Get<T>(string route)
        {
            var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
            var client = GetClient();
            var response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }

        //public async Task<T> GetAsync<T>(string route) {
        //    var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
        //    var client = GetClient();
        //    var response = await client.GetAsync(url);

        //    if (response.IsSuccessStatusCode && response.Content != null) {
        //        var data = response.Content.ReadAsStringAsync().Result;
        //        return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
        //    } else {
        //        if (response.Content != null) {
        //            var result = response.Content.ReadAsStringAsync().Result;
        //            throw new Exception(result);
        //        }
        //        throw new Exception("Unknown error");
        //    }
        //}

        public async Task<T> GetAsync<T>(string route)
        {
            var url = string.Format("{0}/{1}", RootUrl.TrimEnd('/'), route);
            var client = GetClient();
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
            }
            else
            {
                if (response.Content != null)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }

        public object Post(string route, object body)
        {
            var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
            var client = GetClient();
            var dataAsString = JsonConvert.SerializeObject(body);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(data, _jsonSerializerSettings);
            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }

        public async Task<T> PostChatGPTAsync<T>(string route, object body)
        {
            var url = $"{RootUrl.TrimEnd('/')}/{route}";
            var client = GetClient();
            client.Timeout = TimeSpan.FromMinutes(5);

            var jsonRequest = JsonConvert.SerializeObject(body);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");



            //var dataAsString = JsonConvert.SerializeObject(body, new JsonSerializerSettings
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //});
            //var content = new StringContent(dataAsString);
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                // var responseData = JsonSerializer.Deserialize<JsonElement>(responseContent);
                return JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                    //var resultDetails = new {Error = result.SerializeObjectWithoutNull(), Body  = body.SerializeObjectWithoutNull(), Request = route };
                    //throw new Exception(resultDetails.SerializeObjectWithoutNull());
                }

                throw new Exception("Unknown error");
            }
        }



        public async Task<T> PostAsync<T>(string route, object body)
        {
            var url = route;
            var client = GetClient();
            client.Timeout = TimeSpan.FromMinutes(5);
            var dataAsString = JsonConvert.SerializeObject(body, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                    //var resultDetails = new {Error = result.SerializeObjectWithoutNull(), Body  = body.SerializeObjectWithoutNull(), Request = route };
                    //throw new Exception(resultDetails.SerializeObjectWithoutNull());
                }

                throw new Exception("Unknown error");
            }
        }
        public async Task<T> PostBulkAsync<T>(string route, object body)
        {
            var url = $"{RootUrl.TrimEnd('/')}/{route}";
            var client = GetClient();
            client.Timeout = TimeSpan.FromMinutes(3);
            var dataAsString = JsonConvert.SerializeObject(body, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                    //var resultDetails = new {Error = result.SerializeObjectWithoutNull(), Body  = body.SerializeObjectWithoutNull(), Request = route };
                    //throw new Exception(resultDetails.SerializeObjectWithoutNull());
                }

                throw new Exception("Unknown error");
            }
        }

        public object GetPdfBinaryData(string route)
        {
            var url = route;//  $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
            var client = GetClient();
            var response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsByteArrayAsync().Result;
                if (data == null && data.Length < 0)
                {
                    return null;
                }
                try
                {
                    return Convert.ToBase64String(data);
                }
                catch
                {
                    return data;
                }

            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }

        //public async Task<T> PostAsync<T>(string route, object body, string contentType = null) {
        //    var url = string.Format("{0}/{1}", RootUrl.TrimEnd('/'), route);
        //    if (!string.IsNullOrWhiteSpace(contentType)) {
        //        AddHeader("Content-Type", contentType);
        //    }
        //    var client = GetClient();
        //    var response = await client.PostAsJsonAsync(url, body);
        //    if (response.IsSuccessStatusCode && response.Content != null) {
        //        var data = await response.Content.ReadAsStringAsync();
        //        return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
        //    } else {
        //        if (response.Content != null) {
        //            var result = await response.Content.ReadAsStringAsync();
        //            throw new Exception(result);
        //        }
        //        throw new Exception("Unknown error");
        //    }
        //}

        public T Post<T>(string route, object body)
        {
            var url = $"{RootUrl.TrimEnd('/')}";
            if (!string.IsNullOrWhiteSpace(route))
            {
                url = $"{RootUrl.TrimEnd('/')}/{route}";
            }
            var client = GetClient();
            var dataAsString = JsonConvert.SerializeObject(body, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = client.PostAsync(url, content);
            if (response.Result.IsSuccessStatusCode && response.Result.Content != null)
            {
                var data = response.Result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
            }
            else
            {
                if (response.Result.Content != null)
                {
                    var result = response.Result.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                    //var resultDetails = new {Error = result.SerializeObjectWithoutNull(), Body  = body.SerializeObjectWithoutNull(), Request = route };
                    //throw new Exception(resultDetails.SerializeObjectWithoutNull());
                }

                throw new Exception("Unknown error");
            }
        }

        //public T PostOld<T>(string route, object body)
        //{
        //    var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
        //    var client = GetClient();
        //    var response = client.PostAsJsonAsync(url, body).Result;
        //    if (response.IsSuccessStatusCode && response.Content != null)
        //    {
        //        var data = response.Content.ReadAsStringAsync().Result;
        //        return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
        //    }
        //    else
        //    {
        //        if (response.Content != null)
        //        {
        //            var result = response.Content.ReadAsStringAsync().Result;
        //            var resultDetails = $@"{url}####{JsonConvert.SerializeObject(body)}###{result}";

        //            throw new Exception(resultDetails);
        //        }
        //        throw new Exception("Unknown error");
        //    }
        //}

        public T PostAsHttpContent<T>(string route, HttpContent body)
        {
            var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
            var client = GetClient();
            var response = client.PostAsync(url, body).Result;
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }

        public object Put(string route, object body)
        {
            var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
            var client = GetClient();
            var dataAsString = JsonConvert.SerializeObject(body, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = client.PutAsync(url, content).Result;
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(data, _jsonSerializerSettings);
            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }

        //public T Put<T>(string route, object body)
        //{
        //    var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
        //    var client = GetClient();
        //    var response = client.PutAsJsonAsync(url, body).Result;
        //    if (response.IsSuccessStatusCode && response.Content != null)
        //    {
        //        var data = response.Content.ReadAsStringAsync().Result;
        //        return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
        //    }
        //    else
        //    {
        //        if (response.Content != null)
        //        {
        //            var result = response.Content.ReadAsStringAsync().Result;
        //            throw new Exception(result);
        //        }
        //        throw new Exception("Unknown error");
        //    }
        //}

        //public T Patch<T>(string route, object body)
        //{
        //    var url = $"{RootUrl.TrimEnd('/')}/{route}";
        //    var client = GetClient();
        //    var response = client.PatchAsync(url, body).Result;
        //    if (response.IsSuccessStatusCode && response.Content != null)
        //    {
        //        var data = response.Content.ReadAsStringAsync().Result;
        //        return JsonConvert.DeserializeObject<T>(data);
        //    }
        //    else
        //    {
        //        if (response.Content != null)
        //        {
        //            var result = response.Content.ReadAsStringAsync().Result;
        //            throw new Exception(result);
        //        }
        //        throw new Exception("Unknown error");
        //    }
        //}

        public object Delete(string route)
        {
            var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
            var client = GetClient();
            var response = client.DeleteAsync(url).Result;
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(data);
            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }

        public T Delete<T>(string route)
        {
            var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
            var client = GetClient();
            var response = client.DeleteAsync(url).Result;
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                if (response.Content != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }
        //public async Task<T> DeleteAsync<T>(string route) {
        //    var url = $"{RootUrl.TrimEnd('/')}/{route}".TrimEnd('/');
        //    var client = GetClient();
        //    var response = await client.DeleteAsync(url);
        //    if (response.IsSuccessStatusCode && response.Content != null) {
        //        var data = response.Content.ReadAsStringAsync().Result;
        //        return JsonConvert.DeserializeObject<T>(data);
        //    } else {
        //        if (response.Content != null) {
        //            var result = response.Content.ReadAsStringAsync().Result;
        //            throw new Exception(result);
        //        }
        //        throw new Exception("Unknown error");
        //    }
        //}

        public async Task<T> DeleteAsync<T>(string route)
        {
            var url = string.Format("{0}/{1}", RootUrl.TrimEnd('/'), route);
            var client = GetClient();
            var response = await client.DeleteAsync(url);
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                if (response.Content != null)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    throw new Exception(result);
                }
                throw new Exception("Unknown error");
            }
        }


        public void AddHeader(string key, string value)
        {
            if (_headers.ContainsKey(key))
                _headers[key] = value;
            else
                _headers.Add(key, value);
        }

        private HttpClient GetClient()
        {
            var httpClient = new HttpClient();
            if (!string.IsNullOrWhiteSpace(_userName) && !string.IsNullOrWhiteSpace(_password))
            {
                var basicParams = $"{_userName}:{_password}";
                var basicAuthorization = $"Basic {Base64Encode(basicParams)}";
                httpClient.DefaultRequestHeaders.Add("Authorization", basicAuthorization);
            }
            foreach (var header in _headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}

       