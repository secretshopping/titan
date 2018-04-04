using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.Leadership
{
    [Serializable]
    public class LeadershipRankRequirements : BaseTableObject
    {
        private static string ActiveCondition = string.Format(@"AND (AccountStatusInt = {0} OR AccountStatusInt = {1}) AND IsRented != 1",
                                                        (int)MemberStatus.Active, (int)MemberStatus.VacationMode);
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "LeadershipRanksRequirements"; } }

        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string Rank = "Rank";
            public const string RestrictionKind = "RestrictionKind";
            public const string RestrictionValue = "RestrictionValue";
            public const string RankStatus = "RankStatus";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Rank)]
        public int Rank { get { return rank; } set { rank = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RestrictionKind)]
        private int RestKind { get { return _restrictionKind; } set { _restrictionKind = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RestrictionValue)]
        public int RestrictionValue { get { return _restrictionValue; } set { _restrictionValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RankStatus)]
        protected int RankStatus { get { return _rankStatus; } set { _rankStatus = value; SetUpToDateAsFalse(); } }

        public RestrictionKind RestrictionKind
        {
            get { return (RestrictionKind)RestKind; }
            set { RestKind = (int)value; }
        }

        public UniversalStatus Status
        {
            get { return (UniversalStatus)RankStatus; }
            set { RankStatus = (int)value; }
        }

        public static List<LeadershipRankRequirements> RequirementsList
        {
            get
            {
                var cache = new LeadershipSystemRequirementsCache();
                return (List<LeadershipRankRequirements>)cache.Get();
            }
        }

        public static string RequirementsSortedListQuery
        {
            get

            {
                return String.Format(
                  @"SELECT
                        q.*
                    FROM
                        {0} q
                        INNER JOIN {1} r ON q.{2} = r.Id
                    WHERE
                        q.RankStatus <> {5}
                    ORDER BY r.{3} ASC, q.{4}",
                    LeadershipRankRequirements.TableName, LeadershipRank.TableName,
                    LeadershipRankRequirements.Columns.Rank, LeadershipRank.Columns.Rank, LeadershipRankRequirements.Columns.RestrictionKind
                    , (int)UniversalStatus.Deleted);
            }
        }

        private int _id, rank, _rankStatus, _restrictionKind, _restrictionValue;        

        public LeadershipRankRequirements() : base()
        {
            Status = UniversalStatus.Active;
        }

        public LeadershipRankRequirements(int id) : base(id) { }

        public LeadershipRankRequirements(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public LeadershipRankRequirements(int rank, RestrictionKind restriction, int value) : this()
        {
            Rank = rank;
            RestrictionKind = restriction;
            RestrictionValue = value;
            this.Save();
        }

        public static bool HaveToCheckDirectReferral()
        {
            var list = RequirementsList;
            
            foreach (var item in list)            
                if (item.RestrictionKind == RestrictionKind.DirectReferralsActiveAdPacks
                    || item.RestrictionKind == RestrictionKind.DirectReferralsActiveAdPacksPrice
                    || item.RestrictionKind == RestrictionKind.InvestedMoneyStructure)
                    return true;            

            return false;
        }

        public void Update(RestrictionKind restriction, int value)
        {
            RestrictionKind = restriction;
            RestrictionValue = value;
            this.Save();
        }

        public static void UpdateRankStatus(int rankId, UniversalStatus status)
        {
            Dictionary<string, object> updateSet = new Dictionary<string, object>();
            updateSet.Add(Columns.RankStatus, (int)status);

            Dictionary<string, object> updateWhere = new Dictionary<string, object>();
            updateWhere.Add(Columns.Rank, rankId);

            TableHelper.UpdateRows(TableName, updateSet, updateWhere);
        }

        public bool CheckRequirement(int userId)
        {
            switch (RestrictionKind)
            {
                case RestrictionKind.ActiveAdPacks:
                    return ActiveAdPacks(userId);
                case RestrictionKind.ActiveAdPacksPrice:
                    return ActiveAdPacksPrice(userId);
                case RestrictionKind.DirectReferralsActiveAdPacks:
                    return DirectReferrallsActiveAdPacks(userId);
                case RestrictionKind.DirectReferralsActiveAdPacksPrice:
                    return DirectReferrallsActiveAdPacksPrice(userId);
                case RestrictionKind.DirectReferralsCount:
                    return DirectReferrallsCount(userId);
                case RestrictionKind.Points:
                    return Points(userId);
                case RestrictionKind.RequiredMembership:
                    return RequiredMembership(userId);
                case RestrictionKind.DirectReferralsTotalPaidIn:
                    return DirectReferralsTotalPaidIn(userId);
                case RestrictionKind.InvestedMoney:
                    return InvestedMoneyIntoPlans(userId);
                case RestrictionKind.InvestedMoneyStructure:
                    return InvestedMoneyStructure(userId);
            }
            return false;
        }

        public static object GetRequirmentProgress(int userId, RestrictionKind restrictionKind)
        {
            switch(restrictionKind)
            {
                case RestrictionKind.ActiveAdPacks:
                    return GetActiveAdPacks(userId);
                case RestrictionKind.ActiveAdPacksPrice:
                    return GetActiveAdPacksPrice(userId);
                case RestrictionKind.DirectReferralsActiveAdPacks:
                    return GetDirectReferrallsActiveAdPacks(userId);
                case RestrictionKind.DirectReferralsActiveAdPacksPrice:
                    return GetDirectReferrallsActiveAdPacksPrice(userId);
                case RestrictionKind.DirectReferralsCount:
                    return GetDirectReferrallsCount(userId);
                case RestrictionKind.Points:
                    return GetPoints(userId);
                case RestrictionKind.DirectReferralsTotalPaidIn:
                    return GetDirectReferralsTotalPaidIn(userId);
                case RestrictionKind.InvestedMoney:
                    return GetInvestedMoneyIntoPlans(userId);
                case RestrictionKind.InvestedMoneyStructure:
                    return GetInvestedMoneyStructure(userId);
            }
            return null;
        }

        public static bool IsRestrictionKindUsed(RestrictionKind restrictionKind)
        {
            var query = string.Format(@"SELECT Count(1) FROM LeadershipRanksRequirements WHERE RestrictionKind = {0}", (int)restrictionKind);
            try
            {
                return (int)TableHelper.SelectScalar(query) > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static Money GetInvestedMoneyStructure(int userId)
        {
            var query = string.Format(@"SELECT InvestedIntoPlansStructure FROM Users WHERE UserId = {0}", userId);
            try
            {
                return new Money((decimal)TableHelper.SelectScalar(query));
            }
            catch (Exception e)
            {
                return Money.Zero;
            }
        }

        private bool InvestedMoneyStructure(int userId)
        {
            return GetInvestedMoneyStructure(userId) >= new Money(RestrictionValue) ? true : false;
        }

        private static Money GetInvestedMoneyIntoPlans(int userId)
        {
            var query = string.Format(@"SELECT InvestedIntoPlans FROM Users WHERE UserId = {0}", userId);
            try
            {
                return new Money((decimal)TableHelper.SelectScalar(query));
            }
            catch (Exception e)
            {
                return Money.Zero;
            }
        }

        private bool InvestedMoneyIntoPlans(int userId)
        {
            return GetInvestedMoneyIntoPlans(userId) >= new Money(RestrictionValue) ? true : false;
        }

        private static Money GetDirectReferralsTotalPaidIn(int userId)
        {
            var query = string.Format(@"SELECT SUM(TotalIn) FROM PaymentProportions WHERE UserId IN (SELECT UserId FROM Users WHERE ReferrerId = {0} {1})", userId, ActiveCondition);
            try
            {
                return new Money((decimal)TableHelper.SelectScalar(query));
            }
            catch(Exception e)
            {
                return Money.Zero;
            }
        }

        private bool DirectReferralsTotalPaidIn(int userId)
        {
            return GetDirectReferralsTotalPaidIn(userId) >= new Money(RestrictionValue) ? true : false;
        }

        private static int GetActiveAdPacks(int userId)
        {
            var query = string.Format(@"SELECT COUNT (Id) FROM AdPacks WHERE UserId = {0} AND MoneyReturned < MoneyToReturn", userId);
            return (int)TableHelper.SelectScalar(query);
        }

        private bool ActiveAdPacks(int userId)
        {
            return GetActiveAdPacks(userId) >= RestrictionValue ? true : false;
        }

        private static Money GetActiveAdPacksPrice(int userId)
        {
            var query = string.Format(@"SELECT SUM(apt.Price) FROM AdPacks ap JOIN AdPackTypes apt ON ap.AdPackTypeId = apt.Id WHERE ap.UserId = {0} AND ap.MoneyReturned < ap.MoneyToReturn", userId);
            try
            {
                return new Money((decimal)TableHelper.SelectScalar(query));
            }
            catch (Exception e)
            {
                return Money.Zero;
            }
        }

        private bool ActiveAdPacksPrice(int userId)
        {
            return GetActiveAdPacksPrice(userId) >= new Money(RestrictionValue) ? true : false;           
        }
        
        private static int GetDirectReferrallsActiveAdPacks(int userId)
        {
            var query = string.Format(@"SELECT COUNT (Id) FROM AdPacks WHERE UserId IN (SELECT userid FROM Users WHERE ReferrerId = {0} {1}) AND MoneyReturned < MoneyToReturn", userId, ActiveCondition);
            return (int)TableHelper.SelectScalar(query);
        }

        private bool DirectReferrallsActiveAdPacks(int userId)
        {
            return GetDirectReferrallsActiveAdPacks(userId) >= RestrictionValue ? true : false;
        }

        //FRESH FUNDS
        private static Money GetDirectReferrallsActiveAdPacksPrice(int userId)
        {
            var query = string.Format(@"SELECT SUM(apt.Price) FROM AdPacks ap JOIN AdPackTypes apt ON ap.AdPackTypeId = apt.Id WHERE ap.BalanceBoughtType = 2 AND ap.UserId IN (SELECT userid FROM Users WHERE ReferrerId = {0} {1}) AND ap.MoneyReturned < ap.MoneyToReturn", userId, ActiveCondition);
            try
            {
                return new Money((decimal)TableHelper.SelectScalar(query));
            }
            catch (Exception e)
            {
                return Money.Zero;
            }
        }

        //FRESH FUNDS
        private bool DirectReferrallsActiveAdPacksPrice(int userId)
        {
            return GetDirectReferrallsActiveAdPacksPrice(userId) >= new Money(RestrictionValue) ? true : false;           
        }

        private static int GetDirectReferrallsCount(int userId)
        {
            var query = string.Format(@"SELECT Count(UserId) FROM Users WHERE UserId IN (SELECT userid FROM Users WHERE ReferrerId = {0} {1})", userId, ActiveCondition);
            return (int)TableHelper.SelectScalar(query);
        }

        private bool DirectReferrallsCount(int userId)
        {
            return GetDirectReferrallsCount(userId) >= RestrictionValue ? true : false;
        }

        private static int GetPoints(int userId)
        {
            var query = string.Format(@"SELECT Balance4 FROM Users WHERE UserId = {0}", userId);
            return (int)TableHelper.SelectScalar(query);
        }

        private bool Points(int userId)
        {
            return GetPoints(userId) >= RestrictionValue ? true : false;
        }

        private bool RequiredMembership(int userid)
        {
            var query = string.Format(@"SELECT MembershipId FROM [Memberships] WHERE DisplayOrder <= (SELECT DisplayOrder FROM Memberships WHERE MembershipId = {0})", new Member(userid).MembershipId);
            var result = TableHelper.GetListFromRawQuery(query);

            return result.Contains(RestrictionValue) ? true : false;
        }
    }
}