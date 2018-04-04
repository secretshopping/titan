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
using Prem.PTC.Members;


public class GiftCode : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "GiftCodes"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("Value")]
    public string Value { get { return _Value; } set { _Value = value; SetUpToDateAsFalse(); } }

    [Column("GiftCardId")]
    public int GiftCardId { get { return _GiftCardId; } set { _GiftCardId = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

    private int _id, _StatusInt, _GiftCardId;
    private string _Value, _Description;

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

    public GiftCard GiftCard
    {
        get
        {
            return new GiftCard(GiftCardId);
        }
        set
        {
            GiftCardId = value.Id;
        }
    }

    public GiftCode()
        : base() { }

    public GiftCode(int id) : base(id) { }

    public GiftCode(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

    #endregion Columns

    public static List<KeyValuePair<GiftCode, int>> GetActiveCodesForMember(Member user, int giftCardId)
    {
        //Codes active
        var where = TableHelper.MakeDictionary("GiftCardId", giftCardId);
        where.Add("Status", (int)BaseStatus.Active);
        var result = TableHelper.SelectRows<GiftCode>(where);

        var list = new List<KeyValuePair<GiftCode, int>>();

        foreach (var code in result)
        {
            int price = GiftCodeExchangeRate.GetPrice(user, code.Id);
            if (price != -1)
            {
                list.Add(new KeyValuePair<GiftCode, int>(code, price));
            }
        }

        return list;
    }

    /// <summary>
    /// Check before submit request
    /// </summary>
    /// <returns></returns>
    public bool IsActiveAtTheMoment(Member user)
    {
        var cards = GiftCard.GetActiveCards();
        bool isOK = false;

        foreach (var card in cards)
        {
            var codes = GiftCode.GetActiveCodesForMember(user, card.Id);
            foreach (var code in codes)
            {
                if (code.Key.Id == this.Id)
                    isOK = true;
            }
        }

        return isOK;
    }

}
