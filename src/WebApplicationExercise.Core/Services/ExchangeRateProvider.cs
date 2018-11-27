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
        private HttpClient client;

        public ExchangeRateProvider()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<decimal> GetExchangeRate(string from, string to)
        {
            var requestUrl = $"https://free.currencyconverterapi.com/api/v6/convert?q={from}_{to}&compact=y";

            HttpResponseMessage response = null;
            try
            {
                response = await client.GetAsync(requestUrl);
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
                JToken val = content.First.First["val"];
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
