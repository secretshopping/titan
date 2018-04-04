using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Offers;
using Prem.PTC.Memberships;
using Prem.PTC.Contests;
using MarchewkaOne.Titan.Balances;
using Titan;

namespace Titan
{
    public class OfferwallCrediter : Crediter
    {

        public Offerwall Wall { get; set; }

        public OfferwallCrediter(Member User, Offerwall offerwall)
            : base(User)
        {
            Wall = offerwall;
        }

        /// <summary>
        /// Fired when credited
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="As"></param>
        /// <param name="OfferwallName"></param>
        /// <param name="RequiresConversion"></param>
        /// <returns></returns>               //Wall.CreditAs, Wall.DisplayName, Wall.RequiresConversion
        public Money CreditMember(Money Input, Offerwall Wall)
        {
            Money Calculated = Credit(Input, Wall.CreditAs, Wall.DisplayName, Wall.RequiresConversion);

            User.CompletedOffersFromOfferwallsToday += 1;

            if (Calculated.GetRealTotals() >= 100)
                User.CompletedOffersMoreThan100pFromOfferwallsToday += 1;

            if (Wall.DisplayName.Contains("Wannads"))
                User.CompletedDailyOffersFromOfferwallsToday += 1;

            //Slot machine chances
            SlotMachine.SlotMachine.TryAddChances(User);

            User.Save();

            ContestManager.IMadeAnAction(ContestType.Offerwalls, User.Name, null, 1);

            //Offerwalls included in Clicks Contest
            if (Wall.IsIncludedInPTCContest)
                ContestManager.IMadeAnAction(ContestType.Click, User.Name, null, 1);

            return Calculated;
        }

        /// <summary>
        /// Fired when reversed
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="As"></param>
        /// <param name="OfferwallName"></param>
        /// <param name="RequiresConversion"></param>
        /// <returns></returns>
        public Money ReverseCredit(Money Input, CreditAs creditAs, string OfferwallName, bool RequiresConversion = false)
        {
            Money Calculated = Credit(Input.Negatify(), creditAs, OfferwallName + " - REVERSAL", RequiresConversion);

            User.CompletedOffersFromOfferwallsToday -= 1;

            if (Calculated.GetRealTotals() <= -100)
                User.CompletedOffersMoreThan100pFromOfferwallsToday -= 1;

            if (OfferwallName.Contains("Wannads"))
                User.CompletedDailyOffersFromOfferwallsToday -= 1;

            User.Save();

            ContestManager.IMadeAnAction(ContestType.Offerwalls, User.Name, null, -1);

            //Offerwalls included in Clicks Contest
            Offerwall currentOfferWall = new Offerwall(OfferwallName);

            if (currentOfferWall.IsIncludedInPTCContest)
                ContestManager.IMadeAnAction(ContestType.Click, User.Name, null, -1);

            return Calculated;
        }

        private Money Credit(Money Input, CreditAs creditAs, string OfferwallName, bool RequiresConversion = false, bool IsReversal = false)
        {
            Money Calculated = CalculatedAndConversion(Input, base.User, Wall);

            if (creditAs == CreditAs.Points)
            {
                Calculated = CLP.CLPManager.ProceedWithOfferwall(Calculated.GetRealTotals(), User);
                base.CreditPoints(Calculated.GetRealTotals(), "Offer Wall: " + OfferwallName, BalanceLogType.Other);
                base.CreditReferersPoints(Calculated.GetRealTotals(), "Offer Wall /ref/ " + User.Name, BalanceLogType.Other);

            }
            if (creditAs == CreditAs.MainBalance)
            {
                Calculated = CLP.CLPManager.ProceedWithOfferwall(Calculated, User);
                base.CreditMainBalance(Calculated, "Offer Wall: " + OfferwallName, BalanceLogType.Other);
                base.CreditReferersMainBalance(Calculated, "Offer Wall /ref/ " + User.Name, BalanceLogType.Other);
            }
            return Calculated;
        }

        protected static Money CalculateRefererEarnings(Member User, Money Input)
        {
            return Money.MultiplyPercent(Input, User.Membership.RefPercentEarningsOfferwalls);
        }

        public static Money CalculatedAndConversion(Money Input, Member member, Offerwall Wall)
        {
            Money Calculated = new Money(Input.ToDecimal());

            //Money to Money in diffrent Currency
            if (Wall.WhatIsSent == WhatIsSent.Money && Wall.CreditAs == CreditAs.MainBalance && AppSettings.Site.CurrencyCode != Wall.CurrencyCode)
            {
                Calculated = Calculated.ExchangeFrom(Wall.CurrencyCode);
            }
            //Poinst to Money
            else if (Wall.WhatIsSent == WhatIsSent.Points && Wall.CreditAs == CreditAs.MainBalance)
            {
                if (AppSettings.Points.PointsEnabled && Wall.UseVirtualCurrencySetting)
                {
                    Calculated = PointsConverter.ToMoney(Calculated.ToDecimal());
                    Calculated = Calculated.ExchangeFrom(Wall.CurrencyCode);
                }
                else
                {
                    Calculated = PointsConverter.ToMoney(Calculated.ToDecimal(), Wall.MoneyToPointsRate.ToDecimal());
                    Calculated = Calculated.ExchangeFrom(Wall.CurrencyCode);
                }
            }
            //Money to Points
            else if (Wall.WhatIsSent == WhatIsSent.Money && Wall.CreditAs == CreditAs.Points)
            {
                Calculated = Calculated.ExchangeFrom(Wall.CurrencyCode);
                Calculated = new Money(PointsConverter.ToPoints(Calculated));
            }
            //Points to Points in diffrent Currency
            else if (Wall.WhatIsSent == WhatIsSent.Points && Wall.CreditAs == CreditAs.Points && AppSettings.Site.CurrencyCode != Wall.CurrencyCode)
            {
                Calculated = PointsConverter.ToMoney(Calculated.ToDecimal(), Wall.MoneyToPointsRate.ToDecimal());
                Calculated = Calculated.ExchangeFrom(Wall.CurrencyCode);
                Calculated = new Money(PointsConverter.ToPoints(Calculated));
            }
            //No conversion

            //Modify by credit percentage
            Calculated = Money.MultiplyPercent(Calculated, Wall.CreditPercentage);

            ///Membership fee
            Calculated = Money.MultiplyPercent(Calculated, member.Membership.OfferwallsProfitPercent);

            return Calculated;
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.MembershipId == user.Membership.Id && c.RefLevel == tier);

            if (commission == null)
                return Money.Zero;

            return Money.MultiplyPercent(amount, commission.OfferwallPercent);
        }
    }
}