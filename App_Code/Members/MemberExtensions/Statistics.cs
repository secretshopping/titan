using Prem.PTC.Payments;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using ExtensionMethods;
using System.Text;
using Prem.PTC.Advertising;
using Titan.Cryptocurrencies;
using System.Data;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        #region Columns

        [Column(Columns.TotalEarned)]
        public Money TotalEarned { get { return tte; } set { tte = value; SetUpToDateAsFalse(); } }

        [Column("TotalPointsEarned")]
        public int TotalPointsEarned { get { return _TotalPointsEarned; } set { _TotalPointsEarned = value; SetUpToDateAsFalse(); } }

        [Column("TotalDRPTCClicks")]
        public int TotalDRPTCClicks { get { return _TotalDRPTCClicks; } set { _TotalDRPTCClicks = value; SetUpToDateAsFalse(); } }

        [Column("TotalPTCClicksToDReferer")]
        public int TotalPTCClicksToDReferer { get { return _TotalPTCClicksToDReferer; } set { _TotalPTCClicksToDReferer = value; SetUpToDateAsFalse(); } }

        [Column("TotalDirectReferralsEarned")]
        public Money TotalDirectReferralsEarned { get { return _TotalDirectReferralsEarned; } set { _TotalDirectReferralsEarned = value; SetUpToDateAsFalse(); } }

        [Column("TotalEarnedToDReferer")]
        public Money TotalEarnedToDReferer { get { return _TotalEarnedToDReferer; } set { _TotalEarnedToDReferer = value; SetUpToDateAsFalse(); } }

        [Column("TotalDirectReferralsPointsEarned")]
        public int TotalDirectReferralsPointsEarned { get { return _TotalDirectReferralsPointsEarned; } set { _TotalDirectReferralsPointsEarned = value; SetUpToDateAsFalse(); } }

        [Column("TotalPointsEarnedToDReferer")]
        public int TotalPointsEarnedToDReferer { get { return _TotalPointsEarnedToDReferer; } set { _TotalPointsEarnedToDReferer = value; SetUpToDateAsFalse(); } }

        [Column("TotalDREarnedFromAdPacks")]
        public Money TotalDREarnedFromAdPacks { get { return _TotalDREarnedFromAdPacks; } set { _TotalDREarnedFromAdPacks = value; SetUpToDateAsFalse(); } }

        [Column("TotalAdPacksToDReferer")]
        public Money TotalAdPacksToDReferer { get { return _TotalAdPacksToDReferer; } set { _TotalAdPacksToDReferer = value; SetUpToDateAsFalse(); } }

        [Column("TotalEarnedFromCashLinks")]
        public Money TotalEarnedFromCashLinks { get { return _TotalEarnedFromCashLinks; } set { _TotalEarnedFromCashLinks = value; SetUpToDateAsFalse(); } }

        [Column("TotalEarnedFromDRCashLinks")]
        public Money TotalEarnedFromDRCashLinks { get { return _TotalEarnedFromDRCashLinks; } set { _TotalEarnedFromDRCashLinks = value; SetUpToDateAsFalse(); } }

        [Column("TotalCashLinksToDReferer")]
        public Money TotalCashLinksToDReferer { get { return _TotalCashLinksToDReferer; } set { _TotalCashLinksToDReferer = value; SetUpToDateAsFalse(); } }

        [Column(Columns.StatsClicks)]
        public string StatsClicks { get { return stats_clicks; } set { stats_clicks = value; SetUpToDateAsFalse(); } }

        [Column(Columns.StatsEarned)]
        public string StatsEarned { get { return _StatsEarned; } set { _StatsEarned = value; SetUpToDateAsFalse(); } }

        [Column(Columns.UserClicksStats)]
        public string UserClicksStats { get { return _UserClicksStats; } set { _UserClicksStats = value; SetUpToDateAsFalse(); } }

        [Column("RawDirectReferralsClicks")]
        public string RawDirectReferralsClicks { get { return _RawDirectReferralsClicks; } set { _RawDirectReferralsClicks = value; SetUpToDateAsFalse(); } }

        [Column("RawRentedReferralsClicks")]
        public string RawRentedReferralsClicks { get { return _RawRentedReferralsClicks; } set { _RawRentedReferralsClicks = value; SetUpToDateAsFalse(); } }

        [Column("StatsDirectReferralsEarned")]
        public string StatsDirectReferralsEarned { get { return _StatsDirectReferralsEarned; } set { _StatsDirectReferralsEarned = value; SetUpToDateAsFalse(); } }

        [Column("StatsPointsEarned")]
        public string StatsPointsEarned { get { return _StatsPointsEarned; } set { _StatsPointsEarned = value; SetUpToDateAsFalse(); } }

        [Column("StatsDirectReferralsPointsEarned")]
        public string StatsDirectReferralsPointsEarned { get { return _StatsDirectReferralsPointsEarned; } set { _StatsDirectReferralsPointsEarned = value; SetUpToDateAsFalse(); } }

        [Column("StatsDRAdPacksEarned")]
        public string StatsDRAdPacksEarned { get { return _StatsDRAdPacksEarned; } set { _StatsDRAdPacksEarned = value; SetUpToDateAsFalse(); } }

        [Column("StatsCashLinksEarned")]
        public string StatsCashLinksEarned { get { return _StatsCashLinksEarned; } set { _StatsCashLinksEarned = value; SetUpToDateAsFalse(); } }

        [Column("StatsDRCashLinksEarned")]
        public string StatsDRCashLinksEarned { get { return _StatsDRCashLinksEarned; } set { _StatsDRCashLinksEarned = value; SetUpToDateAsFalse(); } }

        [Column("StatsArticlesTotalSharesMoney")]
        public string StatsArticlesTotalSharesMoney { get { return _StatsArticlesTotalSharesMoney; } set { _StatsArticlesTotalSharesMoney = value; SetUpToDateAsFalse(); } }

        [Column("LastDRActivity")]
        public DateTime LastDRActivity { get { return _LastDRActivity; } set { _LastDRActivity = value; SetUpToDateAsFalse(); } }

        [Column("ReferralSince")]
        public DateTime ReferralSince { get { return _ReferralSince; } set { _ReferralSince = value; SetUpToDateAsFalse(); } }

        [Column("PtcSurfClicksThisMonth")]
        public int PtcSurfClicksThisMonth { get { return _PtcSurfClicksThisMonth; } set { _PtcSurfClicksThisMonth = value; SetUpToDateAsFalse(); } }

        [Column("PtcAutoSurfClicksThisMonth")]
        public int PtcAutoSurfClicksThisMonth { get { return _PtcAutoSurfClicksThisMonth; } set { _PtcAutoSurfClicksThisMonth = value; SetUpToDateAsFalse(); } }

        [Column("TotalERC20TokensEarnedToDReferer")]
        public Decimal TotalERC20TokensEarnedToDReferer { get { return _TotalERC20TokensEarnedToDReferer; } set { _TotalERC20TokensEarnedToDReferer = value; SetUpToDateAsFalse(); } }

        [Column("StatsRevShareLastWeekIncome")]
        public Money StatsRevShareLastWeekIncome { get { return _StatsRevShareLastWeekIncome; } set { _StatsRevShareLastWeekIncome = value; SetUpToDateAsFalse(); } }

        [Column("StatsRevShareLastMonthIncome")]
        public Money StatsRevShareLastMonthIncome { get { return _StatsRevShareLastMonthIncome; } set { _StatsRevShareLastMonthIncome = value; SetUpToDateAsFalse(); } }

        [Column("StatsRevShareCurrentWeekIncome")]
        public Money StatsRevShareCurrentWeekIncome { get { return _StatsRevShareCurrentWeekIncome; } set { _StatsRevShareCurrentWeekIncome = value; SetUpToDateAsFalse(); } }

        [Column("StatsRevShareCurrentMonthIncome")]
        public Money StatsRevShareCurrentMonthIncome { get { return _StatsRevShareCurrentMonthIncome; } set { _StatsRevShareCurrentMonthIncome = value; SetUpToDateAsFalse(); } }

        [Column("StatsCommissionsLastWeekIncome")]
        public Money StatsCommissionsLastWeekIncome { get { return _StatsCommissionsLastWeekIncome; } set { _StatsCommissionsLastWeekIncome = value; SetUpToDateAsFalse(); } }

        [Column("StatsCommissionsLastMonthIncome")]
        public Money StatsCommissionsLastMonthIncome { get { return _StatsCommissionsLastMonthIncome; } set { _StatsCommissionsLastMonthIncome = value; SetUpToDateAsFalse(); } }

        [Column("StatsCommissionsCurrentWeekIncome")]
        public Money StatsCommissionsCurrentWeekIncome { get { return _StatsCommissionsCurrentWeekIncome; } set { _StatsCommissionsCurrentWeekIncome = value; SetUpToDateAsFalse(); } }

        [Column("StatsCommissionsCurrentMonthIncome")]
        public Money StatsCommissionsCurrentMonthIncome { get { return _StatsCommissionsCurrentMonthIncome; } set { _StatsCommissionsCurrentMonthIncome = value; SetUpToDateAsFalse(); } }

        private string _RawDirectReferralsClicks, _RawRentedReferralsClicks, _StatsDirectReferralsEarned, _UserClicksStats, _StatsEarned, stats_clicks, _StatsPointsEarned,
    _StatsDirectReferralsPointsEarned, _StatsDRAdPacksEarned, _StatsCashLinksEarned, _StatsDRCashLinksEarned, _StatsArticlesTotalSharesMoney;
        private Money tte, _TotalDirectReferralsEarned, _TotalDREarnedFromAdPacks, _TotalEarnedFromCashLinks, _TotalEarnedFromDRCashLinks,
            _TotalCashLinksToDReferer, _TotalAdPacksToDReferer, _TotalEarnedToDReferer,
            _StatsRevShareLastWeekIncome, _StatsRevShareLastMonthIncome, _StatsRevShareCurrentWeekIncome, _StatsRevShareCurrentMonthIncome,
            _StatsCommissionsLastWeekIncome, _StatsCommissionsLastMonthIncome, _StatsCommissionsCurrentWeekIncome, _StatsCommissionsCurrentMonthIncome;
        private int _TotalPointsEarned, _TotalDirectReferralsPointsEarned, _TotalPTCClicks, _TotalDRPTCClicks, _TotalPointsEarnedToDReferer, _TotalPTCClicksToDReferer,
            _PtcSurfClicksThisMonth, _PtcAutoSurfClicksThisMonth;
        private DateTime _LastDRActivity, _ReferralSince;
        private Decimal _TotalERC20TokensEarnedToDReferer;

        #endregion

        #region Lists

        public List<Money> StatisticsCashLinksEarned
        {
            get
            {
                return TableHelper.GetMoneyListFromString(StatsCashLinksEarned);
            }
        }
        public List<Money> StatisticsDRCashLinksEarned
        {
            get
            {
                return TableHelper.GetMoneyListFromString(StatsDRCashLinksEarned);
            }
        }

        /// <summary>
        /// User clicks made to that account 
        /// [0] today [1] yesterday itd.. upto [9]
        /// </summary>
        public List<int> UserStatisticsClicks
        {
            get
            {
                return TableHelper.GetIntListFromString(UserClicksStats);
            }
        }

        /// <summary>
        /// All clicks made to that account (even those not credited)
        /// [0] today [1] yesterday itd.. upto [9]
        /// </summary>
        public List<int> StatisticsClicks
        {
            get
            {
                return TableHelper.GetIntListFromString(StatsClicks);
            }
        }

        /// <summary>
        /// All credited money
        /// [0] today [1] yesterday itd.. upto [9]
        /// </summary>
        public List<Money> StatisticsDRAdPacksEarned
        {
            get
            {
                return TableHelper.GetMoneyListFromString(StatsDRAdPacksEarned);
            }
        }


        public List<Money> StatisticsDirectReferralsEarned
        {
            get
            {
                return TableHelper.GetMoneyListFromString(StatsDirectReferralsEarned);
            }
        }
        public List<Money> StatisticsEarned
        {
            get
            {
                return TableHelper.GetMoneyListFromString(StatsEarned);
            }
        }

        public List<int> StatisticsPointsEarned
        {
            get
            {
                return TableHelper.GetIntListFromString(StatsPointsEarned);
            }
        }

        public List<int> StatisticsDirectRefPointsEarned
        {
            get
            {
                return TableHelper.GetIntListFromString(StatsDirectReferralsPointsEarned);
            }
        }

        //Titan News
        public List<Money> StatisticsArticlesTotalSharesMoney
        {
            get
            {
                return TableHelper.GetMoneyListFromString(StatsArticlesTotalSharesMoney);
            }
        }

        //Titan News
        public List<int> StatisticsArticlesTotalSharesReads
        {
            get
            {
                List<int> resultList = new List<int>();
                DataTable dataTable;

                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    dataTable = bridge.Instance.ExecuteRawCommandToDataTable(String.Format("" +
                        @"SELECT TOP 7 DATEADD(DAY,0, DATEDIFF(DAY,0, ViewDate)) AS ViewDay, COUNT(Id) AS ViewCount FROM ArticleViews WHERE InfluencerUserId = {0}
                          GROUP BY DATEADD(DAY,0, DATEDIFF(DAY,0, ViewDate)) ORDER BY DATEADD(DAY,0, DATEDIFF(DAY,0, ViewDate)) DESC", this.Id));
                }

                for (int i = 0; i < 7; i++)
                {
                    DateTime CurrentDate = AppSettings.ServerTime.AddDays(-i).Date;
                    int currentDateResult = 0;

                    foreach (DataRow row in dataTable.Rows)
                        if (DateTime.Parse(row["ViewDay"].ToString()).Date == CurrentDate.Date)
                            currentDateResult = Convert.ToInt32(row["ViewCount"].ToString());

                    resultList.Add(currentDateResult);
                }

                return resultList;
            }
        }

        /// <summary>
        /// User clicks made (and credited) to account
        /// [0] today [1] yesterday itd.. upto [9]
        /// </summary>
        public List<int> StatisticsOnlyUserClicks
        {
            get
            {
                return TableHelper.GetIntListFromString(UserClicksStats);
            }
        }

        /// <summary>
        /// User DR clicks made to account
        /// [0] today [1] yesterday itd.. upto [9]
        /// </summary>
        public List<int> DirectReferralsClicks
        {
            get
            {
                return TableHelper.GetIntListFromString(RawDirectReferralsClicks);
            }
        }

        /// <summary>
        /// User RR clicks made to account
        /// [0] today [1] yesterday itd.. upto [9]
        /// </summary>
        public List<int> RentedReferralsClicks
        {
            get
            {
                return TableHelper.GetIntListFromString(RawRentedReferralsClicks);
            }
        }

        #endregion

        #region Generated Stats


        public bool HasEverBoughtAdPacks
        {
            get
            {
                string query = string.Format("SELECT TOP 1 * FROM AdPacks WHERE UserId = {0}", Id);
                var adPacks = TableHelper.GetListFromRawQuery<AdPack>(query);
                return adPacks.Count > 0;
            }
        }

        /// <summary>
        /// AdPacks that have not returned all the money yet
        /// </summary>
        public int ActiveAdPacks
        {
            get
            {
                return AdPackManager.GetNumberOfUsersAdPacks(Id, activePacks: true);

            }
        }

        /// <summary>
        /// AdPacks that returned all the money
        /// </summary>
        public int ExpiredAdPacks
        {
            get
            {
                return AdPackManager.GetNumberOfUsersAdPacks(Id, activePacks: false);

            }
        }

        /// <summary>
        /// Total money returned from AdPacks
        /// </summary>
        public Money EarningsFromAdPacks
        {
            get
            {
                return AdPackManager.GetUsersTotalMoneyReturnedFromAdPacks(Id);
            }
        }

        /// <summary>
        /// Referrals who earned you something (Money or Points)
        /// </summary>
        public int ActiveReferrals
        {
            get
            {
                return (int)TableHelper.SelectScalar(string.Format(
@"SELECT COUNT(*) FROM Users 
WHERE Referer = '{0}' 
AND(TotalEarnedToDReferer > 0 OR TotalPointsEarnedToDReferer > 0)", Name));
            }
        }

        public int InactiveReferrals
        {
            get
            {
                // = ALL - Active
                int allReferrals = (int)TableHelper.SelectScalar(string.Format(
@"SELECT COUNT(*) FROM Users 
WHERE Referer = '{0}'", Name));
                return allReferrals - ActiveReferrals;
            }
        }

        public int CustomGroupsCreated
        {
            get
            {
                return CustomGroupManager.GetNumberOfUsersCreatedGroups(Id);
            }
        }

        /// <summary>
        /// Closed groups that contain AdPacks which have not returned all money yet
        /// </summary>
        public int ActiveCustomGroupsCreated
        {
            get
            {
                return CustomGroupManager.GetNumberOfUsersGroups(Id, CustomGroupStatus.Active);
            }
        }

        /// <summary>
        /// Open groups (require more AdPacks to be closed)
        /// </summary>
        public int InactiveCustomGroupsCreated
        {
            get
            {
                return CustomGroupManager.GetNumberOfUsersGroups(Id, CustomGroupStatus.InProgress);
            }
        }

        /// <summary>
        /// Closed groups with AdPacks that returned all money
        /// </summary>
        public int ClosedCustomGroupsCreated
        {
            get
            {
                return CustomGroupManager.GetNumberOfUsersGroups(Id, CustomGroupStatus.Expired);
            }
        }


        /// <summary>
        /// ComissionsToday = Money credited on Commission Balance today.
        /// </summary>
        public Money ComissionsToday
        {
            get
            {
                var query = String.Format(
                    "SELECT SUM(Amount) FROM BalanceLogs WHERE UserId = {0} AND Balance = {1} " +
                    "AND DATEADD(dd, 0, DATEDIFF(dd, 0, DateOccured)) = DATEADD(dd, 0, DATEDIFF(dd, 0, '{2}')) AND Amount > 0",
                    this.Id, (int)BalanceType.CommissionBalance, AppSettings.ServerTime.ToDBString());

                var result = TableHelper.SelectScalar(query);

                return (result is DBNull) ? Money.Zero : new Money(Convert.ToDecimal(result));
            }
        }

        /// <summary>
        /// Total money paid out today (includes manual payout requests made today)
        /// </summary>
        public Money PaidOutToday
        {
            get
            {
                Money result = Money.Zero;

                string requestsQuery = string.Format("SELECT * FROM PayoutRequests WHERE Username = '{0}' AND BalanceType = {1}",
                   this.Name, (int)BalanceType.MainBalance);

                var requests = TableHelper.GetListFromRawQuery<PayoutRequest>(requestsQuery);
                foreach (var request in requests)
                    if (request.RequestDate.Date == AppSettings.ServerTime.Date)
                        result += request.Amount;

                StringBuilder query = new StringBuilder();
                query.AppendFormat(@"SELECT SUM(MoneyAmount) FROM BlockioIPNs 
                WHERE UserId = {0} AND OperationType = {1} AND CONVERT(date, OperationDate) = '{2}' AND IsExecuted = 'true'"
                , this.Id, (int)OperationType.Withdrawal, AppSettings.ServerTime.Date.ToDBString());

                try
                {
                    result += new Money((decimal)TableHelper.SelectScalar(query.ToString()));
                }
                catch { }

                return result;
            }
        }

        public Money PaidOutThisWeek
        {
            get
            {
                Money result = Money.Zero;
                string requestsQuery = string.Format("SELECT * FROM PayoutRequests WHERE Username = '{0}' AND BalanceType = {1}",
                    this.Name, (int)BalanceType.MainBalance);

                var requests = TableHelper.GetListFromRawQuery<PayoutRequest>(requestsQuery);
                foreach (var request in requests)
                    if (request.RequestDate.Date > AppSettings.ServerTime.Date.AddDays(-7) && request.RequestDate.Date <= AppSettings.ServerTime.Date)
                        result += request.Amount;

                StringBuilder query = new StringBuilder();
                query.AppendFormat(@"SELECT SUM(MoneyAmount) FROM BlockioIPNs 
                WHERE UserId = {0} AND OperationType = {1} AND CONVERT(date, OperationDate) > '{2}' AND CONVERT(date, OperationDate) <= '{3}' AND IsExecuted = 'true'"
                , this.Id, (int)OperationType.Withdrawal, AppSettings.ServerTime.Date.AddDays(-7).ToDBString(), AppSettings.ServerTime.Date.ToDBString());

                try
                {
                    result += new Money((decimal)TableHelper.SelectScalar(query.ToString()));
                }
                catch { }

                return result;
            }
        }

        public int NumberOfPayoutsToday
        {
            get
            {
                var btcRequests = (int)TableHelper.SelectScalar(string.Format(@"SELECT COUNT(*) FROM CryptocurrencyWithdrawRequests 
                    WHERE UserId = {0} AND CONVERT(date, RequestDate) = '{1}'", this.Id, AppSettings.ServerTime.Date.ToDBString()));

                var fiatRequests = (int)TableHelper.SelectScalar(string.Format(@"SELECT COUNT(*) FROM PayoutRequests 
                    WHERE Username = '{0}' AND CONVERT(date, RequestDate) = '{1}'AND BalanceType = {2}",
                    this.Name, AppSettings.ServerTime.Date.ToDBString(), (int)BalanceType.MainBalance));

                return btcRequests + fiatRequests;
            }
        }

        public int NumberOfCommissionPayoutsToday
        {
            get
            {
                var requests = (int)TableHelper.SelectScalar(string.Format(@"SELECT COUNT(*) FROM PayoutRequests 
                    WHERE Username = '{0}' AND CONVERT(date, RequestDate) = '{1}'AND BalanceType = {2}",
                    this.Name, AppSettings.ServerTime.Date.ToDBString(), (int)BalanceType.CommissionBalance));

                return requests;
            }
        }

        #endregion

        #region Methods

        public void IncreasePointsEarnings(int value)
        {
            StatsPointsEarned = IncreaseStats(StatsPointsEarned, value);
            TotalPointsEarned += value;
        }

        public void IncreaseDirectRefPointsEarnings(int value)
        {
            StatsDirectReferralsPointsEarned = IncreaseStats(StatsDirectReferralsPointsEarned, value);
            TotalDirectReferralsPointsEarned += value;
        }

        public void IncreaseAdPackEarningsFromDR(Money value)
        {
            StatsDRAdPacksEarned = IncreaseStats(StatsDRAdPacksEarned, value);
            TotalDREarnedFromAdPacks += value;
        }

        public void IncreaseEarningsFromDirectReferral(Money value)
        {
            StatsDirectReferralsEarned = IncreaseStats(StatsDirectReferralsEarned, value);
            TotalDirectReferralsEarned += value;
        }

        public void IncreaseCashLinksEarnings(Money value)
        {
            StatsCashLinksEarned = IncreaseStats(StatsCashLinksEarned, value);
            TotalEarnedFromCashLinks += value;
        }

        public void IncreaseDRCashLinksEarnings(Money value)
        {
            StatsDRCashLinksEarned = IncreaseStats(StatsDRCashLinksEarned, value);
            TotalEarnedFromDRCashLinks += value;
        }

        public void IncreaseStatClicks(int value)
        {
            StatsClicks = IncreaseStats(StatsClicks, value);
        }

        public void IncreaseDRClicks(int value = 1)
        {
            RawDirectReferralsClicks = IncreaseStats(RawDirectReferralsClicks, value);
            TotalDRPTCClicks += value;
        }

        public void IncreaseRRClicks(int value = 1)
        {
            RawRentedReferralsClicks = IncreaseStats(RawRentedReferralsClicks, value);
        }

        /// <summary>
        /// +Total clicks
        /// </summary>
        /// <param name="value"></param>
        public void IncreaseUserStatClicks(int value)
        {
            UserClicksStats = IncreaseStats(UserClicksStats, value);
            TotalClicks += value;
        }

        /// <summary>
        /// +Total earned
        /// </summary>
        /// <param name="value"></param>
        public void IncreaseEarnings(Money value)
        {
            StatsEarned = IncreaseStats(StatsEarned, value);
            TotalEarned += value;
        }

        private string IncreaseStats(string statType, Money value)
        {
            List<Money> statlist1 = TableHelper.GetMoneyListFromString(statType);
            statlist1[0] = statlist1[0] + value;
            return TableHelper.GetStringFromMoneyList(statlist1);
        }

        private string IncreaseStats(string statType, int value)
        {
            List<int> statlist1 = TableHelper.GetIntListFromString(statType);
            statlist1[0] = statlist1[0] + value;
            return TableHelper.GetStringFromIntList(statlist1);
        }

        public void IncreaseERC20TokensEarningsForDRef(Decimal value)
        {
            TotalERC20TokensEarnedToDReferer += value;
        }

        #endregion

        #region Helpers

        public void SaveStatistics()
        {
            PropertyInfo[] balancePropertiesToSave = buildStatistics();
            //
            SavePartially(IsUpToDate, balancePropertiesToSave);
        }

        private PropertyInfo[] buildStatistics()
        {
            PropertyBuilder<Member> builder = new PropertyBuilder<Member>();

            builder.Append(x => x.StatsDirectReferralsPointsEarned)
                   .Append(x => x.TotalDirectReferralsPointsEarned)
                   .Append(x => x.StatsDirectReferralsEarned)
                   .Append(x => x.TotalDirectReferralsEarned)
                   .Append(x => x.StatsPointsEarned)
                   .Append(x => x.TotalPointsEarned)
                   .Append(x => x.StatsDRAdPacksEarned)
                   .Append(x => x.TotalDREarnedFromAdPacks)
                   .Append(x => x.StatsCashLinksEarned)
                   .Append(x => x.TotalEarnedFromCashLinks)
                   .Append(x => x.StatsDRCashLinksEarned)
                   .Append(x => x.TotalEarnedFromDRCashLinks)
                   .Append(x => x.TotalDRPTCClicks)
                   .Append(x => x.LastDRActivity)
                   .Append(x => x.ReferralSince)
                   .Append(x => x.TotalPTCClicksToDReferer)
                   .Append(x => x.TotalEarnedToDReferer)
                   .Append(x => x.TotalPointsEarnedToDReferer)
                   .Append(x => x.TotalAdPacksToDReferer)
                   .Append(x => x.TotalCashLinksToDReferer)
                   .Append(x => x.PtcSurfClicksThisMonth)
                   .Append(x => x.PtcAutoSurfClicksThisMonth)
                   .Append(x => x.TotalERC20TokensEarnedToDReferer)
                   .Append(x => x.StatsRevShareCurrentMonthIncome)
                   .Append(x => x.StatsRevShareCurrentWeekIncome)
                   .Append(x => x.StatsRevShareLastMonthIncome)
                   .Append(x => x.StatsRevShareLastWeekIncome)
                   .Append(x => x.StatsCommissionsCurrentMonthIncome)
                   .Append(x => x.StatsCommissionsCurrentWeekIncome)
                   .Append(x => x.StatsCommissionsLastMonthIncome)
                   .Append(x => x.StatsCommissionsLastWeekIncome)
                   ;

            return builder.Build();
        }

        public void SaveStatisticsAndBalances()
        {
            PropertyInfo[] balancePropertiesToSave = buildStatistics();
            PropertyInfo[] balancePropertiesToSave2 = buildBalances();

            var finalPropertiesToSave = balancePropertiesToSave.Union(balancePropertiesToSave2).ToArray();
            //
            SavePartially(IsUpToDate, finalPropertiesToSave);
        }

        #endregion
    }
}