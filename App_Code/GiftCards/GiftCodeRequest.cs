using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Prem.PTC.Advertising;
using System.Web;
using System.Web.UI;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Statistics;

public class GiftCodeRequest : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "GiftCodeRequests"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("GiftCodeId")]
    public int GiftCodeId { get { return _GiftCodeId; } set { _GiftCodeId = value; SetUpToDateAsFalse(); } }

    [Column("PointsPaid")]
    public int PointsPaid { get { return _PointsPaid; } set { _PointsPaid = value; SetUpToDateAsFalse(); } }

    [Column("DateRequested")]
    public DateTime DateRequested { get { return _DateRequested; } set { _DateRequested = value; SetUpToDateAsFalse(); } }

    [Column("StatusInt")]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

    [Column("DateSent")]
    public DateTime DateSent { get { return _DateSent; } set { _DateSent = value; SetUpToDateAsFalse(); } }

    [Column("CodeSent")]
    public string CodeSent { get { return _CodeSent; } set { _CodeSent = value; SetUpToDateAsFalse(); } }


    private int _id, _GiftCodeId, _UserId, _PointsPaid, _StatusInt;
    private DateTime _DateRequested, _DateSent;
    private string _CodeSent;

    public GiftCode GiftCode
    {
        get
        {
            return new GiftCode(GiftCodeId);
        }
        set
        {
            GiftCodeId = value.Id;
        }
    }

    public Member User
    {
        get
        {
            return new Member(UserId);
        }
        set
        {
            UserId = value.Id;
        }
    }

    public GiftCodeRequestStatus Status
    {
        get
        {
            return (GiftCodeRequestStatus)StatusInt;
        }
        set
        {
            StatusInt = (int)value;
        }
    }

    public GiftCodeRequest()
        : base() { }

    public GiftCodeRequest(int id) : base(id) { }

    public GiftCodeRequest(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

    #endregion Columns

    /// <summary>
    /// Rejects the request. Points are returned to member account. Request is being deleted.
    /// </summary>
    public void Reject()
    {
        Member Target = this.User;

        //Return money
        Target.AddToPointsBalance(this.PointsPaid, "Gift Code rejected");
        Target.TotalPointsExchanged -= this.PointsPaid;
        Target.SaveBalances();

        //Points generated stats
        ApplyToStats(-this.PointsPaid);

        //Reject
        this.Status = GiftCodeRequestStatus.Rejected;
        this.Save();
    }

    public void AcceptAndSendEmail(string code)
    {
        Member Target = this.User;

        Mailer.SendGiftCodeEmail(Target.Email, Target.Name, code, this.GiftCode.Value, this.GiftCode.GiftCard.Title);

        this.Status = GiftCodeRequestStatus.Completed;
        this.DateSent = DateTime.Now;
        this.CodeSent = code;
        this.Save();
    }

    /// <summary>
    /// Adds the request, takes the Points too
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="giftCodeId"></param>
    /// <param name="pointsPaid"></param>
    public static void Add(Member user, int giftCodeId, int pointsPaid)
    {
        //Take points
        user.SubtractFromPointsBalance(pointsPaid, "Gift Code");
        user.TotalPointsExchanged += pointsPaid;
        user.Save();

        //Add
        GiftCodeRequest Target = new GiftCodeRequest();
        Target.UserId = user.Id;
        Target.GiftCodeId = giftCodeId;
        Target.DateRequested = DateTime.Now;
        Target.Status = GiftCodeRequestStatus.Pending;
        Target.DateSent = DateTime.Now;
        Target.CodeSent = "";
        Target.PointsPaid = pointsPaid;
        Target.Save();

        //Points generated stats
        ApplyToStats(pointsPaid);
    }

    private static void ApplyToStats(int points)
    {
        AppSettings.Points.Reload();

        //Points generated stats
        AppSettings.Points.TotalExchanged += points;
        AppSettings.Points.Save();
        StatisticsManager.Add(StatisticsType.PointsExchanged, points);
    }

}

public enum GiftCodeRequestStatus
{
    Pending = 0,
    Completed = 1,
    Rejected = 2
}
