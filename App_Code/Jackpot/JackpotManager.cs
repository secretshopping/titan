using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


public static class JackpotManager
{
    public static int GetNumberOfTickets(int jackpotId)
    {
        int numberOfTickets = (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM JackpotTickets WHERE JackpotId = {0}", jackpotId));
        return numberOfTickets;
    }

    public static int GetNumberOfParticipants(int jackpotId)
    {
        int numberOfParticipants = (int)TableHelper.SelectScalar(string.Format(@"SELECT COUNT(DISTINCT UserId) FROM JackpotTickets
        WHERE JackpotId = {0}", jackpotId));
        return numberOfParticipants;
    }

    public static int GetNumberOfUsersTickets(int jackpotId, int userId)
    {
        int numberOfTickets = (int)TableHelper.SelectScalar(string.Format(@"SELECT COUNT(*) FROM JackpotTickets WHERE JackpotId = {0}
            AND Userid = {1} ", jackpotId, userId));
        return numberOfTickets;
    }

    public static void Finish(Jackpot jackpot)
    {
        try
        {
            jackpot.Status = JackpotStatus.Finished;
            Dictionary<int, int> winnerDictionary = GetWinner(jackpot);

            //Update Jackpot stats before deleting JackpotTickets data
            if (winnerDictionary != null)
            {
                foreach (var winnerData in winnerDictionary)
                {
                    JackpotWinningTicket winningTicket = new JackpotWinningTicket();
                    winningTicket.JackpotId = jackpot.Id;
                    winningTicket.UserWinnerId = winnerData.Value;
                    winningTicket.WinningTicketNumber = winnerData.Key;
                    winningTicket.Save();

                    //Credit winner
                    Member memberWinner = new Member((int)winningTicket.UserWinnerId);
                    
                    CreditWinner(jackpot, winningTicket, memberWinner);
                }
            }

            jackpot.NumberOfParticipants = GetNumberOfParticipants(jackpot.Id);
            jackpot.NumberOfTickets = GetNumberOfTickets(jackpot.Id);
          
            //Delete JackpotTickets data
            TableHelper.DeleteRows<JackpotTicket>(TableHelper.MakeDictionary("JackpotId", jackpot.Id));
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            throw new MsgException(ex.Message);
        }
        finally
        {
            jackpot.Status = JackpotStatus.Finished;
            jackpot.Save();
        }
    }

    private static void CreditWinner(Jackpot jackpot, JackpotWinningTicket winningTicket, Member winner)
    {
        var prizeType = GetPrizeType(jackpot, winner);

        switch (prizeType)
        {
            case JackpotPrize.MainBalance:
                var mainBalancePrize = jackpot.MainBalancePrize / jackpot.NumberOfWinningTickets;
                winner.AddToMainBalance(mainBalancePrize, "Jackpot win", BalanceLogType.Other);
                winner.SaveBalances();
                History.AddJackpotWin(winner.Name, string.Format("{0} - {1}", L1.MAINBALANCE, mainBalancePrize.ToString()), winningTicket.WinningTicketNumber.ToString(), jackpot.Name);
                PoolDistributionManager.SubtractProfit(ProfitSource.Jackpots, mainBalancePrize);
                break;
            case JackpotPrize.AdBalance:
                var adBalancePrize = jackpot.AdBalancePrize / jackpot.NumberOfWinningTickets;
                winner.AddToPurchaseBalance(adBalancePrize, "Jackpot win", BalanceLogType.Other);
                winner.SaveBalances();
                History.AddJackpotWin(winner.Name, string.Format("{0} - {1}", U6012.PURCHASEBALANCE, adBalancePrize.ToString()), winningTicket.WinningTicketNumber.ToString(), jackpot.Name);
                PoolDistributionManager.SubtractProfit(ProfitSource.Jackpots, adBalancePrize);
                break;
            case JackpotPrize.LoginAdsCredits:
                var loginAdsCreditsPrize = jackpot.LoginAdsCreditsPrize / jackpot.NumberOfWinningTickets;
                winner.AddToLoginAdsCredits(loginAdsCreditsPrize, "Jackpot win");
                winner.SaveBalances();
                History.AddJackpotWin(winner.Name, string.Format("{0} - {1}", U5008.LOGINADSCREDITS, loginAdsCreditsPrize.ToString()), winningTicket.WinningTicketNumber.ToString(), jackpot.Name);
                break;
            case JackpotPrize.Upgrade:
                var upgradeMembershipDaysPrize = jackpot.UpgradeDaysPrize / jackpot.NumberOfWinningTickets;

                if (winner.MembershipId == Membership.Standard.Id)
                    winner.Upgrade(new Membership(jackpot.UpgradeIdPrize), new TimeSpan(upgradeMembershipDaysPrize, 0, 0, 0));
                else
                    winner.Upgrade(new Membership(jackpot.UpgradeIdPrize), winner.MembershipExpires.HasValue
                            ? winner.MembershipExpires.Value.AddDays(upgradeMembershipDaysPrize)
                            : AppSettings.ServerTime.AddDays(upgradeMembershipDaysPrize));
                History.AddJackpotWin(winner.Name, string.Format("{0} - {1}", U5008.UPGRADED, winner.Membership.Name), winningTicket.WinningTicketNumber.ToString(), jackpot.Name);
                break;
            default: break;
        }
        JackpotTicketPrize prize = new JackpotTicketPrize();
        prize.JackpotId = jackpot.Id;
        prize.PrizeType = prizeType;
        prize.Save();
    }
    private static JackpotPrize GetPrizeType(Jackpot jackpot, Member winner)
    {
        var options = new List<JackpotPrize>();

        if (jackpot.MainBalancePrizeEnabled)
            options.Add(JackpotPrize.MainBalance);

        if (jackpot.AdBalancePrizeEnabled)
            options.Add(JackpotPrize.AdBalance);

        if (jackpot.LoginAdsCreditsPrizeEnabled)
            options.Add(JackpotPrize.LoginAdsCredits);

        if (jackpot.UpgradePrizeEnabled && CanWinMembership(jackpot, winner))
            options.Add(JackpotPrize.Upgrade);

        if (options.Count == 0)
            throw new MsgException("No prize available for this Jackpot.");

        var random = new Random().Next(0, options.Count);

        var prize = options[random];
        return prize;
    }

    private static bool CanWinMembership(Jackpot jackpot, Member user)
    {
        if (!AppSettings.Points.LevelMembershipPolicyEnabled &&
            new Membership(jackpot.UpgradeIdPrize).Status == MembershipStatus.Active &&
            (user.Membership.Id == Membership.Standard.Id || user.Membership.Id == jackpot.UpgradeIdPrize))
            return true;
        return false;
    }

    /// <summary>
    /// Returns a Dictionary that contains Winning Ticket Number and Winner Id
    /// </summary>
    /// <param name="jackpot"></param>
    /// <returns></returns>
    private static Dictionary<int, int> GetWinner(Jackpot jackpot)
    {
        var jackpotTickets = TableHelper.SelectRows<JackpotTicket>(TableHelper.MakeDictionary("JackpotId", jackpot.Id));
        if (jackpotTickets.Count <= 0)
            return null;

        Random random = new Random();
        var winningTickets = new Dictionary<int, int>();

        for (int i = 0; i < jackpot.NumberOfWinningTickets; i++)
        {
            if (jackpotTickets.Count > 0)
            {
                var tempWinnerIndex = random.Next(0, jackpotTickets.Count);
                int winnerId = jackpotTickets[tempWinnerIndex].UserId;
                var winningTicket = jackpotTickets[tempWinnerIndex].Id;

                winningTickets.Add(winningTicket, winnerId);

                jackpotTickets.RemoveAt(tempWinnerIndex);
            }
        }

        return winningTickets;
    }

    public static void GiveTickets(Jackpot jackpot, Member user, int tickets)
    {
        if (!AppSettings.TitanFeatures.MoneyJackpotEnabled)
            throw new MsgException("Jackpots unavailable");

        for (int i = 0; i < tickets; i++)
        {
            JackpotTicket ticket = new JackpotTicket();
            ticket.JackpotId = jackpot.Id;
            ticket.UserId = user.Id;
            ticket.Save();
        }

        var totalPrice = jackpot.TicketPrice * tickets;
        PoolDistributionManager.AddProfit(ProfitSource.Jackpots, totalPrice);
    }

    public static void BuyTickets(Jackpot jackpot, Member user, int numberOfTickets, BalanceType PurchaseBalanceType = BalanceType.PurchaseBalance)
    {
        if (!AppSettings.TitanFeatures.MoneyJackpotEnabled)
            throw new MsgException("Jackpots unavailable");

        var totalPrice = jackpot.TicketPrice * numberOfTickets;

        if (PurchaseBalanceType == BalanceType.PurchaseBalance && user.PurchaseBalance < totalPrice)
            throw new MsgException(L1.NOTENOUGHFUNDS);

        if (PurchaseBalanceType == BalanceType.CashBalance && user.CashBalance < totalPrice)
            throw new MsgException(L1.NOTENOUGHFUNDS);
        
        if (PurchaseBalanceType == BalanceType.PurchaseBalance)
        {
            user.SubtractFromPurchaseBalance(totalPrice, string.Format("Purchased {0} Jackpot tickets", numberOfTickets), BalanceLogType.Other);
        }
        else if (PurchaseBalanceType == BalanceType.CashBalance)
        {
            user.SubtractFromCashBalance(totalPrice, string.Format("Purchased {0} Jackpot tickets", numberOfTickets), BalanceLogType.Other);
        }

        user.SaveBalances();

        GiveTickets(jackpot, user, numberOfTickets);
    }

    public static List<Jackpot> GetJackpots(bool active)
    {
        int status = active? (int)JackpotStatus.Active : (int)JackpotStatus.Finished;
 
        var jackpots = TableHelper.GetListFromRawQuery<Jackpot>(string.Format(@"SELECT * FROM Jackpots WHERE Status = {0} ORDER BY EndDate DESC;", status));

        if (active)
        {
            foreach (var jackpot in jackpots)
            {
                TryFinish(jackpot);
            }

            return TableHelper.GetListFromRawQuery<Jackpot>(string.Format(@"SELECT * FROM Jackpots WHERE Status = {0} AND StartDate <= GETDATE() ORDER BY EndDate;", status));
        }
        return jackpots;
    }

    public static List<JackpotTicket> GetUsersTickets(int jackpotId, int userId)
    {
        var tickets = TableHelper.GetListFromQuery<JackpotTicket>(string.Format(@"WHERE JackpotId = {0} AND UserId = {1}", jackpotId, userId));
        return tickets;
    }

    public static void TryFinish(Jackpot jackpot)
    {
        if (jackpot.EndDate <= DateTime.Now)
            Finish(jackpot);
    }

    public static bool GetLastJackpotsWinners(out StringBuilder content)
    {
        content = new StringBuilder();

        if (GetJackpots(active: false).Where(x => x.EndDate.AddDays(1) > DateTime.Now).ToList().Count == 0)
            return false;

        var lastFinishedJackpot = GetJackpots(active: false).Where(x => x.EndDate.AddDays(1) > DateTime.Now).First();
        var winners = lastFinishedJackpot.GetDistinctUserWinnerIds();

        if (winners.Count > 0)
        {
            content.Append(U5003.JACKPOT + ": " + lastFinishedJackpot.Name + ", " + U6006.WINNERS + ": ");

            for (int i = 0; i < winners.Count; i++)
            {
                var name = new Member(winners.ElementAt(i)).Name;
                content.Append(name);

                if (i != winners.Count - 1)
                    content.Append(", ");
            }
        }

        if (string.IsNullOrEmpty(content.ToString()))
            return false;

        return true;
    }

    public static void CRON()
    {
        try
        {
            var jackpots = TableHelper.GetListFromRawQuery<Jackpot>(string.Format(@"SELECT * FROM Jackpots WHERE Status = {0} ORDER BY EndDate DESC;", (int)JackpotStatus.Active));
            foreach (var jackpot in jackpots)
            {
                TryFinish(jackpot);
            }
        }
        catch (Exception ex) { ErrorLogger.Log(ex); }
    }
}