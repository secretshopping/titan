using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Prem.PTC.Advertising;
using System.Web;
using System.Web.UI;

public class GiftCard : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "GiftCards"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("Title")]
    public string Title { get { return _Title; } set { _Title = value; SetUpToDateAsFalse(); } }

    [Column("Description")]
    public string Description { get { return _Description; } set { _Description = value; SetUpToDateAsFalse(); } }

    [Column("ImageUrl")]
    public string ImageUrl { get { return _ImageUrl; } set { _ImageUrl = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

    private int _id, _StatusInt;
    private string _Title, _Description, _ImageUrl;

    public BaseStatus Status
    {
        get
        {
            return (BaseStatus)StatusInt;
        }
        set
        {
            StatusInt = (int)value;
        }
    }

    protected Banner _bannerImage;
    public Banner BannerImage
    {
        get { return _bannerImage; }
        set
        {
            _bannerImage = value;
            SetUpToDateAsFalse();
        }
    }

    public GiftCard()
        : base() { }

    public GiftCard(int id) : base(id) { }

    public GiftCard(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

    #endregion Columns

    public static List<GiftCard> GetActiveCards()
    {
        //Codes active
        var where = TableHelper.MakeDictionary("Status", (int)BaseStatus.Active);
        return TableHelper.SelectRows<GiftCard>(where);
    }
}
