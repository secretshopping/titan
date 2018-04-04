using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using Titan.CryptocurrencyPlatform;
using ExtensionMethods;

public class CryptocurrencyTradeTransaction : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }

    protected override string dbTable { get { return "CryptocurrencyTradeTransactions"; } }

    public static new string TableName { get { return "CryptocurrencyTradeTransactions"; } }

    #region Constructors
    public CryptocurrencyTradeTransaction()
    : base()
    {
    }

    public CryptocurrencyTradeTransaction(int id)
        : base(id)
    {
    }

    public CryptocurrencyTradeTransaction(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate)
    {
    }
    #endregion

    #region Columns
    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("OfferId")]
    public int OfferId { get { return _OfferId; } set { _OfferId = value; SetUpToDateAsFalse(); } }

    [Column("ClientId")]
    public int ClientId { get { return _ClientId; } set { _ClientId = value; SetUpToDateAsFalse(); } }

    [Column("ExecutionTime")]
    public DateTime ExecutionTime { get { return _ExecutionTime; } set { _ExecutionTime = value; SetUpToDateAsFalse(); } }

    [Column("PaymentStatus")]
    private int IntPaymentStatus { get { return _IntPaymentStatus; } set { _IntPaymentStatus = value; SetUpToDateAsFalse(); } }
    public CryptocurrencyTransactionStatus PaymentStatus
    {
        get
        {
            return (CryptocurrencyTransactionStatus)IntPaymentStatus;
        }

        set
        {
            IntPaymentStatus = (int)value;
        }
    }

    [Column("CCAmount")]
    public CryptocurrencyMoney CCAmount { get { return _CCAmount; } set { _CCAmount = value; SetUpToDateAsFalse(); } }

    [Column("SellerDescription")]
    public String SellerDescription { get { return _SellerDescription; } set { _SellerDescription = value; SetUpToDateAsFalse(); } }

    private int _Id, _ClientId, _IntPaymentStatus, _OfferId;
    private DateTime _ExecutionTime;
    private CryptocurrencyMoney _CCAmount;
    private String _SellerDescription;
    #endregion

    #region Helpers
    public void Save(bool forceSave = false)
    {
        base.Save(forceSave);
    }

    public static String GetGridViewStringForUserActualTransactions(int ClientId, CryptocurrencyOfferType SiteType)
    {
        CryptocurrencyOfferType Type1 = (SiteType == CryptocurrencyOfferType.Buy) ? CryptocurrencyOfferType.Sell : CryptocurrencyOfferType.Buy;
        CryptocurrencyOfferType Type2 = (SiteType == CryptocurrencyOfferType.Buy) ? CryptocurrencyOfferType.Buy : CryptocurrencyOfferType.Sell;

        return String.Format(@"SELECT * FROM CryptoCurrencyTradeTransactions
                             WHERE 
                             ((ClientId={0} AND 
							 OfferId IN (
								SELECT Id 
								FROM CryptocurrencyTradeOffers
								WHERE OfferKind={1}))
                              OR
                             (OfferId IN(
                                SELECT Id 
                                FROM CryptocurrencyTradeOffers 
                                WHERE CreatorId={0}
                                AND OfferKind={2})))
							 AND PaymentStatus IN ({3},{4},{5},{6})",
                             ClientId,
                             (int)Type1,
                             (int)Type2,
                             (int)CryptocurrencyTransactionStatus.AwaitingPayment,
                             (int)CryptocurrencyTransactionStatus.AwaitingPaymentConfirmation,
                             (int)CryptocurrencyTransactionStatus.NotPaid,
                             (int)CryptocurrencyTransactionStatus.PaymentNotConfirmed);
        
    }

    public static String GetGridViewStringForUnfinishedTransactions()
    {
        return String.Format(@"SELECT CTT.Id, CTT.OfferId, CTT.ClientId, CTT.ExecutionTime, CTT.PaymentStatus, CTT.CCAmount, CTO.OfferKind, CTO.CreatorId, CTO.EscrowTime 
                               FROM CryptoCurrencyTradeTransactions CTT
                                LEFT JOIN CryptocurrencyTradeOffers CTO on CTO.Id = CTT.OfferId
                               WHERE CTT.PaymentStatus IN({0}, {1})",
                             (int)CryptocurrencyTransactionStatus.NotPaid,
                             (int)CryptocurrencyTransactionStatus.PaymentNotConfirmed);

    }

    public static List<CryptocurrencyTradeTransaction> GetAllActualTransactionsFittedToEscrow(int MinEscrowTime)
    {
        DateTime MinAllowedTime = AppSettings.ServerTime.AddMinutes(-MinEscrowTime);

        return TableHelper.GetListFromQuery<CryptocurrencyTradeTransaction>(String.Format("WHERE ExecutionTime<'{0}' AND PaymentStatus IN ({1},{2})",
                                                                                                MinAllowedTime.ToDBString(),
                                                                                                (int)CryptocurrencyTransactionStatus.AwaitingPayment,
                                                                                                (int)CryptocurrencyTransactionStatus.AwaitingPaymentConfirmation));
    }
    #endregion

    #region
    public static void CheckIfEscrowPassedForAllActiveTransactions()
    {
        CheckIfEscrowPassed(CryptocurrencyTransactionStatus.AwaitingPayment);
        CheckIfEscrowPassed(CryptocurrencyTransactionStatus.AwaitingPaymentConfirmation);
    }

    private static void CheckIfEscrowPassed(CryptocurrencyTransactionStatus statusBefore)
    {
        var statusAfter = statusBefore == CryptocurrencyTransactionStatus.AwaitingPayment ? CryptocurrencyTransactionStatus.NotPaid : CryptocurrencyTransactionStatus.PaymentNotConfirmed;

        String UpdateQuery = String.Format(@"UPDATE CryptoCurrencyTradeTransactions SET PaymentStatus = {0} WHERE PaymentStatus = {1} AND 
                                                DATEADD(MINUTE, (SELECT EscrowTime FROM CryptocurrencyTradeOffers WHERE Id = CryptoCurrencyTradeTransactions.OfferId), ExecutionTime) 
                                                    < '{2}'", (int)statusAfter, (int)statusBefore, AppSettings.ServerTime.ToDBString());

        TableHelper.ExecuteRawCommandNonQuery(UpdateQuery);
    }
    #endregion
}