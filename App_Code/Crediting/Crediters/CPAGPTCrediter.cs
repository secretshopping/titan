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
using Prem.PTC.Offers;
using MarchewkaOne.Titan.Balances;

namespace Titan
{
    public class CPAGPTCrediter : Crediter
    {
        public bool isLocked { get; set; }
        public CPAGPTCrediter(Member User)
            : base(User)
        {
        }

        #region Credit

        public void CreditManual(OfferRegisterEntry Entry)
        {
            Money Calculated = CalculateManual(Entry);
            Credit(Calculated, Entry.Offer.GetFinalCreditAs(), Entry.Offer.Id, Entry.Offer.Title, 1, false, "");
        }

        public Money CreditFromPostback(Money Input, CreditAs As, string NetworkName, int offerId, string OfferTitle, int CpaOfferId,
            bool RequiresConversion = false, int CPACompletedIncreasedBy = 1)
        {
            Money Calculated = CalculatePostback(Input, RequiresConversion, base.User, As);
            return Credit(Calculated, As, offerId, OfferTitle, CPACompletedIncreasedBy, false, NetworkName);
        }

        #endregion Credit

        #region Reverse

        public void ReverseManual(OfferRegisterEntry Entry)
        {
            Money Calculated = CalculateManual(Entry);
            Reverse(Calculated.Negatify(), Entry.Offer.GetFinalCreditAs(), Entry.Offer.Id, "<b>REVERSAL -</b> " + Entry.Offer.Title);
        }


        public Money ReverseCreditFromPostback(Money Input, CreditAs As, string NetworkName, int offerId,
            string OfferTitle, bool RequiresConversion = false)
        {
            Money Calculated = CalculatePostback(Input, RequiresConversion, base.User, As);
            return Reverse(Calculated.Negatify(), As, offerId, "<b>REVERSAL -</b> " + OfferTitle, NetworkName);
        }

        #endregion Reverse

        #region Base

        public Money Credit(Money Calculated, CreditAs As, int offerId, string OfferTitle, int CPACompletedIncreasedBy = 1, bool isReversal = false,
            string NetworkName = "")
        {
            string balanceLogsNote = "CPA offer (" + (NetworkName != "" ? NetworkName + ": " : "") + OfferTitle + ") ";
            string balanceLogsNoteRef = "CPA offer " + "/ref/" + User.Name + " (" + (NetworkName != "" ? NetworkName + ": " : "") + OfferTitle + ") ";


            if (As == CreditAs.Points)
            {
                Calculated = CLP.CLPManager.ProceedWithCPA(Calculated.GetRealTotals(), User, isReversal);
                base.CreditPoints(Calculated.GetRealTotals(), balanceLogsNote, BalanceLogType.Other);
                base.CreditReferersPoints(Calculated.GetRealTotals(), balanceLogsNoteRef, BalanceLogType.Other);

                if (isReversal == false)
                    History.AddCPAOfferCompleted(User.Name, OfferTitle, Calculated.GetRealTotals().ToString() + " " + AppSettings.PointsName, NetworkName, offerId);

                Mailer.TryToSendCPACreditedMessage(User, Resources.L1.HISTORY_10 + ": " + OfferTitle + "(" + Calculated.GetRealTotals().ToString() + " " + AppSettings.PointsName + ")");
            }
            if (As == CreditAs.MainBalance)
            {
                Calculated = CLP.CLPManager.ProceedWithCPA(Calculated, User, isReversal);
                base.CreditMainBalance(Calculated, balanceLogsNote, BalanceLogType.Other);
                base.CreditReferersMainBalance(Calculated, balanceLogsNoteRef, BalanceLogType.Other, 1, null, true);

                if (isReversal == false)
                    History.AddCPAOfferCompleted(User.Name, OfferTitle, Calculated.ToString(), NetworkName, offerId);

                Mailer.TryToSendCPACreditedMessage(User, Resources.L1.HISTORY_10 + ": " + OfferTitle + "(" + Calculated.ToString() + ")");
            }

            //Slot machine chances
            SlotMachine.SlotMachine.TryAddChances(User);

            //Achievements trial
            User.TryToAddAchievements(
                Prem.PTC.Achievements.Achievement.GetProperAchievements(
                Prem.PTC.Achievements.AchievementType.AfterCPAOffersCompleted, User.TotalCPACompleted));

            User.Save();

            if (!isReversal)
            {
                if (CPAOfferContestManager.IsIncludedInContest(offerId))
                    ContestManager.IMadeAnAction(Prem.PTC.Contests.ContestType.Offer, User.Name, null, 1);
            }
            else
            {
                if (CPAOfferContestManager.IsIncludedInContest(offerId))
                    ContestManager.IMadeAnAction(Prem.PTC.Contests.ContestType.Offer, User.Name, null, -1);
            }

            return Calculated;
        }

        public Money CalculateManual(OfferRegisterEntry entry)
        {
            Money Calculated = Money.MultiplyPercent(entry.Offer.BaseValue, User.Membership.CPAProfitPercent);

            //Recalculate if needed
            if (entry.Offer.GetFinalCreditAs() == CreditAs.Points)
                Calculated = new Money(PointsConverter.ToPoints(Calculated));

            return Calculated;
        }

        public static Money CalculatePostback(Money Input, bool RequiresConversion, Member member, CreditAs As)
        {
            Money Calculated = Money.MultiplyPercent(Input, member.Membership.CPAProfitPercent);

            if (RequiresConversion)
            {
                if (As == CreditAs.Points)
                    Calculated = new Money(PointsConverter.ToPoints(Calculated));

                if (As == CreditAs.MainBalance)
                    Calculated = PointsConverter.ToMoney(Calculated.GetRealTotals());
            }

            return Calculated;
        }

        public Money Reverse(Money Calculated, CreditAs As, int offerId, string OfferTitle, string NetworkName = "")
        {
            return Credit(Calculated, As, offerId, OfferTitle, -1, true, NetworkName);
        }

        #endregion Base
        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            List<Commission> commissions = (List<Commission>)new CommissionsCache().Get();
            var commission = commissions.FirstOrDefault(c => c.RefLevel == tier && c.MembershipId == user.Membership.Id);

            if (commission == null)
                return Money.Zero;

            return Money.MultiplyPercent(amount, commission.CPAOfferPercent);
        }
    }
}