using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Prem.PTC.Members;

namespace Titan.Leadership
{
    public partial class RanksUsers
    {
        public RanksUsers() : base()
        {
            LeadershipRank first = LeadershipRank.GetFirst();

            if (first != null)
            {
                UserId = Member.CurrentId;
                RankId = first.Id;
                AquairedDate = DateTime.Now;
                IsCurrent = true;
            }
        }

        public RanksUsers(bool isCurrent) : this()
        {
            IsCurrent = isCurrent;
            AquairedDate = DateTime.Now;
        }

        public RanksUsers(int rankId, bool isCurrent) : this(isCurrent)
        {
            RankId = rankId;
        }

        public RanksUsers(int userId, int rankId, bool isCurrent) : this(rankId, isCurrent)
        {
            UserId = userId;
        }

        public RanksUsers(int id) : base(id) { }

        public RanksUsers(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }
        
        public static RanksUsers GetCurrentUserRank(int userId)
        {
            string query = String.Format("WHERE {0} = {2} AND {1} = 1",
                    RanksUsers.Columns.UserId, RanksUsers.Columns.IsCurrent,
                    userId);

            RanksUsers result = TableHelper.GetListFromQuery<RanksUsers>(query).FirstOrDefault();
            
            return result;
        }

        public static List<RanksUsers> GetRanksUsers(int userId)
        {
            var list = TableHelper.SelectAllRows<RanksUsers>();
            return list.Where(x => x.UserId == userId).ToList();
        }

        public static RanksUsers GetByRankId(int userId, int rankId)
        {
            string query = String.Format("WHERE {0} = {2} AND {1} = {3}",
                    RanksUsers.Columns.UserId, RanksUsers.Columns.RankId,
                    userId, rankId);

            RanksUsers result = TableHelper.GetListFromQuery<RanksUsers>(query).FirstOrDefault();

            return result;
        }

        public static RanksUsers GetOrCreateRanksUsers(int userId, int rankId)
        {
            RanksUsers result = GetRanksUsers(userId).FirstOrDefault(x => x.RankId == rankId);

            if (result == null)
            {
                result = new RanksUsers(rankId, true);
            }

            return result;
        }

        public static void UpdateRank(int newRankId, int userId)
        {
            RanksUsers currentRank = RanksUsers.GetCurrentUserRank(userId);

            if (currentRank == null || currentRank.RankId != newRankId)
            {
                currentRank = GetOrCreateRanksUsers(userId, newRankId);
                List<RanksUsers> userRanks = GetRanksUsers(userId);
                if (!userRanks.Any(x => x.RankId == currentRank.RankId))
                {
                    LeadershipRank.GivePrize(currentRank.RankId, new Member(userId));
                }

                currentRank.Save();
            }
        }

        public void SetCurrent()
        {
            List<RanksUsers> userRanks = GetRanksUsers(this.UserId);

            foreach(RanksUsers rank in userRanks)
            {
                rank.IsCurrent = this == rank;
                rank.Save();
            }
        }

        public static void SetRankZero(int userId)
        {
            List<RanksUsers> userRanks = GetRanksUsers(userId);

            foreach (RanksUsers rank in userRanks)
            {
                rank.IsCurrent = false;
                rank.Save();
            }
        }
    }
}