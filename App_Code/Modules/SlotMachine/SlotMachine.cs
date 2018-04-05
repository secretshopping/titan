using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.SlotMachine
{
    public class SlotMachine
    {
        /// <summary>
        /// Requires Save() on Member after using this metod
        /// </summary>
        /// <param name="user"></param>
        /// <param name="chances"></param>
        public static void AddChances(Member user)
        {
            var chances = new Random();
            int chancesWon = chances.Next(AppSettings.SlotMachine.SlotMachineMinChancesGiven, AppSettings.SlotMachine.SlotMachineMaxChancesGiven + 1);

            if (chancesWon > 0)
            {
                user.SlotMachineChances += chancesWon;
                History.AddEntry(user.Name, HistoryType.SlotChancesWon, chancesWon.ToString());
            }
        }

        /// <summary>
        /// Requires Save() on Member after using this metod
        /// </summary>
        /// <param name="user"></param>
        /// <param name="chances"></param>
        public static void TryAddChances(Member user)
        {
            if (AppSettings.TitanFeatures.SlotMachineEnabled)
                AddChances(user);
        }

        public static bool PullTheLever(out int points)
        {
            points = 0;
            if (Member.IsLogged)
            {
                var user = Member.CurrentInCache;
                if (user.SlotMachineChances == 0)
                    return false;

                points = givePoints();

                user.SlotMachineChances--;
                user.AddToPointsBalance(points, "Slot machine reward");
                user.Save();

                if (points >= AppSettings.SlotMachine.SlotMachineMinWinToDisplayInLatestActivity)
                    History.AddEntry(user.Name, HistoryType.SlotMachinePayout, points.ToString());

                return true;
            }
            else
            {
                if ((int)HttpContext.Current.Session["anonymousSlotChances"] == 0)
                    return false;
                else
                {
                    points = givePoints();
                    HttpContext.Current.Session["anonymousSlotChances"] = 0;
                    HttpContext.Current.Session["anonymousSlotMachinePoints"] = points;
                    return true;
                }
            }
        }

        private static int givePoints()
        {
            var points = new Random().Next(AppSettings.SlotMachine.SlotMachineMinRewardValue, AppSettings.SlotMachine.SlotMachineMaxRewardValue);
            var points2 = new Random().Next(AppSettings.SlotMachine.SlotMachineMinRewardValue, AppSettings.SlotMachine.SlotMachineMaxRewardValue);
            return points > points2 ? points2 : points;
        }
    }
}