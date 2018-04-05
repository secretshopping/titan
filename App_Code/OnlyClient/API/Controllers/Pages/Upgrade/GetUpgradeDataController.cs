using Newtonsoft.Json.Linq;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Titan.Pages;

namespace Titan.API
{
    public class GetUpgradeDataController : BaseApiController
    {
        protected override ApiResultMessage HandleRequest(object args)
        {
            string token = ((JObject)args)["token"].ToString();
            int userId = ApiAccessToken.ValidateAndGetUserId(token);

            Member User = new Member(userId);
            Money FirstPackPrice = Money.Zero;

            //Available Balances
            PurchaseOption purchaseOption = PurchaseOption.Get(PurchaseOption.Features.Upgrade);
            List<int> availableBalances = new List<int>();

            if (purchaseOption.PurchaseBalanceEnabled)
                availableBalances.Add((int)PurchaseBalances.Purchase);

            if (purchaseOption.CashBalanceEnabled)
                availableBalances.Add((int)PurchaseBalances.Cash);

            //Always allow to upgrade via payment processor
            availableBalances.Add((int)PurchaseBalances.PaymentProcessor);

            //List of memberships
            var activeMemberships = Membership.GetActiveMembershipsDataTables();

            return new ApiResultMessage
            {
                success = true,
                message = String.Empty,
                data = new ApiUpgradeData
                {
                    warningMessage = UpgradePageHelper.GetWarningMessage(User),
                    availableMembershipPacks = MembershipPack.AllPurchaseablePacks.Select(elem => new ApiMembershipPack(User, elem)).ToList(),
                    availableBalances = availableBalances,
                    memberships = activeMemberships.Rows.Cast<DataRow>().Select(elem => new ApiMembership(elem)).ToList(),
                    balances = ApiMember.GetBalances(User)
                }
            };
        }
    }
}
