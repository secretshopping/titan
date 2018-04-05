using Newtonsoft.Json.Linq;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Titan.API
{
    public class BuyJackpotTicketsController : BaseApiController
    {
        protected override ApiResultMessage HandleRequest(object args)
        {
            string token = ((JObject)args)["token"].ToString();
            ApiJackpotTicketPurchaseData data = ((JObject)args).ToObject<ApiJackpotTicketPurchaseData>();

            int userId = ApiAccessToken.ValidateAndGetUserId(token);

            if (!AppSettings.TitanFeatures.MoneyJackpotEnabled)
                throw new MsgException("Jackpot are disabled.");

            Member user = new Member(userId);
            Jackpot jackpot = new Jackpot(data.jackpotId);

            if (data.tickets <= 0)
                throw new MsgException(U5003.INVALIDNUMBEROFTICKETS);

            PurchaseBalances balance = (PurchaseBalances)data.balance;
            BalanceType targetBalance = PurchaseOption.GetBalanceType(balance);

            var purchaseOption = PurchaseOption.Get(PurchaseOption.Features.Jackpot);

            if (balance == PurchaseBalances.Purchase && !purchaseOption.PurchaseBalanceEnabled)
                throw new MsgException("You can't purchase with that balance.");

            if (balance == PurchaseBalances.Cash && !purchaseOption.CashBalanceEnabled)
                throw new MsgException("You can't purchase with that balance.");

            JackpotManager.BuyTickets(jackpot, user, data.tickets, targetBalance);

            return new ApiResultMessage
            {
                success = true,
                message = U5003.TICKETPURCHASESUCCESS.Replace("%n%", data.tickets.ToString()),
                data = null
            };
        }
    }
}
