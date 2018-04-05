using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using Titan;

public class CaptchaClaim : Crediter
{
    public CaptchaClaim() : this(Member.Current) { }

    public CaptchaClaim(Member User) : base(User) { }

    public bool Verify(string token)
    {
        using (WebClient client = new WebClient())
        {
            byte[] response =
            client.UploadValues("https://api.coinhive.com/token/verify", new NameValueCollection()
            {
               { "secret", AppSettings.CaptchaClaim.CaptchaClaimSecretKey },
               { "token", token },
               { "hashes", AppSettings.CaptchaClaim.CaptchaClaimHashes.ToString() }
            });

            JToken result = JObject.Parse(System.Text.Encoding.UTF8.GetString(response));
            bool isValid = (bool)result.SelectToken("success");

            if (isValid)
                return true;

            return false;
        }
    }

    public void ClaimPrize()
    {
        ClaimPrize(User.Id);
    }

    public void ClaimPrize(int userId)
    {
        Member user = new Member(userId);
        string balanceLogNote = "Coinhive " + U4200.CLAIM.ToLower();
        
        switch(AppSettings.CaptchaClaim.CaptchaClaimPrizeType)
        {
            case BalanceType.MainBalance:
                this.CreditMainBalance(
                    new Money(AppSettings.CaptchaClaim.CaptchaClaimPrize),
                    balanceLogNote,
                    BalanceLogType.CaptchaClaim);
                break;
            case BalanceType.PointsBalance:
                this.CreditPoints(
                    (int)AppSettings.CaptchaClaim.CaptchaClaimPrize,
                    balanceLogNote,
                    BalanceLogType.CaptchaClaim);
                break;
        }
    }

    public bool VerifyAndClaim(string token)
    {
        return VerifyAndClaim(token, User.Id);
    }

    public bool VerifyAndClaim(string token, int userId)
    {
        bool isValid = Verify(token);

        if(isValid)
        {
            ClaimPrize(userId);
        }

        return isValid;
    }

    protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
    {
        throw new NotImplementedException();
    }
}