using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Titan.API;
using Titan;
using Prem.PTC.Members;
using Prem.PTC;
using Newtonsoft.Json.Linq;

public class AuthenticateController : BaseApiController
{
    protected override ApiResultMessage HandleRequest(object args)
    {
        var data = ((JObject)args).ToObject<ApiAuthenticationData>();
        Member user = null;

        if (!data.isFacebook)
        {
            //Standard login procedure
            TitanAuthService.Login(data.username, data.password, data.secondaryPassword, false);
            user = new Member(data.username);
        }
        else
        {
            //Facebook login procedure
            FacebookMember fbUser = new FacebookMember(data.username);
            TitanAuthService.LoginOrRegister(fbUser, false);
            user = TableHelper.SelectRows<Member>(TableHelper.MakeDictionary("FacebookOAuthId", fbUser.FacebookId))[0];
        }

        var token = ApiAccessToken.GetOrCreate(user.Id);

        return new ApiResultMessage
        {
            success = true,
            message = String.Empty,
            data = new JObject(new JProperty("token", token.Token))
        };
    }
}
