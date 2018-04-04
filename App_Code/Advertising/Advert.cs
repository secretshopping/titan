using System;
using System.Data;
using Prem.PTC.Utils;

namespace Prem.PTC.Advertising
{
    [Serializable]
    public abstract class Advert : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static class Columns
        {
            public const string AdvertPackId = "AdvertPackId";
            public const string TargetUrl = "TargetUrl";
            public const string CreatedBy = "CreatedBy";
            public const string CreatorUsername = "CreatorUsername";
            public const string CreatorEmail = "CreatorEmail";
            public const string CreationDate = "CreationDate";
            public const string StartDate = "StartDate";
            public const string EndDate = "EndDate";
            public const string TotalSecsActive = "TotalSecsActive";
            public const string Clicks = "Clicks";
            public const string EndValue = "EndValue";
            public const string EndMode = "EndMode";
            public const string Status = "Status";
            public const string StatusLastChangedDate = "StatusLastChangedDate";
            public const string Price = "Price";
            public const string TargetBalance = "TargetBalance";
        }

        [Column(Columns.AdvertPackId)]
        protected abstract int? AdvertPackId { get; set; }

        [Column(Columns.TargetUrl)]
        public virtual string TargetUrl { get { return UrlCreator.ParseUrl(_targetUrl); } set { _targetUrl = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CreatedBy)]
        protected int CreatedBy { get { return _createdBy; } set { _createdBy = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CreatorUsername)]
        public string CreatorUsername { get { return _creatorUsername; } set { _creatorUsername = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CreatorEmail)]
        protected string CreatorEmail { get { return _creatorEmail; } set { _creatorEmail = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// Date when advert was created
        /// </summary>
        [Column(Columns.CreationDate)]
        public DateTime? CreationDate { get { return _creationDate; } protected set { _creationDate = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// Date when advert was first Active or Paused
        /// </summary>
        [Column(Columns.StartDate)]
        public DateTime? StartDate { get { return _startDate; } protected set { _startDate = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// Date when this advert ended (when advert was Finished or Rejected)
        /// or
        /// Date when this advert should finish (when advert's end mode is days and advert is Active or Paused).
        /// 
        /// Null otherwise.
        /// </summary>
        [Column(Columns.EndDate)]
        public DateTime? EndDate
        {
            get
            {
                if (Status == AdvertStatus.Finished || Status == AdvertStatus.Rejected) return _endDate;
                else if (Ends.EndMode == End.Mode.Days && (Status == AdvertStatus.Active || Status == AdvertStatus.Paused))
                {
                    DateTime now = DateTime.Now;
                    DateTime endDate = now + (TimeSpan.FromDays(EndValue) - TimeSpan.FromSeconds(TotalSecsActive));

                    if (Status == AdvertStatus.Active) endDate -= now - StatusLastChangedDate.Value;

                    return endDate;
                }

                return null;
            }
            protected set { _endDate = value; SetUpToDateAsFalse(); }
        }

        /// Total of seconds when advert was active
        /// not including time since StatusLastChangedDate
        [Column(Columns.TotalSecsActive)]
        protected int TotalSecsActive { get { return _totalSecsActive; } set { _totalSecsActive = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// How many times advertisement was clicked by members and successfully viewed 
        /// (for example they were watching ptc advert for enough time)
        /// May be not quite up to date (to make it up to date call ReloadClicks())
        /// </summary>
        [Column(Columns.Clicks)]
        public int Clicks { get { return _clicks + _thisClicks; } protected set { _clicks = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// Total days or clicks.
        /// </summary>
        [Column(Columns.EndValue)]
        protected int EndValue { get { return _endValue; } set { _endValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.EndMode)]
        protected int EndMode { get { return (int)_endMode; } set { _endMode = (End.Mode)value; SetUpToDateAsFalse(); } }

        [Column(Columns.Status)]
        protected int CurrentStatus { get { return _status; } set { _status = value; SetUpToDateAsFalse(); } }

        [Column(Columns.StatusLastChangedDate)]
        protected DateTime? StatusLastChangedDate { get { return _statusLastEditDate; } set { _statusLastEditDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Price)]
        public Money Price
        {
            get { return _price; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                _price = value;
                SetUpToDateAsFalse();
            }
        }

        [Column(Columns.TargetBalance)]
        protected int TargetBalanceInt { get { return _targetBalance; } set { _targetBalance = value; SetUpToDateAsFalse(); } }

        public PurchaseBalances TargetBalance { get { return (PurchaseBalances)TargetBalanceInt; } set { TargetBalanceInt = (int)value; } }

        private string _targetUrl, _creatorUsername, _creatorEmail;
        private int _createdBy, _clicks, _endValue, _status, _totalSecsActive, _targetBalance;
        private DateTime? _startDate, _creationDate, _endDate, _statusLastEditDate;
        private Money _price;
        private End.Mode _endMode;

        #endregion

        public override bool IsUpToDate
        {
            get { return base.IsUpToDate && _thisClicks == 0; }
        }

        /// <summary>
        /// Returns total time when advertisement was active.
        /// </summary>
        public TimeSpan ActiveTime
        {
            get
            {
                return TimeSpan.FromSeconds(TotalSecsActive) +
                    (Status == AdvertStatus.Active ? DateTime.Now - StatusLastChangedDate.Value : TimeSpan.Zero);
            }
        }

        /// <exception cref="InvalidOperationException"/>
        public AdvertStatus Status
        {
            get { return (AdvertStatus)CurrentStatus; }

            set
            {
                if ((int)value == CurrentStatus) return;

                DateTime now = DateTime.Now;
                changeStatus(now, Status, value);

                CurrentStatus = (int)value;
                StatusLastChangedDate = now;
            }
        }

        private void changeStatus(DateTime now, AdvertStatus oldStatus, AdvertStatus newStatus)
        {
            // Null -> WaitingForAcceptance
            if (oldStatus == AdvertStatus.Null && newStatus == AdvertStatus.WaitingForAcceptance)            
                CreationDate = now;            

            // WaitingForAcceptance -> Active, WaitingForAcceptance -> Paused
            else if (oldStatus == AdvertStatus.WaitingForAcceptance && (newStatus == AdvertStatus.Active || newStatus == AdvertStatus.Paused))            
                StartDate = now;            

            // WaitingForAcceptance -> Rejected, Active -> Finished, Paused -> Finished
            else if ((oldStatus == AdvertStatus.WaitingForAcceptance && newStatus == AdvertStatus.Rejected) ||
                    (oldStatus == AdvertStatus.Active && newStatus == AdvertStatus.Finished) ||
                    (oldStatus == AdvertStatus.Paused && newStatus == AdvertStatus.Finished))            
                EndDate = now;

            // Active -> Paused
            else if (oldStatus == AdvertStatus.Active && newStatus == AdvertStatus.Paused)            
                TotalSecsActive += (int)(now - StatusLastChangedDate.Value).TotalSeconds;            

            // Paused -> Active, Finshed -> Active, Finished -> Pause
            // nothing to do
        }

        /// <summary>
        /// Indicates the way this advert "ends" (after some clicks, days, or never).
        /// Contains total Value of days/clicks that should be completed.
        /// </summary>
        public End Ends
        {
            get { return End.FromModeValue(_endMode, EndValue); }
            set
            {
                EndMode = (int)value.EndMode;
                EndValue = value.EndMode == End.Mode.Endless ? 0 : (int)value.Value;
            }
        }

        public Advertiser Advertiser
        {
            get
            {
                return Advertiser.FromColumns(CreatedBy, CreatorUsername, CreatorEmail);
            }
            set
            {
                CreatedBy = (int)value.CreatedBy;
                CreatorUsername = value.MemberUsername;
                CreatorEmail = value.StrangerEmail;
            }
        }

        #region Constructors

        public Advert(int id)
            : base(id) { }

        public Advert(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        protected Advert()
            : base()
        {
            initializeFieldsAsBlank();
        }

        private void initializeFieldsAsBlank()
        {
            _price = Money.Zero;
        }

        #endregion

        private int _thisClicks;

        /// <summary>
        /// Call when member successfully viewed adversitement.
        /// </summary>
        public virtual void Click()
        {
            ++_thisClicks;
        }

        public virtual void Unclick()
        {
            --_thisClicks;
        }

        public virtual void ReloadClicks()
        {
            ReloadPartially(IsUpToDate, new PropertyBuilder<Advert>().Append(x => x.Clicks).Build());
        }

        public virtual void SaveClicks()
        {
            if (!IsInDatabase)
                throw new InvalidOperationException("Object not saved in database. Use Save() first");

            bool isUpToDate = IsUpToDate;

            ReloadClicks();
            SavePartially(isUpToDate, new PropertyBuilder<Advert>().Append(x => x.Clicks).Build());
            _thisClicks = 0;
        }

        public void ResetClicks()
        {
            if (!IsInDatabase)
                throw new InvalidOperationException("Object not saved in database. Use Save() first");

            bool isUpToDate = IsUpToDate;

            Clicks = 0;
            SavePartially(isUpToDate, new PropertyBuilder<Advert>().Append(x => x.Clicks).Build());
        }

        public virtual void SaveStatus()
        {
            var builder = new PropertyBuilder<Advert>()
                .Append(x => x.CurrentStatus)
                .Append(x => x.TotalSecsActive);

            // HACK: Parser ma problemy z DateTime? == null -> nie uwzględniamy ich w savie
            if (StatusLastChangedDate.HasValue) builder.Append(x => x.StatusLastChangedDate);
            if (CreationDate.HasValue) builder.Append(x => x.CreationDate);
            if (StartDate.HasValue) builder.Append(x => x.StartDate);
            if (EndDate.HasValue) builder.Append(x => x.EndDate);

            SavePartially(IsUpToDate, builder.Build());
        }

        public override void Reload()
        {
            base.Reload();
        }

        public override void Save(bool forceSave = false)
        {
            if (IsInDatabase)
                ReloadClicks();
            base.Save(forceSave);
        }

        public int ProgressInPercent
        {
            get { return AdvertHelper.GetProgressInPercent(this); }
        }


        public bool ShouldBeFinished
        {
            get
            {
                if (Status.IsBefore(AdvertStatus.Finished))
                {
                    if (Ends.EndMode == End.Mode.Clicks) return Clicks >= EndValue;
                    if (Ends.EndMode == End.Mode.Days) return DateTime.Now >= EndDate.Value;
                }

                return false;
            }
        }

        public bool IsProlongable
        {
            get
            {
                return Status == AdvertStatus.WaitingForAcceptance ||
                       Status == AdvertStatus.Active ||
                       Status == AdvertStatus.Paused ||
                       Status == AdvertStatus.Finished;
            }
        }        

        /// <summary>
        /// Prolongs advert (adds more.Value to this.Ends).
        /// </summary>
        /// <param name="more">If this.Ends.EndMode != more.EndMode then this.Ends.EndMode is overriden.</param>
        /// <param name="prolongationCost"></param>
        /// <exception cref="InvalidOperationException">
        /// When advert not in status of waiting for acceptance, active or paused or finished
        /// </exception>
        /// <exception cref="DbException"/>
        public virtual void Prolong(End more, Money prolongationCost)
        {
            prolong(more, prolongationCost);
        }

        ///<exception cref="InvalidOperationException" />
        protected virtual void prolong(End more, Money prolongationCost)
        {
            bool isUpToDate = IsUpToDate;

            if (!IsProlongable)
                throw new InvalidOperationException("Advert cannot be prolonged");

            End newEnds;

            if (Ends.EndMode == End.Mode.Clicks)
                newEnds = End.FromClicks(EndValue);
            else if (Ends.EndMode == End.Mode.Days)
                newEnds = End.FromDays(ActiveTime);
            else newEnds = more;                    // in case nextValue.EndMode == Null | Endless

            Price += prolongationCost;
            Ends = newEnds.AddValue(more.Value);

            var propertiesToSave = new PropertyBuilder<Advert>()
                                        .Append(x => x.EndMode)
                                        .Append(x => x.EndValue)
                                        .Append(x => Price).Build();

            SavePartially(isUpToDate, propertiesToSave);
        }
    }

    [Serializable]
    public abstract class Advert<AP> : Advert where AP : IAdvertPack
    {
        #region Constructors

        /// <summary>
        /// Creates blank instance of FacebookAdvert class
        /// </summary>
        public Advert()
            : base()
        {
            AdvertPackId = NotInDatabaseId;
        }

        public Advert(int id) : base(id) { }

        public Advert(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors


        public abstract AP Pack { get; set; }

        /// <summary>
        /// Prolongs advert (adds more.Ends.Value to current Advert.Ends).
        /// Does not add more.Price to this.Price
        /// </summary>
        /// <param name="more">If Advert.Ends.EndMode != more.Ends.EndMode then Ends.EndMode is overriden.</param>
        /// <param name="prolongationCost"></param>
        /// <exception cref="InvalidOperationException">
        /// When advert not in status of waiting for acceptance, active or paused or finished
        /// </exception>
        /// <exception cref="DbException"/>
        public virtual void Prolong(AP more, Money prolongationCost)
        {
            prolong(more.Ends, prolongationCost);
        }


        /// <summary>
        /// Prolongs advert (adds more.Ends.Value to current Advert.Ends).
        /// DOES ADD more.Price to this.Price
        /// </summary>
        /// <param name="more">If Advert.Ends.EndMode != more.Ends.EndMode then Ends.EndMode is overriden.</param>
        /// <param name="prolongationCost"></param>
        /// <exception cref="InvalidOperationException">
        /// When advert not in status of waiting for acceptance, active or paused
        /// </exception>
        /// <exception cref="DbException"/>
        public virtual void Prolong(AP more)
        {
            prolong(more.Ends, more.Price);
        }
    }
}