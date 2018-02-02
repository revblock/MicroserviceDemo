using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MicroserviceDemo.Shared.Models;
using MicroserviceDemo.Website.Models;
using MicroserviceDemo.Website.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Wrap;

namespace MicroserviceDemo.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ServiceOptions>(Configuration.GetSection("Services"));

            services.AddMemoryCache();
            services.AddMvc();

            services.AddTransient<IMerchantService, MerchantService>();
            services.AddTransient<IPromotionService, PromotionService>();
            services.AddSingleton<IReadOnlyPolicyRegistry<string>, PolicyRegistry>((serviceProvider) => BuildPolicyRegistry());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private PolicyRegistry BuildPolicyRegistry()
        {

            var circuitBreakerPolicy = Policy.Handle<HttpRequestException>()
                                            .CircuitBreakerAsync(1, TimeSpan.FromMinutes(1));

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var memoryCacheProvider = new Polly.Caching.MemoryCache.MemoryCacheProvider(memoryCache);
            var cachePolicy = Policy.CacheAsync(memoryCacheProvider, TimeSpan.FromSeconds(30));

            var cacheAndCircuitBreakerPolicy = cachePolicy.WrapAsync(Policy.Handle<HttpRequestException>()
                                            .CircuitBreakerAsync(1, TimeSpan.FromMinutes(1)));

            var fallbackCacheAndBreaker = Policy<Promotion>
                                                .Handle<BrokenCircuitException>()
                                                .Or<HttpRequestException>()
                                                .FallbackAsync<Promotion>(new Promotion()
                                                {
                                                    Title = "Some Fallback Promotion",
                                                    Description = "The Promotion API is down so we're showing something else",
                                                    GeneratedTimestamp = DateTime.UtcNow
                                                }
                                                )
                                                .WrapAsync(cacheAndCircuitBreakerPolicy);

            var registry = new PolicyRegistry();

            registry.Add("PromoFallback", fallbackCacheAndBreaker);
            registry.Add("Promo", cacheAndCircuitBreakerPolicy);
            registry.Add("Merchant", circuitBreakerPolicy);

            return registry;
        }
    }
}
