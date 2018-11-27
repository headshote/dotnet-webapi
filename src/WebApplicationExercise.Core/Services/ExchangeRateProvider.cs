using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebApplicationExercise.Core.Exceptions;
using WebApplicationExercise.Core.Interfaces;

namespace WebApplicationExercise.Core.Services
{
    public class ExchangeRateProvider : IExchangeRateProvider
    {
        private readonly string _serviceAddress;

        private HttpClient _client;

        public ExchangeRateProvider(string serviceAddress)
        {
            _serviceAddress = serviceAddress;

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<decimal> GetExchangeRate(string from, string to)
        {
            var requestUrl = _serviceAddress + $"api/v6/convert?q={from}_{to}&compact=ultra";

            HttpResponseMessage response = null;
            try
            {
                response = await _client.GetAsync(requestUrl);
            }
            catch (HttpRequestException e)
            {
                throw new BusinessException($"Failed to get exchange rate from '{from}' to '{to}', error:\n'{e.Message}'");
            }
            
            JObject content = await response.Content.ReadAsAsync<JObject>();

            if (!response.IsSuccessStatusCode)
            {
                throw new BusinessException($"Failed to get exchange rate from '{from}' to '{to}', response:\n'{await response.Content.ReadAsStringAsync()}'");
            }

            try
            {
                JToken val = content[$"{from}_{to}"];
                var exRate = (decimal)val;

                return exRate;
            }
            catch (Exception)
            {
                throw new BusinessException($"Failed to parse exchange rate, response:\n'{await response.Content.ReadAsStringAsync()}'");
            }
        }
    }
}
