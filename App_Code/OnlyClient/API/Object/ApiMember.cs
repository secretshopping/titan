using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiMember
    {
        public int id { get; set; }
        public string username { get; set; }
        public ApiMoney totalEarned { get; set; }
        public List<ApiBalance> balances { get; set; }
        public int referralsCount { get; set; }
        public string referralLink { get; set; }

        public ApiMember(Member user)
        {
            id = user.Id;
            username = user.Name;
            totalEarned = new ApiMoney(user.TotalEarned);
            referralsCount = user.GetDirectReferralsCount();
            referralLink = AppSettings.Site.Url + "register.aspx?u=" + user.Id;

            //Balances
            balances = GetBalances(user);
        }

        public static List<ApiBalance> GetBalances(Member user)
        {
            //Balances
            List<ApiBalance> balances = new List<ApiBalance>();
            balances.Add(new ApiBalance("Main", BalanceType.MainBalance, new ApiMoney(user.MainBalance)));
            balances.Add(new ApiBalance("Ad", BalanceType.PurchaseBalance, new ApiMoney(user.PurchaseBalance)));

            if (AppSettings.Payments.CommissionBalanceEnabled)
                balances.Add(new ApiBalance("Commission", BalanceType.CommissionBalance, new ApiMoney(user.PurchaseBalance)));

            if (AppSettings.TitanFeatures.EarnTrafficExchangeEnabled)
                balances.Add(new ApiBalance("Traffic", BalanceType.TrafficBalance, new ApiMoney(user.TrafficBalance)));

            if (AppSettings.Payments.CashBalanceEnabled)
                balances.Add(new ApiBalance("Cash", BalanceType.CashBalance, new ApiMoney(user.CashBalance)));

            //if (AppSettings.Points.PointsEnabled)
            //    balances.Add(new ApiBalance(AppSettings.PointsName, new ApiMoney(new Money(user.PointsBalance))));

            return balances;
        }
    }
}