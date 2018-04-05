using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using System.Data;

public class GeolocatedFeature : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "GeolocatedFeatures"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("CC")]
    public string CC { get { return _CC; } set { _CC = value; SetUpToDateAsFalse(); } }

    [Column("FeatureId")]
    protected int FeatureId { get { return _FeatureId; } set { _FeatureId = value; SetUpToDateAsFalse(); } }

    [Column("Points")]
    public int Points { get { return _Points; } set { _Points = value; SetUpToDateAsFalse(); } }

    private int _id, _FeatureId, _Points;
    private string _CC;

    public GeolocatedFeatureType Type
    {
        get
        {
            return (GeolocatedFeatureType)FeatureId;
        }
        set
        {
            FeatureId = (int)value;
        }
    }

    public GeolocatedFeature()
        : base() { }

    public GeolocatedFeature(int id) : base(id) { }

    public GeolocatedFeature(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

    #endregion Columns

    public override void Save(bool forceSave = false)
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            //Check if not dubling, remove old
            bridge.Instance.ExecuteRawCommandNonQuery("DELETE FROM GeolocatedFeatures WHERE FeatureId = " + this.FeatureId
                + " AND CC = '" + this.CC + "' AND Id <> " + this.Id);

            //Check if all is being added, remove all the rest
            if (this.ForAllCountries)
            {
                bridge.Instance.ExecuteRawCommandNonQuery("DELETE FROM GeolocatedFeatures WHERE FeatureId = " + this.FeatureId
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
            return CC == GiftCodeExchangeRate.AllCountriesRegex;
        }
        set
        {
            CC = value ? GiftCodeExchangeRate.AllCountriesRegex : "";
        }
    }

    public static int GetPrice(Member user, GeolocatedFeatureType feature)
    {
        return GetPrice(user.CountryCode, feature);
    }

    public static int GetPrice(string CountryCode, GeolocatedFeatureType feature)
    {
        //For his CC
        var where2 = TableHelper.MakeDictionary("FeatureId", (int)feature);
        where2.Add("CC", CountryCode);
        var forHisCC = TableHelper.SelectRows<GeolocatedFeature>(where2);

        if (forHisCC.Count > 0)
            return forHisCC[0].Points;

        //For all countries
        var where = TableHelper.MakeDictionary("FeatureId", (int)feature);
        where.Add("CC", GiftCodeExchangeRate.AllCountriesRegex);
        var listForAll = TableHelper.SelectRows<GeolocatedFeature>(where);

        if (listForAll.Count > 0)
            return listForAll[0].Points;

        return -1;
    }
}