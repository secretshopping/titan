using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Titan.Matrix;

public class PurchasedItem : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "PurchasedItems"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } protected set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("UnitPrice")]
    public Money UnitPrice
    {
        get { return _UnitPrice; }
        protected set { _UnitPrice = value; SetUpToDateAsFalse(); }
    }

    [Column("Quantity")]
    public int Quantity { get { return _Quantity; } protected set { _Quantity = value; SetUpToDateAsFalse(); } }

    [Column("Description")]
    public string Description
    {
        get { return _Description; }
        protected set { _Description = value; SetUpToDateAsFalse(); }
    }

    [Column("Tax")]
    public decimal Tax
    {
        get { return _Tax; }
        protected set { _Tax = value; SetUpToDateAsFalse(); }
    }

    [Column("DateAdded")]
    public DateTime DateAdded
    {
        get { return _DateAdded; }
        protected set { _DateAdded = value; SetUpToDateAsFalse(); }
    }

    [Column("ItemType")]
    protected int ItemTypeInt
    {
        get { return _ItemTypeInt; }
        set { _ItemTypeInt = value; SetUpToDateAsFalse(); }
    }

    public PurchasedItemType ItemType
    {
        get
        {
            return (PurchasedItemType)ItemTypeInt;
        }
        set
        {
            ItemTypeInt = (int)value;
        }
    }

    #endregion Columns


    int _Id, _UserId, _Quantity, _ItemTypeInt;
    Money _UnitPrice;
    string _Description;
    decimal _Tax;
    DateTime _DateAdded;
    public PurchasedItem(int id) : base(id) { }

    public PurchasedItem(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

    private PurchasedItem(int userId, Money unitPrice, int quantity, string description, PurchasedItemType itemType, decimal tax = 0)
    {
        UserId = userId;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Description = description;
        ItemType = itemType;
        Tax = tax;
        DateAdded = AppSettings.ServerTime;
    }

    public static PurchasedItem Create(int userId, Money unitPrice, int quantity, string description, PurchasedItemType itemType, decimal tax = 0)
    {
        var item = new PurchasedItem(userId, unitPrice, quantity, description, itemType, tax);
        item.Save();
        return item;
    }

    public static List<PurchasedItem> GetList(int userId)
    {
        string query = string.Format("SELECT * FROM PurchasedItems WHERE UserId = {0}", userId);
        return TableHelper.GetListFromRawQuery<PurchasedItem>(query);
    }

    public static List<PurchasedItem> GetList(int userId, PurchasedItemType itemType)
    {
        string query = string.Format("SELECT * FROM PurchasedItems WHERE UserId = {0} AND ItemType = {1}",
            userId, (int)itemType);
        return TableHelper.GetListFromRawQuery<PurchasedItem>(query);
    }

    public Money GetTotalValue()
    {
        var withoutTax = UnitPrice * Quantity;
        var tax = Money.MultiplyPercent(withoutTax, Tax);
        return withoutTax + tax;
    }
}
public enum PurchasedItemType
{
    Transfer = 1,
    AdPack = 2
}








