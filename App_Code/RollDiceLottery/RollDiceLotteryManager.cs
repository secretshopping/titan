using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;

namespace Titan.RollDiceLottery
{
    [Serializable]
    public class RollDiceLotteryManager
    {
        public static Money GetParticipatePrice  { get { return AppSettings.RollDiceLottery.RollDiceLotteryFeePrice; } }
        public static int GetGameTime { get { return AppSettings.RollDiceLottery.RollDiceLotteryGameTime; } }
        public static Money MainBalancePrize { get { return AppSettings.RollDiceLottery.RollDiceLotteryPrizeMainBalance; } }
        public static Money AdBalancePrize { get { return AppSettings.RollDiceLottery.RollDiceLotteryPrizeAdBalance; } }
        public static int PointsPrize { get { return AppSettings.RollDiceLottery.RollDiceLotteryPrizePoints; } }
        public static int LastResultsCount { get { return (int)AppSettings.RollDiceLottery.RollDiceLotteryLastResultsViewCount; } }

        public int Score;
        public int UserId;        
        public int RollsCount;
        public int[] LastResult;
        public bool IsActive;
        public int Time;
        private DateTime StartTime;

        public void CheckTime()
        {
            Time = (AppSettings.ServerTime - StartTime).Seconds;
            
            if (Time > GetGameTime)
            {
                IsActive = false;
                Time = GetGameTime;
            }
        }

        public RollDiceLotteryManager(int userId)
        {
            Score = 0;
            UserId = userId;
            StartTime = AppSettings.ServerTime;
            RollsCount = 0;
            IsActive = true;
        }

        public void Roll()
        {
            var rnd = new Random();
            var tab = new int[3] { rnd.Next(1, 7), rnd.Next(1, 7), rnd.Next(1, 7) };

            Score += tab[0] + tab[1] + tab[2];
            RollsCount++;
            LastResult = tab;
        }

        public void SendResult()
        {
            try
            {
                AppSettings.DemoCheck();

                if (IsActive)
                {
                    Time = (AppSettings.ServerTime - StartTime).Seconds;
                    IsActive = false;
                }

                if (Time > GetGameTime)
                    Time = GetGameTime;

                if (Score == 0)
                    return;

                var participant = new RollDiceLotteryParticipant
                {
                    UserId = UserId,
                    DateOccured = AppSettings.ServerTime,
                    Score = Score,
                    GameTime = Time,
                    NumberOfRolls = RollsCount,
                    Status = ParticipantStatus.Active
                };
                participant.Save();
            }
            catch (Exception ex) { }
        }

        public static void CRON()
        {
            if (!AppSettings.TitanFeatures.RollDiceLotteryEnabled)
                return;

            try
            {
                var winnerQuery = string.Format("SELECT TOP 1 [UserId] FROM RollDiceLotteryParticipants WHERE [StatusInt] = {0} ORDER BY [Score] DESC, [NumberOfRolls] ASC, [GameTime] ASC", 
                    (int)ParticipantStatus.Active);

                int? userId = (int?)TableHelper.SelectScalar(winnerQuery) ?? null;

                if (userId != null)
                {
                    GivePrizesToWinner(new Member((int)userId));

                    var updateParticipantsStatusQuery = string.Format("UPDATE RollDiceLotteryParticipants SET [StatusInt] = {0} WHERE [StatusInt] = {1}",
                        (int)ParticipantStatus.Recorded, (int)ParticipantStatus.Active);

                    TableHelper.ExecuteRawCommandNonQuery(updateParticipantsStatusQuery);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e);
            }
        }

        private static void GivePrizesToWinner(Member user)
        {
            var note = U6010.ROLLDICELOTTERYWINNER;

            if (MainBalancePrize > Money.Zero)
                user.AddToMainBalance(MainBalancePrize, note);
            if (AdBalancePrize > Money.Zero)
                user.AddToPurchaseBalance(AdBalancePrize, note);
            if (PointsPrize > 0)
                user.AddToPointsBalance(PointsPrize, note);

            user.SaveBalances();
        }
    }
}