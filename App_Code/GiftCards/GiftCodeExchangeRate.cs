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

public class GiftCodeExchangeRate : BaseTableObject
{
    public static readonly string AllCountriesRegex = "**";

    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "GiftCodeExchangeRates"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("CC")]
    public string CC { get { return _Title; } set { _Title = value; SetUpToDateAsFalse(); } }

    [Column("Points")]
    public int Points { get { return _Points; } set { _Points = value; SetUpToDateAsFalse(); } }

    [Column("GiftCodeId")]
    public int GiftCodeId { get { return _GiftCodeId; } set { _GiftCodeId = value; SetUpToDateAsFalse(); } }

    private int _id, _GiftCodeId, _Points;
    private string _Title;

    public GiftCode GiftCode
    {
        get
        {
            return new GiftCode(GiftCodeId);
        }
        set
        {
            GiftCodeId = value.Id;
        }
    }

    public GiftCodeExchangeRate()
        : base() { }

    public GiftCodeExchangeRate(int id) : base(id) { }

    public GiftCodeExchangeRate(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

    #endregion Columns

    public override void Save(bool forceSave = false)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            //Check if not dubling, remove old
            bridge.Instance.ExecuteRawCommandNonQuery("DELETE FROM GiftCodeExchangeRates WHERE GiftCodeId = " + this.GiftCodeId
                + " AND CC = '" + this.CC + "' AND Id <> " + this.Id);

            //Check if all is being added, remove all the rest
            if (this.ForAllCountries)
            {
                bridge.Instance.ExecuteRawCommandNonQuery("DELETE FROM GiftCodeExchangeRates WHERE GiftCodeId = " + this.GiftCodeId
                    + " AND Id <> " + this.Id);
            }
        }

        //Save
        base.Save(forceSave);

    }

    public bool ForAllCountries
    {
        get
        {
            return CC == AllCountriesRegex;
        }
        set
        {
            CC = value ? AllCountriesRegex : "";
        }
    }

    public static int GetPrice(Member user, int giftCodeId)
    {
        //For his CC
        var where2 = TableHelper.MakeDictionary("GiftCodeId", giftCodeId);
        where2.Add("CC", user.CountryCode);
        var forHisCC = TableHelper.SelectRows<GiftCodeExchangeRate>(where2);

        if (forHisCC.Count > 0)
            return forHisCC[0].Points;

        //For all countries
        var where = TableHelper.MakeDictionary("GiftCodeId", giftCodeId);
        where.Add("CC", AllCountriesRegex);
        var listForAll = TableHelper.SelectRows<GiftCodeExchangeRate>(where);

        if (listForAll.Count > 0)
            return listForAll[0].Points;

        return -1;
    }
}
