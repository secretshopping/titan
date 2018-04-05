using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiReferral
    {
        public string username { get; set; }
        public string email { get; set; }
        public DateTime referralSince { get; set; }
        public DateTime lastActivity { get; set; }
        public ApiMoney totalEarned { get; set; }

        public ApiReferral(Member referral)
        {
            username = referral.Name;
            email = referral.Email;

            referralSince = referral.ReferralSince;
            lastActivity = referral.LastDRActivity;

            totalEarned = new ApiMoney(referral.TotalEarnedToDReferer);         
        }
    }
}