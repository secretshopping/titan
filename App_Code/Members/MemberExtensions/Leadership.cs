using ExtensionMethods;
using Prem.PTC.Memberships;
using Prem.PTC.Referrals;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        #region Columns
        [Column("LeadershipLevelId")]
        public int LeadershipLevelId { get { return _LeadershipLevelId; } set { _LeadershipLevelId = value; SetUpToDateAsFalse(); } }

        [Column("LeadershipResetTime")]
        public DateTime LeadershipResetTime { get { return _LeadershipResetTime; } set { _LeadershipResetTime = value; SetUpToDateAsFalse(); } }

        int _LeadershipLevelId;
        DateTime _LeadershipResetTime;

        #endregion

        private ReferralLeadershipLevel _CurrentLeadership;
        public ReferralLeadershipLevel CurrentLeadership
        {
            get
            {
                if (_CurrentLeadership == null)
                {
                    if (LeadershipLevelId == -1)
                        _CurrentLeadership = null;
                    else
                        _CurrentLeadership = new ReferralLeadershipLevel(LeadershipLevelId);
                }
                return _CurrentLeadership;
            }
        }

        private ReferralLeadershipLevel _NextLeadership;
        public ReferralLeadershipLevel NextLeadership
        {
            get
            {
                if (_NextLeadership == null)
                {
                    if (CurrentLeadership == null)
                        _NextLeadership = TableHelper.SelectRows<ReferralLeadershipLevel>(TableHelper.MakeDictionary("Number", 1))[0];
                    else
                        try
                        {
                            _NextLeadership = TableHelper.SelectRows<ReferralLeadershipLevel>(TableHelper.MakeDictionary("Number", CurrentLeadership.Number + 1))[0];
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            //if user reached the highest level
                            _NextLeadership = null;
                        }
                }
                return _NextLeadership;
            }
        }

        public Money TotalTeamDeposits
        {
            get
            {
                if (NextLeadership == null)
                    return Money.Zero;
                IndirectReferralsHelper irh = new IndirectReferralsHelper(this);
                Money totalTeamDeposits = irh.GetTotalIndirectReferralsDeposit(this, 0, AppSettings.Referrals.ReferralEarningsUpToTier, LeadershipResetTime, LeadershipDeadline);
                Money usersDeposits = Money.Zero;

                string query = string.Format(@"SELECT SUM(Amount) FROM CompletedTransactions WHERE WhenMade >= '{0}' AND WhenMade < '{1}' AND UserId = {2}",
                    LeadershipResetTime.ToShortDateDBString(), LeadershipDeadline.ToShortDateDBString(), Id);
                object usersDepositHelper = TableHelper.SelectScalar(query);

                if (!(usersDepositHelper is DBNull))
                    usersDeposits = new Money((decimal)usersDepositHelper);
                return totalTeamDeposits + usersDeposits;
            }
        }

        public DateTime LeadershipDeadline
        {
            get
            {
                return LeadershipResetTime.AddDays(NextLeadership.TotalTimeConstraintDays);
            }
        }

        private List<LeadershipSubDeposit> _TeamSubDeposits;
        public List<LeadershipSubDeposit> TeamSubDeposits
        {
            get
            {
                if (_TeamSubDeposits == null)
                {
                    List<LeadershipSubDeposit> teamSubDeposits = new List<LeadershipSubDeposit>();
                    IndirectReferralsHelper irh = new IndirectReferralsHelper(this);
                    DateTime startDate = LeadershipResetTime;

                    for (int i = 0; i < NextLeadership.NumberOfSubTimeConstraints; i++)
                    {
                        DateTime endDate = startDate.AddDays(NextLeadership.SubTimeConstraint);
                        Money subTeamDeposits = irh.GetTotalIndirectReferralsDeposit(this, 0, AppSettings.Referrals.ReferralEarningsUpToTier, startDate, endDate);
                        Money usersDeposits = Money.Zero;
                        object usersDepositHelper = TableHelper.SelectScalar(string.Format(@"SELECT SUM(Amount) FROM CompletedTransactions WHERE WhenMade >= '{0}' AND WhenMade < '{1}' AND UserId = {2}", startDate.ToShortDateDBString(), endDate.ToShortDateDBString(), Id));

                        if (!(usersDepositHelper is DBNull))
                            usersDeposits = new Money((decimal)usersDepositHelper);

                        subTeamDeposits += usersDeposits;
                        teamSubDeposits.Add(new LeadershipSubDeposit(startDate, endDate, subTeamDeposits));
                        startDate = startDate.AddDays(NextLeadership.SubTimeConstraint);
                    }
                    _TeamSubDeposits = teamSubDeposits;
                }
                return _TeamSubDeposits;
            }
        }

        public int NumberOfTeamPartners
        {
            get
            {
                return (int)TableHelper.SelectScalar(string.Format(@"SELECT COUNT(*) FROM Users WHERE LeadershipLevelId = {0} AND ReferrerId = {1};", LeadershipLevelId, Id));
            }
        }

        /// <summary>
        /// Active Direct Referrals with the highest available membership
        /// </summary>
        public List<Member> DirectReferralsWithHighestMembership
        {
            get
            {
                string query = string.Format(@"SELECT * FROM Users 
WHERE ReferrerId = {0}
AND AccountStatusInt = {1} 
AND UpgradeId = 
    (SELECT MembershipId FROM Memberships 
    WHERE DisplayOrder = (SELECT MAX(DisplayOrder) FROM Memberships) AND Status = {2})", Id, (int)MemberStatus.Active, (int)MembershipStatus.Active);

                return TableHelper.GetListFromRawQuery<Member>(query);
            }
        }

        public int InDirectReferralsWithHighestMembership
        {
            get
            {
                IndirectReferralsHelper IRH = new IndirectReferralsHelper(this, true);

                return IRH.GetIndirectReferralsCountForMember();
            }
        }
        public bool ShouldBeRewarded
        {
            get
            {
                return HasEnoughTeamPartners && HasEnoughDirectReferrals && HasEnoughIndirectReferrals && HasEnoughTotalDeposits && HasEnoughSubDeposits;
            }
        }

        public bool HasEnoughTeamPartners
        {
            get
            {
                return NumberOfTeamPartners >= NextLeadership.TeamPartners;
            }
        }

        public bool HasEnoughDirectReferrals
        {
            get
            {
                return DirectReferralsWithHighestMembership.Count >= NextLeadership.DirectReferrals;
            }
        }

        public bool HasEnoughIndirectReferrals
        {
            get
            {
                return InDirectReferralsWithHighestMembership + DirectReferralsWithHighestMembership.Count >= NextLeadership.IndirectReferrals;
            }
        }

        public bool HasEnoughTotalDeposits
        {
            get
            {
                return TotalTeamDeposits >= NextLeadership.TotalTeamDeposits;
            }
        }

        public bool HasEnoughSubDeposits
        {
            get
            {
                int count = 0;
                for (int i = 0; i < TeamSubDeposits.Count; i++)
                {
                    if (TeamSubDeposits[i].TeamDeposits >= NextLeadership.TeamDepositsPerSubTime)
                        count++;
                    if (TeamSubDeposits[i].TeamDeposits >= NextLeadership.TotalTeamDeposits)
                        return true;
                }
                return count >= NextLeadership.NumberOfSubTimeConstraints;
            }
        }

        public bool ShouldBeReset
        {
            get
            {
                bool shouldBeResetForSubDeposits = false;

                foreach (LeadershipSubDeposit subDeposit in TeamSubDeposits)
                    if (subDeposit.EndDate.Date <= AppSettings.ServerTime.Date && subDeposit.TeamDeposits < NextLeadership.TeamDepositsPerSubTime)
                        shouldBeResetForSubDeposits = true;

                return AppSettings.ServerTime.Date > LeadershipDeadline || shouldBeResetForSubDeposits;
            }
        }
    }
    public class LeadershipSubDeposit
    {
        public LeadershipSubDeposit(DateTime startDate, DateTime endDate, Money teamDeposits)
        {
            StartDate = startDate;
            EndDate = endDate;
            TeamDeposits = teamDeposits;
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Money TeamDeposits { get; set; }
    }
}