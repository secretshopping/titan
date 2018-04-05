using Newtonsoft.Json.Linq;
using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Titan.API
{
    public class GetWithdrawalDataController : BaseApiController
    {
        protected override ApiResultMessage HandleRequest(object args)
        {
            string token = ((JObject)args)["token"].ToString();

            int userId = ApiAccessToken.ValidateAndGetUserId(token);
            Member User = new Member(userId);

            //Available payment processors
            List<ApiPaymentProcessor> PaymentProcessors = GenerateHTMLButtons.CashoutFromItems.
                Select(elem => new ApiPaymentProcessor(elem.Text, elem.Value)).ToList();

            return new ApiResultMessage
            {
                success = true,
                message = String.Empty,
                data = new ApiWithdrawalData(User.GetUnpaidPayoutRequests(), PaymentProcessors, AppSettings.Registration.IsPINEnabled, User.MainBalance)
            };
        }
    }
}
