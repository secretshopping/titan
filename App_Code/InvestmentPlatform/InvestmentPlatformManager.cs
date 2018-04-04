using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using System;
using System.Collections.Generic;
using System.Linq;
using Titan.CustomFeatures;

namespace Titan.InvestmentPlatform
{
    public enum PlanStatus
    {
        Null = 0,
        Active = 1,
        Paused = 2,
        Finished = 3,
        Removed = 4
    }

    public enum SpeedUpOptions
    {
        None = 0,
        Referrals = 1,
        Matrix = 2
    }

    public enum CreditingPolicy
    {
        Automatic = 0,
        Manual = 1
    }

    public enum PlansPolicy
    {
        OneUpgradedPlan = 0,
        UnlimitedPlans = 1
    }

    public class InvestmentPlatformManager
    {
        private static List<InvestmentPlatformPlan> PlatformPlansCache
        {
            get
            {
                var cache = new InvestmentPlatformPlansCache();
                return (List<InvestmentPlatformPlan>)cache.Get();
            }
        }

        private static List<InvestmentUsersPlans> UsersPlansCache
        {
            get
            {
                var cache = new InvestmentUsersPlansCache();
                return (List<InvestmentUsersPlans>)cache.Get();
            }
        }

        public static void CRON()
        {
            try
            {
                if (AppSettings.InvestmentPlatform.InvestmentPlatformEnabled)
                {
                    var activePlans = GetAllActivePlans();

                    foreach (var plan in activePlans)
                    {
                        var usersPlans = GetAllUsersActivePlans().FindAll(x => x.PlanId == plan.Id);

                        if (usersPlans.Count == 0 || plan.Roi == 0 || plan.Time == 0 || plan.Price == Money.Zero)
                            continue;
                        
                        var note = plan.Name + " credit";

                        foreach (var userPlan in usersPlans)
                        {
                            if (userPlan.PurchaseDate.AddDays(plan.EarningDaysDelay) > DateTime.Now)
                                continue;

                            var dailyPool = userPlan.Price * plan.Roi / 100 / plan.Time;

                            //start
                            userPlan.TakeMoneyFromFinishedPlans();

                            userPlan.MoneyReturned += dailyPool;
                            userPlan.MoneyInSystem += dailyPool;

                            Member user = new Member(userPlan.UserId);                            

                            if (AppSettings.InvestmentPlatform.InvestmentPlatformCreditingPolicy == CreditingPolicy.Automatic && userPlan.MoneyInSystem > GetMembershipMinAmountToCredit(user))
                            {
                                Money creditAmount;

                                //Monthly check
                                if (userPlan.CurrentMonthPayout > DateTime.Now.Day * plan.DailyLimit)
                                    userPlan.CurrentMonthPayout = Money.Zero;                                

                                if (!AppSettings.InvestmentPlatform.InvestmentPlatformDailyLimitsEnabled || userPlan.CurrentMonthPayout < plan.MonthlyLimit)
                                {
                                    if (AppSettings.InvestmentPlatform.InvestmentPlatformDailyLimitsEnabled && userPlan.MoneyInSystem > plan.DailyLimit)
                                    {
                                        creditAmount = plan.DailyLimit;
                                        userPlan.MoneyInSystem -= plan.DailyLimit;
                                    }
                                    else
                                    {
                                        creditAmount = userPlan.MoneyInSystem;
                                        userPlan.MoneyInSystem = Money.Zero;
                                    }

                                    //Monthly limit
                                    var leftInMonth = plan.MonthlyLimit - userPlan.CurrentMonthPayout;
                                    if (AppSettings.InvestmentPlatform.InvestmentPlatformDailyLimitsEnabled && creditAmount > leftInMonth)
                                    {
                                        userPlan.MoneyInSystem += (creditAmount - leftInMonth);
                                        creditAmount = leftInMonth;
                                    }

                                    userPlan.CurrentMonthPayout += creditAmount;

                                    var crediter = new InvestmentPlanCrediter(user);
                                    crediter.CreditPlan(creditAmount, BalanceType.MainBalance, note, BalanceLogType.InvestmentPlatformPlanCrediting);
                                    //TO DO AFTER ENABLE INVESTMENT BALANCE
                                    //if (AppSettings.InvestmentPlatform.CreditToInvestmentBalanceEnabled)
                                    //    user.AddToInvestmentBalance(creditAmount, note);
                                    //else
                                    //    user.AddToMainBalance(creditAmount, note);

                                    user.SaveBalances();
                                }
                            }

                            if (userPlan.MoneyReturned >= userPlan.MoneyToReturn)
                            {
                                var overPlus = userPlan.MoneyReturned - userPlan.MoneyToReturn;

                                userPlan.MoneyInSystem += overPlus;
                                userPlan.MoneyReturned = userPlan.MoneyToReturn;
                                userPlan.Finish(false);
                            }
                            userPlan.Save();

                            string historyNote;
                            if (TitanFeatures.IsRetireYoung)
                                historyNote = string.Format("{0} ({1}/{2}) ", note, dailyPool.ToString(), RetireyoungManager.GetAggregate(user.Id));
                            else
                                historyNote = string.Format("{0}: {1}", note, dailyPool.ToString());
                            History.AddEntry(user.Name, HistoryType.InvestmentPlatformDailyCredit, historyNote);                            
                            //end
                        }
                    }
                }
            }
            catch(Exception e)
            {
                ErrorLogger.Log(e);
            }
        }

        private static Money GetMembershipMinAmountToCredit(Member user)
        {
            return new Membership(user.MembershipId).InvestmentPlatformMinAmountToCredited;
        }

        public static void UpdatePlansNumbers()
        {
            var query = string.Format(@"SELECT * FROM {0} WHERE [Status] != {1} ORDER BY [Number]", InvestmentPlatformPlan.TableName, (int)UniversalStatus.Deleted);
            var types = TableHelper.GetListFromRawQuery<InvestmentPlatformPlan>(query);
            var number = 1;
            var resetQuery = string.Format("UPDATE {0} SET {1} = 0", InvestmentPlatformPlan.TableName, InvestmentPlatformPlan.Columns.Number);

            TableHelper.ExecuteRawCommandNonQuery(resetQuery);

            foreach (var type in types)
            {
                type.Number = number;
                type.Save();
                number++;                
            }
        }

        public static List<InvestmentPlatformPlan> GetAllActivePlans()
        {
            if (AppSettings.Side == ScriptSide.AdminPanel)
            {
                var query = string.Format(@"SELECT * FROM {0} WHERE [Status] = {1}", InvestmentPlatformPlan.TableName, (int)UniversalStatus.Active);
                return TableHelper.GetListFromRawQuery<InvestmentPlatformPlan>(query);
            }

            return PlatformPlansCache.FindAll(x => x.Status == UniversalStatus.Active);
        }

        public static List<InvestmentPlatformPlan> GetAllAvailablePlansForUser(int userId)
        {
            var query = Membership.GetSqlQuerryForMembershipsIdListUnderCurrentMembership();
            var membershipIdsList = TableHelper.GetListFromRawQuery(query);
            var activeUserPlans = GetUserActivePlans(userId);

            if (AppSettings.InvestmentPlatform.InvestmentPlatformPlansPolicy == PlansPolicy.UnlimitedPlans || activeUserPlans.Count == 0)
                return GetAllActivePlans().FindAll(x => membershipIdsList.Contains(x.RequiredMembershipId));

            //If PlansPolicy is OneUpgradedPlan users can't have more than 1 active plan!
            if (activeUserPlans.Count > 1)
                throw new MsgException("Inconsistency of data!");

            var plan = new InvestmentPlatformPlan(activeUserPlans[0].PlanId);

            return GetAllActivePlans().FindAll(x => x.Number > plan.Number && membershipIdsList.Contains(x.RequiredMembershipId));
        }

        public static void BuyOrUpgradePlan(Member user, PurchaseBalances targetBalance, InvestmentPlatformPlan newPlan, Money targetPrice = null)
        {
            var userActivePlans = GetUserActivePlans(user.Id);
            var moneyDiff = Money.Zero;

            //UPGRADE
            if (AppSettings.InvestmentPlatform.InvestmentPlatformPlansPolicy == PlansPolicy.OneUpgradedPlan && userActivePlans.Count == 1)
            {
                var activePlan = userActivePlans[0];
                moneyDiff = newPlan.Price - new InvestmentPlatformPlan(activePlan.PlanId).Price;

                activePlan.Finish();
            }

            BuyPlan(user, targetBalance, newPlan, moneyDiff, targetPrice);            
        }

        private static void BuyPlan(Member user, PurchaseBalances targetBalance, InvestmentPlatformPlan plan, Money planDiff, Money targetPrice = null)
        {
            var price = planDiff == Money.Zero ? plan.Price : planDiff;
            var note = string.Format("{0} purchase", plan.Name);

            if (targetPrice != null)
                price = targetPrice;

            //IF TARGET BALANCE != (AR || CASH) IT MEANS THAT WE BUY/UPGRADE FROM PAYMENT BUTTONS
            if (targetBalance == PurchaseBalances.Cash || targetBalance == PurchaseBalances.Purchase)            
                PurchaseOption.ChargeBalance(user, price, PurchaseOption.Features.InvestmentPlatform.ToString(), targetBalance, note, BalanceLogType.InvestmentPlatformPlanPurchase);            
            else
                targetBalance = PurchaseBalances.PaymentProcessor;

            if (AppSettings.InvestmentPlatform.LevelsEnabled)
                InvestmentLevelsManager.CanUserDepositOnLevel(plan, user);

            var userPlan = new InvestmentUsersPlans
            {
                PlanId = plan.Id,
                UserId = user.Id,
                Price = price,
                Status = PlanStatus.Active,
                BalanceBoughtType = targetBalance,
                PurchaseDate = DateTime.Now,
                MoneyReturned = Money.Zero,
                MoneyToReturn = Money.MultiplyPercent(price, plan.Roi),
                CurrentMonthPayout = Money.Zero
            };
            userPlan.Save();

            InvestmentLevelsManager.DepositOnLevel(plan, userPlan.Id, user);

            if(AppSettings.InvestmentPlatform.ProofsEnabled)
            {
                HtmlInvestmentProofGenerator proof;

                if (AppSettings.InvestmentPlatform.LevelsEnabled)
                    proof = new HtmlInvestmentProofGenerator(InvestmentTicket.GetTicket(user.Id, userPlan.Id));                
                else                
                    proof = new HtmlInvestmentProofGenerator(userPlan);

                proof.SendPdfViaEmail();
            }
            
            MatrixBase.TryAddMemberAndCredit(user, price, AdvertType.InvestmentPlan);

            InvestmentPlanCrediter Crediter = new InvestmentPlanCrediter(user);
            Crediter.CreditStructure(price);

            if (user.HasReferer)
            {
                TryToSpeedUpReferrer(user.ReferrerId, price, user.Name);

                Crediter.CreditReferer(price);
            }
        }

        public static bool IsUserHaveActivePlan(Member user, out InvestmentUsersPlans planId)
        {
            planId = GetUserPlanId(user);
            
            return planId != null ? true : false;
        }

        public static List<InvestmentUsersPlans> GetAllUsersActivePlans()
        {
            return UsersPlansCache.FindAll(x => x.Status == PlanStatus.Active);
        }

        public static List<InvestmentUsersPlans> GetUserActivePlans(int userid)
        {
            return GetAllUsersActivePlans().FindAll(x => x.UserId == userid);
        }      

        public static bool DoesAnyUserHaveMoreThanOneActivePlan()
        {
            var query = string.Format("SELECT {0} FROM {1} WHERE {2} = {3} GROUP BY {0} HAVING COUNT({4}) > 1",
                InvestmentUsersPlans.Columns.UserId, InvestmentUsersPlans.TableName, InvestmentUsersPlans.Columns.Status,
                (int)PlanStatus.Active, InvestmentUsersPlans.Columns.Id);

            var list = TableHelper.GetListFromRawQuery<InvestmentUsersPlans>(query);

            if (list.Count > 0)
                return true;

            return false;
        }

        public static InvestmentUsersPlans GetUserPlanId(Member user)
        {
            try
            {
                var id = (UsersPlansCache.FindAll(x => x.UserId == user.Id && x.Status == PlanStatus.Active)).OrderBy(x => x.PurchaseDate).First().Id;
                return new InvestmentUsersPlans(id);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        private static void TryToSpeedUpReferrer(int referrerId, Money planPrice, string username)
        {
            //ONLY Referrals
            //Matrix - TODO
            if (AppSettings.InvestmentPlatform.InvestmentPlatformSpeedUpOption != SpeedUpOptions.Referrals)
                return;

            var query = string.Format(@"
                    WITH CTE AS 
                        (SELECT TOP 1 * FROM InvestmentUsersPlans WHERE UserId = {0} AND Status = {1} ORDER BY PurchasedDate ASC)
                    UPDATE InvestmentUsersPlans 
                        SET MoneyInSystem += 
                            (SELECT BinaryEarning FROM InvestmentPlatformPlans WHERE Id = (SELECT PlanId FROM CTE)) * {2}
                        WHERE Id = (SELECT Id FROM CTE)",
                    referrerId, (int)PlanStatus.Active, planPrice/100);

            TableHelper.ExecuteRawCommandNonQuery(query);

            var bonusQuery = string.Format(@"
                    WITH CTE AS 
                        (SELECT TOP 1 * FROM InvestmentUsersPlans WHERE UserId = {0} AND Status = {1} ORDER BY PurchasedDate ASC)
                    SELECT BinaryEarning FROM InvestmentPlatformPlans WHERE Id = (SELECT PlanId FROM CTE)",
                    referrerId, (int)PlanStatus.Active);

            try
            {
                var user = new Member(referrerId);
                var bonus = (int)TableHelper.SelectScalar(bonusQuery) * planPrice / 100;
                var historyNote = string.Empty;

                if (bonus <= Money.Zero)
                    return;

                if(TitanFeatures.IsRetireYoung)
                    historyNote = string.Format("Investment Plan SpeedUp [ref - {0}] ({1}/{2})", username, bonus.ToString(), RetireyoungManager.GetAggregate(user.Id));
                else
                    historyNote = string.Format("Investment Plan SpeedUp [ref - {0}] ({1})", username, bonus.ToString());

                History.AddEntry(user.Name, HistoryType.InvestmentPlatformSpeedUpBonus, historyNote);
            }
            catch (Exception e) { }
        }
    }
}