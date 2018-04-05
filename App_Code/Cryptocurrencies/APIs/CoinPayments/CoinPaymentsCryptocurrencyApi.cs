using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using Prem.PTC.Utils;
using Resources;
using Prem.PTC.Payments;
using System.IO;
using ExtensionMethods;
using Titan.Cryptocurrencies;

public class CoinPaymentsCryptocurrencyApi : CryptocurrencyApi
{
    protected static CryptocurrencyAPIProvider cryptocurrencyApiProvider = CryptocurrencyAPIProvider.CoinPayments;

    public CoinPaymentsCryptocurrencyApi() : base((int)cryptocurrencyApiProvider) { }
    public override CryptocurrencyAPIProvider Provider { get { return cryptocurrencyApiProvider; } }
    public override bool AllowToUsePaymentButtons() { return true; }

    public override void TryWithDrawCryptocurrencyFromWallet(decimal amountInCryptocurrency, string userAddress, CryptocurrencyType cryptocurrencyType)
    {
        string cmd = "create_withdrawal";

        using (WebClient client = new WebClient())
        {
            var values = new NameValueCollection();
            values["version"] = "1";
            values["key"] = AppSettings.Cryptocurrencies.CoinPaymentsApiKey;
            values["format"] = "json";
            values["cmd"] = cmd;
            values["amount"] = amountInCryptocurrency.ToString();
            values["currency"] = cryptocurrencyType.ToString();
            values["address"] = userAddress;
            values["auto_confirm"] = "1";

            string SpecialParams = String.Format("{0}&amount={1}&currency={3}&address={2}&auto_confirm=1", cmd, amountInCryptocurrency.ToString(), userAddress,
                cryptocurrencyType.ToString());

            client.Headers.Add("HMAC", GetSHA512HMAC(SpecialParams));

            try
            {
                var response = client.UploadValues(baseUrl, values);
                string responseString = Encoding.Default.GetString(response);
                JObject o = JObject.Parse(responseString);

                if (o["error"].ToString() != "ok")
                    throw new MsgException(o["error"].ToString());
            }
            catch (Exception ex)
            {
                throw new MsgException(ex.Message);
            }
        }
    }

    public override Money GetAccountBalance()
    {
        try
        {
            string cmd = "balances";

            using (WebClient client = new WebClient())
            {
                var values = new NameValueCollection();
                values["version"] = "1";
                values["key"] = AppSettings.Cryptocurrencies.CoinPaymentsApiKey;
                values["format"] = "json";
                values["cmd"] = cmd;
                values["all"] = "1";

                string SpecialParams = cmd + "&all=1";

                client.Headers.Add("HMAC", GetSHA512HMAC(SpecialParams));

                var response = client.UploadValues(baseUrl, values);
                string responseString = Encoding.Default.GetString(response);
                JObject accounts = JObject.Parse(responseString);

                var amount = accounts["result"]["BTC"]["balancef"];

                return new Money(Convert.ToDecimal(amount));
            }
        }
        catch
        {
            return Money.Zero;
        }
    }

    public override string CreateNewAddress(int userId)
    {
        string cmd = "get_callback_address";
        string adminAddress = string.Empty;

        using (WebClient client = new WebClient())
        {
            var values = new NameValueCollection();
            values["version"] = "1";
            values["key"] = AppSettings.Cryptocurrencies.CoinPaymentsApiKey;
            values["format"] = "json";
            values["cmd"] = cmd;
            values["currency"] = "BTC";

            string SpecialParams = cmd + "&currency=BTC";

            client.Headers.Add("HMAC", GetSHA512HMAC(SpecialParams));

            try
            {
                var response = client.UploadValues(baseUrl, values);
                string responseString = Encoding.Default.GetString(response);
                JObject o = JObject.Parse(responseString);

                if (o["error"].ToString() == "ok")
                    adminAddress = o["result"]["address"].ToString();
                else
                    ErrorLogger.Log(o["error"].ToString(), LogType.CoinPayments);

                if (!string.IsNullOrWhiteSpace(adminAddress))
                {
                    string query = string.Format("SELECT * FROM BitcoinAddresses WHERE UserId = {0}", userId);
                    BitcoinAddress address = TableHelper.GetListFromRawQuery<BitcoinAddress>(query).FirstOrDefault();
                    if (address == null)
                    {
                        address = new BitcoinAddress();
                        address.UserId = userId;
                    }
                    address.CoinPaymentsAddress = adminAddress;
                    address.Save();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }

            return adminAddress;
        }
    }

    public override decimal GetEstimatedWithdrawalFee(decimal amount, string userAddress)
    {
        return Convert.ToDecimal(Convert.ToDouble(amount) * 0.005);  // Fixed value fee
    }

    public new string TryGetAdminAddress()
    {
        Member user = Member.Current;
        using (WebClient client = new WebClient())
        {
            string tempurl = string.Empty;

            try
            {
                //use existing address if exists
                string adminAddress = string.Empty;
                var address = TableHelper.SelectRows<BitcoinAddress>(TableHelper.MakeDictionary("UserId", user.Id))
                    .SingleOrDefault(a => !string.IsNullOrEmpty(a.CoinPaymentsAddress));

                if (address != null)
                    adminAddress = address.CoinPaymentsAddress;

                if (!string.IsNullOrEmpty(adminAddress))
                {
                    //check if exists on coinpayments.net
                    string cmd = "get_basic_info";
                    string NewAdminAddress = string.Empty;
                    var values = new NameValueCollection();
                    values["version"] = "1";
                    values["key"] = AppSettings.Cryptocurrencies.CoinPaymentsApiKey;
                    values["format"] = "json";
                    values["cmd"] = cmd;
                    string SpecialParams = cmd;
                    client.Headers.Add("HMAC", GetSHA512HMAC(SpecialParams));
                    try
                    {
                        var response = client.UploadValues(baseUrl, values);
                        string responseString = Encoding.Default.GetString(response);
                        JObject o = JObject.Parse(responseString);

                        if (o["error"].ToString() == "ok")
                            return adminAddress;
                        else
                        {
                            adminAddress = CreateNewAddress(user.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex);
                    }
                }
                else
                {
                    adminAddress = CreateNewAddress(user.Id);
                }

                return adminAddress;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                return string.Empty;
            }
        }
    }

    private static string baseUrl = "https://www.coinpayments.net/api.php";

    private static string GetSHA512HMAC(string SpecialParams)
    {
        return HashingManager.SHA512HMAC(AppSettings.Cryptocurrencies.CoinPaymentsSecretPIN,
            string.Format("version=1&key={0}&format=json&cmd=", AppSettings.Cryptocurrencies.CoinPaymentsApiKey) + SpecialParams);
    }
}