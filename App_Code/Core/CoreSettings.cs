using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

public class CoreSettings : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "CoreSettings"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("MaxDecimalPlaces")]
    public int MaxDecimalPlaces { get { return _MaxDecimalPlaces; } set { _MaxDecimalPlaces = value; SetUpToDateAsFalse(); } }

    [Column("CryptocurrencyMaxDecimalPlaces")]
    public int CryptocurrencyMaxDecimalPlaces { get { return _CryptocurrencyMaxDecimalPlaces; } set { _CryptocurrencyMaxDecimalPlaces = value; SetUpToDateAsFalse(); } }

    int _Id, _MaxDecimalPlaces, _CryptocurrencyMaxDecimalPlaces;

    #endregion

    #region Constructors

    public CoreSettings()
            : base()
    {

    }
    public CoreSettings(int id)
            : base(id)
    {

    }
    public CoreSettings(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    {
    }

    #endregion

    public static int GetMaxDecimalPlaces(CurrencyType currency = CurrencyType.Fiat)
    {
        CoreSettings settings = null;
        settings = (CoreSettings)new CoreSettingsCache().Get();

        if (settings == null)
            return 8;
        else
            return (currency == CurrencyType.Fiat) ? settings.MaxDecimalPlaces : settings.CryptocurrencyMaxDecimalPlaces;
    }

    public static void SetMaxDecimalPlaces(int decimalPlaces, CurrencyType currency = CurrencyType.Fiat)
    {
        if (currency == CurrencyType.Fiat && (decimalPlaces < 2 || decimalPlaces > 8))
            throw new MsgException("Value must be between 2 and 8.");

        if (currency == CurrencyType.Crypto && (decimalPlaces < 2 || decimalPlaces > 18))
            throw new MsgException("Value must be between 2 and 18.");

        var settings = TableHelper.SelectAllRows<CoreSettings>().FirstOrDefault();
        if (settings == null)
        {
            settings = new CoreSettings();
            SetMaxDecimalPlaces(settings, decimalPlaces, currency);
        }
        else
            SetMaxDecimalPlaces(settings, decimalPlaces, currency);

        settings.Save();
        HttpContext.Current.Session["CoreSettings"] = settings;
    }

    private static void SetMaxDecimalPlaces(CoreSettings settings, int decimalPlaces, CurrencyType currency)
    {
        if (currency == CurrencyType.Fiat)
            settings.MaxDecimalPlaces = decimalPlaces;
        else
            settings.CryptocurrencyMaxDecimalPlaces = decimalPlaces;
    }

}

public enum CurrencyType
{
    Fiat = 0,
    Crypto = 1
}