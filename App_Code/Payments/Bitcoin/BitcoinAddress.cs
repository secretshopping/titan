using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using Titan.Cryptocurrencies;

public class BitcoinAddress : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "BitcoinAddresses"; } }
    protected override string dbTable { get { return TableName; } }

    public static class Columns
    {
        public const string Id = "Id";
        public const string UserId = "UserId";
        public const string CoinPaymentsAddress = "CoinPaymentsAddress";
        public const string CoinbaseAddress = "CoinbaseAddress";
        public const string BlocktrailAddress = "BlocktrailAddress";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UserId)]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.CoinPaymentsAddress)]
    public string CoinPaymentsAddress { get { return _CoinPaymentsAddress; } set { _CoinPaymentsAddress = value; SetUpToDateAsFalse(); } }

    [Column(Columns.CoinbaseAddress)]
    public string CoinbaseAddress { get { return _CoinbaseAddress; } set { _CoinbaseAddress = value; SetUpToDateAsFalse(); } }

    [Column(Columns.BlocktrailAddress)]
    public string BlocktrailAddress { get { return _BlocktrailAddress; } set { _BlocktrailAddress = value; SetUpToDateAsFalse(); } }
    #endregion

    private int _id, _UserId;
    private string _CoinPaymentsAddress, _CoinbaseAddress, _BlocktrailAddress;

    public BitcoinAddress() : base() { }
    public BitcoinAddress(int id) : base(id) { }
    public BitcoinAddress(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

    [Obsolete]
    public static int GetBlockioUserId(string bitcoinAddress)
    {                                                                                  
        var addresses = TableHelper.SelectRows<BitcoinAddress>(TableHelper.MakeDictionary("BlockioAddress", bitcoinAddress));

        return addresses[0].UserId;
    }

    [Obsolete]
    public static int GetCoinPaymentsUserId(string bitcoinAddress)
    {
        var addresses = TableHelper.SelectRows<BitcoinAddress>(TableHelper.MakeDictionary("CoinPaymentsAddress", bitcoinAddress));

        return addresses[0].UserId;
    }

    public static int GetUserId(string bitcoinAddress, CryptocurrencyAPIProvider type)
    {
        return GetAddress(bitcoinAddress, type).UserId;
    }   

    public static BitcoinAddress GetAddress(string bitcoinAddress, CryptocurrencyAPIProvider type)
    {
        List<BitcoinAddress> addresses = new List<BitcoinAddress>();
        switch (type)
        {
            case CryptocurrencyAPIProvider.CoinPayments:
                addresses = TableHelper.SelectRows<BitcoinAddress>(TableHelper.MakeDictionary("CoinPaymentsAddress", bitcoinAddress));
                break;
            case CryptocurrencyAPIProvider.Coinbase:
                addresses = TableHelper.SelectRows<BitcoinAddress>(TableHelper.MakeDictionary("CoinbaseAddress", bitcoinAddress));
                break;
            case CryptocurrencyAPIProvider.Blocktrail:
                addresses = TableHelper.SelectRows<BitcoinAddress>(TableHelper.MakeDictionary("BlocktrailAddress", bitcoinAddress));
                break;
        }

        if (addresses.Count == 0)
            return null;

        return addresses[0];
    }

    public static string GetAddress(int userId, CryptocurrencyAPIProvider type)
    {
        var address = TableHelper.SelectRows<BitcoinAddress>(TableHelper.MakeDictionary("UserId", userId));

        if (address.Count == 0)
            return String.Empty;

        switch (type)
        {
            case CryptocurrencyAPIProvider.CoinPayments:
                return address[0].CoinPaymentsAddress;
            case CryptocurrencyAPIProvider.Coinbase:
                return address[0].CoinbaseAddress;
            case CryptocurrencyAPIProvider.Blocktrail:
                return address[0].BlocktrailAddress;
        }

        return String.Empty;
    }

    public static void MakeBlank(int userId, CryptocurrencyAPIProvider type)
    {
        var addresses = TableHelper.SelectRows<BitcoinAddress>(TableHelper.MakeDictionary("UserId", userId));
        if (addresses.Count > 0)
        {
            switch (type)
            {
                case CryptocurrencyAPIProvider.CoinPayments:
                    addresses[0].CoinPaymentsAddress = String.Empty;
                    break;
                case CryptocurrencyAPIProvider.Coinbase:
                    addresses[0].CoinbaseAddress = String.Empty;
                    break;
                case CryptocurrencyAPIProvider.Blocktrail:
                    addresses[0].BlocktrailAddress = String.Empty;
                    break;
            }
            
            addresses[0].Save();
        }
    }
}
