using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiUpgradeData
    {
        public string warningMessage { get; set; }
        public List<ApiMembershipPack> availableMembershipPacks { get; set; }
        public List<int> availableBalances { get; set; }
        public List<ApiMembership> memberships { get; set; }
        public List<ApiBalance> balances { get; set; }

        public ApiUpgradeData()
        {
        }
    }
}