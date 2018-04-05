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
    public class JackpotsController : BaseApiController
    {
        protected override ApiResultMessage HandleRequest(object args)
        {
            string token = ((JObject)args)["token"].ToString();
            bool active = (bool)((JObject)args)["active"];

            int userId = ApiAccessToken.ValidateAndGetUserId(token);
            List<Jackpot> jackpots = JackpotManager.GetJackpots(active);

            List<ApiJackpot> apiJackpots = jackpots.Select(elem => new ApiJackpot(elem, userId)).ToList();
            
            return new ApiResultMessage
            {
                success = true,
                message = String.Empty,
                data = apiJackpots
            };
        }
    }
}
