using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Titan.Cryptocurrencies;

public class BlockioIPN : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "BlockioIPNs"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {
        public const string Id = "Id";
        public const string OperationDate = "OperationDate";
        public const string OperationType = "OperationType";
        public const string Confirmations = "Confirmations";
        public const string UserId = "UserId";
        public const string TargetAddress = "TargetAddress";
        public const string BitcoinAmount = "BitcoinAmount";
        public const string MoneyAmount = "MoneyAmount";
        public const string IsExecuted = "IsExecuted";
        public const string BitcoinAPIProvider = "BitcoinAPIProvider";
    }


    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.OperationDate)]
    public DateTime OperationDate { get { return _OperationDate; } set { _OperationDate = value; SetUpToDateAsFalse(); } }

    [Column(Columns.OperationType)]
    public int OperationTypeInt { get { return _OperationType; } set { _OperationType = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Confirmations)]
    public int Confirmations { get { return _Confirmations; } set { _Confirmations = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UserId)]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.TargetAddress)]
    public string TargetAddress { get { return _TargetAddress; } set { _TargetAddress = value; SetUpToDateAsFalse(); } }

    [Column(Columns.BitcoinAmount)]
    public decimal BitcoinAmount { get { return _BitcoinAmount; } set { _BitcoinAmount = value; SetUpToDateAsFalse(); } }

    [Column(Columns.MoneyAmount)]
    public Money MoneyAmount { get { return _MoneyAmount; } set { _MoneyAmount = value; SetUpToDateAsFalse(); } }

    [Column(Columns.IsExecuted)]
    public bool IsExecuted { get { return _IsExecuted; } set { _IsExecuted = value; SetUpToDateAsFalse(); } }

    [Column(Columns.BitcoinAPIProvider)]
    public int BitcoinAPIProvider { get { return _BitcoinAPIProvider; } set { _BitcoinAPIProvider = value; SetUpToDateAsFalse(); } }

    #endregion
    private int _id, _UserId, _OperationType, _Confirmations, _BitcoinAPIProvider;
    private string _TargetAddress;
    private decimal _BitcoinAmount;
    private Money _MoneyAmount;
    private DateTime _OperationDate;
    private bool _IsExecuted;

    public OperationType OperationType
    {
        get { return (OperationType)OperationTypeInt; }
        set { OperationTypeInt = (int)value; }
    }


    public BlockioIPN()
                : base()
    { }

    public BlockioIPN(int id) : base(id) { }

    public BlockioIPN(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate)
    { }

    public static decimal GetTotalBTCTransactionsAmount(OperationType operationType)
    {
        String Query = String.Format("SELECT SUM(BitcoinAmount) FROM BlockioIPNs WHERE OperationType={0} AND IsExecuted=1", (int)operationType);
        try
        {
            return Convert.ToDecimal(TableHelper.SelectScalar(Query));
        }
        catch
        {
            return decimal.Zero;
        }
    }
}
