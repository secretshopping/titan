using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

/// <summary>
/// Summary description for UserBets
/// </summary>
public class CreditLineRequest : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "CreditLineRequests"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("LoanRequested")]
    public Money LoanRequested { get { return _LoanRequested; } set { _LoanRequested = value; SetUpToDateAsFalse(); } }

    [Column("RequestDate")]
    public DateTime RequestDate { get { return _RequestDate; } set { _RequestDate = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

    public CreditLineRequestStatus Status
    {
        get { return (CreditLineRequestStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    #endregion

    private int _id;
    private int _UserId, _Status;
    private Money _LoanRequested;
    private DateTime _RequestDate;


    public CreditLineRequest()
            : base() { }

    public CreditLineRequest(int id) : base(id) { }

    public CreditLineRequest(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
}