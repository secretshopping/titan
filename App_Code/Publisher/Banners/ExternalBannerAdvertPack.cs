using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

[Serializable]
public class ExternalBannerAdvertPack : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "ExternalBannerAdvertPacks"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("Clicks")]
    public int Clicks
    {
        get { return _Clicks; }
        set
        {
            if (value <= 0)
                throw new MsgException("Number of clicks must be greater than 0.");
            _Clicks = value; SetUpToDateAsFalse();
        }
    }

    [Column("Price")]
    public Money Price { get { return _Price; } set { _Price = value; SetUpToDateAsFalse(); } }

    [Column("ExternalBannerDimensionsId")]
    public int ExternalBannerDimensionsId { get { return _ExternalBannerDimensionsId; } set { _ExternalBannerDimensionsId = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }


    #endregion Columns
    public UniversalStatus Status
    {
        get { return (UniversalStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    int _Id, _StatusInt, _ExternalBannerDimensionsId, _Clicks;
    Money _Price;

    public ExternalBannerAdvertPack(int id) : base(id) { }

    public ExternalBannerAdvertPack(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

    private ExternalBannerAdvertPack(int clicks, Money price, int externalBannerDimensionsId)
    {
        Clicks = clicks;
        Price = price;
        Status = UniversalStatus.Paused;
        ExternalBannerDimensionsId = externalBannerDimensionsId;
    }

    public ExternalBannerAdvertDimensions Dimensions
    {
        get
        {
            return new ExternalBannerAdvertDimensions(ExternalBannerDimensionsId);
        }
    }

    public void Activate()
    {
        Status = UniversalStatus.Active;
        this.Save();
    }

    public void Pause()
    {
        Status = UniversalStatus.Paused;
        this.Save();
    }

    public override void Delete()
    {
        Status = UniversalStatus.Deleted;
        this.Save();
    }

    public static void Create(int clicks, Money price, int externalBannerDimensionsId)
    {
        var pack = new ExternalBannerAdvertPack(clicks, price, externalBannerDimensionsId);
        pack.Save();
    }

    public static List<ExternalBannerAdvertPack> GetActive()
    {
        return TableHelper.SelectRows<ExternalBannerAdvertPack>(TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
    }

    public static bool AreAnyActive()
    {
        return TableHelper.RowExists("ExternalBannerAdvertPacks", TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
    }

    public void Edit(int clicks, Money price)
    {
        Clicks = clicks;
        Price = price;
        this.Save();
    }
}