using System;
using System.Net.Http;
using System.Threading.Tasks;
using MicroserviceDemo.Shared.Models;
using MicroserviceDemo.Website.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;

namespace MicroserviceDemo.Website.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly string promotionServiceUrl;

        public PromotionService(IOptions<ServiceOptions> serviceOptions)
        {
            promotionServiceUrl = serviceOptions.Value.PromotionService;
        }

        public async Task<Promotion> GetPromotionByIdAsync(int id)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(promotionServiceUrl);

            var result = await httpClient.GetAsync($"promotions/{id}");

            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Promotion>(json);
        }
    }
}