using Prem.PTC;
using System;
using System.Data;
using Titan.CryptocurrencyPlatform;

public class CryptocurrencyFinishedTradeOffer : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }

    protected override string dbTable { get { return "CryptocurrencyFinishedTradeOffers"; } }

    #region Contructors
    public CryptocurrencyFinishedTradeOffer()
    : base()
    {
    }

    public CryptocurrencyFinishedTradeOffer(int id)
        : base(id)
    {
    }

    public CryptocurrencyFinishedTradeOffer(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate)
    {
    }
    #endregion

    #region Columns
    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("OfferId")]
    public int OfferId { get { return _OfferId; } set { _OfferId = value; SetUpToDateAsFalse(); } }

    [Column("BuyerId")]
    public int BuyerId { get { return _BuyerId; } set { _BuyerId = value; SetUpToDateAsFalse(); } }

    [Column("SellerId")]
    public int SellerId { get { return _SellerId; } set { _SellerId = value; SetUpToDateAsFalse(); } }

    [Column("BuyerComment")]
    public String BuyerComment { get { return _BuyerComment; } set { _BuyerComment = value; SetUpToDateAsFalse(); } }

    [Column("SellerComment")]
    public String SellerComment { get { return _SellerComment; } set { _SellerComment = value; SetUpToDateAsFalse(); } }

    [Column("Rating")]
    private int IntRating { get { return _Rating; } set { _Rating = value; SetUpToDateAsFalse(); } }

    public CryptocurrencyOfferRating Rating
    {
        get
        {
            return (CryptocurrencyOfferRating)IntRating;
        }
        set
        {
            IntRating = (int)value;
        }
    }

    [Column("CCAmount")]
    public CryptocurrencyMoney CCAmount { get { return _CCAmount; } set { _CCAmount = value; SetUpToDateAsFalse(); } }


    private int _Id, _OfferId, _BuyerId, _SellerId, _Rating;
    private String _BuyerComment, _SellerComment;
    private CryptocurrencyMoney _CCAmount;
    #endregion

    #region Helpers
    public void Save(bool forceSave = false)
    {
        base.Save(forceSave);
    }

    public static String GetGridViewStringForUserHistory(int UserId, CryptocurrencyOfferType OffersType)
    {
        String UserType = (OffersType == CryptocurrencyOfferType.Buy) ? "BuyerID" : "SellerId";
        return String.Format("SELECT * FROM CryptocurrencyFinishedTradeOffers WHERE {0}={1}", UserType, UserId);
    }

    public static void CreateNewTemplate(int offerId, int buyerID, int sellerId, CryptocurrencyMoney cryptocurrencyAmount)
    {
        CryptocurrencyFinishedTradeOffer NewFinishedTrade = new CryptocurrencyFinishedTradeOffer()
        {
            OfferId         = offerId,
            BuyerId         = buyerID,
            SellerId        = sellerId,
            Rating          = CryptocurrencyOfferRating.Null,
            CCAmount        = cryptocurrencyAmount,
            BuyerComment    = String.Empty,
            SellerComment   = String.Empty
        };
        NewFinishedTrade.Save(true);
    }

    #endregion
}