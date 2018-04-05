using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC.Advertising;
using Prem.PTC.Contests;
using Prem.PTC.Referrals;
using MarchewkaOne.Titan.Balances;
using Prem.PTC.Offers;

namespace Titan
{
    public class ExternalCpaOfferCrediter : Crediter
    {
        public ExternalCpaOfferSubmission Submission { get; set; }
        public ExternalCpaOfferCrediter(ExternalCpaOfferSubmission submission)
            : base(new Member(submission.PublisherId))
        {
            Submission = submission;
        }

        public Money Credit(Action<Money> afterCredit)
        {
            var baseValue = new CPAOffer(Submission.OfferId).BaseValue;
            var amountToCredit = Money.MultiplyPercent(baseValue, User.Membership.PublishersCpaOfferProfitPercentage);
            base.CreditMainBalance(amountToCredit, "External Cpa Offer Submission", BalanceLogType.ExternalCpaOfferSubmission);

            afterCredit(amountToCredit);

            return baseValue - amountToCredit;
        }

        protected override Money CalculateRefEarnings(Member user, Money input, int tier)
        {
            //if (tier > 1)
            //    return Money.Zero;

            //return Money.MultiplyPercent(input, 100);
            return Money.Zero;
        }
    }
}