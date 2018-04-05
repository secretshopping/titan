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

namespace Titan.API
{
    public class MakePayoutController : BaseApiController
    {
        protected override ApiResultMessage HandleRequest(object args)
        {
            string token = ((JObject)args)["token"].ToString();
            int pin = Convert.ToInt32(((JObject)args)["pin"]);
            decimal amount = Convert.ToDecimal(((JObject)args)["amount"]);
            string processorValue = ((JObject)args)["processor"].ToString();

            int userId = ApiAccessToken.ValidateAndGetUserId(token);
            Member User = new Member(userId);

            User.ValidatePIN(pin.ToString());

            Money Amount = new Money(amount);
            Amount = Money.Parse(Amount.ToShortClearString());

            var userAccountAddress = String.Empty;

            try
            {
                var CustomProcessor = new CustomPayoutProcessor(int.Parse(processorValue));
                userAccountAddress = User.GetPaymentAddress(CustomProcessor.Id);
            }
            catch (Exception)
            {
                var selectedProcessor = processorValue;
                PaymentProcessor targetprocessor = PaymentAccountDetails.GetFromStringType(selectedProcessor);
                userAccountAddress = User.GetPaymentAddress(targetprocessor);
            }

            //Lets process to cashout
            PayoutManager Manager = new PayoutManager(User, Amount, processorValue,
                CustomPayoutProcessor.IsCustomPayoutProcessor(processorValue), CustomPayoutProcessor.GetCustomPayoutProcessorId(processorValue), userAccountAddress);

            string successMessage = Manager.TryMakePayout();

            return new ApiResultMessage
            {
                success = true,
                message = successMessage,
                data = null
            };
        }
    }
}
