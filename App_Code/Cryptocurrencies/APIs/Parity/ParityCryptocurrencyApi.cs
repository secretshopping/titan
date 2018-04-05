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

public class ParityCryptocurrencyApi : CryptocurrencyApi
{
    protected static CryptocurrencyAPIProvider cryptocurrencyApiProvider = CryptocurrencyAPIProvider.Parity;

    public ParityCryptocurrencyApi() : base((int)cryptocurrencyApiProvider) { }
    public override CryptocurrencyAPIProvider Provider { get { return cryptocurrencyApiProvider; } }
    public override bool AllowToUsePaymentButtons() { return false; }

    public override void TryWithDrawCryptocurrencyFromWallet(decimal amountInCryptocurrency, string userAddress, CryptocurrencyType cryptocurrencyType)
    {
        throw new NotImplementedException();
    }

    public override Money GetAccountBalance()
    {
        try
        {
            throw new NotImplementedException();
        }
        catch
        {
            return Money.Zero;
        }
    }

    public override string CreateNewAddress(int userId)
    {
        throw new NotImplementedException();
    }

    public override decimal GetEstimatedWithdrawalFee(decimal amount, string userAddress)
    {
        return new decimal(0);
    }

    public new string TryGetAdminAddress()
    {
        Member user = Member.Current;
        using (WebClient client = new WebClient())
        {
            string tempurl = string.Empty;

            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                return string.Empty;
            }
        }
    }
}