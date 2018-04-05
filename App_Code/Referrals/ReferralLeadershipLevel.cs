using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

namespace Prem.PTC.Referrals
{
    [Serializable]
    public class ReferralLeadershipLevel : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "ReferralLeadershipLevels"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Name")]
        public string Name { get { return _Name; } set { _Name = value; SetUpToDateAsFalse(); } }

        [Column("IndirectReferrals")]
        public int IndirectReferrals { get { return _IndirectReferrals; } set { _IndirectReferrals = value; SetUpToDateAsFalse(); } }

        [Column("DirectReferrals")]
        public int DirectReferrals { get { return _DirectReferrals; } set { _DirectReferrals = value; SetUpToDateAsFalse(); } }

        [Column("TotalTeamDeposits")]
        public Money TotalTeamDeposits { get { return _TotalTeamDeposits; } set { _TotalTeamDeposits = value; SetUpToDateAsFalse(); } }

        [Column("TeamDepositsPerSubTime")]
        public Money TeamDepositsPerSubTime { get { return _TeamDepositsPerSubTime; } set { _TeamDepositsPerSubTime = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// Team Partners are direct referrals who have reached the level
        /// </summary>
        [Column("TeamPartners")]
        public int TeamPartners { get { return _TeamPartners; } set { _TeamPartners = value; SetUpToDateAsFalse(); } }

        [Column("TotalTimeConstraintDays")]
        public int TotalTimeConstraintDays { get { return _TotalTimeConstraintDays; } set { _TotalTimeConstraintDays = value; SetUpToDateAsFalse(); } }

        [Column("NumberOfSubTimeConstraints")]
        public int NumberOfSubTimeConstraints { get { return _NumberOfSubTimeConstraints; } set { _NumberOfSubTimeConstraints = value; SetUpToDateAsFalse(); } }

        [Column("Number")]
        public int Number { get { return _Number; } set { _Number = value; SetUpToDateAsFalse(); } }

        [Column("Reward")]
        public Money Reward { get { return _Reward; } set { _Reward = value; SetUpToDateAsFalse(); } }

        [Column("MatrixCyclesPerDay")]
        public int MatrixCyclesPerDay { get { return _MatrixCyclesPerDay; } set { _MatrixCyclesPerDay = value; SetUpToDateAsFalse(); } }


        public int SubTimeConstraint { get { return TotalTimeConstraintDays / NumberOfSubTimeConstraints; } }

        private int _id, _IndirectReferrals, _DirectReferrals, _TeamPartners, _NumberOfSubTimeConstraints, _TotalTimeConstraintDays, _Number,
            _MatrixCyclesPerDay;
        private Money _TotalTeamDeposits, _TeamDepositsPerSubTime, _Reward;
        private string _Name;
        #endregion Columns

        public ReferralLeadershipLevel()
            : base() { }

        public ReferralLeadershipLevel(int id) : base(id) { }

        public ReferralLeadershipLevel(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    }
}