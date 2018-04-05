using System;
using System.Collections.Generic;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using Prem.PTC.Referrals;

namespace Titan
{
    public class PtcCrediter : Crediter
    {
        public PtcCrediter(Member User) : base(User) { }

        public Money CreditMember(PtcAdvert Ad, bool isFromAutosurf = false)
        {
            Money Calculated = PtcAdvert.CalculateNormalMemberEarnings(User, Ad);

            base.CreditMainBalance(Calculated, "PTC", BalanceLogType.Other);

            //To not overflow the logs
            if (User.Membership.AdvertPointsEarnings > 0)
                base.CreditPoints(User.Membership.AdvertPointsEarnings, "PTC", BalanceLogType.Other);

            //Ad Credits?
            if (AppSettings.PtcAdverts.PTCCreditsEnabled)
            {
                User.AddToPTCCredits(User.Membership.PTCCreditsPerView, "PTC");

                //Credit advertiser
                if (Ad.Advertiser.Is(Advertiser.Creator.Member) && Ad.AdvertiserUserId != User.Id)
                {
                    Member UplineAdvertiser = new Member(Ad.AdvertiserUserId);
                    UplineAdvertiser.AddToPointsBalance(UplineAdvertiser.Membership.PointsYourPTCAdBeingViewed, "PTC Ad Viewed");
                    UplineAdvertiser.SaveBalances();
                    Ad.PointsEarnedFromViews += UplineAdvertiser.Membership.PointsYourPTCAdBeingViewed;
                    Ad.Save();
                }
            }

            //OK mark as watched and credit
            List<int> av = User.AdsViewed;
            av.Add(Ad.Id);
            User.AdsViewed = av;
            User.LastActive = DateTime.Now;

            if (isFromAutosurf)
                User.PtcAutoSurfClicksThisMonth += 1;

            User.PtcSurfClicksThisMonth += 1;

            //Achievements 
            User.TryToAddAchievements(
                Prem.PTC.Achievements.Achievement.GetProperAchievements(
                Prem.PTC.Achievements.AchievementType.AfterClicks, User.TotalClicks + 1));

            Money NewTotalEarned = (User.TotalEarned + Calculated);

            User.TryToAddAchievements(
                Prem.PTC.Achievements.Achievement.GetProperAchievements(
                Prem.PTC.Achievements.AchievementType.AfterEarning, NewTotalEarned.GetRealTotals()));

            User.IncreaseUserStatClicks(1);
            User.IncreaseStatClicks(1);
            User.IncreaseCashLinksEarnings(Calculated);

            //Slot machine chances
            SlotMachine.SlotMachine.TryAddChances(User);

            User.Save();

            //Check the contests
            Prem.PTC.Contests.ContestManager.IMadeAnAction(Prem.PTC.Contests.ContestType.Click, User.Name, null, 1);

            var rrm = new RentReferralsSystem(User.Name, User.Membership);
            rrm.IClicked(Ad);

            TryCreditDirectReferer(Ad);
                    
            return Calculated;
        }

        protected override Money CalculateRefEarnings(Member user, Money amount, int tier)
        {
            return Money.Zero;
        }

        private void TryCreditDirectReferer(PtcAdvert ad)
        {
            if (!User.HasReferer || TitanFeatures.IsGambinos && User.MembershipId == 1)
                return;

            Member referer = new Member(User.ReferrerId);

            if (referer.IsRented)
                return;

            if (referer.HasClickedEnoughToProfitFromReferrals())
            {
                Money amountToCredit = PtcAdvert.CalculateEarningsFromDirectReferral(referer.Membership, ad);

                if(AppSettings.Payments.CommissionBalanceEnabled)
                    referer.AddToCommissionBalance(amountToCredit, "PTC /ref/" + User.Name, BalanceLogType.Other);
                else
                    referer.AddToMainBalance(amountToCredit, "PTC /ref/" + User.Name, BalanceLogType.Other);

                referer.IncreaseEarnings(amountToCredit);
                referer.IncreaseEarningsFromDirectReferral(amountToCredit);
                referer.IncreaseDRClicks();
                referer.IncreaseStatClicks(1);
                referer.IncreaseDRCashLinksEarnings(amountToCredit);

                referer.StatsCommissionsCurrentWeekIncome += amountToCredit;
                referer.StatsCommissionsCurrentMonthIncome += amountToCredit;

                var pointsEarnings = referer.Membership.DirectReferralAdvertClickEarningsPoints;
                referer.AddToPointsBalance(pointsEarnings, "PTC /ref/" + User.Name);
                referer.IncreaseDirectRefPointsEarnings(pointsEarnings);
                referer.IncreasePointsEarnings(pointsEarnings);

                User.TotalPTCClicksToDReferer += 1;
                User.TotalEarnedToDReferer += amountToCredit;
                User.LastDRActivity = AppSettings.ServerTime;
                User.TotalPointsEarnedToDReferer += pointsEarnings;
                User.SaveStatisticsAndBalances();

                bool isFullSaveRequired1 = false;
                bool isFullSaveRequired2 = false;

                //Achievements trial
                isFullSaveRequired1 = referer.TryToAddAchievements(
                    Prem.PTC.Achievements.Achievement.GetProperAchievements(
                    Prem.PTC.Achievements.AchievementType.AfterClicks, referer.TotalClicks + 1));

                string inter = (referer.TotalEarned + amountToCredit).ToClearString();
                Decimal tempMoney = Decimal.Parse(inter, new System.Globalization.CultureInfo("en-US"));

                isFullSaveRequired2 = referer.TryToAddAchievements(
                    Prem.PTC.Achievements.Achievement.GetProperAchievements(
                    Prem.PTC.Achievements.AchievementType.AfterEarning, Convert.ToInt32(tempMoney)));

                if (isFullSaveRequired1 || isFullSaveRequired2)
                    referer.Save();
                else
                    referer.SaveStatisticsAndBalances();
            }
        }

        public static void TryToCreditReferrerAfterPurchase(Member user, Money price, string defaultNote = "PTC purchase")
        {
            if (user.HasReferer)
            {
                var note = string.Format("{0} /ref/{1}", defaultNote, user.Name);
                var referer = new Member(user.ReferrerId);
                var commission = Money.MultiplyPercent(price, referer.Membership.PTCPurchaseCommissionPercent);

                if (commission == Money.Zero)
                    return;

                if (AppSettings.Payments.CommissionBalanceEnabled)
                    referer.AddToCommissionBalance(commission, note);
                else
                    referer.AddToMainBalance(commission, note);

                referer.IncreaseEarnings(commission);
                referer.IncreaseEarningsFromDirectReferral(commission);
                referer.StatsCommissionsCurrentWeekIncome += commission;
                referer.StatsCommissionsCurrentMonthIncome += commission;
                referer.SaveStatisticsAndBalances();

                try
                {
                    if (AppSettings.Emails.NewReferralCommisionEnabled)
                        Mailer.SendNewReferralCommisionMessage(referer.Name, user.Name, commission, referer.Email, note);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                }
            }
        }
    }
}