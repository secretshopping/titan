using Newtonsoft.Json.Linq;
using Prem.PTC;
using System;
using System.Collections.Generic;

namespace Titan.API
{
    public class GetAllowedBalancesController : BaseApiController
    {
        protected override ApiResultMessage HandleRequest(object args)
        {
            string token = ((JObject)args)["token"].ToString();
            int featureId = (int)((JObject)args)["feature"];

            int userId = ApiAccessToken.ValidateAndGetUserId(token);   
            var purchaseOption = PurchaseOption.Get((PurchaseOption.Features)featureId);

            List<int> availableBalances = new List<int>();

            if (purchaseOption.PurchaseBalanceEnabled)
                availableBalances.Add((int)PurchaseBalances.Purchase);

            if (purchaseOption.CashBalanceEnabled)
                availableBalances.Add((int)PurchaseBalances.Cash);

            return new ApiResultMessage
            {
                success = true,
                message = String.Empty,
                data = availableBalances
            };
        }
    }
}
