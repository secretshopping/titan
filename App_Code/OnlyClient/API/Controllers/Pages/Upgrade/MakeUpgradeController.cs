using Newtonsoft.Json.Linq;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using Prem.PTC.Payments;
using Prem.PTC.Memberships;

namespace Titan.API
{
    public class MakeUpgradeController : BaseApiController
    {
        protected override ApiResultMessage HandleRequest(object args)
        {
            var token = ((JObject)args)["token"].ToString();
            var membershipPackId = Convert.ToInt32(((JObject)args)["membershipPackId"]);
            var htmlResponse = false;
            var balance = (PurchaseBalances)Convert.ToInt32(((JObject)args)["balance"]);

            var userId = ApiAccessToken.ValidateAndGetUserId(token);
            var pack = new MembershipPack(membershipPackId);
            var User = new Member(userId);
            var ResultMessage = U3501.UPGRADEOK;

            if (balance == PurchaseBalances.PaymentProcessor)
            {
                var PackPrice = pack.GetPrice(User);
                var bg = new UpgradeMembershipButtonGenerator(User, PackPrice, pack);
                ResultMessage = GenerateHTMLButtons.GetPaymentButtons(bg);
                htmlResponse = true;
            }
            else            
                Membership.BuyPack(User, pack, balance);            

            return new ApiResultMessage
            {
                success = true,
                message = ResultMessage,
                messageIsHtml = htmlResponse,
                data = null
            };
        }
    }
}
