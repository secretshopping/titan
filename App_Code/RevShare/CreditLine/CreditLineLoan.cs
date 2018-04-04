using Prem.PTC;
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
public class CreditLineLoan : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "CreditLineLoans"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Loaned")]
    public Money Loaned { get { return _Loaned; } set { _Loaned = value; SetUpToDateAsFalse(); } }

    [Column("Repaid")]
    public Money Repaid { get { return _Repaid; } set { _Repaid = value; SetUpToDateAsFalse(); } }

    [Column("FirstDeadline")]
    public DateTime FirstDeadline { get { return _FirstDeadline; }  set { _FirstDeadline = value; SetUpToDateAsFalse(); } }

    [Column("SecondDeadline")]
    public DateTime SecondDeadline { get { return _SecondDeadline; } set { _SecondDeadline = value; SetUpToDateAsFalse(); } }

     [Column("FinalDeadline")]
    public DateTime FinalDeadline { get { return _FinalDeadline; }  set { _FinalDeadline = value; SetUpToDateAsFalse(); } }

    [Column("AmounBeforeFirstDeadline")]
    public Money AmounBeforeFirstDeadline { get { return _AmounBeforeFirstDeadline; } set { _AmounBeforeFirstDeadline = value; SetUpToDateAsFalse(); } }

    [Column("AmounBeforeSecondDeadline")]
    public Money AmounBeforeSecondDeadline { get { return _AmounBeforeSecondDeadline; } set { _AmounBeforeSecondDeadline = value; SetUpToDateAsFalse(); } }

    [Column("BorrowDate")]
    public DateTime BorrowDate { get { return _BorrowDate; } set { _BorrowDate = value; SetUpToDateAsFalse(); } }

    #endregion

    private int _id;
    private int _UserId;
    private Money _Loaned, _Repaid, _AmounBeforeFirstDeadline, _AmounBeforeSecondDeadline;
    private DateTime _FirstDeadline, _SecondDeadline, _FinalDeadline, _BorrowDate;


    public CreditLineLoan()
            : base() { }

    public CreditLineLoan(int id) : base(id) { }

    public CreditLineLoan(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
}