using System;
using System.Collections.Generic;
using System.Linq;
using Resources;
using Prem.PTC.Members;
using Prem.PTC;
using System.Web;
using Titan;
using MarchewkaOne.Titan.Contests;

namespace Prem.PTC.Contests
{

    /// <summary>
    /// Summary description for ContestManager
    /// </summary>
    public class ContestManager
    {
        public static List<Contest> GetActiveContests(ContestType Type)
        {
            var where = TableHelper.MakeDictionary("Type", (int)Type);
            var preList = TableHelper.SelectRows<Contest>(where);

            var resultList = new List<Contest>();

            foreach (var elem in preList)
            {
                if (elem.Status == ContestStatus.Active &&
                    elem.DateStart <= DateTime.Now &&
                    elem.DateEnd >= DateTime.Now)
                    resultList.Add(elem);
            }

            return resultList;
        }

        public static List<Contest> GetActiveContestsForMember(ContestType Type, Member User)
        {
            var ActiveContests = GetActiveContests(Type);
            var BlockedContestsForMember = TableHelper.SelectRows<ContestsBlocked>(TableHelper.MakeDictionary("UserId", User.Id));

            var resultList = new List<Contest>();

            foreach (var active in ActiveContests)
            {
                bool IsOk = true;

                foreach (var blocked in BlockedContestsForMember)
                {
                    if (blocked.ContestId == active.Id)
                        IsOk = false;
                }

                if (UsersBannedFromContestsType.IsBannedFromContestType(User.Id, Type))
                    IsOk = false;

                if (IsOk)
                    resultList.Add(active);
            }

            return resultList;
        }

        /// <summary>
        /// Fires when member made any action which can be applied for a contest. It checks if memeber
        /// is participating in any contests. If so, the contest data is being updated.
        /// Leave one credit NULL (int=0 or money=NULL)
        /// </summary>
        /// <param name="ActionType"></param>
        /// <returns></returns>
        public static void IMadeAnAction(ContestType ActionType, string Username,
                                         Money MoneyCredit, int IntCredit)
        {
            List<Contest> ContestList = GetMemberParticipatingContests(ActionType, Username);

            foreach (var contest in ContestList)
            {
                if (ActionType == ContestType.Transfer && contest.MinMembersDeposit > MoneyCredit)
                    continue;
                
                var participant = ContestParticipant.GetParticipant(contest, Username);
                participant.Credit(IntCredit, MoneyCredit);
                participant.Save();
            }
        }


        private static List<Contest> GetMemberParticipatingContests(ContestType Type, string Username)
        {
            var activeList = GetActiveContests(Type);
            var resultList = new List<Contest>();

            foreach (var contest in activeList)
            {
                if (contest.IsMemberParticipating(Username))
                    resultList.Add(contest);
            }

            return resultList;
        }


        /// <summary>
        /// Gets 3 latest winners of this contest type
        /// 1st = [0]
        /// 2nd = [1]
        /// 3rd = [2]
        /// </summary>
        public static List<string> GetLastestWinners(ContestType Type)
        {
            List<string> result = new List<string>();

            var LatestWinners = TableHelper.SelectRows<ContestLatestWinners>
                (TableHelper.MakeDictionary("Type", (int)Type));

            if (LatestWinners.Count > 0)
            {

                string res1 = LatestWinners[0].Winner1;
                string res2 = LatestWinners[0].Winner2;
                string res3 = LatestWinners[0].Winner3;

                try
                {
                    res1 = ContestManager.ReparseId(res1);
                }
                catch (Exception ex) { }

                try
                {
                    res2 = ContestManager.ReparseId(res2);
                }
                catch (Exception ex) { }

                try
                {
                    res3 = ContestManager.ReparseId(res3);
                }
                catch (Exception ex) { }

                result.Add(res1);
                result.Add(res2);
                result.Add(res3);
            }
            else
            {
                result.Add("-"); result.Add("-"); result.Add("-");
            }

            return result;
        }

        public static string ReparseId(string input)
        {
            int id1 = input.IndexOf("%");

            string id = input[id1 + 1].ToString();
            PrizeType pType = (PrizeType)Convert.ToInt32(id);
            string text = input.Replace("%" + id + "%", ContestManager.GetPrizeName(pType));

            return text;
        }


        /// <summary>
        /// Returns int or Money depending on the contest type
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Prize"></param>
        /// <returns></returns>
        public static object GetActionProperObject(ContestType Type, Money Points)
        {
            switch (Type)
            {
                case ContestType.Transfer:
                    return Points;

                case ContestType.Null:
                    return null;

                default:
                    return Points.GetRealTotals();
            }
        }

        /// <summary>
        /// Returns int or Money depending on the PrizeType
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Prize"></param>
        /// <returns></returns>
        public static object GetPrizeProperObject(PrizeType Type, Money Prize)
        {
            switch (Type)
            {
                case PrizeType.PurchaseBalance:
                case PrizeType.MainBalance:
                case PrizeType.RentalBalance:
                    return Prize;

                case PrizeType.DirectRefLimit:
                case PrizeType.Points:
                case PrizeType.RentedReferral:
                    return Prize.GetRealTotals();

                case PrizeType.CustomPrize:
                    return "";

                default:
                    return null;
            }
        }

        public static string GetPrizeName(PrizeType Type)
        {
            switch (Type)
            {
                case PrizeType.PurchaseBalance:
                    return U6012.PURCHASEBALANCE;

                case PrizeType.MainBalance:
                    return L1.MAINBALANCE;

                case PrizeType.RentalBalance:
                    return U4200.TRAFFICBALANCE;

                case PrizeType.DirectRefLimit:
                    return L1.DIRECTREFLIMIT;

                case PrizeType.Points:
                    return L1.POINTS;

                case PrizeType.RentedReferral:
                    return L1.RENTED + " ref.";

                default:
                    return null;
            }
        }

        public static void CheckFinished()
        {
            //Lets check if any contest should be finished
            var where = TableHelper.MakeDictionary("Status", (int)ContestStatus.Active);
            var preList = TableHelper.SelectRows<Contest>(where);

            foreach (var elem in preList)
            {
                if (elem.DateEnd < DateTime.Now)
                    elem.Finish();
            }
        }

        public static void AwardMember(Member Winner, PrizeType Type, Money Value, int place)
        {
            //Go with the Crediter
            ContestCrediter Crediter = (ContestCrediter)CrediterFactory.Acquire(Winner, CreditType.Contest);
            Crediter.Credit(Type, Value, place);
        }

        public static int Minimum(int a, int b)
        {
            if (a < b)
                return a;
            else
                return b;
        }
    }
}