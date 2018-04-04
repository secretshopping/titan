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
using Prem.PTC.Payments;
using Prem.PTC.Memberships;

namespace Titan.API
{
    public class MakeTransferController : BaseApiController
    {
        protected override ApiResultMessage HandleRequest(object args)
        {
            string token = ((JObject)args)["token"].ToString();
            string from = ((JObject)args)["from"].ToString();
            string to = ((JObject)args)["to"].ToString();
            Money amount = new Money(Convert.ToDecimal(((JObject)args)["amount"]));

            bool htmlResponse = false;
            int userId = ApiAccessToken.ValidateAndGetUserId(token);
            Member User = new Member(userId);

            string ResultMessage = TransferHelper.TryInvokeTransfer(from, to, amount, User, ref htmlResponse);

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
