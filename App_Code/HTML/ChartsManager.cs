using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Resources;
using Prem.PTC;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Prem.PTC.Members;

namespace Prem.PTC.Statistics
{
    public class ChartsManager
    {
        public static string Title;
        public static string XName;
        public static string YName;
        public static string Data1Def;
        public static string Data2Def;
        private static JArray JsonArray;
        private static JArray JsonArray2;
        private static JArray jContainer;

        public static string GetJsonDataForCustomCharts(StatisticsType type)
        {
            switch (type)
            {
                case StatisticsType.UserBalancesPercents:
                    var member = Member.CurrentInCache;
                    var array = new JArray();

                    array.Add(GetBalanceObject(U5004.MAIN, member.MainBalance.ToClearString()));
                    array.Add(GetBalanceObject(U6012.PURCHASE, member.PurchaseBalance.ToClearString()));

                    if (AppSettings.Payments.CommissionBalanceEnabled)
                        array.Add(GetBalanceObject(U5004.COMMISSION, member.CommissionBalance.ToClearString()));

                    if (AppSettings.TitanFeatures.EarnTrafficExchangeEnabled)
                        array.Add(GetBalanceObject(U5004.TRAFFIC, member.TrafficBalance.ToClearString()));

                    if (AppSettings.Payments.CashBalanceEnabled)
                        array.Add(GetBalanceObject(U6002.CASH, member.CashBalance.ToClearString()));

                    if (AppSettings.InvestmentPlatform.InvestmentBalanceEnabled)
                        array.Add(GetBalanceObject(U6006.INVESTMENT, member.InvestmentBalance.ToClearString()));

                    if (AppSettings.Payments.MarketplaceBalanceEnabled)
                        array.Add(GetBalanceObject(U5006.MARKETPLACE, member.MarketplaceBalance.ToClearString()));

                    return array.ToString();

                case StatisticsType.CountriesWithMembers:
                    var countries = (List<string>)(new CountriesWithMembersCache()).Get();
                    var json = new JArray(countries);
                    return json.ToString();
            }

            return String.Empty;
        }

        private static JObject GetBalanceObject(string name, string value)
        {
            return new JObject(new JProperty("label", name), new JProperty("value", Convert.ToDecimal(value)));
        }

        public static string GetJsonString(StatisticsType Type, List<int> dataIntList = null, List<Money> dataMoneyList = null)
        {
            XName = L1.DAYSTOCHART;
            YName = L1.CLICKS;
            Title = "";
            JsonArray = new JArray();
            JsonArray2 = new JArray();
            jContainer = new JArray();

            GetChartProperties(Type, dataIntList, dataMoneyList);

            if (Type == StatisticsType.AvailableFunds)
                return jContainer.ToString();

            return JsonArray.ToString().Trim('{', '}');
        }

        public static string GetAdditionalJsonString()
        {
            return JsonArray2.ToString().Trim('{', '}');
        }

        private static void AddValuesToArray(List<int> dataList, JArray jArray, bool noLabels = false)
        {
            var counter = 0;
            for (var i = dataList.Count - 1; i >= 0; --i)
            {
                var jObject = new JObject();

                jObject["label"] = GetDay(i);
                if (!noLabels)
                    jObject["value"] = dataList[i];
                else
                {
                    jObject["x"] = counter++;
                    jObject["y"] = dataList[i];
                }

                jArray.Add(jObject);
            }
        }

        private static void AddValuesToArray(List<Money> dataList, JArray jArray, bool noLabels = false)
        {
            var counter = 0;
            for (var i = dataList.Count - 1; i >= 0; --i)
            {
                var jObject = new JObject();

                jObject["label"] = GetDay(i);
                if (!noLabels)
                    jObject["value"] = dataList[i].ToMulticurrency().ToDecimal();
                else
                {
                    jObject["x"] = counter++;
                    if (!ReferenceEquals(null, dataList[i]))
                        jObject["y"] = dataList[i].ToMulticurrency().ToDecimal();
                    else
                        jObject["y"] = 0;
                }

                jArray.Add(jObject);
            }
        }

        private static void AddValuesToArray(Dictionary<string, int> dictionary, JArray jArray)
        {
            foreach (var elem in dictionary)
            {
                var jObject = new JObject();
                jObject["label"] = elem.Key;
                jObject["value"] = elem.Value;
                jArray.Add(jObject);
            }
        }

        private static void GetChartProperties(StatisticsType Type, List<int> dataIntList = null, List<Money> dataMoneyList = null)
        {
            var sb = new StringBuilder();
            Statistics stat;

            switch (Type)
            {
                case StatisticsType.PTCClicks:                      //ADMIN
                case StatisticsType.FacebookClicks:                 //ADMIN
                case StatisticsType.BannerClicks:                   //ADMIN
                case StatisticsType.NewMembers:                     //ADMIN
                case StatisticsType.AvailableReferrals:             //ADMIN
                case StatisticsType.NormalRentedReferrals:          //ADMIN
                case StatisticsType.BotRentedReferrals:             //ADMIN
                case StatisticsType.PointsExchanged:                //ADMIN
                case StatisticsType.PointsGenerated:                //ADMIN
                case StatisticsType.PointsInSystem:                 //ADMIN
                case StatisticsType.SearchesMade:                   //ADMIN
                case StatisticsType.VideosWatched:                  //ADMIN

                    StatisticsManager.CheckAndUpdateStatistics(Type);

                    switch (Type)
                    {
                        case StatisticsType.NewMembers:
                            YName = "New Members";
                            break;
                        case StatisticsType.AvailableReferrals:
                            YName = "Available referrals";
                            break;
                        case StatisticsType.NormalRentedReferrals:
                            YName = "Normal referrals";
                            break;
                        case StatisticsType.BotRentedReferrals:
                            YName = "Bots";
                            break;
                        case StatisticsType.PointsExchanged:
                        case StatisticsType.PointsInSystem:
                        case StatisticsType.PointsGenerated:
                            YName = AppSettings.PointsName;
                            break;
                        case StatisticsType.VideosWatched:
                            YName = "Videos";
                            break;
                        case StatisticsType.SearchesMade:
                            YName = "Searches";
                            break;
                    }

                    stat = new Statistics(Type);
                    AddValuesToArray(TableHelper.GetIntListFromString(stat.Data1), JsonArray);
                    break;

                case StatisticsType.User_AllCreditedMoney:          //USER
                case StatisticsType.Referrals_AllCreditedMoney:     //USER
                case StatisticsType.Referrals_AdPacks:              //USER
                case StatisticsType.User_CashLinksMoney:            //USER
                case StatisticsType.Referrals_CashLinksMoney:       //USER
                case StatisticsType.User_ArticleSharesMoney:        //USER

                    YName = L1.AMOUNT;
                    AddValuesToArray(dataMoneyList, JsonArray);

                    break;
                //3000
                case StatisticsType.User_AllClicks:
                case StatisticsType.User_Clicks:                    //USER
                case StatisticsType.User_PointsEarned:
                case StatisticsType.User_AllPointsCredited:         //USER
                case StatisticsType.DRClicks:                       //USER
                case StatisticsType.RRClicks:
                case StatisticsType.Referrals_AllCreditedPoints:    //USER
                case StatisticsType.User_ArticleSharesReads:        //USER

                    //Check if Points instead of Clicks
                    if (Type == StatisticsType.User_AllPointsCredited || Type == StatisticsType.User_PointsEarned)
                        YName = AppSettings.PointsName;

                    if (Type == StatisticsType.User_ArticleSharesReads)
                        YName = U6012.READS;

                    AddValuesToArray(dataIntList, JsonArray);

                    break;
                case StatisticsType.PopularUpgrades:

                    XName = "Upgrade packs";
                    YName = "Copies bought";

                    AddValuesToArray(StatisticsManager.GetPopularUpgradesData(), JsonArray);

                    break;
                case StatisticsType.TotalMembersCount:              //ADMIN

                    YName = "Members";

                    StatisticsManager.CheckAndUpdateStatistics(Type);
                    stat = new Statistics(Type);

                    var list1 = TableHelper.GetIntListFromString(stat.Data1);
                    var list2 = TableHelper.GetIntListFromString(stat.Data2);

                    //Data1
                    AddValuesToArray(list1, JsonArray, true);
                    Data1Def = "All active members";
                    //Data2
                    AddValuesToArray(list2, JsonArray2, true);
                    Data2Def = "Upgraded members";

                    break;

                case StatisticsType.Cashflow:                       //ADMIN

                    YName = "Cashflow";

                    StatisticsManager.CheckAndUpdateStatistics(Type);
                    stat = new Statistics(Type);

                    var list3 = TableHelper.GetMoneyListFromString(stat.Data1);
                    var list4 = TableHelper.GetMoneyListFromString(stat.Data2);

                    //Data1
                    AddValuesToArray(list3, JsonArray, true);
                    Data1Def = "Income";
                    //Data2
                    AddValuesToArray(list4, JsonArray2, true);
                    Data2Def = "Outcome";

                    break;

                case StatisticsType.MembersPerCountry:              //ADMIN

                    AddValuesToArray(StatisticsManager.GetMembersPerCountryData(), JsonArray);
                    break;

                case StatisticsType.AvailableFunds:                 //ADMIN

                    StatisticsManager.CheckAndUpdateStatistics(Type);
                    YName = "Available funds";

                    var gatlist = TableHelper.SelectRows<Statistics>(TableHelper.MakeDictionary("Type", (int)Type));

                    if (gatlist.Count > 0)
                    {
                        //Generate gateways list
                        int index;
                        string name;

                        foreach (var elem in gatlist)
                        {
                            var thelist = TableHelper.GetMoneyListFromString(elem.Data2);
                            var array = new JArray();
                            AddValuesToArray(thelist, array, true);
                            var jObject = new JObject();

                            name = elem.Data1;
                            index = name.IndexOf(":", StringComparison.Ordinal);
                            if (index > 0)
                                name = name.Substring(0, index);

                            jObject["key"] = name;
                            jObject["values"] = array;
                            jContainer.Add(jObject);
                        }
                    }
                    break;
                case StatisticsType.Null:
                    break;
                case StatisticsType.AdPackClicks:
                    break;
                case StatisticsType.MoneyDistributedPerAdPack:
                    break;
            }
        }

        /// <summary>
        /// Gets day number. Offset = 0, TOday (Tod) Offset = 1 yesterday(03)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static string GetDay(int offset)
        {
            if (offset == 0) return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(L1.TODAY);
            var result = DateTime.Now.AddDays(-offset).Day.ToString();
            if (result.Length == 1)
                result = "0" + result;
            return result;
        }
    }
}
