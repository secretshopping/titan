using Prem.PTC.Members;
using System;
using Prem.PTC;
using Resources;
using Titan;

public static class JackpotPvpManager
{
    public const int BattlesAmountPerStage = 3;
    public const int BotId = -1;

    private static Money CashToWin = Money.Zero;

    #region Actions

    public static bool PlayBattleWithBot(int userId, int selectedStageId)
    {
        if (!JackpotPvpStageBought.CheckIfThereAreBattlesLeftForThisStage(userId, selectedStageId, BattlesAmountPerStage))
            throw new MsgException("There are no battles left for you on this stage."); //Unexpected error

        var CurrentStage = new JackpotPvpStage(selectedStageId);
        decimal PercentForWin = Decimal.Parse(CurrentStage.WinPercent.ToString()) / 100;

        CashToWin = (CurrentStage.Cost / BattlesAmountPerStage) * PercentForWin;

        int AmountOfBattlesInHistoryForCurrentStage = JackpotPvpStageBought.GetSumOfBattlesDoneForStage(userId, selectedStageId);
        JackpotPvpStageBought.IncreaseBattlesCounterFotStage(userId, selectedStageId);

        if (AppSettings.Addons.PvpJackpotForceEveryUsertoAlwaysWin)
        {
            int stagesDoneBefore = AmountOfBattlesInHistoryForCurrentStage / BattlesAmountPerStage;

            int CurrentWinsAmount = 0;
            int CurrentLosesAmount = 0;

            int AmountOfBattlesInCurrentStage = AmountOfBattlesInHistoryForCurrentStage % BattlesAmountPerStage;
            if (AmountOfBattlesInCurrentStage != 0)
            {
                CurrentWinsAmount = JackpotPvpBattleHistory.GetAmountOfWonBattles(userId, selectedStageId) - stagesDoneBefore * 2;
                CurrentLosesAmount = JackpotPvpBattleHistory.GetAmountOfLostBattles(userId, selectedStageId) - stagesDoneBefore;
            }

            if (AmountOfBattlesInCurrentStage == 2 && CurrentWinsAmount == 2)
            {   //give lose for user
                JackpotPvpBattleHistory.AddNewHistoryBattle(selectedStageId, BotId, userId);
                return false;
            }
            else if (AmountOfBattlesInCurrentStage == 0 || (CurrentWinsAmount == 1 && CurrentLosesAmount == 0))
                return RandomAndCreditWinner(userId, selectedStageId);
            else
            {
                //Give win for user
                JackpotPvpBattleHistory.AddNewHistoryBattle(selectedStageId, userId, BotId);
                CreditUserCashBalancForWin(userId, selectedStageId);
                return true;
            }
        }
        else
            return RandomAndCreditWinner(userId, selectedStageId, true);
    }

    public static void AddStageForUser(Member currentMember, int stageId)
    {
        if (!AppSettings.Payments.CashBalanceEnabled)
            throw new MsgException(U6012.CASHBALANCEDISABLED); 

        var SelectedStage = new JackpotPvpStage(stageId);

        if (currentMember.CashBalance < SelectedStage.Cost)
            throw new MsgException(L1.NOTENOUGHFUNDS);

        //Get user's cash
        JackpotPvpCrediter Crediter = new JackpotPvpCrediter(currentMember);
        Crediter.BuyStage(SelectedStage.Cost);

        JackpotPvpStageBought.AddNewStageBought(stageId, currentMember.Id);
    }


    private static void CreditUserCashBalancForWin(int userId, int stageId)
    {
        var CurrentUser = new Member(userId);
        var CurrentStage = new JackpotPvpStage(stageId);

        var Crediter = new JackpotPvpCrediter(CurrentUser);
        Crediter.CreditWin(CashToWin);
    }

    /// <summary>
    /// Return true if user wins. False is bot wins.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="stageId"></param>
    /// <param name="forceBotWinChance"></param>
    /// <returns></returns>
    private static bool RandomAndCreditWinner(int userId, int stageId, bool forceBotWinChance = false)
    {
        Random RND = new Random();
        int CurrentUserLoss = RND.Next(1, 100);
        int WinChancePercent = 50;

        if (forceBotWinChance)
            WinChancePercent = AppSettings.Addons.PvpJackpotBotWinChancePercent;

        if (CurrentUserLoss > WinChancePercent)
        {
            JackpotPvpBattleHistory.AddNewHistoryBattle(stageId, userId, BotId);
            CreditUserCashBalancForWin(userId, stageId);
            return true;
        }
        else
            JackpotPvpBattleHistory.AddNewHistoryBattle(stageId, BotId, userId);
        return false;
    }

    #endregion


    #region Helpers

    public static void TryCheckSystemPoolsCash(int selectedStageId)
    {
        var CurrentStage = new JackpotPvpStage(selectedStageId);
        Money MoneyToWin = (CurrentStage.Cost / BattlesAmountPerStage) * Decimal.Parse(CurrentStage.WinPercent.ToString()) / 100;

        if (PoolDistributionManager.GetGlobalPoolSumInMoney(PoolsHelper.GetBuiltInProfitPoolId(Pools.PvpJackpotGamePool)) < MoneyToWin)
        {
            if (TitanFeatures.IsNightwolf)
            {
                if (PoolDistributionManager.GetGlobalPoolSumInMoney(PoolsHelper.GetBuiltInProfitPoolId(Pools.AdministratorProfit)) < MoneyToWin)
                    throw new MsgException(U6011.NOCREDITSINSYSTEM);
            }
            else
                throw new MsgException(U6011.NOCREDITSINSYSTEM);
        }
    }

    #endregion

}