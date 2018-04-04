using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

[Serializable]
public class BannerAdvertDimensions : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "BannerAdvertDimensions"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("Width")]
    public int Width { get { return _Width; } set { _Width = value; SetUpToDateAsFalse(); } }

    [Column("Height")]
    public int Height { get { return _Height; } set { _Height = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

    #endregion Columns
    public UniversalStatus Status
    {
        get { return (UniversalStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    int _Id, _Width, _Height, _StatusInt;

    public BannerAdvertDimensions(int id) : base(id) { }

    public BannerAdvertDimensions(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

    private BannerAdvertDimensions(int width, int height)
    {
        Width = width;
        Height = height;
        Status = UniversalStatus.Paused;
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

    public static void Create(int width, int height)
    {
        var dimensions = new BannerAdvertDimensions(width, height);
        dimensions.Save();
    }

    public static List<BannerAdvertDimensions> GetActive()
    {
        return TableHelper.SelectRows<BannerAdvertDimensions>(
            TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
    }

    public static List<BannerAdvertDimensions> GetAll()
    {
        return TableHelper.GetListFromQuery<BannerAdvertDimensions>("WHERE Status != " + (int)UniversalStatus.Deleted);
    }

    public static BannerAdvertDimensions Get(int width, int height)
    {
        var list = TableHelper.GetListFromRawQuery<BannerAdvertDimensions>(String.Format(
            "SELECT * FROM BannerAdvertDimensions WHERE Width = {0} AND Height = {1} AND Status = {2}", width, height, (int)UniversalStatus.Active));

        if (list.Count > 0)
            return list[0];

        return null;
    }

    public static ListItem[] GetActiveItems()
    {
        var query = from BannerAdvertDimensions dimension in GetActive()
                    select new ListItem(dimension.ToString(), dimension.Id + "");

        return query.ToArray();
    }

    public static ListItem[] GetAllItems()
    {
        var query = from BannerAdvertDimensions dimension in GetAll()
                    select new ListItem(dimension.ToString(), dimension.Id + "");

        return query.ToArray();
    }

    public override string ToString()
    {
        return String.Format("{0}x{1}", this.Width, this.Height);
    }

    public static void CheckDefaultDimensionsForPaidToPromote()
    {
        if(new BannerAdvertDimensions(AppSettings.PaidToPromote.BannerDimensionId).Status != UniversalStatus.Active)
        {
            try
            {
                AppSettings.PaidToPromote.BannerDimensionId = GetActive()[0].Id;
                AppSettings.PaidToPromote.Save();
            }
            catch(Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }
    }
}