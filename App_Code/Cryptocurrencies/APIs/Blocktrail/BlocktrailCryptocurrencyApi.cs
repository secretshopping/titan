using System;
using System.Collections.Generic;
using System.Linq;
using Prem.PTC;
using Titan.Cryptocurrencies;

public class BlocktrailCryptocurrencyApi : CryptocurrencyApi
{
    protected static CryptocurrencyAPIProvider cryptocurrencyApiProvider = CryptocurrencyAPIProvider.Blocktrail;

    public BlocktrailCryptocurrencyApi() : base((int)cryptocurrencyApiProvider) { }
    public override CryptocurrencyAPIProvider Provider { get { return cryptocurrencyApiProvider; } }
    public override bool AllowToUsePaymentButtons() { return false; }

    public override void TryWithDrawCryptocurrencyFromWallet(decimal amountInCryptocurrency, string userAddress, CryptocurrencyType cryptocurrencyType)
    {
        try
        {
            BlocktrailAPI api = new BlocktrailAPI(AppSettings.Cryptocurrencies.BlocktrailAPIKey, AppSettings.Cryptocurrencies.BlocktrailAPIKeySecret);
            api.SendTransaction(userAddress, amountInCryptocurrency);
        }
        catch (Exception ex)
        {
            throw new MsgException(ex.Message);
        }
    }

    public override Money GetAccountBalance()
    {
        try
        {
            BlocktrailAPI api = new BlocktrailAPI(AppSettings.Cryptocurrencies.BlocktrailAPIKey, AppSettings.Cryptocurrencies.BlocktrailAPIKeySecret);
            return api.GetBalance();
        }
        catch (Exception)
        {
            return Money.Zero;
        }
    }

    public void TryWithDrawBitcoinsFromWalletInBatchTransaction(List<KeyValuePair<string, decimal>> addressesWithAmounts)
    {
        try
        {
            BlocktrailAPI api = new BlocktrailAPI(AppSettings.Cryptocurrencies.BlocktrailAPIKey, AppSettings.Cryptocurrencies.BlocktrailAPIKeySecret);
            api.SendTransactions(addressesWithAmounts);
        }
        catch (Exception ex)
        {
            throw new MsgException(ex.Message);
        }
    }

    public override string CreateNewAddress(int userId)
    {
        BlocktrailAPI api = new BlocktrailAPI(AppSettings.Cryptocurrencies.BlocktrailAPIKey, AppSettings.Cryptocurrencies.BlocktrailAPIKeySecret);

        string adminAddress = string.Empty;

        try
        {
            adminAddress = api.GenerateBTCAddress();

            if (!string.IsNullOrWhiteSpace(adminAddress))
            {
                string query = string.Format("SELECT * FROM BitcoinAddresses WHERE UserId = {0}", userId);
                BitcoinAddress address = TableHelper.GetListFromRawQuery<BitcoinAddress>(query).FirstOrDefault();
                if (address == null)
                {
                    address = new BitcoinAddress();
                    address.UserId = userId;
                }
                address.BlocktrailAddress = adminAddress;
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
        try
        {
            if (TitanFeatures.IsAhmed) //He only wants to use his own fee
                return new decimal(0);

            BlocktrailAPI api = new BlocktrailAPI(AppSettings.Cryptocurrencies.BlocktrailAPIKey, AppSettings.Cryptocurrencies.BlocktrailAPIKeySecret);
            return api.GetFees(userAddress, amount);
        }
        catch (Exception ex)
        {
            throw new MsgException(ex.Message);
        }
    }




}