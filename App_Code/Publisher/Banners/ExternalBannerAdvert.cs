using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System.Collections.Generic;
using System.Data;

public class ExternalBannerAdvert : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "ExternalBannerAdverts"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Url")]
    public string Url { get { return _Url; } set { _Url = value; SetUpToDateAsFalse(); } }

    [Column("ImagePath")]
    public string ImagePath { get { return _ImagePath; } set { _ImagePath = value; SetUpToDateAsFalse(); } }

    [Column("PublishersWebsiteCategoryId")]
    public int PublishersWebsiteCategoryId { get { return _PublishersWebsiteCategoryId; } set { _PublishersWebsiteCategoryId = value; SetUpToDateAsFalse(); } }

    [Column("ExternalBannerAdvertPackId")]
    public int ExternalBannerAdvertPackId { get { return _ExternalBannerAdvertPackId; } set { _ExternalBannerAdvertPackId = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

    [Column("ClicksReceived")]
    public int ClicksReceived { get { return _ClicksReceived; } set { _ClicksReceived = value; SetUpToDateAsFalse(); } }

    [Column("ClicksBought")]
    public int ClicksBought { get { return _ClicksBought; } set { _ClicksBought = value; SetUpToDateAsFalse(); } }

    [Column("PricePaid")]
    public Money PricePaid { get { return _PricePaid; } set { _PricePaid = value; SetUpToDateAsFalse(); } }

    [Column("MoneyPerClick")]
    public Money MoneyPerClick { get { return _MoneyPerClick; } set { _MoneyPerClick = value; SetUpToDateAsFalse(); } }

    [Column("TargetBalance")]
    protected int TargetBalanceInt { get { return _TargetBalance; } set { _TargetBalance = value; SetUpToDateAsFalse(); } }

    public PurchaseBalances TargetBalance { get { return (PurchaseBalances)TargetBalanceInt; } set { TargetBalanceInt = (int)value; } }

    #endregion Columns

    public AdvertStatus Status
    {
        get { return (AdvertStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    public Banner Image
    {
        get
        {
            return _Image ?? Banner.FromFile(ImagePath);
        }
        set
        {
            if (IsImageValid(value))
            {
                _Image = value;
                _ImagePath = value.Path;
                SetUpToDateAsFalse();
            }
            else throw new MsgException("Invalid banner image");
        }
    }

    public ExternalBannerAdvertPack Pack
    {
        get
        {
            return new ExternalBannerAdvertPack(ExternalBannerAdvertPackId);
        }
    }

    int _Id, _UserId, _PublishersWebsiteCategoryId, _StatusInt, _ExternalBannerAdvertPackId, _ClicksReceived, _ClicksBought, _TargetBalance;
    string _Url, _ImagePath;
    Banner _Image;
    Money _PricePaid, _MoneyPerClick;

    public ExternalBannerAdvert(int id) : base(id) { }

    public ExternalBannerAdvert(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

    private ExternalBannerAdvert(int userId, string url, int categoryId, ExternalBannerAdvertPack pack, Banner image, PurchaseBalances targetBalance)
    {
        UserId = userId;
        Url = url;
        Status = AdvertStatus.WaitingForAcceptance;
        PublishersWebsiteCategoryId = categoryId;
        ExternalBannerAdvertPackId = pack.Id;
        Image = image;
        ClicksReceived = 0;
        ClicksBought = pack.Clicks;
        PricePaid = pack.Price;
        MoneyPerClick = PricePaid / ClicksBought;
        TargetBalance = targetBalance;
    }
    public void Accept()
    {
        Status = AdvertStatus.Active;
        this.Save();
    }

    public void Reject()
    {
        Status = AdvertStatus.Rejected;
        PurchaseOption.CreditAfterCampaignRejection(this);
        this.Save();
    }

    public static void Buy(Member user, string url, int categoryId, Banner image, ExternalBannerAdvertPack pack, PurchaseBalances targetBalance)
    {
        PurchaseOption.ChargeBalance(user, pack.Price, PurchaseOption.Features.ExternalBanner.ToString(), targetBalance, "External Banner advertisement");

        var banner = new ExternalBannerAdvert(user.Id, url, categoryId, pack, image, targetBalance);
        banner.Save();

        History.AddPurchase(user.Name, pack.Price, "Banner campaign");

        MatrixBase.TryAddMemberAndCredit(user, pack.Price, AdvertType.ExternalBanner);
    }

    private bool IsImageValid(Banner image)
    {
        return image != null && image.IsSaved;
    }

    public static bool DoesImageHaveValidDimensions(Banner image, ExternalBannerAdvertPack pack)
    {
        var dimensions = pack.Dimensions;
        return image.Width == dimensions.Width && image.Height == dimensions.Height;
    }

    public static bool IsBuyingAvaliable()
    {
        return ExternalBannerAdvertPack.AreAnyActive() && PublishersWebsiteCategory.AreAnyActive();
    }

    public void AddClick()
    {
        ClicksReceived++;

        if (ClicksReceived >= ClicksBought)
            this.Status = AdvertStatus.Finished;

        this.Save();
    }

    public static void ChangeAllStatuses(IEnumerable<int> externalBannersIds, AdvertStatus status)
    {
        foreach (var externalBannerId in externalBannersIds)
        {
            var externalBanner = new ExternalBannerAdvert(externalBannerId);

            if (status == AdvertStatus.Active)
                externalBanner.Accept();
            else if (status == AdvertStatus.Rejected)
                externalBanner.Reject();
        }
    }
}