using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Titan.Cryptocurrencies;

public class CryptocurrencyWithdrawalAddress : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "CryptocurrencyWithdrawalAddresses"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    //not logged in = -1
    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Address")]
    public string Address
    {
        get { return _Address; }
        set
        {
            if (CryptocurrencyCode == "BTC")
                BitcoinValidator.BitcoinValidator.ValidateBitcoinAddress(value);

            _Address = value;
            SetUpToDateAsFalse();
        }
    }

    [Column("DateAdded")]
    public DateTime DateAdded { get { return _DateAdded; } set { _DateAdded = value; SetUpToDateAsFalse(); } }

    [Column("CryptocurrencyCode")]
    public string CryptocurrencyCode { get { return _CryptocurrencyCode; } set { _CryptocurrencyCode = value; SetUpToDateAsFalse(); } }

    [Column("IsNew")]
    public bool IsNew { get { return _IsNew; } set { _IsNew = value; SetUpToDateAsFalse(); } }

    public CryptocurrencyType Cryptocurrency
    {
        get { return (CryptocurrencyType)Enum.Parse(typeof(CryptocurrencyType), CryptocurrencyCode); }
        set { CryptocurrencyCode = value.ToString(); }
    }

    bool _IsNew;
    int _Id, _UserId;
    string _Address, _CryptocurrencyCode;
    DateTime _DateAdded;


    private CryptocurrencyWithdrawalAddress()
            : base()
    {
    }
    public CryptocurrencyWithdrawalAddress(int id)
            : base(id)
    {
    }
    public CryptocurrencyWithdrawalAddress(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    {
    }

    #endregion

    private CryptocurrencyWithdrawalAddress(int userId, string address, CryptocurrencyType cryptocurrenyType)
    {
        this.CryptocurrencyCode = cryptocurrenyType.ToString();
        this.UserId = userId;
        this.Address = address;
        this.DateAdded = AppSettings.ServerTime;
        this.IsNew = true;
    }

    public static void AddIfNotExists(int userId, string address, CryptocurrencyType cryptocurrenyType)
    {
        var currentAddresses = GetAddress(userId, cryptocurrenyType);
        if (currentAddresses == null)
        {
            var newAddress = new CryptocurrencyWithdrawalAddress(userId, address, cryptocurrenyType);
            newAddress.Save();
        }
        else if(currentAddresses.Address != address)
        {
            currentAddresses.Address = address;
            currentAddresses.DateAdded = AppSettings.ServerTime;
            currentAddresses.IsNew = false;
            currentAddresses.Save();
        }
    }

    private static List<CryptocurrencyWithdrawalAddress> GetList(int userId, CryptocurrencyType cryptocurrenyType)
    {
        string query = String.Format("SELECT * FROM CryptocurrencyWithdrawalAddresses WHERE UserId = {0} AND CryptocurrencyCode = '{1}'", userId, cryptocurrenyType);
        return TableHelper.GetListFromRawQuery<CryptocurrencyWithdrawalAddress>(query);
    }

    public static CryptocurrencyWithdrawalAddress GetAddress(int userId, CryptocurrencyType cryptocurrenyType)
    {
        string query = String.Format("SELECT * FROM CryptocurrencyWithdrawalAddresses WHERE UserId = {0} AND CryptocurrencyCode = '{1}'", userId, cryptocurrenyType);
        return TableHelper.GetListFromRawQuery<CryptocurrencyWithdrawalAddress>(query).FirstOrDefault();
    }

}