using Newtonsoft.Json.Linq;
using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Resources;

namespace Titan.API
{
    public class ReferralsController : BaseApiController
    {
        protected override ApiResultMessage HandleRequest(object args)
        {
            string token = ((JObject)args)["token"].ToString();

            int userId = ApiAccessToken.ValidateAndGetUserId(token);
            Member user = new Member(userId);
            List<Member> referrals = user.GetDirectReferralsList();
            List<ApiReferral> apiReferrals = referrals.Select(elem => new ApiReferral(elem)).ToList();

            return new ApiResultMessage
            {
                success = true,
                message = (apiReferrals.Count > 0)? String.Empty : L1.NODIRECTREFERRALS,
                data = apiReferrals
            };
        }
    }
}
