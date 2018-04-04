using Prem.PTC.Memberships;
using Prem.PTC.Utils;
using System;
using System.Data;
using System.Reflection;
using System.Web;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Prem.PTC.Utils.NVP;
using MarchewkaOne.Titan.Balances;
using System.Web.Security;
using Resources;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        [Column(Columns.AccountStatus)]
        protected string AccountStatus { get { return _accountStatus; } set { _accountStatus = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsLocked)]
        public bool IsLocked { get { return _isLocked; } set { _isLocked = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DetailedBanReason)]
        public string DetailedBanReason { get { return _detailedBanReason; } set { _detailedBanReason = value; SetUpToDateAsFalse(); } }

        [Column("AccountStatusInt")]
        protected int AccountStatusInt { get { return _AccountStatusInt; } set { _AccountStatusInt = value; SetUpToDateAsFalse(); } }

        [Column("VerificationStatusInt")]
        protected int VerificationStatusInt { get { return _VerificationStatusInt; } set { _VerificationStatusInt = value; SetUpToDateAsFalse(); } }

        [Column("VerificationDocumentUrl")]
        public string VerificationDocumentUrl { get { return _VerificationDocumentUrl; } set { _VerificationDocumentUrl = value; SetUpToDateAsFalse(); } }

        [Column("AccountActivated")]
        public bool IsAccountActivationFeePaid { get { return _IsAccountActivationFeePaid; } set { _IsAccountActivationFeePaid = value; SetUpToDateAsFalse(); } }

        [Column("QuickGuideViewed")]
        public bool IsQuickGuideViewed { get { return _IsQuickGuideViewed; } set { _IsQuickGuideViewed = value; SetUpToDateAsFalse(); } }

        public VerificationStatus VerificationStatus
        {
            get
            {
                return (VerificationStatus)VerificationStatusInt;
            }
            set
            {
                VerificationStatusInt = (int)value;
            }
        }

        private int _AccountStatusInt, _VerificationStatusInt;
        private string _VerificationDocumentUrl;
        private bool _IsAccountActivationFeePaid, _IsQuickGuideViewed;

        #region Status

        public MemberStatus Status
        {
            get
            {
                return (MemberStatus)AccountStatusInt;
            }
            set
            {
                AccountStatusInt = (int)value;

                if (value != MemberStatus.Locked)
                    AccountStatus = Enum.GetName(typeof(MemberStatus), value);

                IsLocked = (value == MemberStatus.Locked);
            }
        }

        /// <summary>
        /// True if member account is banned for some reason (TOS, ManualRules, Cheating, etc.) or locked
        /// </summary>
        public bool IsBanned
        {
            get
            {
                if (Status == MemberStatus.BannedBlacklisted ||
                    Status == MemberStatus.BannedOfCheating ||
                    Status == MemberStatus.BannedOfTos ||
                    IsLocked)
                    return true;

                return false;
            }
        }

        #endregion

        #region Save
        /// <summary>
        /// Saves Member.Status, Member.IsLocked, Member.DetailedBanReason property
        /// </summary>
        /// <exception cref="DbException" />
        public void SaveStatus()
        {
            SaveStatus(this.IsUpToDate);
        }

        /// <summary>
        /// Saves Member.Status, Member.IsLocked, Member.DetailedBanReason property
        /// </summary>
        /// <exception cref="DbException" />
        private void SaveStatus(bool isUpToDate)
        {
            PropertyBuilder<Member> builder = new PropertyBuilder<Member>();
            builder
                .Append(x => x.AccountStatus)
                .Append(x => x.DetailedBanReason)
                .Append(x => x.AccountStatusInt)
                .Append(x => x.FailedPINAttemptCount)
                .Append(x => x.IsLocked)
                .Append(x => x.IsAccountActivationFeePaid)
                .Append(x => x.IsQuickGuideViewed);

            SavePartially(isUpToDate, builder.Build());
        }
        #endregion

        #region Lock
        /// <summary>
        /// Locks member. Member is locked until Member.Unlock() is called.
        /// When member is already locked, old lock is overriden.
        /// </summary>
        /// <exception cref="InvalidOperationException">When member is not registered</exception>
        /// <exception cref="DbException" />
        public void Lock()
        {
            bool isUpToDate = IsUpToDate;
            IsLocked = true;
            SaveStatus(isUpToDate);
        }

        /// <summary>
        /// Locks member for given reason. Member is locked until Member.Unlock() is called.
        /// When member is already locked, old lock is overriden.
        /// </summary>
        /// <exception cref="InvalidOperationException">When member is not registered</exception>
        /// <exception cref="DbException" />
        public void Lock(string reason)
        {
            bool isUpToDate = IsUpToDate;
            IsLocked = true;
            DetailedBanReason = reason;
            SaveStatus(isUpToDate);
        }

        /// <summary>
        /// Locks this member for given reason and for specified time.
        /// Member is locked for given time span or until Member.Unlock() is called.
        /// When member is already locked, old lock is overriden.
        /// </summary>
        /// <exception cref="InvalidOperationException">When member is not registered</exception>
        /// <exception cref="DbException" />
        public void Lock(string reason, TimeSpan time)
        {
            bool isUpToDate = IsUpToDate;
            IsLocked = true;
            DetailedBanReason = reason;
            SaveStatus(isUpToDate);
        }

        /// <summary>
        /// Unlocks this member. If this member is already unlocked, does nothing.
        /// </summary>
        /// <exception cref="InvalidOperationException">When member is not registered</exception>
        /// <exception cref="DbException" />
        public void Unlock()
        {
            if (!IsRegistered)
                throw new InvalidOperationException("Only registered members can be unlocked");

            if (IsLocked)
            {
                bool isUpToDate = IsUpToDate;
                IsLocked = false;
                FailedPINAttemptCount = 0;
                DetailedBanReason = String.Empty; //?
                SaveStatus(isUpToDate);
            }
        }
        #endregion

        #region Activate

        /// <summary>
        /// Activates member and lift all bans
        /// </summary>
        /// <exception cref="DbException" />
        public void Activate()
        {
            bool isUpToDate = IsUpToDate;

            Status = MemberStatus.Active;

            //Return to vacation mode after unban?
            if (VacationModeEnds.HasValue && VacationModeEnds > AppSettings.ServerTime)
                Status = MemberStatus.VacationMode;

            DetailedBanReason = string.Empty;
            FailedPINAttemptCount = 0;

            SaveStatus(isUpToDate);
        }


        public void Reactivate()
        {
            _mainBalance = new Money(0);
            _trafficBalance = new Money(0);
            _advertisingBalance = new Money(0);
            _pointsBalance = 0;

            //DR
            foreach (var elem in GetDirectReferralsList())
            {
                elem.RemoveReferer();
                elem.Save();
            }

            //RR
            try
            {
                var rrm = new Prem.PTC.Referrals.RentReferralsSystem(this.Name, this.Membership);
                rrm.DeleteRentedReferrals(rrm.GetUserRentedReferralsCount());
            }
            catch (Exception ex) { }

            //Status
            MakeActive();

            SetUpToDateAsFalse();
        }

        #endregion

        #region Ban&Others

        public void BanCheater(string detailedReason)
        {
            Status = MemberStatus.BannedOfCheating;
            DetailedBanReason = detailedReason;
            SaveStatus();
        }

        [Obsolete]
        public void MakeActive()
        {
            Activate();
        }

        public void BanBlacklist(string what)
        {
            Status = MemberStatus.BannedBlacklisted;
            DetailedBanReason = what;
            SaveStatus();
        }

        public void BanToS(string what)
        {
            Status = MemberStatus.BannedOfTos;
            DetailedBanReason = what;
            SaveStatus();
        }

        public void CancelAccount()
        {
            Status = MemberStatus.Cancelled;
            SaveStatus();
        }

        public void RequireSMSPin()
        {
            Status = MemberStatus.AwaitingSMSPIN;
            SaveStatus();
        }

        public void ValidatePIN(string pin)
        {
            if (AppSettings.Registration.IsPINEnabled && this.PIN != Convert.ToInt32(pin))
            {
                this.IncreaseFailedPINAttemptCount();
                this.Save();
                throw new MsgException(L1.ER_BADPIN2);
            }
            this.FailedPINAttemptCount = 0;
        }

        #endregion
    }
}