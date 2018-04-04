using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SiteInvestment
/// </summary>
public class SiteInvestment : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "SiteInvestments"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {

        public const string Id = "Id";
        public const string UserId = "UserId";
        public const string Amount = "Amount";
        public const string Kelly = "Kelly";
        public const string KellyInt = "KellyInt";
        public const string OperationDate = "OperationDate";

    }

    [Column(Columns.UserId)]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Amount)]
    public Money Amount { get { return _Amount; } set { _Amount = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Kelly)]
    public decimal Kelly { get { return _Kelly; } set { _Kelly = value; SetUpToDateAsFalse(); } }

    [Column(Columns.KellyInt)]
    public int KellyInt { get { return _KellyInt; } set { _KellyInt = value; SetUpToDateAsFalse(); } }

    [Column(Columns.OperationDate)]
    public DateTime OperationDate { get { return _OperationDate; } set { _OperationDate = value; SetUpToDateAsFalse(); } }

    #endregion

    public Member User
    {
        get
        {
            if (_user == null)
                _user = new Member(UserId);
            return _user;
        }
        set
        {
            _user = value;
            UserId = value.Id;
        }
    }

    private Member _user;
    private Decimal _Kelly;
    Money _Amount;
    private int _id, _KellyInt;
    private DateTime _OperationDate;
    private int _UserId;


    public SiteInvestment()
            : base()
    { }

    public SiteInvestment(int id) : base(id) { }

    public SiteInvestment(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    { }

    public static List<SiteInvestment> GetAll(int userId)
    {
        List<SiteInvestment> userInvestments = TableHelper.SelectRows<SiteInvestment>(TableHelper.MakeDictionary("UserId", userId));
        return userInvestments;
    }
    


}