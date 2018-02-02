using System.Collections.Generic;
using MicroserviceDemo.Shared.Models;

namespace MicroserviceDemo.Website.Models
{
    public class HomeViewModel
    {
        public Promotion MainPromotion { get; set; }
        public Promotion SidePromotionOne { get; set; }
        public Promotion SidePromotionTwo { get; set; }
        public IEnumerable<Merchant> Merchants { get; set; }
        public bool PromoApiFaulted { get; set; }
        public bool MerchantApiFaulted { get; set; }
    }
}