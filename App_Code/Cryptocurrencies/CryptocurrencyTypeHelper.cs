using Prem.PTC;
using Prem.PTC.Payments;
using System;
using System.Collections.Generic;

namespace Titan.Cryptocurrencies
{
    public class CryptocurrencyTypeHelper
    {
        public static BalanceType ConvertToBalanceType(CryptocurrencyType type)
        {
            switch (type)
            {
                case CryptocurrencyType.BTC:
                    return BalanceType.BTC;

                case CryptocurrencyType.ETH:
                    return BalanceType.ETH;

                case CryptocurrencyType.XRP:
                    return BalanceType.XRP;

                case CryptocurrencyType.ERC20Token:
                    return BalanceType.Token;

                case CryptocurrencyType.ERCFreezed:
                    return BalanceType.FreezedToken;

                default:
                    return BalanceType.BTC;
            }
        }

        public static PaymentProcessor ConvertToPaymentProcessor(CryptocurrencyType type)
        {
            switch (type)
            {
                case CryptocurrencyType.BTC:
                    return PaymentProcessor.BTC;

                case CryptocurrencyType.ETH:
                    return PaymentProcessor.ETH;

                case CryptocurrencyType.XRP:
                    return PaymentProcessor.ETH;

                case CryptocurrencyType.ERC20Token:
                    return PaymentProcessor.Null;

                default:
                    return PaymentProcessor.BTC;
            }
        }

        public static CryptocurrencyType ConvertToCryptocurrencyType(BalanceType type)
        {
            switch (type)
            {
                case BalanceType.BTC:
                    return CryptocurrencyType.BTC;

                case BalanceType.ETH:
                    return CryptocurrencyType.ETH;

                case BalanceType.XRP:
                    return CryptocurrencyType.XRP;

                case BalanceType.Token:
                    return CryptocurrencyType.ERC20Token;

                case BalanceType.FreezedToken:
                    return CryptocurrencyType.ERCFreezed;

                default:
                    return CryptocurrencyType.BTC;
            }
        }

        public static int GetDecimalPlaces(CryptocurrencyType type)
        {
            switch (type)
            {
                case CryptocurrencyType.BTC:
                    return 8;

                case CryptocurrencyType.ETH:
                    return 18;

                case CryptocurrencyType.XRP:
                    return 6;

                case CryptocurrencyType.ERC20Token:
                    return CoreSettings.GetMaxDecimalPlaces(CurrencyType.Crypto);

                default:
                    return 8;
            }
        }
    }
}