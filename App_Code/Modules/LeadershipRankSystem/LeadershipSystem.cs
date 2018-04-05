using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Titan.Leadership
{
    /// <summary>
    /// After adding new requirment you have to extend these metods:
    /// - this.ReturnName
    /// - LeadershipRankRequirements.CheckRequirement
    /// - this.IsMoneyRestriction (if it is a money restriction)
    /// add CheckSystem method in script in places where your requirement is increased
    /// if restrictionValue will be different then INT, you have to extend LeadershipRankSystem.aspx and LeadershipRankSystem.aspx.cs
    /// leadershipsystem.aspx.cs - add info to account details
    /// </summary>
    public enum RestrictionKind
    {
        ActiveAdPacks = 0,
        ActiveAdPacksPrice = 1,
        DirectReferralsCount = 2,
        DirectReferralsActiveAdPacks = 3,
        DirectReferralsActiveAdPacksPrice = 4,
        Points = 5,
        RequiredMembership = 6,
        DirectReferralsTotalPaidIn = 7,

        InvestedMoney = 8,
        InvestedMoneyStructure = 9
    }

    /// <summary>
    /// After adding new prize you have to extend these metods:
    /// - this.ReturnName
    /// - LeadershipRank.GivePrize
    /// </summary>
    public enum PrizeKind
    {
        MainBalance = 0,
        Points = 1,
        CustomPrize = 2
    }

    public class LeadershipSystem
    {
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _name, _description;

        public LeadershipSystem() { }

        public static void DeleteSystem()
        {
            ClearRanks();
            ClearRestrictions();
            ClearRanksUsers();
            AppSettings.LeadershipSystem.Name = "";
            AppSettings.LeadershipSystem.Description = "";
            AppSettings.LeadershipSystem.Save();
        }

        public static List<T> GetListFromEnums<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type!");
            }

            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static void ClearRanks()
        {
            TableHelper.DeleteAllRows<LeadershipRank>();
        }

        public static void ClearRestrictions()
        {
            TableHelper.DeleteAllRows<LeadershipRankRequirements>();
        }

        public static void ClearRanksUsers()
        {
            TableHelper.DeleteAllRows<RanksUsers>();
        }

        public static void CheckSystem(List<RestrictionKind> restriction, Member user, int checkReferrerLevels = 0)
        {
            if (!AppSettings.TitanFeatures.LeaderShipSystemEnabled)
                return;
            var list = LeadershipRankRequirements.RequirementsList;

            for (int i = 0; i < list.Count; i++)
            {
                if (restriction.Contains(list[i].RestrictionKind))
                {
                    CheckRankUpDown(user);
                    if (checkReferrerLevels > 0 && user.HasReferer && LeadershipRankRequirements.HaveToCheckDirectReferral())
                        CheckRankUpDown(new Member(user.ReferrerId), --checkReferrerLevels);
                    break;
                }
            }

            LeadershipRank.DeletePausedIfPossible();
        }

        private static void CheckRankUpDown(Member user, int checkReferrerLevels = 0)
        {
            if (!AppSettings.TitanFeatures.LeaderShipSystemEnabled)
                return;

            RanksUsers currentRank = RanksUsers.GetCurrentUserRank(user.Id);
            int newRankId = -1;

            LeadershipRank rank = null;

            if (currentRank == null)
            {
                rank = LeadershipRank.GetFirst();

                if (rank != null && LeadershipRank.IsRankAccured(rank.Id, user))
                {
                    RanksUsers.UpdateRank(rank.Id, user.Id);
                    currentRank = RanksUsers.GetByRankId(user.Id, rank.Id);
                }
                else
                {
                    return;
                }
            }

            rank = new LeadershipRank(currentRank.RankId);
            if (!LeadershipRank.IsRankAccured(rank.Id, user)) newRankId = CheckRankDown(user, rank);
            else newRankId = CheckRankUp(user, rank);

            RanksUsers newRank = RanksUsers.GetByRankId(user.Id, newRankId);
            if (newRank != null) newRank.SetCurrent();
            else RanksUsers.SetRankZero(user.Id);

            if (checkReferrerLevels > 0 && user.HasReferer && LeadershipRankRequirements.HaveToCheckDirectReferral())
                CheckRankUpDown(new Member(user.ReferrerId), --checkReferrerLevels);
        }

        private static int CheckRankUp(Member user, LeadershipRank rank)
        {
            if (rank.NextRankId.HasValue && rank.NextRankId.Value != -1)
            {
                LeadershipRank nextRank = new LeadershipRank(rank.NextRankId.Value);

                if (nextRank.Status != UniversalStatus.Active || LeadershipRank.IsRankAccured(nextRank.Id, user))
                {
                    if(nextRank.Status == UniversalStatus.Active) RanksUsers.UpdateRank(nextRank.Id, user.Id);

                    return CheckRankUp(user, nextRank);
                }
                else if(rank.Status != UniversalStatus.Active)
                {
                    return CheckRankDown(user, rank);
                }
                else
                {
                    return rank.Id;
                }
            }
            else if (rank.Status != UniversalStatus.Active)
            {
                return CheckRankDown(user, rank);
            }
            else
            {
                return rank.Id;
            }
        }

        private static int CheckRankDown(Member user, LeadershipRank rank)
        {
            if (rank.PrevRankId.HasValue && rank.PrevRankId.Value != -1)
            {
                LeadershipRank prevRank = new LeadershipRank(rank.PrevRankId.Value);

                if (prevRank.Status != UniversalStatus.Active || !LeadershipRank.IsRankAccured(prevRank.Id, user))
                {
                    return CheckRankDown(user, prevRank);
                }
                else
                {
                    return rank.PrevRankId.Value;
                }
            }
            else
            {
                return -1;
            }
        }

        public static void CheckRequirementForRanks()
        {
            if (!IsRestrictionForEachRank())
            {
                var querry = string.Format(@"SELECT * FROM {0} WHERE {1} NOT IN (SELECT {2} FROM {3})",
                                           LeadershipRank.TableName, LeadershipRank.Columns.Id, LeadershipRankRequirements.Columns.Rank,
                                           LeadershipRankRequirements.TableName);
                var rankList = TableHelper.GetListFromRawQuery<LeadershipRank>(querry);

                LeadershipRank.DeleteRange(rankList);
            }
        }
        
        public static object ReturnName(object ob)
        {
            if (ob is RestrictionKind)
                return ReturnName((RestrictionKind)ob);

            if (ob is PrizeKind)
                return ReturnName((PrizeKind)ob);

            return ob.ToString();
        }

        public static string ReturnName(PrizeKind prizeKind)
        {
            switch (prizeKind)
            {
                case PrizeKind.MainBalance:
                    return L1.MAINBALANCE;
                case PrizeKind.CustomPrize:
                    return U6005.CUSTOMPRIZE;
                case PrizeKind.Points:
                    return AppSettings.PointsName;
            }
            return null;
        }

        public static string ReturnName(RestrictionKind prizeKind)
        {
            switch (prizeKind)
            {
                case RestrictionKind.DirectReferralsCount:
                    return U6005.DIRECTREFERRALSCOUNT;
                case RestrictionKind.ActiveAdPacks:
                    return string.Format("{0} {1} {2}", U5003.ACTIVE, AppSettings.RevShare.AdPack.AdPackNamePlural, U6005.COUNT);
                case RestrictionKind.ActiveAdPacksPrice:
                    return string.Format("{0} {1} {2}", U5003.ACTIVE, AppSettings.RevShare.AdPack.AdPackNamePlural, L1.PRICE);
                case RestrictionKind.DirectReferralsActiveAdPacks:
                    return string.Format("{0} {1} {2}", U6005.DIRECTREFERRALSACTIVE, AppSettings.RevShare.AdPack.AdPackNamePlural, U6005.COUNT);
                case RestrictionKind.DirectReferralsActiveAdPacksPrice:
                    return string.Format("{0} {1} {2}", U6005.DIRECTREFERRALSACTIVE, AppSettings.RevShare.AdPack.AdPackNamePlural, L1.AMOUNT);
                case RestrictionKind.Points:
                    return AppSettings.PointsName;
                case RestrictionKind.RequiredMembership:
                    return L1.MEMBERSHIP;
                case RestrictionKind.DirectReferralsTotalPaidIn:
                    return U6006.DIRECTREFERRALSTOTALPAIDIN;
                case RestrictionKind.InvestedMoney:
                    return U6012.INVESTEDMONEYINTOPLANS;
                case RestrictionKind.InvestedMoneyStructure:
                    return U6012.INVESTEDMONEYINTOPLANSSTRUCTURE;
            }
            return null;
        }

        public static bool IsRestrictionForEachRank()
        {
            var querry = string.Format(@"SELECT {0} FROM {1} WHERE {0} NOT IN (SELECT {2} FROM {3})",
                                       LeadershipRank.Columns.Rank, LeadershipRank.TableName, LeadershipRankRequirements.Columns.Rank,
                                       LeadershipRankRequirements.TableName);
            var count = TableHelper.SelectScalar(querry);

            if (count == null)
                return true;
            else
                return false;
        }

        public static bool HasOnlyOneRequirement(int rankId)
        {
            var query = string.Format(@"SELECT COUNT(Id) FROM LeadershipRanksRequirements WHERE [Rank] = {0}", rankId);

            return (int)TableHelper.SelectScalar(query) <= 1;
        }

        public static bool IsRestrictionOfThisKindExist(RestrictionKind kind)
        {
            var list = LeadershipRankRequirements.RequirementsList;

            foreach (var item in list)
            {
                if (item.RestrictionKind == kind)
                    return true;
            }
            return false;
        }

        public static bool IsMoneyRestriction(RestrictionKind kind)
        {
            switch (kind)
            {
                case RestrictionKind.ActiveAdPacksPrice:
                case RestrictionKind.DirectReferralsActiveAdPacksPrice:
                case RestrictionKind.DirectReferralsTotalPaidIn:
                case RestrictionKind.InvestedMoney:
                case RestrictionKind.InvestedMoneyStructure:
                    return true;
                default:
                    return false;
            }
        }

        public void Save()
        {
            AppSettings.LeadershipSystem.Name = _name;
            AppSettings.LeadershipSystem.Description = _description;
            AppSettings.LeadershipSystem.Save();
        }

        public void AddRestriction(int rank, RestrictionKind restriction, int value)
        {
            var newRestriction = new LeadershipRankRequirements(rank, restriction, value);
        }

        public void RemoveRestriction(int Id)
        {
            var restriction = new LeadershipRankRequirements(Id);
            if (restriction != null)
            { 
                if (HasOnlyOneRequirement(restriction.Rank))
                    throw new MsgException("You can't remove this Requirement. It's last for this rank. If you want to remove rank use Rank tab.");
                
                restriction.Delete();
            }
        }

        public void AddRank(string name, PrizeKind prize, string prizeValue, string note = null)
        {
            var newRank = new LeadershipRank(name, prize, prizeValue, note);
            newRank.Save();
        }

        public void RemoveRank(int rankId, int rankNumber)
        {
            var rank = new LeadershipRank(rankId);
            if(rank != null) rank.Delete();
        }

        public void EditRank(int Id)
        {
            var rank = new LeadershipRank(Id);
        }
    }
}