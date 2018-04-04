using System;
using System.Collections.Generic;
using System.Linq;
using Prem.PTC;
using Newtonsoft.Json.Linq;
using System.Text;
using Titan.Cryptocurrencies;

public class BlocktrailAPI
{
    private string ApiKey;
    private string ApiSecret;
    private string EndpointUrl = "https://api.blocktrail.com/v1/btc/";

    public BlocktrailAPI(string apiKey, string apiSecret)
    {
        this.ApiKey = apiKey;
        this.ApiSecret = apiSecret;
    }

    public string GenerateBTCAddress()
    {
        string adminAddress = string.Empty;

        using (MyWebClient client = new MyWebClient())
        {
            var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

            //We have a Blocktrail SDK installed on apis.usetitan.com
            string requestUrl = String.Format("https://apis.usetitan.com/btg.php?command=generate&key={0}&secret={1}&walletName={2}&walletPass={3}&confirmations={4}",
               ApiKey, ApiSecret, AppSettings.Cryptocurrencies.BlocktrailWalletIdentifier, AppSettings.Cryptocurrencies.BlocktrailWalletPassword, BtcCryptocurrency.DepositMinimumConfirmations);

            var responseString = client.DownloadString(requestUrl);

            if (!responseString.StartsWith("OK"))
                throw new MsgException(responseString);

            adminAddress = responseString.Split(':')[1];     
        }

        return adminAddress;
    }

    public void SendTransaction(string address, decimal amountInBTC)
    {
        using (MyWebClient client = new MyWebClient())
        {
            //We have a Blocktrail SDK installed on apis.usetitan.com
            string requestUrl = String.Format("https://apis.usetitan.com/btg.php?command=pay&key={0}&secret={1}&address={2}&amount={3}&walletName={4}&walletPass={5}", 
               ApiKey, ApiSecret, address, amountInBTC, AppSettings.Cryptocurrencies.BlocktrailWalletIdentifier, AppSettings.Cryptocurrencies.BlocktrailWalletPassword);

            var responseString = client.DownloadString(requestUrl);

            if (!responseString.StartsWith("OK"))
                throw new MsgException(responseString);
        }
    }

    public void SendTransactions(List<KeyValuePair<string, decimal>> addressesWithAmounts)
    {
        var arrayForQueryString = new StringBuilder();
        for (int i=0; i<addressesWithAmounts.Count; i++)
        {
            arrayForQueryString.AppendFormat("&tra[{0}]={1}", addressesWithAmounts.ElementAt(i).Key,
                Convert.ToInt32(addressesWithAmounts.ElementAt(i).Value * 100000000)); //Already converting to Satoshis
        }

        using (MyWebClient client = new MyWebClient())
        {
            //We have a Blocktrail SDK installed on apis.usetitan.com
            string requestUrl = String.Format("https://apis.usetitan.com/btg.php?command=payInBatch&key={0}&secret={1}&walletName={2}&walletPass={3}{4}",
               ApiKey, ApiSecret, AppSettings.Cryptocurrencies.BlocktrailWalletIdentifier, AppSettings.Cryptocurrencies.BlocktrailWalletPassword, arrayForQueryString.ToString());

            var responseString = client.DownloadString(requestUrl);

            if (!responseString.StartsWith("OK"))
                throw new MsgException(responseString);
        }
    }

    public decimal GetFees(string address, decimal amountInBTC)
    {
        int amountInSatoshis = Convert.ToInt32(amountInBTC * 100000000);
        var coinSelection = PostCoinSelection(address, amountInSatoshis);

        if (coinSelection["msg"] != null)
            throw new MsgException("API ERROR: " + coinSelection["msg"].ToString());

        return Convert.ToDecimal((decimal)Convert.ToUInt32(coinSelection["fee"].ToString())/(decimal)100000000);
    }

    public Money GetBalance()
    {
        var command = String.Format("wallet/{0}/balance", AppSettings.Cryptocurrencies.BlocktrailWalletIdentifier);

        using (MyWebClient client = new MyWebClient())
        {
            string requestUrl = String.Format("{0}{1}?api_key={2}", EndpointUrl, command, ApiKey);
            var responseString = client.DownloadJsonString(requestUrl);
            var satoshiBalance = Convert.ToInt32(JObject.Parse(responseString)["confirmed"]);

            return new Money((Decimal)satoshiBalance / new decimal(100000000));
        }
    }

    public JToken GetTransactionInformation(string hash)
    {
        string command = "transaction/" + hash;

        using (MyWebClient client = new MyWebClient())
        {
            string requestUrl = String.Format("{0}{1}?api_key={2}", EndpointUrl, command, ApiKey);
            var responseString = client.DownloadJsonString(requestUrl);
            return JObject.Parse(responseString);
        }
    }

    private JToken GetWallet()
    {
        string command = "wallet/" + AppSettings.Cryptocurrencies.BlocktrailWalletIdentifier;

        using (MyWebClient client = new MyWebClient())
        {
            string requestUrl = String.Format("{0}{1}?api_key={2}", EndpointUrl, command, ApiKey);
            var responseString = client.DownloadJsonString(requestUrl);
            return JObject.Parse(responseString);
        }
    }

    private JToken PostCoinSelection(string address, int satoshis)
    {
        var command = String.Format("wallet/{0}/coin-selection", AppSettings.Cryptocurrencies.BlocktrailWalletIdentifier);

        using (MyWebClient client = new MyWebClient())
        {
            client.Headers["Content-Type"] = "application/json";

            var requestBody = (new JObject(new JProperty(address, satoshis))).ToString();
            var requestUrl = String.Format("{0}{1}?lock=1&zeroconf=0&api_key={2}", EndpointUrl, command, ApiKey);
            var responseString = client.UploadJsonData(requestUrl, requestBody);
            return JObject.Parse(responseString);
        }
    }
}