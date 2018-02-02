using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MicroserviceDemo.API.Promotion.Controllers
{
    [Route("[controller]")]
    public class PromotionsController : Controller
    {
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var promotion = new MicroserviceDemo.Shared.Models.Promotion()
            {
                Id = id,
                Title = $"Promotion {id}",
                Description = $"Description for Promotion {id}",
                GeneratedTimestamp = DateTime.UtcNow
            };

            return Ok(promotion);            
        }
    }
}
