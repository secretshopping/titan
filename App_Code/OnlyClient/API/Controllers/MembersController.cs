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
    public class MembersController : BaseApiController
    {
        protected override ApiResultMessage HandleRequest(object args)
        {
            string token = ((JObject)args)["token"].ToString();

            int userId = ApiAccessToken.ValidateAndGetUserId(token);

            return new ApiResultMessage
            {
                success = true,
                message = String.Empty,
                data = new ApiMember(new Member(userId))
            };
        }
    }
}
