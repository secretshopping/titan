using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using Prem.PTC.Utils;
using System.Collections.Generic;

namespace Prem.PTC.Advertising
{
    /// <summary>
    /// Summary description for AdvertPack
    /// </summary>
    public abstract class AdvertPack : BaseTableObject, IAdvertPack
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static class Columns
        {
            [Obsolete]
            public const string Name = "Name";
            public const string IsVisibleByMembers = "IsVisibleByMembers";
            public const string EndValue = "EndValue";
            public const string EndMode = "EndMode";
            public const string Price = "Price";
        }

        //TODO: delete in future
        [Obsolete]
        [Column(Columns.Name)]
        public string Name { get { return _name; } set { _name = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsVisibleByMembers)]
        public bool IsVisibleByMembers { get { return _isVisibleByMembers; } set { _isVisibleByMembers = value; SetUpToDateAsFalse(); } }

        [Column(Columns.EndValue)]
        protected int EndValue { get { return _endClicks; } set { _endClicks = value; SetUpToDateAsFalse(); } }

        [Column(Columns.EndMode)]
        protected int EndMode { get { return (int)_endMode; } set { _endMode = (End.Mode)value; SetUpToDateAsFalse(); } }

        [Column(Columns.Price)]
        public Money Price
        {
            get { return _price; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _price = value; SetUpToDateAsFalse();
            }
        }

        private string _name;
        private bool _isVisibleByMembers;
        private int _endClicks;
        private Money _price;
        private End.Mode _endMode;

        #endregion


        public virtual End Ends
        {
            get
            {
                return End.FromModeValue(_endMode, EndValue);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                int endValue = value.EndMode == End.Mode.Endless ? 0 : (int)value.Value;

                EndMode = (int)value.EndMode;
                EndValue = endValue;
            }
        }


        #region Constructors

        public AdvertPack() : base() { initializeFieldsAsBlank(); }
        public AdvertPack(int id) : base(id) { }
        public AdvertPack(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        private void initializeFieldsAsBlank()
        {
            _price = Money.Zero;
            _endMode = (int)End.Mode.Null;
        }

        #endregion


        /// <exception cref="DbException"/>
        protected static ListItem[] ListControlSource(string tableName, string idCol)
        {

            var whatColumns = Parser.Columns(idCol, AdvertPack.Columns.Name, AdvertPack.Columns.Price);
            DataTable advertPacksTable;
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                advertPacksTable =
                    bridge.Instance.Select(tableName, whatColumns, null);
            }

            var sort = from DataRow row in advertPacksTable.Rows
                       orderby row.Field<decimal>(AdvertPack.Columns.Price) ascending
                       select new ListItem(row.Field<string>(AdvertPack.Columns.Name),
                           Convert.ToString(row.Field<int>(idCol)));

            return sort.ToArray();
        }
    }

    public class BannerAdvertPack : AdvertPack, IBannerAdvertPack
    {
        #region Columns

        public static new string TableName { get { return AppSettings.TableNames.BannerAdvertPacks; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "BannerAdvertPackId";
            public const string Type = "Type";
            public const string Status = "Status";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Type)]
        protected int TypeInt { get { return _TypeInt; } set { _TypeInt = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Status)]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

        public UniversalStatus Status
        {
            get { return (UniversalStatus)StatusInt; }
            set { StatusInt = (int)value; }
        }

        public BannerAdvertDimensions Type
        {
            get
            {
                return new BannerAdvertDimensions(TypeInt);
            }
            set
            {
                TypeInt = value.Id;
            }
        }

        private int _id, _TypeInt, _StatusInt;

        #endregion


        #region Constructors

        public BannerAdvertPack() : base() { }
        public BannerAdvertPack(int id) : base(id) { }
        public BannerAdvertPack(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion

        public void Activate()
        {
            Status = UniversalStatus.Active;
            this.Save();
        }

        public void Pause()
        {
            Status = UniversalStatus.Paused;
            this.Save();
        }

        public override void Delete()
        {
            Status = UniversalStatus.Deleted;
            this.Save();
        }

        /// <param name="name">Name of BannerAdvertPack</param>
        /// <exception cref="DbException" />
        public static bool Exists(string name)
        {
            return TableHelper.RowExists(BannerAdvertPack.TableName, AdvertPack.Columns.Name, name);
        }

        /// <param name="name">Name of BannerAdvertPack</param>
        /// <exception cref="DbException" />
        public static void Delete(string name)
        {
            TableHelper.DeleteRows<BannerAdvertPack>(AdvertPack.Columns.Name, name);
        }

        /// <param name="id">id of BannerAdvertPack</param>
        /// <exception cref="DbException" />
        public static void Delete(int id)
        {
            TableHelper.DeleteRows<BannerAdvertPack>(BannerAdvertPack.Columns.Id, id);
        }

        /// <exception cref="DbException"/>
        public static new ListItem[] ListControlSource
        {
            get { return ListControlSource(BannerAdvertPack.TableName, BannerAdvertPack.Columns.Id); }
        }

        public static bool HaveAllActiveDimensionsAnAdvertPackAssigned()
        {
            var activeBannerDimensions = TableHelper.GetListFromRawQuery(string.Format("SELECT Id FROM BannerAdvertDimensions WHERE Status = {0}", (int)UniversalStatus.Active));

            foreach (var bannerDimension in activeBannerDimensions)
            {
                var numberOfPack = (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(BannerAdvertPackId) FROM BannerAdvertPacks WHERE Type = {0} AND IsVisibleByMembers = {1} AND Status = {2}", bannerDimension, 1, (int)UniversalStatus.Active));

                if (numberOfPack == 0)
                    return false;
            }
            return true;
        }
    }

    public class PtcAdvertPack : AdvertPack, IPtcAdvertPack
    {
        #region Columns

        public static new string TableName { get { return AppSettings.TableNames.PtcAdvertPacks; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "PtcAdvertPackId";
            public const string DisplayTime = "DisplayTimeSeconds";
            public const string PTCCreditsPerDayOrClick = "PTCCreditsPerDayOrClick";
            public const string PTCPackCashBackPercent = "PTCPackCashBackPercent";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DisplayTime)]
        protected int DisplayTimeSeconds { get { return _displayTimeSeconds; } set { _displayTimeSeconds = value; SetUpToDateAsFalse(); } }

        [Column("MinUserBalance")]
        public Money MinUserBalance
        {
            get { return _MinUserBalance; }
            set
            {
                _MinUserBalance = value; SetUpToDateAsFalse();
            }
        }

        [Column("MaxUserBalance")]
        public Money MaxUserBalance
        {
            get { return _MaxUserBalance; }
            set
            {
                _MaxUserBalance = value; SetUpToDateAsFalse();
            }
        }

        [Column(Columns.PTCCreditsPerDayOrClick)]
        public decimal PTCCreditsPerDayOrClick { get { return _PTCCreditsPerDayOrClick; } set { _PTCCreditsPerDayOrClick = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PTCPackCashBackPercent)]
        public int PTCPackCashBackPercent { get { return _PTCPackCashBackPercent; } set { _PTCPackCashBackPercent = value; SetUpToDateAsFalse(); } }

        private int _id, _displayTimeSeconds, _PTCPackCashBackPercent;
        private Money _MinUserBalance, _MaxUserBalance;
        private decimal _PTCCreditsPerDayOrClick;

        #endregion


        public TimeSpan DisplayTime
        {
            get { return TimeSpan.FromSeconds(DisplayTimeSeconds); }
            set { DisplayTimeSeconds = (int)value.TotalSeconds; }
        }


        #region Constructors

        /// <exception cref="DbException" />
        public PtcAdvertPack() : base() { }
        /// <exception cref="DbException" />
        public PtcAdvertPack(int id) : base(id) { }
        /// <exception cref="DbException" />
        public PtcAdvertPack(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion


        /// <param name="name">Name of PtcAdvertPack</param>
        /// <exception cref="DbException" />
        public static bool Exists(string name)
        {
            return TableHelper.RowExists(PtcAdvertPack.TableName, AdvertPack.Columns.Name, name);
        }

        /// <param name="name">Name of PtcAdvertPack</param>
        /// <exception cref="DbException" />
        public static void Delete(string name)
        {
            TableHelper.DeleteRows<PtcAdvertPack>(AdvertPack.Columns.Name, name);
        }

        /// <param name="id">id of PtcAdvertPack</param>
        /// <exception cref="DbException" />
        public static void Delete(int id)
        {
            TableHelper.DeleteRows<PtcAdvertPack>(PtcAdvertPack.Columns.Id, id);
        }

        /// <exception cref="DbException"/>
        public static new ListItem[] ListControlSource
        {
            get { return ListControlSource(PtcAdvertPack.TableName, PtcAdvertPack.Columns.Id); }
        }
    }

    public class TrafficExchangeAdvertPack : AdvertPack, IPtcAdvertPack
    {
        #region Columns

        public static new string TableName { get { return "TrafficExchangeAdvertPacks"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "TrafficExchangeAdvertPackId";
            public const string DisplayTime = "DisplayTimeSeconds";
            public const string TimeBetweenViewsInMinutes = "TimeBetweenViewsInMinutes";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DisplayTime)]
        protected int DisplayTimeSeconds { get { return _displayTimeSeconds; } set { _displayTimeSeconds = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TimeBetweenViewsInMinutes)]
        public int TimeBetweenViewsInMinutes { get { return _timeBetweenViewsInMinutes; } set { _timeBetweenViewsInMinutes = value; SetUpToDateAsFalse(); } }

        [Column("MinUserBalance")]
        public Money MinUserBalance
        {
            get { return _MinUserBalance; }
            set
            {
                _MinUserBalance = value; SetUpToDateAsFalse();
            }
        }

        [Column("MaxUserBalance")]
        public Money MaxUserBalance
        {
            get { return _MaxUserBalance; }
            set
            {
                _MaxUserBalance = value; SetUpToDateAsFalse();
            }
        }

        private int _id, _displayTimeSeconds, _timeBetweenViewsInMinutes;
        private Money _MinUserBalance, _MaxUserBalance;
        #endregion


        public TimeSpan DisplayTime
        {
            get { return TimeSpan.FromSeconds(DisplayTimeSeconds); }
            set { DisplayTimeSeconds = (int)value.TotalSeconds; }
        }


        #region Constructors

        /// <exception cref="DbException" />
        public TrafficExchangeAdvertPack() : base() { }
        /// <exception cref="DbException" />
        public TrafficExchangeAdvertPack(int id) : base(id) { }
        /// <exception cref="DbException" />
        public TrafficExchangeAdvertPack(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion


        /// <param name="name">Name of PtcAdvertPack</param>
        /// <exception cref="DbException" />
        public static bool Exists(string name)
        {
            return TableHelper.RowExists(TrafficExchangeAdvertPack.TableName, AdvertPack.Columns.Name, name);
        }

        /// <param name="name">Name of PtcAdvertPack</param>
        /// <exception cref="DbException" />
        public static void Delete(string name)
        {
            TableHelper.DeleteRows<TrafficExchangeAdvertPack>(AdvertPack.Columns.Name, name);
        }

        /// <param name="id">id of PtcAdvertPack</param>
        /// <exception cref="DbException" />
        public static void Delete(int id)
        {
            TableHelper.DeleteRows<TrafficExchangeAdvertPack>(TrafficExchangeAdvertPack.Columns.Id, id);
        }

        /// <exception cref="DbException"/>
        public static new ListItem[] ListControlSource
        {
            get { return ListControlSource(TrafficExchangeAdvertPack.TableName, PtcAdvertPack.Columns.Id); }
        }
    }

    public class TrafficGridAdvertPack : AdvertPack, ITrafficGridAdvertPack
    {
        #region Columns

        public static new string TableName { get { return "TrafficGridAdvertPacks"; } }
        protected override string dbTable { get { return TableName; } }

        public static new class Columns
        {
            public const string Id = "PtcAdvertPackId";
            public const string DisplayTime = "DisplayTimeSeconds";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DisplayTime)]
        protected int DisplayTimeSeconds { get { return _displayTimeSeconds; } set { _displayTimeSeconds = value; SetUpToDateAsFalse(); } }

        private int _id, _displayTimeSeconds;

        #endregion


        public TimeSpan DisplayTime
        {
            get { return TimeSpan.FromSeconds(DisplayTimeSeconds); }
            set { DisplayTimeSeconds = (int)value.TotalSeconds; }
        }


        #region Constructors

        /// <exception cref="DbException" />
        public TrafficGridAdvertPack() : base() { }
        /// <exception cref="DbException" />
        public TrafficGridAdvertPack(int id) : base(id) { }
        /// <exception cref="DbException" />
        public TrafficGridAdvertPack(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion


        /// <param name="name">Name of PtcAdvertPack</param>
        /// <exception cref="DbException" />
        public static bool Exists(string name)
        {
            return TableHelper.RowExists(TrafficGridAdvertPack.TableName, AdvertPack.Columns.Name, name);
        }

        /// <param name="name">Name of PtcAdvertPack</param>
        /// <exception cref="DbException" />
        public static void Delete(string name)
        {
            TableHelper.DeleteRows<TrafficGridAdvertPack>(AdvertPack.Columns.Name, name);
        }

        /// <param name="id">id of PtcAdvertPack</param>
        /// <exception cref="DbException" />
        public static void Delete(int id)
        {
            TableHelper.DeleteRows<TrafficGridAdvertPack>(TrafficGridAdvertPack.Columns.Id, id);
        }

        /// <exception cref="DbException"/>
        public static new ListItem[] ListControlSource
        {
            get { return ListControlSource(TrafficGridAdvertPack.TableName, TrafficGridAdvertPack.Columns.Id); }
        }
    }

    public class FacebookAdvertPack : AdvertPack, IFacebookAdvertPack
    {
        #region Columns

        protected override string dbTable { get { return TableName; } }
        public static new string TableName { get { return "FacebookAdvertPacks"; } }

        public static class Columns
        {
            public const string Id = "FbAdvertPackId";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        private int _id;

        #endregion Columns


        #region Constructors

        /// <exception cref="DbException" />
        public FacebookAdvertPack() : base() { }
        /// <exception cref="DbException" />
        public FacebookAdvertPack(int id) : base(id) { }
        /// <exception cref="DbException" />
        public FacebookAdvertPack(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion

        /// <exception cref="ArgumentException">
        /// When attempting to set End wirj "Days" End.Mode
        /// </exception>
        public override End Ends
        {
            get { return base.Ends; }
            set
            {
                if (value.EndMode == End.Mode.Days)
                    throw new ArgumentException("Ends: End.Mode.Days");

                base.Ends = value;
            }
        }


        /// <param name="id">id of FacebookAdvertPack</param>
        /// <exception cref="DbException" />
        public static void Delete(int id)
        {
            TableHelper.DeleteRows<FacebookAdvertPack>(FacebookAdvertPack.Columns.Id, id);
        }
    }

}