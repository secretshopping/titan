using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Prem.PTC.Memberships;
using MarchewkaOne.Titan.Balances;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {

        [Column(Columns.MembershipId)]
        public int MembershipId { get { return _membershipId; } set { _membershipId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MembershipName)]
        public string MembershipName { get { return _membershipName; } set { _membershipName = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MembershipExpires)]
        public DateTime? MembershipExpires { get { return _membershipExpires; } protected set { _membershipExpires = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MembershipWhen)]
        public DateTime? MembershipWhen { get { return _membershipWhen; } protected set { _membershipWhen = value; SetUpToDateAsFalse(); } }

        [Column("IsNewLevelSpotted")]
        public bool IsNewLevelSpotted { get { return _IsNewLevelSpotted; } set { _IsNewLevelSpotted = value; SetUpToDateAsFalse(); } }

        [Column("IsDowngradeSpotted")]
        public bool IsDowngradeSpotted { get { return _IsDowngradeSpotted; } set { _IsDowngradeSpotted = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ResolveReferralsLimitDate)]
        public DateTime? ResolveReferralsLimitDate { get { return _ResolveReferralsLimitDate; } set { _ResolveReferralsLimitDate = value; SetUpToDateAsFalse(); } }

        private int _membershipId;
        private string _membershipName;
        private DateTime? _membershipExpires, _membershipWhen, _ResolveReferralsLimitDate;
        private bool _IsNewLevelSpotted, _IsDowngradeSpotted;

        /// <exception cref="DbException" />
        public void Upgrade(MembershipPack pack)
        {
            if (this.Membership.Name == pack.Membership.Name)
            {
                //The same bought, lets sum up
                DateTime Expires = (DateTime)this.MembershipExpires;
                setMembership(pack.Membership, AppSettings.ServerTime, Expires.AddDays(pack.TimeDays));
            }
            else
            {
                setMembership(pack.Membership, AppSettings.ServerTime, AppSettings.ServerTime.AddDays(pack.TimeDays));
            }
            ResolveReferralLimits(pack.Membership);

            if (AppSettings.Misc.SpilloverEnabled)
            {
                SpilloverManager spillover = new SpilloverManager(this);
                spillover.ResolveReferrals();
            }

        }

        public void Upgrade(IMembership membership)
        {
            setMembership(membership, AppSettings.ServerTime, null);
            ResolveReferralLimits(membership);
        }

        public void Upgrade(IMembership membership, DateTime expirationDate)
        {
            setMembership(membership, AppSettings.ServerTime, expirationDate);
            ResolveReferralLimits(membership);
        }
        public void Upgrade(IMembership membership, TimeSpan duration)
        {
            setMembership(membership, AppSettings.ServerTime, AppSettings.ServerTime + duration);
            ResolveReferralLimits(membership);
        }

        /// <summary>
        /// Downgrades membership to Standard
        /// </summary>
        /// <exception cref="DbException" />
        public void Downgrade(bool resolveReferrals = true)
        {
            Downgrade(Memberships.Membership.Standard, true, resolveReferrals);
        }        

        public void Downgrade(IMembership membership, bool setMinPoints = true, bool resolveReferrals = true)
        {
            setMembership(membership, AppSettings.ServerTime, null);

            if(resolveReferrals)
                ResolveReferralLimits(membership);

            if (AppSettings.Points.LevelMembershipPolicyEnabled && setMinPoints)
            {
                if (PointsBalance > membership.MinPointsToHaveThisLevel)
                {
                    int substracted = PointsBalance - membership.MinPointsToHaveThisLevel;
                    PointsBalance = membership.MinPointsToHaveThisLevel;
                    SaveBalances();

                    BalanceLog.Add(this, BalanceType.PointsBalance, -substracted, "Level downgrade", BalanceLogType.Other);
                }
            }
        }

        public string FormattedMembershipName
        {
            get
            {
                if (Membership.Name != Prem.PTC.Memberships.Membership.Standard.Name)
                    return "<span style='color:" + Membership.Color + "'>" + MembershipName + "</span>";
                else
                    return MembershipName;
            }
        }

        public void ResolveReferralLimits(IMembership membership)
        {
            //Direct
            if (GetDirectReferralsCount() > membership.DirectReferralsLimit + (long)DirectReferralLimitEnlargedBy)
            {
                var list = GetDirectReferralsList();
                long howmany = list.Count - (long)membership.DirectReferralsLimit - DirectReferralLimitEnlargedBy;
                     
                for (int i = 0; i < howmany; ++i)
                {
                    list[i].RemoveReferer();
                    list[i].Save();
                }
            }

            //Rented
            if (AppSettings.TitanFeatures.ReferralsRentedEnabled)
            {
                var rrm = new Prem.PTC.Referrals.RentReferralsSystem(Name, membership);
                if (AppSettings.TitanFeatures.ReferralsRentedEnabled && (rrm.GetUserRentedReferralsCount() > membership.RentedReferralsLimit))
                {
                    int howmany = rrm.GetUserRentedReferralsCount() - membership.RentedReferralsLimit;
                    rrm.DeleteRentedReferrals(howmany);
                }
            }
        }

        /// <exception cref="DbException" />
        private void setMembership(IMembership membership, DateTime? when, DateTime? expires)
        {
            bool isUpToDate = IsUpToDate;

            Membership = membership;
            MembershipWhen = when;
            MembershipExpires = expires;
            HasEverUpgraded = true;

            SaveMembership();
        }

        /// <summary>
        /// Checks if User has at least the specified Membership
        /// Returns true if Memberships are disabled
        /// </summary>
        /// <param name="requiredMembershipId"></param>
        /// <returns></returns>
        public bool HasThisMembershipOrHigher(int requiredMembershipId)
        {
            if (AppSettings.TitanFeatures.UpgradeEnabled)
                return this.Membership.DisplayOrder >= new Membership(requiredMembershipId).DisplayOrder;
            return true;
        }

        private PropertyInfo[] buildMembership()
        {
            return new PropertyBuilder<Member>()
                .Append(x => x.MembershipId)
                .Append(x => x.MembershipName)
                .Append(x => x.MembershipWhen)
                .Append(x => x.MembershipExpires)
                .Append(x => x.HasEverUpgraded)
                .Append(x => x.ResolveReferralsLimitDate)
                .Build();
        }

        public IMembership GetPreviousMembership()
        {
            int currentMembershipDisplayOrder = this.Membership.DisplayOrder;
            if (currentMembershipDisplayOrder == Memberships.Membership.Standard.DisplayOrder)
                return Membership;

            string query = string.Format(@"SELECT TOP 1 * FROM Memberships WHERE DisplayOrder < {0} AND Status = {1} ORDER BY DisplayOrder DESC;", currentMembershipDisplayOrder, (int)MembershipStatus.Active);

            return TableHelper.GetListFromRawQuery<Membership>(query).FirstOrDefault();
        }

        public IMembership GetNextMembership()
        {
            if (this.Membership.Id == Member.GetHighestMembershipId())
                return this.Membership;

            int currentMembershipDisplayOrder = this.Membership.DisplayOrder;
            string query = string.Format(@"SELECT TOP 1 * FROM Memberships WHERE DisplayOrder > {0} AND Status = {1} ORDER BY DisplayOrder DESC;", currentMembershipDisplayOrder, (int)MembershipStatus.Active);

            return TableHelper.GetListFromRawQuery<Membership>(query).FirstOrDefault();
        }

        public static int GetHighestMembershipId()
        {
            return (int)TableHelper.SelectScalar(string.Format("SELECT TOP 1 MembershipId FROM MEMBERSHIPS WHERE Status = {0} ORDER BY DisplayOrder DESC", (int)MembershipStatus.Active));
        }

        public void SaveMembership()
        {
            SavePartially(IsUpToDate, buildMembership());
        }
    }
}