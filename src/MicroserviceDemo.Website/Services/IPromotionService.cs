using System.Threading.Tasks;
using MicroserviceDemo.Shared.Models;

namespace MicroserviceDemo.Website.Services
{
    public interface IPromotionService
    {
        Task<Promotion> GetPromotionByIdAsync(int id);
    }
}