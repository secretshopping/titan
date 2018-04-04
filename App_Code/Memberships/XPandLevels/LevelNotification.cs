using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Resources;
using System.Text;
/// <summary>
/// Summary description for UniqueAdClick
/// </summary>
public class LevelNotification : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "LevelNotifications"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("IsDisplayed")]
    public bool IsDisplayed { get { return _IsDisplayed; } set { _IsDisplayed = value; SetUpToDateAsFalse(); } }

    [Column("IsUpgrade")]
    public bool IsUpgrade { get { return _IsUpgrade; } set { _IsUpgrade = value; SetUpToDateAsFalse(); } }

    [Column("PTCCreditsReward")]
    public decimal? PTCCreditsReward { get { return _PTCCreditsReward; } set { _PTCCreditsReward = value; SetUpToDateAsFalse(); } }

    [Column("DRLimitReward")]
    public int? DRLimitReward { get { return _DRLimitReward; } set { _DRLimitReward = value; SetUpToDateAsFalse(); } }

    [Column("PointsReward")]
    public int? PointsReward { get { return _PointsReward; } set { _PointsReward = value; SetUpToDateAsFalse(); } }

    [Column("MembershipName")]
    public string MembershipName { get { return _MembershipName; } set { _MembershipName = value; SetUpToDateAsFalse(); } }


    int _Id, _UserId;
    int? _DRLimitReward, _PointsReward;
    bool _IsDisplayed, _IsUpgrade;
    decimal? _PTCCreditsReward;
    string _MembershipName;

    #endregion

    #region Constructors

    public LevelNotification()
            : base()
    {

    }
    public LevelNotification(int id)
            : base(id)
    {

    }
    public LevelNotification(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    {

    }
    #endregion

    public static List<LevelNotification> Get(int userId, bool isDisplayed)
    {
        return TableHelper.GetListFromRawQuery<LevelNotification>(string.Format("SELECT * FROM LevelNotifications WHERE UserId = {0} AND IsDisplayed = '{1}'", userId, isDisplayed));
    }

    public bool HasRewards
    {
        get
        {
            return PTCCreditsReward.HasValue || PointsReward.HasValue || DRLimitReward.HasValue;
        }
    }

    public static void TryDisplay(LevelNotification notification)
    {
        if (notification == null || notification.IsDisplayed)
            return;

        string title = U5006.GRATZ;
        string membershipName = notification.MembershipName;

        StringBuilder sb = new StringBuilder();
        sb.Append("<img src=\"Images/Misc/newlevel.png\"/><br/>");

        if (notification.IsUpgrade)
        {
            sb.Append(U5006.REACHED);
            sb.AppendFormat(" <b>{0}</b>.", membershipName);
            if (notification.HasRewards)
            {
                sb.Append("<br>" + U5008.REWARDS + ":");
                if (notification.PTCCreditsReward.HasValue)
                    sb.Append("<br>" + notification.PTCCreditsReward.Value + " " + U5006.ADCREDITS);
                if (notification.PointsReward.HasValue)
                    sb.Append("<br>" + notification.PointsReward.Value + " " + AppSettings.PointsName);
                if (notification.DRLimitReward.HasValue)
                    sb.Append("<br>" + U5008.DRLIMITENLARGEDBY + " " + notification.DRLimitReward.Value);
            }
        }
        else
        {
            title = U5008.SORRYDOWNGRADE;
            sb.Append(U5008.SORRYDOWNGRADETO);
            sb.AppendFormat(" <b>{0}</b>.", membershipName);
        }
        notification.IsDisplayed = true;
        notification.Save();

        NotificationUtils.DisplayCenteredNotification(title, sb.ToString());
    }
}