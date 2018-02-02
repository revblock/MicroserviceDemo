using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MicroserviceDemo.Shared.Models;
using MicroserviceDemo.Website.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MicroserviceDemo.Website.Services
{
    public class MerchantService : IMerchantService
    {
        private readonly string merchantServiceUrl;

        public MerchantService(IOptions<ServiceOptions> serviceOptions)
        {
            merchantServiceUrl = serviceOptions.Value.MerchantService;
        }

        public async Task<IEnumerable<Merchant>> GetMerchantsAsync(int amount)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(merchantServiceUrl);

            var result = await httpClient.GetAsync($"merchants/{amount}");

            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<Merchant>>(json);
        }
    }
}