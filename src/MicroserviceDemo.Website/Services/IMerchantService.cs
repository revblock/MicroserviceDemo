using System.Collections.Generic;
using System.Threading.Tasks;
using MicroserviceDemo.Shared.Models;

namespace MicroserviceDemo.Website.Services
{
    public interface IMerchantService
    {
        Task<IEnumerable<Merchant>> GetMerchantsAsync(int amount);
    }
}