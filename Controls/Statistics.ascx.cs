using Prem.PTC.Members;
using Prem.PTC.Statistics;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using Prem.PTC;

public partial class Controls_Statistics : System.Web.UI.UserControl
{
    public StatisticsType StatType { get; set; }
    public string StatTitle { get; set; }
    public Unit Width { get; set; }
    public Unit Height { get; set; }

    public bool IsInt { get; set; }

    public string StatisticsID
    {
        get
        {
            return this.ClientID + "Chart";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            MapLayoutPlaceholder.Visible = false;
            DefaultLayoutPlaceholder.Visible = true;

            if (StatType == StatisticsType.UserBalancesPercents)
            {
                CustomChartPlaceHolder.Visible = true;
                StandardChartPlaceHolder.Visible = false;
            }
            if (StatType == StatisticsType.CountriesWithMembers)
            {
                MapChartPlaceholder.Visible = true;
                StandardChartPlaceHolder.Visible = false;

                MapLayoutPlaceholder.Visible = true;
                DefaultLayoutPlaceholder.Visible = false;

                int MaxStatsRows = 5;
                if (TitanFeatures.IsTrafficThunder) MaxStatsRows = 10;

                if (TitanFeatures.IsMuhdnasir) AdditionalMapStatsPlaceHolder.Visible = false;

                //COUNTRY STATS
                var userDict = new CountryUserStatistic().GetDictionary(true, MaxStatsRows);
                var sb = new StringBuilder();
                sb.Append((String.Format("<h4>{0}</h4>", String.Format(U6000.TOPCOUNTRIES, MaxStatsRows))).ToUpper());
                var sum = AppSettings.TotalMembers;
                foreach (var x in userDict)
                {
                    var percent = Convert.ToDecimal(x.Value) / Convert.ToDecimal(sum) * 100;
                    sb.Append(string.Format("<div class='progress'><div class='progress-bar progress-bar-success' style='width:{1:f2}%;'>{0} ({1:f2}%)</div></div>", x.Key.ToString(), percent));
                }
                membersStatsLiteral.Text = sb.ToString();

                //EARNERS STATS
                var earnerDict = new TopEarnersStatistic().GetDictionary(true, MaxStatsRows);
                if (earnerDict.Count > 0)
                {
                    var sb2 = new StringBuilder();
                    sb2.Append((String.Format("<h4>{0}</h4>", String.Format(U6000.TOPEARNERS, MaxStatsRows))).ToUpper());
                    decimal MaxValue = earnerDict.Values.Max();
                    foreach (var topEarner in earnerDict)
                    {
                        var Percent = topEarner.Value == MaxValue ? 100 : topEarner.Value / MaxValue * 100;
                        sb2.Append(string.Format("<div class='progress'><div class='progress-bar progress-bar-success' style='width:{1:f4}%;'>{0} ({2:f4})</div></div>", topEarner.Key, Percent, Money.Parse(MaxValue.ToString())));
                    }
                    topEarnersStatsLiteral.Text = sb2.ToString();
                }
            }
        }
    }

    public string GetChartData()
    {
        return GetChartString(StatType);
    }

    public string GetyAxisName()
    {
        return ChartsManager.YName;
    }
    public string GetxAxisName()
    {
        return ChartsManager.XName;
    }

    public string GetChartTitle()
    {
        return ChartsManager.Title;
    }

    protected string GetChartString(StatisticsType type)
    {
        switch (type)
        {
            case StatisticsType.User_AllCreditedMoney:
                return ChartsManager.GetJsonString(type, null, (Member.CurrentInCache).StatisticsEarned);

            case StatisticsType.Referrals_AllCreditedMoney:
                return ChartsManager.GetJsonString(type, null, (Member.CurrentInCache).StatisticsDirectReferralsEarned);

            case StatisticsType.User_AllPointsCredited:
                return ChartsManager.GetJsonString(type, (Member.CurrentInCache).StatisticsPointsEarned, null);

            case StatisticsType.Referrals_AllCreditedPoints:
                return ChartsManager.GetJsonString(type, (Member.CurrentInCache).StatisticsDirectRefPointsEarned, null);

            case StatisticsType.Referrals_AdPacks:
                return ChartsManager.GetJsonString(type, null, (Member.CurrentInCache).StatisticsDRAdPacksEarned);

            case StatisticsType.User_CashLinksMoney:
                return ChartsManager.GetJsonString(type, null, (Member.CurrentInCache).StatisticsCashLinksEarned);

            case StatisticsType.Referrals_CashLinksMoney:
                return ChartsManager.GetJsonString(type, null, (Member.CurrentInCache).StatisticsDRCashLinksEarned);

            case StatisticsType.User_Clicks:
                return ChartsManager.GetJsonString(type, (Member.CurrentInCache).StatisticsOnlyUserClicks, null);

            case StatisticsType.DRClicks:
                return ChartsManager.GetJsonString(type, (Member.CurrentInCache).DirectReferralsClicks, null);

            case StatisticsType.User_ArticleSharesReads:
                return ChartsManager.GetJsonString(type, (Member.CurrentInCache).StatisticsArticlesTotalSharesReads, null);

            case StatisticsType.User_ArticleSharesMoney:
                return ChartsManager.GetJsonString(type, null, (Member.CurrentInCache).StatisticsArticlesTotalSharesMoney);


            case StatisticsType.UserBalancesPercents:
            case StatisticsType.CountriesWithMembers:
                return ChartsManager.GetJsonDataForCustomCharts(type);

            default:
                return string.Empty;

        }
    }
}