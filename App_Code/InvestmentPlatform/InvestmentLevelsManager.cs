using ExtensionMethods;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;

namespace Titan.InvestmentPlatform
{
    public class InvestmentLevelsManager
    {
        public static bool LevelsEnabled
        {
            get { return AppSettings.InvestmentPlatform.LevelsEnabled; }
        }

        public static void ChangePlans(DateTime? startDate)
        {
            var query = string.Format(@"UPDATE InvestmentPlatformPlans SET                                
                                        Time = 0,
                                        BinaryEarning = 0,
                                        DailyLimit = 0,
                                        MonthlyLimit = 0,
                                        EndBonus = 0,
                                        EarningDaysDelay = 0,
                                        MaxPrice = 0,
                                        AvailableFromDate = '{0}'",
                                        startDate);

            TableHelper.ExecuteRawCommandNonQuery(query);
        }
        
        public static void DepositOnLevel(InvestmentPlatformPlan platformPlan, int userPlanId, Member user)
        {
            if (!LevelsEnabled)
                return;

            var ticket = AddNewTicket(platformPlan, user.Id, userPlanId);

            //We want to give users money only on every second deposit
            if (ticket.TicketNumber % 2 == 0)
            {
                var targetTicket = GetFirstUnpaidTicketFromLevel(ticket.Level);
                var targetUser = new Member(targetTicket.UserId);
                var targetPP = platformPlan.PaymentProcessor;
                var payoutManager = new PayoutManager(targetUser, targetTicket.LevelEarnings, targetPP.ToString(), false, 0, string.Empty);

                if (payoutManager.TryMakeInvestmentLevelsPayout())
                {
                    targetTicket.Status = TicketStatus.Finished;
                    targetTicket.Save();

                    var targetPlan = new InvestmentUsersPlans(targetTicket.UserPlanId);
                    targetPlan.Finish();
                }
            }
        }

        public static InvestmentTicket AddNewTicket(InvestmentPlatformPlan platformPlan, int userId, int userPlanId)
        {
            var ticket = new InvestmentTicket
            {
                UserPlanId = userPlanId,
                UserId = userId,
                Date = AppSettings.ServerTime,
                Status = TicketStatus.WaitingInQueue,
                LevelPrice = platformPlan.Price,
                LevelFee = platformPlan.LevelFee,
                LevelEarnings = platformPlan.Price * platformPlan.Roi / 100,
                Level = platformPlan.Number,
                TicketNumber = GetTicketNumber(platformPlan.Number)
            };
            ticket.Save();

            return ticket;
        }

        private static int GetTicketNumber(int level)
        {
            var query = string.Format("SELECT MAX({0}) FROM {1} WHERE {2} = {3}",
                InvestmentTicket.Columns.TicketNumber, InvestmentTicket.TableName, InvestmentTicket.Columns.Level, level);

            try
            {
                return (int)TableHelper.SelectScalar(query) + 1;
            }
            catch (Exception e)
            {
                return 1;
            }
        }

        public static InvestmentTicket GetFirstUnpaidTicketFromLevel(int level)
        {
            var query = string.Format("SELECT TOP 1 * FROM {0} WHERE StatusInt = {1} AND {2} = {3} ORDER BY {4} ASC",
                InvestmentTicket.TableName, (int)TicketStatus.WaitingInQueue, InvestmentTicket.Columns.Level, level, InvestmentTicket.Columns.TicketNumber);

            return TableHelper.GetListFromRawQuery<InvestmentTicket>(query)[0];
        }

        public static DateTime GetLastDepositDate(int userId)
        {
            var query = string.Format("SELECT TOP 1 {0} FROM {1} WHERE {2} = {3} ORDER BY {0} DESC",
                InvestmentTicket.Columns.Date, InvestmentTicket.TableName, InvestmentTicket.Columns.UserId, userId);

            try
            {
                return (DateTime)TableHelper.SelectScalar(query);
            }
            catch (Exception e)
            {
                //If user don't have any deposits
                return AppSettings.ServerTime.Zero();
            }
        }

        public static int GetTodaysDepositsOnLevel(int level, int userId)
        {
            var query = string.Format("SELECT COUNT(ID) FROM {0} WHERE CAST({1} AS DATE) = TRY_CONVERT(DATETIME, '{2}', 102) AND {3} = {4}",
                InvestmentTicket.TableName, InvestmentTicket.Columns.Date, AppSettings.ServerTime.Date, InvestmentTicket.Columns.Level, level);

            return (int)TableHelper.SelectScalar(query);
        }

        public static bool IsLevelAvailable(InvestmentPlatformPlan plan)
        {
            return AppSettings.ServerTime >= plan.AvailableFromDate;
        }

        public static bool CanUserDepositOnLevel(InvestmentPlatformPlan plan, Member user)
        {
            var timeDiff = AppSettings.InvestmentPlatform.MinimumTimeBetweenDeposits;
            if ((AppSettings.ServerTime - GetLastDepositDate(user.Id)).TotalMinutes <= timeDiff)
                throw new MsgException(string.Format(U6012.MINIMUMTIMEBETWEENDEPOSITS, timeDiff));

            var plansMaxCount = plan.LevelMaxDepositPerDay;
            if (GetTodaysDepositsOnLevel(plan.Number, user.Id) > plansMaxCount)
                throw new MsgException(string.Format(U6012.MAXDAILYDEPOSITED, plansMaxCount));

            if (!IsLevelAvailable(plan))
                throw new MsgException(string.Format(U6013.LEVELNOTAVAILABLEYET, plan.AvailableFromDate));

            return true;
        }
    }
}