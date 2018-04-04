using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using System.Text;
using System.Web;

namespace Titan.Cryptocurrencies
{
    [Serializable]
    public abstract class CryptocurrencyApi : CryptocurrencyApiTable
    {
        public abstract CryptocurrencyAPIProvider Provider { get; }

        public abstract Money GetAccountBalance();

        public abstract void TryWithDrawCryptocurrencyFromWallet(decimal amountInCryptocurrency, string userAddress, CryptocurrencyType cryptocurrencyType);

        public abstract decimal GetEstimatedWithdrawalFee(decimal amount, string userAddress);

        public abstract string CreateNewAddress(int userId);

        public abstract bool AllowToUsePaymentButtons();

        #region Constructors

        public CryptocurrencyApi(int tableObjectId) : base(tableObjectId) { }

        public CryptocurrencyApi(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public static CryptocurrencyApi Get(CryptocurrencyAPIProvider cryptocurrencyApiProvider)
        {
            return CryptocurrencyApiFactory.Get(cryptocurrencyApiProvider);
        }

        #endregion

        //public static string UpperCaseUrlEncode(string s)
        //{
        //    char[] temp = HttpUtility.UrlEncode(s).ToCharArray();
        //    for (int i = 0; i < temp.Length - 2; i++)
        //    {
        //        if (temp[i] == '%')
        //        {
        //            temp[i + 1] = char.ToUpper(temp[i + 1]);
        //            temp[i + 2] = char.ToUpper(temp[i + 2]);
        //        }
        //    }
        //    return new string(temp);
        //}

        public string TryGetAdminAddress()
        {
            try
            {
                string adminAddress = BitcoinAddress.GetAddress(Member.CurrentId, ApiType);

                if (String.IsNullOrEmpty(adminAddress))
                    adminAddress = CreateNewAddress(Member.CurrentId);

                return adminAddress;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                return string.Empty;
            }
        }

        public static bool IsAdministratorAddress(string address, CryptocurrencyAPIProvider cryptocurrencyAPI)
        {
            var adminAddresses = BitcoinAddress.GetAddress(address, cryptocurrencyAPI);

            if (adminAddresses != null)
                return true;

            return false;
        }

        public ButtonGenerationStrategy GetStrategy()
        {
            //Only 2 CryptocurrencyAPIProviders available
            if (Provider == CryptocurrencyAPIProvider.CoinPayments)
                return new CoinPaymentsButtonGenerationStrategy();

            return new CoinbaseButtonGenerationStrategy();
        }

        [Obsolete]
        public static PaymentProcessor[] AllBTCProcessors
        {
            get
            {
                return new PaymentProcessor[] { PaymentProcessor.Coinbase, PaymentProcessor.CoinPayments, PaymentProcessor.Blocktrail };
            }
        }


    }
}