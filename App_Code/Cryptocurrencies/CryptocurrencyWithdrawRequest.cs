using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using Titan.Cryptocurrencies;

public class CryptocurrencyWithdrawRequest : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "CryptocurrencyWithdrawRequests"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Address")]
    public string Address
    {
        get { return _Address; }
        set
        {
            if (CryptocurrencyCode == "BTC")
                BitcoinValidator.BitcoinValidator.ValidateBitcoinAddress(value);

            _Address = value;
            SetUpToDateAsFalse();
        }
    }

    [Column("RequestDate")]
    public DateTime RequestDate { get { return _RequestDate; } set { _RequestDate = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

    public WithdrawRequestStatus Status
    {
        get { return (WithdrawRequestStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    [Column("Amount")]
    public Money Amount { get { return _Amount; } set { _Amount = value; SetUpToDateAsFalse(); } }

    [Column("CryptocurrencyCode")]
    public string CryptocurrencyCode
    {
        get { return _CryptocurrencyCode; }
        set
        {
            _CryptocurrencyCode = value;
            SetUpToDateAsFalse();
        }
    }

    public CryptocurrencyType Cryptocurrency
    {
        get { return (CryptocurrencyType)Enum.Parse(typeof(CryptocurrencyType), CryptocurrencyCode); }
        set { CryptocurrencyCode = value.ToString(); }
    }

    [Column("WithdrawalSource")]
    protected int WithdrawalSourceInt { get { return _WithdrawalSourceInt; } set { _WithdrawalSourceInt = value; SetUpToDateAsFalse(); } }

    public WithdrawalSourceBalance WithdrawalSourceBalance
    {
        get { return (WithdrawalSourceBalance)WithdrawalSourceInt; }
        set { WithdrawalSourceInt = (int)value; }
    }

    [Column("AmountWithFee")]
    public Money AmountWithFee { get { return _AmountWithFee; } set { _AmountWithFee = value; SetUpToDateAsFalse(); } }

    public Cryptocurrency CryptocurrencyObject { get; set; }

    int _Id, _UserId, _StatusInt, _WithdrawalSourceInt;
    string _Address, _CryptocurrencyCode;
    DateTime _RequestDate;
    Money _Amount, _AmountWithFee;
    #endregion

    #region Constructors

    private CryptocurrencyWithdrawRequest() : base() { }
    public CryptocurrencyWithdrawRequest(int id) : base(id)
    {
        CryptocurrencyObject = CryptocurrencyFactory.Get(Cryptocurrency);
    }
    public CryptocurrencyWithdrawRequest(DataRow row, bool isUpToDate = true) : base(row, isUpToDate)
    {
        CryptocurrencyObject = CryptocurrencyFactory.Get(Cryptocurrency);
    }

    public CryptocurrencyWithdrawRequest(int userId, string address, Money amount, Money amountWithFee, CryptocurrencyType cryptocurrency, WithdrawalSourceBalance sourceBalance)
    {
        this.CryptocurrencyCode = cryptocurrency.ToString();
        this.UserId = userId;
        this.Address = address;
        this.RequestDate = AppSettings.ServerTime;
        this.Status = WithdrawRequestStatus.Pending;
        this.Amount = amount;
        this.AmountWithFee = amountWithFee;
        this.WithdrawalSourceBalance = sourceBalance;
        this.CryptocurrencyObject = CryptocurrencyFactory.Get(Cryptocurrency);
    }

    #endregion

    public static void ValidatePendingRequests(int userId, Money amount, Cryptocurrency cryptocurrency)
    {
        if (GetList(userId, WithdrawRequestStatus.Pending).Count > 0)
            throw new MsgException(U6000.WAITUNTILRESOLVED);

        CheckMaxValueOfPendingRequestsPerDay(amount, cryptocurrency);
    }

    public void Accept()
    {
        try
        {
            var user = new Member(UserId);

            CryptocurrencyObject.MakeWithdrawal(Amount, Address, user, WithdrawalSourceBalance);
            Status = WithdrawRequestStatus.Accepted;
        }
        catch (MsgException msg)
        {
            throw msg;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex.Message, ErrorLoggerHelper.GetTypeFromProcessor(CryptocurrencyObject.WithdrawalApiProcessor), true);
        }
        finally
        {
            Save();
        }
    }

    public void Reject()
    {
        this.Status = WithdrawRequestStatus.Rejected;
        this.Save();

        var user = new Member(UserId);

        if (WithdrawalSourceBalance == WithdrawalSourceBalance.MainBalance)
        {
            user.MoneyCashout -= AmountWithFee;
            user.AddToMainBalance(AmountWithFee, CryptocurrencyObject.Code + " withdrawal rejected");
        }
        else
        {
            user.MoneyCashout -= CryptocurrencyObject.ConvertToMoney(AmountWithFee.ToDecimal());
            user.AddToCryptocurrencyBalance(Cryptocurrency, AmountWithFee.ToDecimal(), CryptocurrencyObject.Code = " withdrawal rejected");
        }

        user.Save();
    }

    public void MarkAsPaid()
    {
        Status = WithdrawRequestStatus.Accepted;
        var user = new Member(UserId);

        if (WithdrawalSourceBalance == WithdrawalSourceBalance.MainBalance)
        {
            PaymentProportionsManager.MemberPaidOut(Amount, CryptocurrencyTypeHelper.ConvertToPaymentProcessor(Cryptocurrency), user);
            CryptocurrencyObject.AddPaymentProof(Amount, user);
        }
        else
            CryptocurrencyObject.AddPaymentProof(CryptocurrencyObject.ConvertToMoney(Amount.ToDecimal()), user);

        Save();
    }

    public void MarkAsPaidInBatchPayment()
    {
        MarkAsPaid();
        BitcoinIPNManager.AddIPNLog(AppSettings.ServerTime, OperationType.Withdrawal, null, UserId, Address, CryptocurrencyObject.ConvertFromMoney(Amount), Amount, CryptocurrencyObject.WithdrawalApiProcessor, true);
    }

    public static List<CryptocurrencyWithdrawRequest> GetList(int userId, WithdrawRequestStatus status)
    {
        var whereDict = TableHelper.MakeDictionary("UserId", userId);
        whereDict.Add("Status", (int)status);
        return TableHelper.SelectRows<CryptocurrencyWithdrawRequest>(whereDict);
    }

    public static string GetBitcoinRequestViewSQLConditions()
    {
        return String.Format(@"{0} ORDER BY RequestDate DESC", GetBitcoinRequestSQLConditions());
    }

    public static string GetBitcoinRequestSQLConditions()
    {
        return String.Format(@"WHERE Status = {0}", (int)WithdrawRequestStatus.Pending);
    }

    public static Money GetSumOfAllPendingPayoutRequests()
    {
        var result = TableHelper.SelectScalar(
            @"SELECT SUM(Amount) FROM CryptocurrencyWithdrawRequests " + GetBitcoinRequestSQLConditions());

        if (!(result is DBNull))
            return new Money((Decimal)result);

        return Money.Zero;
    }

    public static Money GetSumOfAllAcceptedWithdrawal()
    {
        var query = string.Format("SELECT SUM(Amount) FROM CryptocurrencyWithdrawRequests WHERE Status = {0}", (int)WithdrawRequestStatus.Accepted);
        var scalar = TableHelper.SelectScalar(query);
        var result = scalar.ToString() != string.Empty ? scalar : 0m;

        return new Money((decimal)result);
    }

    private static void CheckMaxValueOfPendingRequestsPerDay(Money amount, Cryptocurrency cryptocurrency)
    {
        if (cryptocurrency.MaxValueOfPendingRequests < new Money(2000000000))
        {
            var sum = TableHelper.SelectScalar(
            @"WITH 
                AllRequests AS
                (
                SELECT CAST(RequestDate AS DATE) AS DateDay FROM CryptocurrencyWithdrawRequests WHERE Status = 1 GROUP BY CAST(RequestDate AS DATE) 
                )
                SELECT SUM(Amount) FROM CryptocurrencyWithdrawRequests WHERE Status IN (1,2,4) AND CAST(RequestDate AS DATE) IN (SELECT DateDay FROM AllRequests)"
            );

            if (sum != null && !(sum is DBNull))
            {
                Money PendingRequestsValue = new Money((Decimal)sum);

                if (PendingRequestsValue + amount > cryptocurrency.MaxValueOfPendingRequests)
                    throw new MsgException(U6006.PROCESSORLIMIT);
            }
        }
    }
}

public enum WithdrawRequestStatus
{
    Pending = 1,
    Accepted = 2,
    Rejected = 3,
    Failed = 4 //OBSOLETE
}