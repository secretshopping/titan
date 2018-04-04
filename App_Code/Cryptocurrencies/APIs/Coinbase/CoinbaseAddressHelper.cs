using Prem.PTC;
using Prem.PTC.Payments;
using Prem.PTC.Utils;
using Resources;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Titan.Cryptocurrencies;

public class CoinbaseAddressHelper
{
    /// <summary>
    /// emailOrWallet set only when AddressesPolicy == EmailOrBTCWallet. Set 1 for email, 2 for wallet
    /// </summary>
    /// <param name="emailOrWallet"></param>
    /// <returns></returns>
    public static string GetAddress(int userId, int emailOrWallet = 0)
    {
        switch (AppSettings.Cryptocurrencies.CoinbaseAddressesPolicy)
        {
            case CoinbaseAddressesPolicy.BTCWallet:
                return TryGetBTCWallet(userId);
            case CoinbaseAddressesPolicy.CoinbaseEmail:
                return TryGetEmailAddress(userId);
            case CoinbaseAddressesPolicy.CoinbaseEmailOrBTCWallet:
                if(emailOrWallet == 1)
                    return TryGetEmailAddress(userId); 
                else if(emailOrWallet == 2)
                    return TryGetBTCWallet(userId);
                break;
        }

        //for compilator 
        return string.Empty;
    }    

    private static string TryGetBTCWallet(int userId)
    {
        var currentAddress = GetBTCWallet(userId);

        if (currentAddress != null)
            return currentAddress.Address;

        return string.Empty;
    }

    private static CryptocurrencyWithdrawalAddress GetBTCWallet(int userId)
    {
        return CryptocurrencyWithdrawalAddress.GetAddress(userId, CryptocurrencyType.BTC);
    }

    private static string TryGetEmailAddress(int userId)
    {
        var coinbaseAddress = GetEmailAddress(userId);

        if (coinbaseAddress != null)
            return coinbaseAddress.PaymentAddress;

        return string.Empty;
    }

    private static UsersPaymentProcessorsAddress GetEmailAddress(int userId)
    {
        return UsersPaymentProcessorsAddress.GetAddress(userId, new PaymentProcessorInfo(PaymentProcessor.Coinbase)); ;
    }

    /// <summary>
    /// emailOrWallet set only when AddressesPolicy == EmailOrBTCWallet. Set 1 for email, 2 for wallet
    /// </summary>
    /// <param name="emailOrWallet"></param>
    /// <returns></returns>
    public static string TryToGetAndUseAddress(int userId, int emailOrWallet = 0)
    {

        switch (AppSettings.Cryptocurrencies.CoinbaseAddressesPolicy)
        {
            case CoinbaseAddressesPolicy.CoinbaseEmail:
                return GetAndUseEmail(userId);
            case CoinbaseAddressesPolicy.BTCWallet:
                return GetAndUseBTCWallet(userId);
            case CoinbaseAddressesPolicy.CoinbaseEmailOrBTCWallet:
                if(emailOrWallet == 1)
                    return GetAndUseEmail(userId);
                else if(emailOrWallet == 2)
                    return GetAndUseBTCWallet(userId);
                return string.Empty;
            default:
                return string.Empty;
        }
    }

    private static string GetAndUseEmail(int userId)
    {
        var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

        var daysToActivate = BtcCryptocurrency.ActivateUserAddressAfterDays;
        var coinbaseAddress = GetEmailAddress(userId);

        if (coinbaseAddress == null)
            throw new MsgException(U6000.ADDBTCADDRESSFIRST);

        if (coinbaseAddress.LastChanged.AddDays(daysToActivate) > AppSettings.ServerTime)
            throw new MsgException(string.Format(U6000.CANTWITHDRAWBTCUNTIL, (coinbaseAddress.LastChanged.AddDays(daysToActivate) - AppSettings.ServerTime).ToFriendlyDisplay(2)));

        return coinbaseAddress.PaymentAddress;
    }

    private static string GetAndUseBTCWallet(int userId)
    {
        var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

        var daysToActivate = BtcCryptocurrency.ActivateUserAddressAfterDays;
        var btcAddress = CryptocurrencyWithdrawalAddress.GetAddress(userId, CryptocurrencyType.BTC);

        if (btcAddress == null)
            throw new MsgException(U6000.ADDBTCADDRESSFIRST);
        
        if (!btcAddress.IsNew && btcAddress.DateAdded.AddDays(daysToActivate) > AppSettings.ServerTime)
            throw new MsgException(string.Format(U6000.CANTWITHDRAWBTCUNTIL, (btcAddress.DateAdded.AddDays(daysToActivate) - AppSettings.ServerTime).ToFriendlyDisplay(2)));

        return btcAddress.Address.Replace(" ", String.Empty);
    }
}