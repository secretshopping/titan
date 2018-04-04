using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DiceGameManager
/// </summary>
public class DiceGameManager
{
    /// <summary>
    /// Converts five characters of a hexadecimal string 
    /// to a decimal number from 0 to 99.9999
    /// </summary>
    /// <param name="hexValue"></param>
    /// <returns></returns>
    public static decimal RollTheDice(string hexValue)
    {
        string hexToConvert = "";
        decimal intValue;
        decimal returnValue;
        int count = 0;
        try
        {
            for (int i = 0; i < 5; i++)
            {
                hexToConvert += hexValue[i];
            }
            intValue = Convert.ToInt64(hexToConvert, 16);

            while (intValue >= 1000000)
            {
                hexToConvert = "";
                count += 5;
                for (int i = count; i < count + 5; i++)
                {
                    hexToConvert += hexValue[i];
                }
                intValue = Convert.ToInt64(hexToConvert, 16);
            }
            returnValue = intValue / 10000;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            hexToConvert = "";
            for (int i = 0; i < 4; i++)
            {
                hexToConvert += hexValue[i];
            }
            intValue = Convert.ToInt64(hexToConvert, 16);
            returnValue = intValue / 10000;
        }
        if (returnValue < 0 || returnValue >= 100)
        {
            ErrorLogger.Log(U4200.ROLLERRORLOG);
            throw new MsgException(U4200.ROLLERROR);
        }

        return returnValue;
    }

    /// <summary>
    /// Checks if player won the round
    /// </summary>
    /// <param name="chance">the chance the player chose</param>
    /// <param name="low">true if player chose to bet on values lower than chance</param>
    /// <param name="diceValue">the actual value returned by the dice roll</param>
    /// <returns></returns>
    public static bool HasWon(decimal chance, bool low, decimal diceValue)
    {
        if (low)
        {
            if (diceValue < chance)
                return true;
        }
        else
        {
            if (diceValue >= 100 - chance)
                return true;
        }
        return false;
    }


    public static void TryToBet(Money maxProfit, Money minBet, decimal formChance, Money formBetAmount, int houseEdge, Money formProfit, bool low)
    {
        if (formBetAmount < minBet)
            throw new MsgException(U4200.BETTOOLOW + " " + minBet);
        else if (formChance <= 0)
            throw new MsgException(U4200.CHANCEBELOWZERO);
        else if (formChance > AppSettings.DiceGame.MaxChance)
            throw new MsgException(U4200.CHANCETOOHIGH);

        Money houseProfit = formBetAmount * houseEdge / formChance;

        if (formProfit > maxProfit)
            throw new MsgException(U4200.PROFITTOOHIGH + " " + maxProfit);
        else if (formProfit <= Money.Zero)
            throw new MsgException(U4200.PROFITBELOWZERO);
        else if (houseProfit < new Money(0.00000001))
            throw new MsgException(U4200.HOUSEPROFITTOOLOW);

        Member User = Member.Current;

        DiceGameHash CurrentDiceGameHash = DiceGameHash.Get(User);
        UserBet Bet = new UserBet();
        Bet.UserId = User.Id;

        if (User.PurchaseBalance < formBetAmount)
        {
            throw new MsgException(L1.NOTENOUGHFUNDS);
        }

        string serverSeed = CurrentDiceGameHash.ServerSeedCurrent;
        string clientSeed = CurrentDiceGameHash.ClientSeedCurrent;
        string salt = DiceGameHashLogic.GenerateSalt(clientSeed, Bet.UserId);
        string hashToCompute = DiceGameHashLogic.ComputeHash(salt, serverSeed);
        decimal diceRoll = RollTheDice(hashToCompute);
        bool hasWon = HasWon(formChance, low, diceRoll);
        string query;

        Bet.BetSize = formBetAmount;
        Bet.BetDate = DateTime.Now;
        Bet.Chance = formChance;
        Bet.Low = low;
        if (hasWon)
        {
            Bet.Profit = formProfit;
            User.AddToMainBalance(Bet.Profit, "Dice bet win", BalanceLogType.Other);
            query = SiteInvestmentManager.GetUpdateAmountQuery(formProfit.Negatify());

            //To do: should investors lose money based on betsize or profit?
            //AppSettings.DiceGame.HouseProfit += (houseProfit);
        }
        else
        {
            Bet.Profit = Bet.BetSize.Negatify();
            User.SubtractFromPurchaseBalance(Bet.BetSize, "Dice bet lose", BalanceLogType.Other);
            
            query = SiteInvestmentManager.GetUpdateAmountQuery(formBetAmount);
            //AppSettings.DiceGame.HouseProfit += ((0.01m * formBetAmount));
        }
        Bet.Roll = diceRoll;
        Bet.Save();

        string clearTableQuery = " DELETE FROM SiteInvestments WHERE Amount = 0;";
        TableHelper.ExecuteRawCommandNonQuery(query + clearTableQuery);

        AppSettings.DiceGame.Save();
        User.SaveBalances();
    }

    public static int GetTotalWinsLosses(bool isWin, int? userId = null)
    {
        string and = string.Empty;
        string sign = ">";
        if (!isWin)
            sign = "<";
        if (userId.HasValue)
            and = string.Format("AND UserId = {0}", userId.ToString());

        return Convert.ToInt32(TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM UserBets WHERE Profit {0} 0 {1};", sign, and)));
    }

    public static Money GetTotalProfit(int? userId = null)
    {
        string where = string.Empty;

        if (userId.HasValue)
            where = string.Format("WHERE UserId = {0}", userId.ToString());

        dynamic profit = TableHelper.SelectScalar(string.Format("SELECT SUM(Profit) FROM UserBets {0};", where));
        if (profit is DBNull)
            profit = Money.Zero;
        else profit = Money.Parse(((object)profit).ToString());

        return profit;
    }

    public static Money GetTotalWagered(int? userId = null)
    {
        string where = string.Empty;

        if (userId.HasValue)
            where = string.Format("WHERE UserId = {0}", userId.ToString());

        object wagered = (TableHelper.SelectScalar(string.Format("SELECT SUM(BetSize) FROM UserBets {0};", where)));

        if (wagered is DBNull)
            return Money.Zero;
        else
            return Money.Parse(wagered.ToString());
    }

    //Not included - might be used in future
    //public static decimal GetMaxProfit()
    //{
    //    string query = string.Format("select SUM(Kelly * Amount) from SiteInvestments");
    //    object sum = TableHelper.SelectScalar(query);
    //    decimal maxProfit;
    //    if (String.IsNullOrWhiteSpace(sum.ToString()))
    //        maxProfit = 0;
    //    else
    //        maxProfit = Convert.ToDecimal(((decimal)sum / 100).ToString("N8"));
    //    return maxProfit;
    //}maxprofit=11

}