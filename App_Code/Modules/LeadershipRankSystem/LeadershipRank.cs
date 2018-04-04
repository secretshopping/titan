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
    public class LeadershipRank : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "LeadershipRanks"; } }

        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string Rank = "Rank";
            public const string RankName = "RankName";
            public const string PrizeKind = "PrizeKind";
            public const string PrizeValue = "PrizeValue";
            public const string Note = "Note";
            public const string MatrixCyclesPerDay = "MatrixCyclesPerDay";
            public const string PrevRankId = "PrevRankId";
            public const string NextRankId = "NextRankId";
            public const string RankStatus = "RankStatus";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Rank)]
        public int Rank { get { return _rank; } set { _rank = value;  SetUpToDateAsFalse(); } }

        [Column(Columns.RankName)]
        public string RankName { get { return _rankName; } set { _rankName = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PrizeKind)]
        private int _PrizeKind { get { return prizeKind; } set { prizeKind = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PrizeValue)]
        public string PrizeValue { get { return _prizeValue; } set { _prizeValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MatrixCyclesPerDay)]
        public int MatrixCyclesPerDay { get { return _MatrixCyclesPerDay; } set { _MatrixCyclesPerDay = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Note)]
        public string Note { get { return _note; } set { _note = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PrevRankId)]
        public int? PrevRankId { get { return _PrevRankId; } set { _PrevRankId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.NextRankId)]
        public int? NextRankId { get { return _NextRankId; } set { _NextRankId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.RankStatus)]
        protected int RankStatus { get { return _rankStatus; } set { _rankStatus = value; SetUpToDateAsFalse(); } }

        public PrizeKind PrizeKind
        {
            get { return (PrizeKind)_PrizeKind; }
            set { _PrizeKind = (int)value; }
        }

        public UniversalStatus Status
        {
            get { return (UniversalStatus)RankStatus; }
            set { RankStatus = (int)value; }
        }

        public static List<LeadershipRank> RankList
        {
            get
            {
                var cache = new LeadershipSystemRanksCache();
                return (List<LeadershipRank>)cache.Get();
            }
        }

        public static string SelectCommand
        {
            get
            {
                return string.Format(@"SELECT * FROM {0} WHERE {1} <> {3} ORDER BY {2} ASC",
                    LeadershipRank.TableName,
                    LeadershipRank.Columns.RankStatus, LeadershipRank.Columns.Rank,
                    (int)UniversalStatus.Deleted);
            }
        }

        private int _id, _rank, prizeKind, _MatrixCyclesPerDay, _rankStatus;
        private int? _PrevRankId, _NextRankId;
        private string _rankName, _prizeValue, _note;

        public LeadershipRank() : base()
        {
            PrevRankId = -1;
            NextRankId = -1;
            Status = UniversalStatus.Active;
        }

        public LeadershipRank(int id) : base(id) { }

        public LeadershipRank(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public LeadershipRank(string name, PrizeKind prize, string prizeValue, string note = null) : this()
        {
            RankName = name;
            PrizeKind = prize;
            PrizeValue = prizeValue;
            Note = note;

            SetRankIds();
        }

        /// <summary>
        /// Returns first rank Active rank
        /// </summary>
        /// <returns></returns>
        public static LeadershipRank GetFirst()
        {
            LeadershipRank result = LeadershipRank.RankList.FirstOrDefault(x => !x.PrevRankId.HasValue || x.PrevRankId.Value == -1);

            if (result != null && result.Status != UniversalStatus.Active)
                result = result.GetNext();

            return result;
        }

        /// <summary>
        /// Returns next Active rank
        /// </summary>
        /// <returns></returns>
        public LeadershipRank GetNext()
        {
            LeadershipRank result = null;
            
            if (this.NextRankId.HasValue && this.NextRankId.Value != -1)
            {
                result = new LeadershipRank(this.NextRankId.Value);
                if (result.Status != UniversalStatus.Active)
                    result = result.GetNext();
            }

            return result;
        }

        /// <summary>
        /// Returns previously Active rank
        /// </summary>
        /// <returns></returns>
        public LeadershipRank GetPrev()
        {
            LeadershipRank result = null;

            if (this.PrevRankId.HasValue && this.PrevRankId.Value != -1)
            {
                result = new LeadershipRank(this.PrevRankId.Value);
                if (result.Status != UniversalStatus.Active)
                    result = result.GetPrev();
            }

            return result;
        }

        /// <summary>
        /// Returns last rank
        /// </summary>
        /// <returns></returns>
        public static LeadershipRank GetLast()
        {
            LeadershipRank result = LeadershipRank.RankList.FirstOrDefault(x => !x.NextRankId.HasValue || x.NextRankId.Value == -1);

            if (result != null && result.Status != UniversalStatus.Active)
                result = result.GetPrev();

            return result;
        }

        /// <summary>
        /// Get active rank by rank place
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public static LeadershipRank GetRank(int rank)
        {
            return LeadershipRank.RankList.FirstOrDefault(x => x.Rank == rank);
        }

        public static List<LeadershipRank> GetUserRanks(int userId)
        {
            string query = String.Format(
                @"SELECT
	                r.*
                FROM
	                RanksUsers u
	                INNER JOIN LeadershipRanks r ON u.RankId = r.Id
                WHERE
	                r.RankStatus = {0}
	                AND u.UserId = {1}",
                (int)UniversalStatus.Active, userId);


            List<LeadershipRank> result = TableHelper.GetListFromRawQuery<LeadershipRank>(query);

            return result;
        }

        public static LeadershipRank GetMaxUserRank(int userId)
        {
            LeadershipRank result = null;

            List<LeadershipRank> list = GetUserRanks(userId);

            if(list.Count > 0)
                result = list.FirstOrDefault(x => x.Rank == list.Max(y => y.Rank));

            return result;
        }

        public static bool HasRows()
        {
            string query = string.Format(@"IF EXISTS(SELECT * FROM {0}) SELECT CAST(1 AS bit) ELSE SELECT CAST(0 AS bit)", TableName);
            return (bool)TableHelper.SelectScalar(query);
        }

        public void UpdateRankStatus(UniversalStatus status)
        {
            LeadershipRankRequirements.UpdateRankStatus(this.Id, status);

            Dictionary<string, object> updateSet = new Dictionary<string, object>();
            updateSet.Add(Columns.RankStatus, (int)status);

            if(status == UniversalStatus.Deleted)
            {
                updateSet.Add(Columns.PrevRankId, -2);
                updateSet.Add(Columns.NextRankId, -2);

                this.ReorderIds();
            }

            Dictionary<string, object> updateWhere = new Dictionary<string, object>();
            updateWhere.Add(Columns.Id, this.Id);

            TableHelper.UpdateRows(TableName, updateSet, updateWhere);
        }

        public override void Delete()
        {
            UniversalStatus status = this.SomeoneHasRank() ? UniversalStatus.Paused : UniversalStatus.Deleted;
            this.UpdateRankStatus(status);
            UpdateRanksOrder();
        }

        protected void ReorderIds()
        {
            LeadershipRank prev = null;
            LeadershipRank next = null;

            if (this.PrevRankId.HasValue && this.PrevRankId.Value != -1)
                prev = new LeadershipRank(this.PrevRankId.Value);
            if (this.NextRankId.HasValue && this.NextRankId.Value != -1)
                next = new LeadershipRank(this.NextRankId.Value);

            if (prev != null && next != null)
            {
                prev.NextRankId = next.Id;
                next.PrevRankId = prev.Id;

                prev.Save();
                next.Save();
            }
            else if (prev != null && next == null) //Delete Last
            {
                prev.NextRankId = -1;
                prev.Save();
            }
            else if (prev == null && next != null) //Delete First
            {
                next.PrevRankId = -1;
                next.Save();
            }
        }

        /// <summary>
        /// Only used for auto-delete when rank do not have requirement
        /// </summary>
        /// <param name="list"></param>
        public static void DeleteRange(List<LeadershipRank> list)
        {
            LeadershipRank prev = null;
            LeadershipRank next = null;

            bool prevSaveDB = false;
            bool nextSaveDB = false;

            foreach (LeadershipRank rank in list)
            {
                if (rank.PrevRankId.HasValue && rank.PrevRankId.Value != -1)
                {
                    prev =
                        list.FirstOrDefault(x => x.Id == rank.PrevRankId.Value) ??
                        new LeadershipRank(rank.PrevRankId.Value);

                    prevSaveDB = !list.Any(x => x.Id == prev.Id);
                }
                if (rank.NextRankId.HasValue && rank.NextRankId.Value != -1)
                {
                    next =
                        list.FirstOrDefault(x => x.Id == rank.NextRankId.Value) ??
                        new LeadershipRank(rank.NextRankId.Value);

                    nextSaveDB = !list.Any(x => x.Id == next.Id);
                }
                
                if (prev != null && next != null)
                {
                    prev.NextRankId = rank.NextRankId;
                    next.PrevRankId = rank.PrevRankId;

                    if (prevSaveDB) prev.Save();
                    if (nextSaveDB) next.Save();
                }
                else if (prev != null && next == null) //Delete Last
                {
                    prev.NextRankId = -1;
                    if (prevSaveDB) prev.Save();
                }
                else if (prev == null && next != null) //Delete First
                {
                    next.PrevRankId = -1;
                    if (nextSaveDB) next.Save();
                }
                
                TableHelper.DeleteRows(TableName, Columns.Id, rank.Id);

                prevSaveDB = false;
                nextSaveDB = false;
            }

            UpdateRanksOrder();
        }

        public static LeadershipRank GetByRank(int rank)
        {
            var query = string.Format(@"SELECT * FROM {0} WHERE {1} = {2}", TableName, Columns.Rank, rank);
            return TableHelper.GetListFromRawQuery<LeadershipRank>(query).FirstOrDefault();
        }

        public static LeadershipRank GetByName(string rankName)
        {
            var query = string.Format(@"SELECT * FROM {0} WHERE {1} = '{3}' AND {2} <> {4}",
                TableName,
                Columns.RankName, Columns.RankStatus,
                rankName, (int)UniversalStatus.Deleted);
            return TableHelper.GetListFromRawQuery<LeadershipRank>(query).FirstOrDefault();
        }

        public static string GetRankName(int rankId)
        {
            try
            {
                var item = RankList.Find(e => e.Id == rankId);
                return item.RankName;
            }
            catch (Exception e)
            {
                return "-";
            }
        }

        public static void GivePrize(int rankId, Member user)
        {
            var rank = RankList.Find(e => e.Id == rankId);             

            switch (rank.PrizeKind)
            {
                case PrizeKind.MainBalance:
                    user.AddToMainBalance(new Money(int.Parse(rank.PrizeValue)), "LeadershipSystem Rank " + rank.RankName);
                    user.SaveBalances();
                    break;
                case PrizeKind.Points:
                    user.AddToPointsBalance(int.Parse(rank.PrizeValue), "LeadershipSystem Rank " + rank.RankName, BalanceLogType.Other, true, false);
                    user.SaveBalances();
                    break;
                case PrizeKind.CustomPrize:
                    //NOTHING
                    break;
            }
        }

        public static bool IsRankAccured(int rankId, Member user)
        {
            var rankRequirementsList = LeadershipRankRequirements.RequirementsList.
                                       Where<LeadershipRankRequirements>(item => item.Rank == rankId).ToList(); 
                                    
            for (int i = 0; i < rankRequirementsList.Count; i++)
            {
                if (!rankRequirementsList[i].CheckRequirement(user.Id))
                    return false;
            }
            
            return true;
        }

        public static List<int> GetRanksIdList()
        {
            List<LeadershipRank> list = RankList;
            var intList = new List<int>();

            for (int i = 0; i < list.Count; i++)
            {
                intList.Add(list[i].Id);
            }

            return intList;
        }

        public void UpdateRank(string name, PrizeKind prize, string prizeValue, int matrixCyclesPerDay, string note = null)
        {
            RankName = name;
            PrizeKind = prize;
            PrizeValue = prizeValue;
            if (prize != PrizeKind.CustomPrize)
                Note = "";
            else
                Note = note;
            MatrixCyclesPerDay = matrixCyclesPerDay;
            this.Save();
        }

        public void MoveRankUp()
        {
            if (!this.PrevRankId.HasValue || this.PrevRankId.Value == -1)
                return;

            LeadershipRank rankSwap = new LeadershipRank(this.PrevRankId.Value);

            this.Rank ^= rankSwap.Rank;
            rankSwap.Rank ^= this.Rank;
            this.Rank ^= rankSwap.Rank;

            this.PrevRankId = rankSwap.PrevRankId;
            rankSwap.PrevRankId = this.Id;

            rankSwap.NextRankId = this.NextRankId;
            this.NextRankId = rankSwap.Id;

            if(this.PrevRankId.HasValue && this.PrevRankId.Value != -1)
            {
                LeadershipRank rankLower = new LeadershipRank(this.PrevRankId.Value);
                rankLower.NextRankId = this.Id;
                rankLower.Save();
            }

            if (rankSwap.NextRankId.HasValue && rankSwap.NextRankId.Value != -1)
            {
                LeadershipRank rankHigher = new LeadershipRank(rankSwap.NextRankId.Value);
                rankHigher.PrevRankId = rankSwap.Id;
                rankHigher.Save();
            }

            this.Save();
            rankSwap.Save();
        }

        private static void UpdateRanksOrder()
        {
            var query = string.Format(@"WITH r AS
                                        (
	                                        SELECT
		                                        *
		                                        , 1 as rn
	                                        FROM
		                                        {0}
	                                        WHERE
		                                        PrevRankId = -1
	                                        UNION ALL
	                                        SELECT
		                                        l.*
		                                        , rn+1 AS rn
	                                        FROM
		                                        r
		                                        INNER JOIN {0} l ON r.NextRankId = l.Id
                                        )
                                        SELECT * FROM r ORDER BY rn", TableName);
            var tab = TableHelper.GetListFromRawQuery<LeadershipRank>(query);

            for(int i = 0; i < tab.Count; i++)
            {
                tab[i].Rank = i + 1;
                tab[i].Save();
            }
        }

        private void SetRankIds()
        {
            LeadershipRank row = LeadershipRank.GetLast();

            if (row != null)
            {
                Rank = row.Rank + 1;
                PrevRankId = row.Id;
                row.NextRankId = TableHelper.GetNextId(TableName);
                row.Save();
            }
            else
            {
                Rank = 1;
                PrevRankId = -1;
                NextRankId = -1;
            }
        }

        public bool SomeoneHasRank()
        {
            var query = string.Format(@"SELECT COUNT(Id) FROM RanksUsers WHERE RankId = {0} AND IsCurrent = 1", this.Id);

            return (int)TableHelper.SelectScalar(query) > 0;
        }

        public static void DeletePausedIfPossible()
        {
            string query = String.Format(
                @"SELECT
	                l.*
                FROM
	                LeadershipRanks l
                WHERE
	                l.RankStatus = {0}
	                AND NOT EXISTS(SELECT Id FROM RanksUsers WHERE l.Id = RankId AND IsCurrent = 1)",
                (int)UniversalStatus.Paused);

            List<LeadershipRank> paused = TableHelper.GetListFromRawQuery<LeadershipRank>(query);

            if (paused.Count > 0)
            {
                foreach (LeadershipRank rank in paused)
                {
                    rank.UpdateRankStatus(UniversalStatus.Deleted);
                }

                UpdateRanksOrder();
            }
        }
    }
}