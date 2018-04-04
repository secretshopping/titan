using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan;

/// <summary>
/// Summary description for ShoutboxMessageRestrictions
/// </summary>
public static class TipCommand
{
    public static void TryTipUser(string message, Member currentUser)
    {
        throw new NotImplementedException();
        decimal amount;
        string userToTipName = message.Substring(5).Split(' ')[0];
        string stringamount = message.Substring(5).Split(' ')[1];

        try
        {
            amount = Convert.ToDecimal(message.Substring(5).Split(' ')[1]);

            //Check if number of decimal points == 8
            long intamount = Convert.ToInt64(Convert.ToDecimal(message.Substring(5).Split(' ')[1]) * 100000000);
            decimal testamount = Convert.ToDecimal(intamount) / 100000000;

            if (testamount != amount)
                amount = 0m;
        }
        catch (Exception ex)
        {
            throw new MsgException(U4200.INVALIDSHOUTBOXCOMMAND);
        }
        if (userToTipName == currentUser.Name)
            throw new MsgException("You can't send BTC to yourself");

        Member userToTip = TryFindUser(userToTipName);


        if (userToTip == null)
            throw new MsgException(U4200.INVALIDSHOUTBOXCOMMAND);

        //if (amount > currentUser.BitCoinBalance.ToDecimal())
        //    throw new MsgException(L1.NOTENOUGHFUNDS);

        if (amount <= 0)
            throw new MsgException(U4200.MUSTTIPMORETHANZERO);

        //currentUser.SubtractFromBitCoinBalance(amount, U4200.TIPPED + " " + userToTip.Name, BalanceLogType.Other);
        //userToTip.AddToBitCoinBalance(amount, U4200.TIPFROM + " " + currentUser.Name, BalanceLogType.Other);

        currentUser.SaveBalances();
        userToTip.SaveBalances();

        throw new SuccessMsgException(U4200.SUCCESSFULTIP.Replace("%n%", amount.ToString()) + " " + userToTip.Name);
    }

    private static Member TryFindUser(string user)
    {
        List<Member> member = TableHelper.SelectRows<Member>(TableHelper.MakeDictionary("Username", user));
        if (member.Count != 1)
            throw new MsgException(L1.ER_USER_NOTFOUND);

        return member[0];
    }
}