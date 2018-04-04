using System;
using System.Collections.Generic;
using System.Linq;
using Prem.PTC.Members;
using System.Web;
using System.Data;
using Prem.PTC.Referrals;
using Prem.PTC.Payments;
using Prem.PTC.Memberships;

namespace Prem.PTC.Statistics
{

    public class StatisticsManager
    {
        /// <summary>
        /// Recounts all GLOBAL statistics (do NOT handle user statistics)
        /// </summary>
        public static void CRON()
        {
            try
            {
                //TraficGridDailyMoneyLeft daily update
                AppSettings.TrafficGrid.TrafficGridDailyMoneyLeft = AppSettings.TrafficGrid.Limit;
                AppSettings.TrafficGrid.Save();

                //First lets update with current situation
                CheckAndUpdateStatistics(StatisticsType.TotalMembersCount);
                CheckAndUpdateStatistics(StatisticsType.AvailableReferrals);
                CheckAndUpdateStatistics(StatisticsType.NormalRentedReferrals);
                CheckAndUpdateStatistics(StatisticsType.BotRentedReferrals);
                CheckAndUpdateStatistics(StatisticsType.PointsInSystem);
                CheckAndUpdateStatistics(StatisticsType.PointsGenerated);
                CheckAndUpdateStatistics(StatisticsType.MoneyDistributedPerAdPack);

                //Move to yesterday
                var YesterdayStat = new Statistics(StatisticsType.PointsGenerated);
                var YesterdayStatList = TableHelper.GetIntListFromString(YesterdayStat.Data1);
                AppSettings.Points.TotalGeneratedUpToYesterday += YesterdayStatList[0];
                AppSettings.Points.Save();

                //Now we can recount
                var statlist = TableHelper.SelectAllRows<Statistics>();
                foreach (var elem in statlist)
                {
                    if (elem.Type != StatisticsType.AvailableFunds)
                        elem.Data1 = TableHelper.FastRecalculate(elem.Data1);

                    if (!string.IsNullOrEmpty(elem.Data2))
                        elem.Data2 = TableHelper.FastRecalculate(elem.Data2);

                    elem.Save();
                }

                //And populate the cashflow
                Statistics Stat = new Statistics(StatisticsType.Cashflow);
                var list1 = TableHelper.GetMoneyListFromString(Stat.Data1);
                var list2 = TableHelper.GetMoneyListFromString(Stat.Data2);

                list1[0] = list1[1];
                Stat.Data1 = TableHelper.GetStringFromMoneyList(list1);

                list2[0] = list2[1];
                Stat.Data2 = TableHelper.GetStringFromMoneyList(list2);

                Stat.Save();


                //Last, the most risky with exceptions
                CheckAndUpdateStatistics(StatisticsType.AvailableFunds);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }

        public static void CheckAndUpdateStatistics(StatisticsType Type)
        {
            Statistics Stat;

            switch (Type)
            {
                case StatisticsType.TotalMembersCount:
                    Stat = new Statistics(Type);
                    var where1 = new Dictionary<string, object>();

                    where1.Add("AccountStatusInt", (int)MemberStatus.Active);
                    int totalmember = TableHelper.CountOf<Member>(where1);

                    where1.Add(Member.Columns.MembershipName, Memberships.Membership.Standard.Name);
                    int upgradedmember = TableHelper.CountOf<Member>(where1);
                    upgradedmember = totalmember - upgradedmember;

                    //Now update
                    var list1 = TableHelper.GetIntListFromString(Stat.Data1);
                    list1[0] = totalmember;
                    Stat.Data1 = TableHelper.GetStringFromIntList(list1);

                    var list2 = TableHelper.GetIntListFromString(Stat.Data2);
                    list2[0] = upgradedmember;
                    Stat.Data2 = TableHelper.GetStringFromIntList(list2);

                    Stat.Save();
                    break;

                case StatisticsType.PointsInSystem:
                    Stat = new Statistics(Type);
                  
                    //Now update
                    var list888 = TableHelper.GetIntListFromString(Stat.Data1);
                    list888[0] = AppSettings.Points.TotalInSystem;
                    Stat.Data1 = TableHelper.GetStringFromIntList(list888);

                    Stat.Save();
                    break;

                case StatisticsType.PointsGenerated:
                    Stat = new Statistics(Type);

                    //Now update
                    var list999 = TableHelper.GetIntListFromString(Stat.Data1);
                    int totalNow = AppSettings.Points.TotalGenerated;
                    int totalYesterday = AppSettings.Points.TotalGeneratedUpToYesterday;
                    int totalToday = totalNow - totalYesterday;

                    list999[0] = totalToday;
                    Stat.Data1 = TableHelper.GetStringFromIntList(list999);

                    Stat.Save();
                    break;

                case StatisticsType.AvailableReferrals:
                    Stat = new Statistics(Type);
                    //Now update
                    var list3 = TableHelper.GetIntListFromString(Stat.Data1);
                    list3[0] = RentReferralsSystem.GetAvailableNormalReferralsCount();
                    Stat.Data1 = TableHelper.GetStringFromIntList(list3);

                    Stat.Save();
                    break;

                case StatisticsType.NormalRentedReferrals:
                    Stat = new Statistics(Type);
                    //Now update
                    var list4 = TableHelper.GetIntListFromString(Stat.Data1);
                    list4[0] = RentReferralsSystem.GetRentedNormalReferralsCount();
                    Stat.Data1 = TableHelper.GetStringFromIntList(list4);

                    Stat.Save();
                    break;

                case StatisticsType.BotRentedReferrals:
                    Stat = new Statistics(Type);
                    //Now update
                    var list5 = TableHelper.GetIntListFromString(Stat.Data1);
                    list5[0] = RentReferralsSystem.GetRentedBotReferralsCount();
                    Stat.Data1 = TableHelper.GetStringFromIntList(list5);

                    Stat.Save();
                    break;

                case StatisticsType.MoneyDistributedPerAdPack:
                    Stat = new Statistics(Type);
                    //Now update
                    //TO DO: review
                    var list6 = TableHelper.GetMoneyListFromString(Stat.Data1);
                    AdPackType type = AdPackType.Standard;
                    list6[0] = new Money(type.Price.ToDecimal() * (type.FixedDistributionValuePercent / (Decimal)100));
                    Stat.Data1 = TableHelper.GetStringFromMoneyList(list6);

                    Stat.Save();
                    break;

                case StatisticsType.AvailableFunds:

                    //Handle
                    var list = PaymentAccountDetails.AllGateways;

                    foreach (var account in list)
                    {
                        var where = TableHelper.MakeDictionary("Type", (int)StatisticsType.AvailableFunds);
                        where.Add("Data1", StatisticsManager.GetPaymentAccountName(account));

                        var TempStatList = TableHelper.SelectRows<Statistics>(where);
                        if (TempStatList.Count > 0)
                        {
                            Statistics stat = TempStatList[0];
                            if (stat != null)
                            {
                                stat.SetData2(account.Account.Balance);
                                stat.Save();
                            }
                        }
                    }
                    
                    //var where2 = TableHelper.MakeDictionary("Type", (int)StatisticsType.AvailableFunds);
                    //where2.Add("Data1", AppSettings.Cryptocurrencies.APIProvider.ToString());

                    //var tempStatList2 = TableHelper.SelectRows<Statistics>(where2);
                    //if (tempStatList2.Count > 0)
                    //{
                    //    var stat = tempStatList2[0];
                    //    if (stat != null)
                    //    {
                    //        stat.SetData2(BitcoinAPIFacotry.Get().GetAccountBalance());
                    //        stat.Save();
                    //    }
                    //}

                    break;
            }
        }

        public static Dictionary<string, int> GetMembersPerCountryData()
        {
            var dict = new Dictionary<string, int>();
            DataTable dt;

            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                string Command = "SELECT Country, COUNT([UserId]) AS [NN] FROM [Users] WHERE AccountStatusInt = 1 GROUP BY [Country]";
                dt = bridge.Instance.ExecuteRawCommandToDataTable(Command);
            }

            foreach (DataRow dr in dt.Rows)
            {
                dict.Add(dr["Country"].ToString(), Convert.ToInt32(dr["NN"]));
            }

            return dict;
        }

        public static Dictionary<string, int> GetPopularUpgradesData()
        {
            var dict = new Dictionary<string, int>();

            var availableOptions = TableHelper.SelectAllRows<MembershipPack>();
            foreach (MembershipPack pack in availableOptions)
            {
                if (pack.Membership.Name != Membership.Standard.Name)
                {
                    dict.Add(pack.Membership.Name + " (" + pack.TimeDays.ToString() + " days)", pack.CopiesBought);
                }
            }
            return dict;
        }

        public static string GetPaymentAccountName(PaymentAccountDetails account)
        {
            return account.AccountType + ":" + account.Username;
        }

        public static void Add(StatisticsType type, int quantity)
        {
            var stats = new Statistics(type);
            stats.AddToData1(quantity);
            stats.Save();
        }

    }

}