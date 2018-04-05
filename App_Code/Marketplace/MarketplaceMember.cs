using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.Marketplace
{
    public class MarketplaceMember
    {
        public const string AnonymousUsername = "anonymousUser";
        public const int AnonymousUserId = -1;
        public string Name { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }

        public string DeliveryAddress { get; set; }

        public MarketplaceMember(string email, string deliveryAddress, string username = AnonymousUsername, int userId = AnonymousUserId)
        {
            this.Name = username;
            this.Id = userId;
            this.Email = email;
            this.DeliveryAddress = deliveryAddress;
        }
    }
}