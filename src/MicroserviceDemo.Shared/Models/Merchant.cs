using System;

namespace MicroserviceDemo.Shared.Models
{
    public class Merchant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime GeneratedTimestamp { get; set; }
        
    }
}