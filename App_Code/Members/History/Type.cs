using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Members
{
    /// <summary>
    /// Corresponding AchievementType is always related to the Quantity
    /// </summary>
    public enum HistoryType
    {
        Null = 0, 

        Registration = 1, 

        UpgradePurchase = 2,
        UpgradeExpiration = 3, 

        Cashout = 4, 
        Transfer = 5,

        Purchase = 6, 

        // NOTE: When you add new type, make sure to add ALL proper founctions to the History class

        Offerwalls = 7,
        TrafficGrid = 8,
        Contest = 9,

        CPAOffer = 10,
        CPAOfferDenied = 11,

        None = 12,
        CashoutRejection = 13,
        MoneyReturned = 14,

        //CLP
        EliteStatus = 15,
        PrestigeMode = 16,
        JackpotWin = 17,
        BurnCardWin = 18,

        BidPlaced = 19,
        AuctionWon = 20,

        LockedFunds = 21,

        OfferwallRevers = 22,
        Edit = 23,
        LevelExpiration = 24,

        SlotChancesWon = 25,
        SlotMachinePayout = 26,

        InvestmentPlatformDailyCredit = 27,
        InvestmentPlatformBonus = 28,
        InvestmentPlatformReferralCommission = 29,
        InvestmentPlatformSpeedUpBonus = 30,

        InvestmentLevelCashout = 31
    };

}