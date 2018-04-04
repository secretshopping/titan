using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Leadership;

namespace Prem.PTC.Members
{
    public partial class Member
    {
        public LeadershipRank GetRank()
        {
            return Member.GetRank(this.Id);
        }

        public int GetRankId()
        {
            return Member.GetRankId(this.Id);
        }

        public static LeadershipRank GetRank(int userId)
        {
            LeadershipRank result = null;
            RanksUsers ranksUsers = RanksUsers.GetCurrentUserRank(userId);

            if (ranksUsers != null)
                result = new LeadershipRank(ranksUsers.RankId);

            return result;
        }

        public static int GetRankId(int userId)
        {
            int result = -1;
            RanksUsers ranksUsers = RanksUsers.GetCurrentUserRank(userId);

            if (ranksUsers != null)
                result = ranksUsers.RankId;

            return result;
        }

        public static LeadershipRank GetMaxRank(int userId)
        {
            return LeadershipRank.GetMaxUserRank(userId);
        }

        public LeadershipRank GetMaxRank()
        {
            return GetMaxRank(this.Id);
        }
    }
}