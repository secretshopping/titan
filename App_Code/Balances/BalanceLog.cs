using System;
using Prem.PTC;
using System.Data;
using Prem.PTC.Members;
using Titan.Cryptocurrencies;

public class BalanceLog : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "BalanceLogs"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("DateOccured")]
    public DateTime DateOccured { get { return date; } set { date = value; SetUpToDateAsFalse(); } }

    [Column("Note")]
    public string Note
    {
        get { return name2; }
        set
        {
            name2 = value;

            if (value != null)
            {
                if (value.Length > 199)
                    name2 = name2.Substring(0, 199);
                else
                    name2 = value;
            }
            SetUpToDateAsFalse();
        }
    }

    [Column("UserId")]
    public int UserId { get { return name3; } set { name3 = value; SetUpToDateAsFalse(); } }

    [Column("Amount")]
    public decimal Amount { get { return amount; } set { amount = value; SetUpToDateAsFalse(); } }

    [Column("Balance")]
    private int BalanceInt { get { return balance; } set { balance = value; SetUpToDateAsFalse(); } }

    [Column("AccountState")]
    public Money AccountState { get { return _state; } set { _state = value; SetUpToDateAsFalse(); } }

    [Column("BalanceLogType")]
    protected int IntBalanceLogType { get { return _IntBalanceLogType; } set { _IntBalanceLogType = value; SetUpToDateAsFalse(); } }

    public BalanceLogType BalanceLogType
    {
        get
        {
            return (BalanceLogType)IntBalanceLogType;
        }

        set
        {
            IntBalanceLogType = (int)value;
        }
    }

    public BalanceType Type
    {
        get
        {
            return (BalanceType)BalanceInt;
        }

        set
        {
            BalanceInt = (int)value;
        }
    }

    private int _id, name3, balance, _IntBalanceLogType;
    private string name, name1, name2;
    private DateTime date;
    private Money _state;
    private Decimal bitCoinAmount, _btcState, amount;

    #endregion Columns

    public BalanceLog() : base() { }
    public BalanceLog(int id) : base(id) { }
    public BalanceLog(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate)
    { }


    public static void Add(Member User, BalanceType account, Money Before, Money After, string Note, BalanceLogType balanceLogType)
    {
        Add(User, account, (After - Before).ToDecimal(), Note, balanceLogType);
    }

    public static void Add(Member User, BalanceType account, Int32 Before, Int32 After, string Note, BalanceLogType balanceLogType)
    {
        Add(User, account, After - Before, Note, balanceLogType);
    }

    public static void Add(Member User, BalanceType account, decimal difference, string Note, BalanceLogType balanceLogType)
    {
        if (difference != 0)
        {
            BalanceLog Entry = new BalanceLog();
            Entry.DateOccured = AppSettings.ServerTime;
            Entry.UserId = User.Id;
            Entry.Type = account;
            Entry.Amount = difference;
            Entry.Note = Note;
            switch (account)
            {
                case BalanceType.MainBalance:
                    Entry.AccountState = User.MainBalance;
                    break;
                case BalanceType.PurchaseBalance:
                    Entry.AccountState = User.PurchaseBalance;
                    break;
                case BalanceType.TrafficBalance:
                    Entry.AccountState = User.TrafficBalance;
                    break;
                case BalanceType.CashBalance:
                    Entry.AccountState = User.CashBalance;
                    break;
                case BalanceType.CommissionBalance:
                    Entry.AccountState = User.CommissionBalance;
                    break;
                case BalanceType.InvestmentBalance:
                    Entry.AccountState = User.InvestmentBalance;
                    break;
                case BalanceType.BTC:
                    Entry.AccountState = User.GetCryptocurrencyBalance(CryptocurrencyType.BTC);
                    break;
                case BalanceType.XRP:
                    Entry.AccountState = User.GetCryptocurrencyBalance(CryptocurrencyType.XRP);
                    break;
                case BalanceType.ETH:
                    Entry.AccountState = User.GetCryptocurrencyBalance(CryptocurrencyType.ETH);
                    break;
                case BalanceType.Token:
                    Entry.AccountState = User.GetCryptocurrencyBalance(CryptocurrencyType.ERC20Token);
                    break;
                case BalanceType.FreezedToken:
                    Entry.AccountState = User.GetCryptocurrencyBalance(CryptocurrencyType.ERCFreezed);
                    break;
            }
            Entry.BalanceLogType = balanceLogType;
            Entry.Save();
        }
    }

    public static void Add(Member User, BalanceType account, Int32 difference, string Note, BalanceLogType balanceLogType)
    {
        if (difference != 0) //Not logging 0
        {
            BalanceLog Entry = new BalanceLog();
            Entry.DateOccured = AppSettings.ServerTime;
            Entry.UserId = User.Id;
            Entry.Type = account;
            Entry.Amount = difference;
            switch (account)
            {
                case BalanceType.PointsBalance:
                    Entry.AccountState = new Money(User.PointsBalance);
                    break;
                case BalanceType.LoginAdsCredits:
                    Entry.AccountState = new Money(User.LoginAdsCredits);
                    break;
            }                    
            Entry.Note = Note;
            Entry.BalanceLogType = balanceLogType;
            Entry.Save();
        }
    }

    public static void AddPtcCreditLog(Member user, Decimal difference, string note)
    {
        if (difference != 0) //Not logging 0
        {
            BalanceLog Entry = new BalanceLog();
            Entry.DateOccured = AppSettings.ServerTime;
            Entry.UserId = user.Id;
            Entry.Type = BalanceType.PTCCredits;
            Entry.Amount = difference;
            Entry.AccountState = new Money(user.PTCCredits);
            Entry.Note = note;
            Entry.BalanceLogType = BalanceLogType.Other;
            Entry.Save();
        }
    }
}

public enum BalanceLogType
{
    CashLinkPurchase = 1, // Note = Cash Link
    CashLinkClick = 2, // Note = CashLink
    CashLinkRefClick = 3, // Note = CashLink /ref/ + User.Name
    AdPackPurchase = 4, //Note = AppSettings.RevShare.AdPack.AdPackName + purchase
    AdPackRefPurchase = 5, // Note =  AdPack /ref/  + User.Name
    AdPackROI = 6, // Note = AppSettings.RevShare.AdPack.AdPackName +  revenue  ADSDIVISION Note = AdPack
    LeadershipReward = 7, // Note = Leadership Reward
    MarketplacePurchase = 8, // Note = Marketplace Purchase
    MarketplaceSale = 9, // Note = Marketplace Sale
    CreditLine = 10, // Note = Credit Line
    DirectRefPurchase = 11, //Note = Direct ref purchase
    UniqueAdClick = 12, //Note = Unique Ad Click
    UniqueAdRefClick = 13, //Note = Unique Ad /ref/ + User.Name
    BannerRefPurchase = 14, // Note = Banner purchase /ref/
    TrafficGridRefPurchase = 15, // Note =  TrafficGrid purchase /ref/  + User.Name
    GPTSearch = 16, // Note =  GPT Search
    GPTVideo = 17, // Note =  GPT Video
    ExternalBannerClick = 18, // External Banner click
    ExternalCpaOfferSubmission = 19, // External Cpa Offer Submission
    CashBalanceDepositComission = 20,
    InvestmentPlatformPlanPurchase = 21,
    InvestmentPlatformRefPlanPurchase = 22,
    RepresentativeDeposit = 23,
    RepresentativeWithdrawal = 24,
    InvestmentPlatformPlanCrediting = 25,
    InvestmentPlatformWithdrawal = 26,
    CryptocurrencyTrade = 27,
    AccountActivationFee = 28,
    PvpJackpotWin = 29,
    PvpJackpotBuy = 30,
    RegistrationBonus = 31,
    WalletDeposit = 32,
    CoinPurchase = 33,
    PurchaseTransfer = 34,
    ReversedPurchaseTransfer = 35,
    
    ArticleWriting = 36,
    ArticleSharing = 37,

    CaptchaClaim = 38,

    Other = 100
}

