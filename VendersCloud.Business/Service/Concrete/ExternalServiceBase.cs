﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendersCloud.Business.Service.Concrete
{
    public abstract class ExternalServiceBase
    {
        protected HttpService _httpService;
        public ExternalServiceBase(string rootUrl, string authorizationHeader = "", string apiKey = "")
        {
            _httpService = new HttpService(rootUrl);
            _httpService.AddHeader("Authorization", authorizationHeader);
            _httpService.AddHeader("ApiSecret", authorizationHeader);

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
