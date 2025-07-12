namespace VendersCloud.Business.Service.Concrete
{
    public abstract class ExternalServiceBase
    {
        protected HttpService _httpService;
        public ExternalServiceBase(string rootUrl, string authorizationHeader = "", string apiKey = "")
        {
            _httpService = new HttpService(rootUrl);

            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                _httpService.AddHeader("Authorization", authorizationHeader);
                _httpService.AddHeader("ApiSecret", authorizationHeader); // optional if required
            }

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                _httpService.AddHeader("api-key", apiKey);
            }
        }

        public ExternalServiceBase()
        {

        }
    }
}
