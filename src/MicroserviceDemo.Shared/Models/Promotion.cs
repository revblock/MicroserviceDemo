using System;

namespace MicroserviceDemo.Shared.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }    
        public DateTime GeneratedTimestamp { get; set; }    
    }
}