using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Titan.Pages;

namespace Titan.API
{
    public class ApiMembershipPack
    {
        public string description { get; set; }
        public int packId { get; set; }
        public ApiMoney price { get; set; }

        public ApiMembershipPack(Member user, MembershipPack pack)
        {
            this.description = UpgradePageHelper.GetMembershipPackDescription(user, pack);
            this.packId = pack.Id;
            this.price = new ApiMoney(pack.GetPrice(user));
        }
    }
}