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
public class UserBet : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "UserBets"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {
        
        public const string Id = "Id";
        public const string UserId = "UserId";
        public const string BetSize = "BetSize";
        public const string Profit = "Profit";
        public const string Low = "Low";
        public const string Chance = "Chance";
        public const string Roll = "Roll";
        public const string BetDate = "BetDate";
        
    }


    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UserId)]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.BetSize)]
    public Money BetSize { get { return _BetSize; }  set { _BetSize = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Profit)]
    public Money Profit { get { return _Profit; }  set { _Profit = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Low)]
    public bool Low { get { return _Low; }  set { _Low = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Chance)]
    public Decimal Chance { get { return _Chance; }  set { _Chance = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Roll)]
    public Decimal Roll { get { return _Roll; } set { _Roll = value; SetUpToDateAsFalse(); } }

    [Column(Columns.BetDate)]
    public DateTime BetDate { get { return _BetDate; }  set { _BetDate = value; SetUpToDateAsFalse(); } }

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
    private Decimal _Chance, _Roll;
    private int _id;
    private bool _Low;
    private DateTime _BetDate;
    private int _UserId;
    Money _Profit, _BetSize;

    public UserBet()
            : base() { }

    public UserBet(int id) : base(id) { }

    public UserBet(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
}