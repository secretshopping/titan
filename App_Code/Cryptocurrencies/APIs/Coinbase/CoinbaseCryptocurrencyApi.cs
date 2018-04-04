using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Prem.PTC.Utils;
using Resources;
using Prem.PTC.Payments;
using System.IO;
using ExtensionMethods;
using Coinbase;
using Coinbase.ObjectModel;
using LukeSkywalker.IPNetwork;
using Titan.Cryptocurrencies;

public class CoinbaseCryptocurrencyApi : CryptocurrencyApi
{
    protected static CryptocurrencyAPIProvider cryptocurrencyApiProvider = CryptocurrencyAPIProvider.Coinbase;

    public CoinbaseCryptocurrencyApi() : base((int)cryptocurrencyApiProvider) { }
    public override CryptocurrencyAPIProvider Provider { get { return cryptocurrencyApiProvider; } }
    public override bool AllowToUsePaymentButtons() { return AppSettings.Cryptocurrencies.IsCoinbaseMerchant; }

    public override Money GetAccountBalance()
    {
        try
        {
            CoinbaseApi api = new CoinbaseApi(AppSettings.Cryptocurrencies.CoinbaseAPIKey, AppSettings.Cryptocurrencies.CoinbaseAPISecret, false);

            var accounts = api.SendRequest("/accounts", null, RestSharp.Method.GET);

            var account = accounts.Data.First(acc => acc["currency"].ToString() == "BTC");
            var balance = account["balance"];
            var amount = balance["amount"].ToString();

            return new Money(Convert.ToDecimal(amount));
        }
        catch (Exception ex)
        {
            return Money.Zero;
        }
    }

    public override void TryWithDrawCryptocurrencyFromWallet(decimal amountInCryptocurrency, string userAddress, CryptocurrencyType cryptocurrencyType)
    {
        CoinbaseApi api = new CoinbaseApi(AppSettings.Cryptocurrencies.CoinbaseAPIKey, AppSettings.Cryptocurrencies.CoinbaseAPISecret, false);
        var options = new { type = "send", to = userAddress, amount = amountInCryptocurrency, currency = cryptocurrencyType.ToString() };

        var response = api.SendRequest(String.Format("/accounts/{0}/transactions", GetAccountId(api)),
                options, RestSharp.Method.POST);

        if (response.Errors != null)
            throw new MsgException(response.Errors[0].Message);
    }

    public override string CreateNewAddress(int userId)
    {
        CoinbaseApi api = new CoinbaseApi(AppSettings.Cryptocurrencies.CoinbaseAPIKey, AppSettings.Cryptocurrencies.CoinbaseAPISecret, false);

        var options = new { name = DateTime.Now.ToString() };
        string adminAddress = string.Empty;

        try
        {
            var request = api.SendRequest(String.Format("/accounts/{0}/addresses", GetAccountId(api)),
                options, RestSharp.Method.POST);

            if (request.Errors != null)
            {
                ErrorLogger.Log(request.Errors[0].Message, LogType.Coinbase);
                return adminAddress;
            }

            adminAddress = request.Data["address"].ToString();

            if (!string.IsNullOrWhiteSpace(adminAddress))
            {
                string query = string.Format("SELECT * FROM BitcoinAddresses WHERE UserId = {0}", userId);
                BitcoinAddress address = TableHelper.GetListFromRawQuery<BitcoinAddress>(query).FirstOrDefault();
                if (address == null)
                {
                    address = new BitcoinAddress();
                    address.UserId = userId;
                }
                address.CoinbaseAddress = adminAddress;
                address.Save();
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }

        return adminAddress;
    }

    public override decimal GetEstimatedWithdrawalFee(decimal amount, string userAddress)
    {
        return Convert.ToDecimal(0);  //No fees
    }

    public static string GetAccountId(CoinbaseApi api)
    {
        var accounts = api.SendRequest("/accounts", null, RestSharp.Method.GET);

        if (accounts.Errors != null)
            throw new MsgException(accounts.Errors[0].Message);

        return accounts.Data.First(account => account["currency"].ToString() == "BTC")["id"].ToString();
    }
}