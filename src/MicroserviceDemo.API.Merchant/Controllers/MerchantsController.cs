using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MicroserviceDemo.Shared.Models;

namespace MicroserviceDemo.API.Merchant.Controllers
{
    [Route("[controller]")]
    public class MerchantsController : Controller
    {
        [HttpGet("{amount}")]
        public IActionResult Get(int amount)
        {
            var merchants = new List<MicroserviceDemo.Shared.Models.Merchant>();

            for (int i = 1; i < amount + 1; i++)
            {
                merchants.Add(new MicroserviceDemo.Shared.Models.Merchant() {
                    Id = i,
                    Name = $"Merchant {i}",
                    Description = $"Description for Merchant {i}",
                    GeneratedTimestamp = DateTime.UtcNow
                });
            }

            return Ok(merchants);
        }
    }
}
