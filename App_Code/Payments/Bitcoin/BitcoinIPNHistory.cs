using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Titan.Cryptocurrencies;

public class BitcoinIPNHistory : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "BitcoinIPNHistory"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {
        public const string Id = "Id";
        public const string IPNId = "IPNId";
        public const string UserId = "UserId";
        public const string BitcoinCryptocurrencyAPIProvider = "BitcoinCryptocurrencyAPIProvider";
    }


    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UserId)]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.IPNId)]
    public string IPNId { get { return _IPNId; } set { _IPNId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.BitcoinCryptocurrencyAPIProvider)]
    protected int BitcoinCryptocurrencyAPIProviderInt { get { return _BitcoinCryptocurrencyAPIProvider; } set { _BitcoinCryptocurrencyAPIProvider = value; SetUpToDateAsFalse(); } }

    #endregion
    private int _id,  _BitcoinCryptocurrencyAPIProvider, _UserId;
    private string _IPNId;

    public CryptocurrencyAPIProvider CryptocurrencyAPIProvider
    {
        get { return (CryptocurrencyAPIProvider)BitcoinCryptocurrencyAPIProviderInt; }
        set { BitcoinCryptocurrencyAPIProviderInt = (int)value; }
    }


    public BitcoinIPNHistory()
                : base()
    { }

    public BitcoinIPNHistory(int id) : base(id) { }

    public BitcoinIPNHistory(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate)
    { }


    public static void Add(CryptocurrencyAPIProvider provider, string id, int userId)
    {
        BitcoinIPNHistory history = new BitcoinIPNHistory();
        history.UserId = userId;
        history.IPNId = id;
        history.CryptocurrencyAPIProvider = provider;
        history.Save();
    }

    public static bool Contains(CryptocurrencyAPIProvider provider, string id, int userId)
    {
        var count = (int)TableHelper.SelectScalar(
            String.Format("SELECT COUNT(*) FROM BitcoinIPNHistory WHERE UserId = {0} AND BitcoinCryptocurrencyAPIProvider = {1} AND IPNId = '{2}'",
            userId, (int)provider, id));

        return count > 0;
    }
}
