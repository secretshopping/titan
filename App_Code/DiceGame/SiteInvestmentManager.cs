using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SiteInvestmentManager
/// </summary>
public static class SiteInvestmentManager
{
    //might be used in future
    //public static decimal GetUsersShare(Member user)
    //{
    //    int id = user.Id;
    //    decimal share;
    //    string query2 = string.Format("select SUM(Kelly * Amount)/100 from SiteInvestments");
    //    object sum = TableHelper.SelectScalar(query2);
    //    string query = string.Format("select (si.Kelly * si.Amount) from SiteInvestments si WHERE si.UserId = {0}", id);
    //    object mySelect = TableHelper.SelectScalar(query);
    //    if (String.IsNullOrWhiteSpace(mySelect.ToString()))
    //        share = 0;
    //    else

    //    share = (decimal)mySelect / (100 * AppSettings.DiceGame.MaxBitCoinProfitPercent * GetCurrentBankroll());

    //    return share;
    //}

    public static Money GetCurrentBankroll()
    {
        Money bankroll;
        string query = string.Format("SELECT SUM(Amount) from SiteInvestments;");
        object mySelect = TableHelper.SelectScalar(query);
        if (mySelect is DBNull)
            bankroll = Money.Zero;
        else
            bankroll = Money.Parse(mySelect.ToString());
        return bankroll;
    }
    public static string GetUpdateAmountQuery(Money bet)
    {
        return string.Format("UPDATE SiteInvestments SET Amount = Amount + ((Amount * KellyInt/100)/(SELECT SUM(Amount * KellyInt/100) from SiteInvestments)) * {0};", bet.ToClearString());
    }


    public static Money GetInvestments(Member user, int? kelly)
    {
        string query;
        if (String.IsNullOrWhiteSpace(kelly.ToString()))
            query = string.Format("SELECT SUM(si.Amount) from SiteInvestments si " +
                                     "LEFT JOIN Users us on us.UserId = si.UserId " +
                                     "WHERE us.UserId = {0}", user.Id);
        else
            query = string.Format("SELECT SUM(si.Amount) from SiteInvestments si " +
                                         "LEFT JOIN Users us on us.UserId = si.UserId " +
                                         "WHERE us.UserId = {0} " +
                                         "AND si.KellyInt = {1}", user.Id, kelly);

        Money totalInvestment;
        object sum = TableHelper.SelectScalar(query);
        if (sum is DBNull)
            totalInvestment = Money.Zero;
        else
            totalInvestment = Money.Parse(sum.ToString());
        return totalInvestment;
    }
    public static void TryDivestAll(Member user)
    {
        TryDivest(null, null, user);
    }

    public static void TryDivest(Money amountToDivest, int? kelly, Member user)
    {
        Money _amountToDivest;
        Money totalInvestment;
        if (amountToDivest == null || kelly == null)
            _amountToDivest = totalInvestment = GetInvestments(user, null);
        else
        {
            if (kelly <= 0 || kelly > AppSettings.DiceGame.MaxKellyLevel)
                throw new MsgException(U4200.KELLYERROR + ' ' + AppSettings.DiceGame.MaxKellyLevel);
            _amountToDivest = amountToDivest;
            totalInvestment = GetInvestments(user, kelly);
        }

        if (totalInvestment == Money.Zero || _amountToDivest > totalInvestment)
            throw new MsgException(U4200.DIVESTTOOHIGH);
        else if (_amountToDivest <= Money.Zero)
            throw new MsgException(U4200.DIVESTTOOLOW);

        user.AddToMainBalance(_amountToDivest, "Dice Game Divestment", BalanceLogType.Other);

        var siteInvestments = SiteInvestment.GetAll(user.Id);

        int i = 0;
        while (_amountToDivest > Money.Zero && i < siteInvestments.Count)
        {
            if (kelly == null || kelly == siteInvestments[i].KellyInt)
            {
                if (_amountToDivest > siteInvestments[i].Amount)
                {
                    siteInvestments[i].Amount -= siteInvestments[i].Amount;
                    _amountToDivest -= siteInvestments[i].Amount;
                }
                else
                {
                    siteInvestments[i].Amount -= _amountToDivest;
                    _amountToDivest = Money.Zero;
                }
                siteInvestments[i].OperationDate = DateTime.Now;
                siteInvestments[i].Save();
                if (siteInvestments[i].Amount == Money.Zero)
                    siteInvestments[i].Delete();
            }
            i++;
        }
        user.SaveBalances();
    }

    public static void TryInvest(Money amountToInvest, int kelly, Member user)
    {
        if (amountToInvest <= Money.Zero)
            throw new MsgException(U4200.INVESTMENTTOOLOW);
        if (amountToInvest > user.PurchaseBalance)
            throw new MsgException(L1.NOTENOUGHFUNDS);
        if (kelly <= 0 || kelly > AppSettings.DiceGame.MaxKellyLevelInt)
            throw new MsgException(U4200.KELLYERROR + ": " + AppSettings.DiceGame.MaxKellyLevelInt);

        user.SubtractFromPurchaseBalance(amountToInvest, "Dice Game Investment");

        SiteInvestment Investment = new SiteInvestment();
        Investment.UserId = user.Id;
        Investment.Amount = amountToInvest;
        Investment.KellyInt = kelly;
        Investment.OperationDate = DateTime.Now;
        Investment.Save();

        user.SaveBalances();

        //might be used in future

        //AppSettings.DiceGame.MaxBitCoinProfit = DiceGameManager.GetMaxProfit();
        //AppSettings.Save();
    }



}

