using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Payments;
using Titan;
using Prem.PTC.Members;
using System.Linq;
using Titan.Cryptocurrencies;

public class CompletedPaymentLog : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "CompletedTransactions"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("PaymentProcessor")]
    protected int IntPaymentProcessor { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

    [Column("IsGuest")]
    public bool IsGuest { get { return points; } set { points = value; SetUpToDateAsFalse(); } }

    [Column("WhenMade")]
    public DateTime When { get { return date; } set { date = value; SetUpToDateAsFalse(); } }

    [Column("TransactionId")]
    public string TransactionId { get { return name; } set { name = parseLength(value, 100); SetUpToDateAsFalse(); } }

    [Obsolete]
    [Column("Username")]
    public string Username { get { return name1; } set { name1 = parseLength(value, 100); SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("PaidFor")]
    public string PaidFor { get { return name3; } set { name3 = parseLength(value, 100); SetUpToDateAsFalse(); } }

    [Column("Amount")]
    public Money Amount { get { return e22; } set { e22 = value; SetUpToDateAsFalse(); } }

    [Column("Fees")]
    public Money Fees { get { return _Fees; } set { _Fees = value; SetUpToDateAsFalse(); } }

    [Column("Successful")]
    public bool Successful { get { return _Successful; } set { _Successful = value; SetUpToDateAsFalse(); } }

    [Column("CryptoCurrencyInfo")]
    public string CryptoCurrencyInfo { get { return _cryptoCurrencyInfo; } set { _cryptoCurrencyInfo = value; SetUpToDateAsFalse(); } }

    private int _id, cpoints, type, _UserId;
    private string name, name1, name2, name3, name4, e1, e2, _cryptoCurrencyInfo;
    private DateTime date;
    private Money e22, _Fees;
    private bool points, _Successful;

    private string parseLength(string Input, int Allowed)
    {
        if (Input.Length > Allowed)
            return Input.Substring(0, Allowed - 1);
        return Input;
    }

    #endregion Columns

    public CompletedPaymentLog()
        : base() { }

    public CompletedPaymentLog(int id) : base(id) { }

    public CompletedPaymentLog(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }


    public PaymentProcessor PaymentProcessor
    {
        get
        {
            return (PaymentProcessor)IntPaymentProcessor;
        }
        set
        {
            IntPaymentProcessor = (int)value;
        }
    }

    public static CompletedPaymentLog Create(PaymentProcessor PP, string PaidFor, string TransactionId, bool IsGuest, string Username,
        Money Amount, Money fees, bool successful, string cryptoCurrencyInfo = "")
    {
        CompletedPaymentLog ol = new CompletedPaymentLog();
        ol.When = DateTime.Now;
        ol.PaymentProcessor = PP;

        if (IsGuest)
        {
            ol.UserId = -1;
            ol.Username = "-";
        }
        else
        {
            ol.UserId = new Member(Username).Id;
            ol.Username = Username;
        }

        ol.IsGuest = IsGuest;
        ol.PaidFor = PaidFor;
        ol.TransactionId = TransactionId;
        ol.Amount = Amount;
        ol.Fees = fees;
        ol.Successful = successful;
        ol.CryptoCurrencyInfo = cryptoCurrencyInfo;
        ol.Save();

        return ol;
    }

    public static CompletedPaymentLog GetTransactionIfExistsCreateOtherwise(PaymentProcessor paymentProcessor, string transactionId,
                                            string paidFor, string username, Money amount, Money fees, string cryptoCurrencyInfo)
    {
        var CompletedPaymentLog = PaymentHandler.GetTransaction(paymentProcessor, transactionId);

        if (CompletedPaymentLog == null)
            CompletedPaymentLog = CompletedPaymentLog.Create(paymentProcessor, paidFor,
                transactionId, false, username, amount, fees, false, cryptoCurrencyInfo);

        return CompletedPaymentLog;
    }

    public static CompletedPaymentLog GetPendingBTCDeposits(int userId)
    {
        var processors = CryptocurrencyApi.AllBTCProcessors.ToList().Select(item => ((int)item).ToString());

        var PendingDeposits = TableHelper.GetListFromRawQuery<CompletedPaymentLog>(String.Format(
            "SELECT * FROM CompletedTransactions WHERE Successful = 0 AND UserId = {0} AND PaymentProcessor IN ({1})",
            userId, string.Join(",", processors)));

        if (PendingDeposits.Count() == 0)
            return null;

        return PendingDeposits[0];
    }

    public static int GetTransactionsAmount()
    {
        return Convert.ToInt32(TableHelper.SelectScalar("SELECT COUNT(*) FROM CompletedTransactions WHERE Successful = 1").ToString());
    }

    public static Money GetTransactionsValue()
    {
        return Money.Parse(TableHelper.SelectScalar("SELECT SUM(Amount) FROM CompletedTransactions WHERE Successful = 1").ToString());
    }


}
