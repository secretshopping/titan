using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

public class ExternalBannerAdvertDimensions : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "ExternalBannerAdvertDimensions"; } }
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

    public ExternalBannerAdvertDimensions(int id) : base(id) { }

    public ExternalBannerAdvertDimensions(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

    private ExternalBannerAdvertDimensions(int width, int height)
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
        var dimensions = new ExternalBannerAdvertDimensions(width, height);
        dimensions.Save();
    }

    public static List<ExternalBannerAdvertDimensions> GetActive()
    {
        return TableHelper.SelectRows<ExternalBannerAdvertDimensions>(
            TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
    }
}