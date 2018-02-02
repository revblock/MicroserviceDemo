using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MicroserviceDemo.Website.Models;
using MicroserviceDemo.Website.Services;
using Polly;
using Polly.CircuitBreaker;
using MicroserviceDemo.Shared.Models;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Polly.Caching;
using Polly.Wrap;
using Polly.Fallback;
using Polly.Registry;

namespace MicroserviceDemo.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMerchantService merchantService;
        private readonly IPromotionService promotionService;
        private IAsyncPolicy<Promotion> mainPromotionPolicy;
        private IAsyncPolicy sidePromotionPolicy;
        private IAsyncPolicy merchantPolicy;

        public HomeController(IMerchantService merchantService, IPromotionService promotionService, IReadOnlyPolicyRegistry<string> policyRegistry)
        {
            this.merchantService = merchantService;
            this.promotionService = promotionService;

            mainPromotionPolicy = policyRegistry.Get<IAsyncPolicy<Promotion>>("PromoFallback");
            sidePromotionPolicy = policyRegistry.Get<IAsyncPolicy>("Promo");
            merchantPolicy = policyRegistry.Get<IAsyncPolicy>("Merchant");
        }

        private async Task<PolicyResult<Promotion>> GetMainPromotion()
        {
            return await mainPromotionPolicy
            .ExecuteAndCaptureAsync(async (context) =>
            {
                return await promotionService.GetPromotionByIdAsync(1);
            }, new Context("PromoApi1"));
        }

        private async Task<PolicyResult<Promotion>> GetSinglePromotion(int id)
        {
            return await sidePromotionPolicy.ExecuteAndCaptureAsync(async (context) =>
            {
                return await promotionService.GetPromotionByIdAsync(id);
            }, new Context($"PromoApi{id}"));
        }

        private async Task<PolicyResult<IEnumerable<Merchant>>> GetMerchants(int amount)
        {
            return await merchantPolicy.ExecuteAndCaptureAsync(async (context) =>
                        {
                            return await merchantService.GetMerchantsAsync(amount);
                        }, new Context("MerchantApi"));
        }

        public async Task<IActionResult> Index()
        {
            var promoOne = await GetMainPromotion();
            var promoTwo = await GetSinglePromotion(2);
            var promoThree = await GetSinglePromotion(3);
            var merchants = await GetMerchants(10);

            var vm = new HomeViewModel()
            {
                MainPromotion = promoOne.Result,
                SidePromotionOne = promoTwo.Result,
                SidePromotionTwo = promoThree.Result,
                Merchants = merchants.Result,
                PromoApiFaulted = promoTwo.Outcome == OutcomeType.Failure || promoThree.Outcome == OutcomeType.Failure,
                MerchantApiFaulted = merchants.Outcome == OutcomeType.Failure
            };

            return View(vm);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
