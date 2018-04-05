using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Statistics
{

    public enum StatisticsType
    {
        Null = 0,

        //Advertising
        PTCClicks = 1,
        BannerClicks = 2,
        FacebookClicks = 3,
        AdPackClicks = 32,

        //Members
        TotalMembersCount = 4,
        MembersPerCountry = 5,
        NewMembers = 6,
        PopularUpgrades = 8,
        NormalRentedReferrals = 9,
        BotRentedReferrals = 13,
        AvailableReferrals = 10,

        //Money
        Cashflow = 11,
        AvailableFunds = 12,

        //USER BASED
        User_Clicks = 16,
        User_AllClicks = 17,
        User_AllCreditedMoney = 18,

        //GPT
        User_PointsEarned = 21,
        User_AllPointsCredited = 22,

        //4000
        PointsGenerated = 24,
        PointsExchanged = 25,
        PointsInSystem = 26,
        SearchesMade = 27,
        VideosWatched = 28,

        DRClicks = 30,
        RRClicks = 31,
        Referrals_AllCreditedMoney = 33,
        Referrals_AllCreditedPoints = 34,
        Referrals_AdPacks = 35,
        User_CashLinksMoney = 36,
        Referrals_CashLinksMoney = 37,

        //5003
        MoneyDistributedPerAdPack = 38,

        //New charts
        UserBalancesPercents = 39,
        CountriesWithMembers = 40,

        //Titan News
        User_ArticleSharesReads = 41,
        User_ArticleSharesMoney = 42
    }
}