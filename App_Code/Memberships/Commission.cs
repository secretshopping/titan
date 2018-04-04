using Prem.PTC;
using Prem.PTC.Memberships;
using System.Data;
using System.Linq;

public class Commission : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "Commissions"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("RefLevel")]
    public int RefLevel { get { return _RefLevel; } set { _RefLevel = value; SetUpToDateAsFalse(); } }

    [Column("MembershipId")]
    public int MembershipId { get { return _MembershipId; } set { _MembershipId = value; SetUpToDateAsFalse(); } }

    [Column("MembershipPurchasePercent")]
    public decimal MembershipPurchasePercent { get { return _MembershipPurchasePercent; } set { _MembershipPurchasePercent = value; SetUpToDateAsFalse(); } }

    [Column("BannerPurchasePercent")]
    public decimal BannerPurchasePercent { get { return _BannerPurchasePercent; } set { _BannerPurchasePercent = value; SetUpToDateAsFalse(); } }

    [Column("AdPackPurchasePercent")]
    public decimal AdPackPurchasePercent { get { return _AdPackPurchasePercent; } set { _AdPackPurchasePercent = value; SetUpToDateAsFalse(); } }

    [Column("PointsForAdPackPurchasePercent")]
    public decimal PointsForAdPackPurchasePercent { get { return _PointsForAdPackPurchasePercent; } set { _PointsForAdPackPurchasePercent = value; SetUpToDateAsFalse(); } }

    [Column("OfferwallPercent")]
    public decimal OfferwallPercent { get { return _OfferwallPercent; } set { _OfferwallPercent = value; SetUpToDateAsFalse(); } }

    [Column("CPAOfferPercent")]
    public decimal CPAOfferPercent { get { return _CPAOfferPercent; } set { _CPAOfferPercent = value; SetUpToDateAsFalse(); } }

    [Column("TrafficGridPurchasePercent")]
    public decimal TrafficGridPurchasePercent { get { return _TrafficGridPurchasePercent; } set { _TrafficGridPurchasePercent = value; SetUpToDateAsFalse(); } }

    [Column("VideoViewPercent")]
    public decimal VideoViewPercent { get { return _VideoViewPercent; } set { _VideoViewPercent = value; SetUpToDateAsFalse(); } }

    [Column("CashBalanceDepositPercent")]
    public decimal CashBalanceDepositPercent { get { return _CashBalanceDepositPercent; } set { _CashBalanceDepositPercent = value; SetUpToDateAsFalse(); } }

    [Column("InvestmentPlanPurchasePercent")]
    public decimal InvestmentPlanPurchasePercent { get { return _InvestmentPlanPurchasePercent; } set { _InvestmentPlanPurchasePercent = value; SetUpToDateAsFalse(); } }

    [Column("AccountActivationFeePercent")]
    public decimal AccountActivationFeePercent { get { return _AccountActivationFeePercent; } set { _AccountActivationFeePercent = value; SetUpToDateAsFalse(); } }

    [Column("JackpotPvpStageBuyFeePercent")]
    public decimal JackpotPvpStageBuyFeePercent { get { return _JackpotPvpStageBuyFeePercent; } set { _JackpotPvpStageBuyFeePercent = value; SetUpToDateAsFalse(); } }

    #endregion

    private decimal _MembershipPurchasePercent, _BannerPurchasePercent, _AdPackPurchasePercent, _InvestmentPlanPurchasePercent, _OfferwallPercent,
         _CPAOfferPercent, _TrafficGridPurchasePercent, _VideoViewPercent, _PointsForAdPackPurchasePercent, _CashBalanceDepositPercent, _AccountActivationFeePercent,
        _JackpotPvpStageBuyFeePercent;
    private int _Id, _RefLevel, _MembershipId;


    public Commission()
            : base()
    { }

    public Commission(int id) : base(id) { }

    public Commission(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    { }

    /// <summary>
    /// Should be fired each time user adds a Membership or changes max supported tiers.
    /// </summary>
    public static void UpdateCommissions()
    {
        var memberships = Membership.GetAll();
        var levels = AppSettings.Referrals.ReferralEarningsUpToTier;
        foreach (var m in memberships)
        {
            for (int i = 1; i <= levels; i++)
            {
                var commission = TableHelper.GetListFromRawQuery<Commission>(
                    string.Format("SELECT * FROM Commissions WHERE MembershipId = {0} AND RefLevel = {1}"
                    , m.Id, i)).FirstOrDefault();

                if (commission == null)
                {
                    commission = new Commission()
                    {
                        RefLevel = i,
                        MembershipId = m.Id,
                        MembershipPurchasePercent = 0,
                        BannerPurchasePercent = 0,
                        AdPackPurchasePercent = 0,
                        PointsForAdPackPurchasePercent = 0,
                        OfferwallPercent = 0,
                        CPAOfferPercent = 0,
                        TrafficGridPurchasePercent = 0,
                        VideoViewPercent = 0,
                        CashBalanceDepositPercent = 0,
                        InvestmentPlanPurchasePercent = 0,
                        AccountActivationFeePercent = 0,
                        JackpotPvpStageBuyFeePercent = 0
                    };
                    commission.Save();
                }
            }
        }
    }

    public static void UpdateCommissions(Membership membership)
    {
        var levels = AppSettings.Referrals.ReferralEarningsUpToTier;
        for (int i = 1; i <= levels; i++)
        {
            var commission = TableHelper.GetListFromRawQuery<Commission>(
                string.Format("SELECT * FROM Commissions WHERE MembershipId = {0} AND RefLevel = {1}"
                , membership.Id, i)).FirstOrDefault();

            if (commission == null)
            {
                commission = new Commission()
                {
                    RefLevel = i,
                    MembershipId = membership.Id,
                    MembershipPurchasePercent = 0,
                    BannerPurchasePercent = 0,
                    AdPackPurchasePercent = 0,
                    PointsForAdPackPurchasePercent = 0,
                    OfferwallPercent = 0,
                    CPAOfferPercent = 0,
                    TrafficGridPurchasePercent = 0,
                    VideoViewPercent = 0,
                    CashBalanceDepositPercent = 0,
                    InvestmentPlanPurchasePercent = 0,
                    AccountActivationFeePercent = 0,
                    JackpotPvpStageBuyFeePercent = 0
                };
                commission.Save();
            }
        }
    }

    /// <summary>
    /// Only for update 5009
    /// </summary>
    public static void UpdateCommissionSystem()
    {
        var memberships = Membership.GetAll();
        var levels = AppSettings.Referrals.ReferralEarningsUpToTier;

        foreach (var m in memberships)
        {
            for (int i = 1; i <= levels; i++)
            {
                decimal tierEarnings = 1;
                if (i != 1)
                {
                    var intEarnings = (int)TableHelper.SelectScalar(string.Format("SELECT ReferralEarningsFromTier{0} FROM ApplicationSettings", i));
                    tierEarnings = ((int)TableHelper.SelectScalar(string.Format("SELECT ReferralEarningsFromTier{0} FROM ApplicationSettings", i))) / 100m;
                }

                var commission = TableHelper.GetListFromRawQuery<Commission>(
                    string.Format("SELECT * FROM Commissions WHERE MembershipId = {0} AND RefLevel = {1}"
                    , m.Id, i)).FirstOrDefault();

                if (commission == null)
                {
                    commission = new Commission()
                    {
                        RefLevel = i,
                        MembershipId = m.Id,
                        MembershipPurchasePercent = m.DirectReferralMembershipPurchaseEarnings * tierEarnings,
                        BannerPurchasePercent = m.DirectReferralBannerPurchaseEarnings * tierEarnings,
                        AdPackPurchasePercent = m.DirectReferralAdPackPurchaseEarnings * tierEarnings,
                        OfferwallPercent = m.RefPercentEarningsOfferwalls * tierEarnings,
                        CPAOfferPercent = m.RefPercentEarningsCPA * tierEarnings,
                        TrafficGridPurchasePercent = m.DirectReferralTrafficGridPurchaseEarnings * tierEarnings,
                        PointsForAdPackPurchasePercent = 0,
                        AccountActivationFeePercent = 0,
                        JackpotPvpStageBuyFeePercent = 0
                    };
                    commission.Save();
                }
            }
        }

    }
}