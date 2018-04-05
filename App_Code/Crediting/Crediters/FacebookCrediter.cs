using System;
using System.Collections.Generic;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC.Advertising;

namespace Titan
{
    public class FacebookCrediter : Crediter
    {
        public FacebookCrediter(Member User)
            : base(User)
        {
        }

        public void CreditMember(FacebookAdvert Ad, bool credit)
        {
            //OK mark as watched and credit
            List<int> av = User.AdsLiked;
            int userPoints, referrerPoints;

            if (credit)
            {
                av.Add(Ad.Id);
                userPoints = AppSettings.Facebook.PointsPerLike;
                referrerPoints = AppSettings.Facebook.DirectReferralPointsPerLike;
                User.FbLikesToday++;
            }
            else
            {
                av.Remove(Ad.Id);
                userPoints = -AppSettings.Facebook.PointsPerLike;
                referrerPoints = -AppSettings.Facebook.DirectReferralPointsPerLike;
            }
            User.AdsLiked = av;

            //base.CreditMainBalance(Calculated);
            base.CreditPoints(userPoints, "Facebook", BalanceLogType.Other);

            try
            {
                if (AppSettings.Facebook.CreditOnlyUsersWhoPuchasedFacebookMembership && User.Membership.Id == Membership.Standard.Id)
                {
                }
                else
                {
                    var referrer = new Member(User.ReferrerId);
                    CreditPoints(referrer, referrerPoints, "Facebook /ref/" + User.Name, BalanceLogType.Other);
                    referrer.Save();
                }

            }
            catch (Exception e) { }
            User.Save();
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            return Money.Zero;
        }
    }
}