using Prem.PTC;
using System;
using System.Data;
using Titan.CryptocurrencyPlatform;

public class CryptocurrencyTradeOffer : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }

    protected override string dbTable { get { return "CryptocurrencyTradeOffers"; } }

    #region Constructors
    public CryptocurrencyTradeOffer()
    : base()
    {
    }

    public CryptocurrencyTradeOffer(int id)
        : base(id)
    {
    }

    public CryptocurrencyTradeOffer(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate)
    {
    }
    #endregion

    #region Columns
    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("OfferKind")]
    private int IntOfferKind { get { return _IntOfferKind; } set { _IntOfferKind = value; SetUpToDateAsFalse(); } }

    public CryptocurrencyOfferType OfferKind
    {
        get
        {
            return (CryptocurrencyOfferType)IntOfferKind;
        }
        set
        {
            IntOfferKind = (int)value;
        }
    }

    [Column("Status")]
    private int IntStatus { get { return _IntStatus; } set { _IntStatus = value; SetUpToDateAsFalse(); } }

    public CryptocurrencyOfferStatus Status
    {
        get
        {
            return (CryptocurrencyOfferStatus)IntStatus;
        }
        set
        {
            IntStatus = (int)value;
        }
    }

    [Column("Amount")]
    public CryptocurrencyMoney Amount { get { return _Amount; } set { _Amount = value; SetUpToDateAsFalse(); } }

    [Column("AmountLeft")]
    public CryptocurrencyMoney AmountLeft { get { return _AmountLeft; } set { _AmountLeft = value; SetUpToDateAsFalse(); } }

    [Column("CreatorId")]
    public int CreatorId { get { return _CreatorId; } set { _CreatorId = value; SetUpToDateAsFalse(); } }

    [Column("DateAdded")]
    public DateTime DateAdded { get { return _DateAdded; } set { _DateAdded = value; SetUpToDateAsFalse(); } }

    [Column("DateFinished")]
    public DateTime DateFinished { get { return _DateFinished; } set { _DateFinished = value; SetUpToDateAsFalse(); } }

    [Column("MinPrice")]
    public Money MinPrice { get { return _MinPrice; } set { _MinPrice = value; SetUpToDateAsFalse(); } }

    [Column("MaxPrice")]
    public Money MaxPrice { get { return _MaxPrice; } set { _MaxPrice = value; SetUpToDateAsFalse(); } }

    [Column("MinOfferValue")]
    public Money MinOfferValue { get { return _MinOfferValue; } set { _MinOfferValue = value; SetUpToDateAsFalse(); } }

    [Column("MaxOfferValue")]
    public Money MaxOfferValue { get { return _MaxOfferValue; } set { _MaxOfferValue = value; SetUpToDateAsFalse(); } }

    [Column("EscrowTime")]
    public int EscrowTime { get { return _EscrowTime; } set { _EscrowTime = value; SetUpToDateAsFalse(); } }

    [Column("Description")]
    public String Description { get { return _Description; } set { _Description = parseLength(value, 500); SetUpToDateAsFalse(); } }

    private int      _Id, _IntOfferKind, _IntStatus, _CreatorId, _EscrowTime;
    private DateTime _DateAdded, _DateFinished;
    private Money   _MinPrice, _MaxPrice, _MinOfferValue, _MaxOfferValue;
    private String   _Description;
    private CryptocurrencyMoney _Amount, _AmountLeft;
    #endregion

    #region Helpers

    public void Save(bool forceSave = false)
    {
        base.Save(forceSave);
    }

    protected string parseLength(string Input, int Allowed)
    {
        if (Input == null)
            return Input;

        if (Input.Length > Allowed)
            return Input.Substring(0, Allowed - 1);
        return Input;
    }

    public void Activate()
    {
        Status = CryptocurrencyOfferStatus.Active;
        this.Save();
    }

    public void Pause()
    {
        Status = CryptocurrencyOfferStatus.Paused;
        this.Save();
    }

    public override void Delete()
    {
        Status = CryptocurrencyOfferStatus.Deleted;
        this.Save();
    }

    public static String GetGridViewStringForAllActiveOffersForUser(int UserId, CryptocurrencyOfferType OfferType)
    {
        return String.Format("SELECT * FROM CryptocurrencyTradeOffers WHERE [OfferKind]={0} AND [Status] = {1} AND [CreatorId] NOT LIKE {2}",
                             (int)OfferType,
                             (int)CryptocurrencyOfferStatus.Active,
                             UserId);
    }
    public static String GetGridViewStringForUserActualOffers(int CreatorId, CryptocurrencyOfferType TypeOfOffers)
    {
        return String.Format(@"SELECT * FROM CryptocurrencyTradeOffers WHERE [CreatorId] = {0} AND [OfferKind]={1} AND [Status] IN ({2},{3})",
                                CreatorId,
                                (int)TypeOfOffers,
                                (int)CryptocurrencyOfferStatus.Active,
                                (int)CryptocurrencyOfferStatus.Paused);
    }

    public static void CreateNewOffer(CryptocurrencyOfferType OfferType, int CreatorId, Money MinPrice, Money MaxPrice, Money MinOfferValue, Money MaxOfferValue, int EscrowTime, String Description, CryptocurrencyMoney CryptocurrencyAmount)
    {
        CryptocurrencyTradeOffer NewTradeOffer = new CryptocurrencyTradeOffer();
        
        NewTradeOffer.Status        = CryptocurrencyOfferStatus.Active;
        NewTradeOffer.DateAdded     = DateTime.Now;
        NewTradeOffer.DateFinished  = DateTime.Now.AddDays(365);

        NewTradeOffer.OfferKind     = OfferType;
        NewTradeOffer.CreatorId     = CreatorId;
        NewTradeOffer.MinPrice      = MinPrice;
        NewTradeOffer.MaxPrice      = MaxPrice;
        NewTradeOffer.MinOfferValue = MinOfferValue;
        NewTradeOffer.MaxOfferValue = MaxOfferValue;
        NewTradeOffer.Description   = Description;
        NewTradeOffer.EscrowTime    = EscrowTime;
        NewTradeOffer.Amount        = CryptocurrencyAmount;
        NewTradeOffer.AmountLeft    = CryptocurrencyAmount;

        NewTradeOffer.Save(true);

    }

    #endregion
}