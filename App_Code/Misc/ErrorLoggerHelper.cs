using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Threading;
using ExtensionMethods;
using Prem.PTC.Payments;
using Titan.Cryptocurrencies;

namespace Prem.PTC
{
    public static class ErrorLoggerHelper
    {
        public static LogType GetTypeFromProcessor(PaymentProcessor processor)
        {
            LogType Type = LogType.Other;
            switch (processor)
            {
                case PaymentProcessor.PayPal:
                    Type = LogType.PayPal;
                    break;
                case PaymentProcessor.PerfectMoney:
                    Type = LogType.PerfectMoney;
                    break;
                case PaymentProcessor.Payza:
                    Type = LogType.Payza;
                    break;
                case PaymentProcessor.SolidTrustPay:
                    Type = LogType.SolidTrustPay;
                    break;
                case PaymentProcessor.Payeer:
                    Type = LogType.Payeer;
                    break;
                case PaymentProcessor.Neteller:
                    Type = LogType.Neteller;
                    break;
                case PaymentProcessor.AdvCash:
                    Type = LogType.AdvCash;
                    break;
                case PaymentProcessor.OKPay:
                    Type = LogType.OKPay;
                    break;
                case PaymentProcessor.Papara:
                    Type = LogType.Papara;
                    break;
                case PaymentProcessor.MPesa:
                    Type = LogType.MPesa;
                    break;
                case PaymentProcessor.MPesaAgent:
                    Type = LogType.MPesaAgent;
                    break;
                case PaymentProcessor.LocalBitcoins:
                    Type = LogType.LocalBitcoins;
                    break;
                case PaymentProcessor.Revolut:
                    Type = LogType.Revolut;
                    break;
            }
            return Type;
        }

        public static LogType GetTypeFromProcessor(CryptocurrencyAPIProvider CryptocurrencyAPIProvider)
        {
            LogType Type = LogType.Other;
            switch (CryptocurrencyAPIProvider)
            {
                case CryptocurrencyAPIProvider.CoinPayments:
                    Type = LogType.CoinPayments;
                    break;
                case CryptocurrencyAPIProvider.Coinbase:
                    Type = LogType.Coinbase;
                    break;
                case CryptocurrencyAPIProvider.Blocktrail:
                    Type = LogType.Blocktrail;
                    break;
            }
            return Type;
        }

        public static bool IsPayment(LogType Type)
        {
            switch (Type)
            {
                case LogType.PayPal:
                case LogType.PerfectMoney:
                case LogType.Payza:
                case LogType.SolidTrustPay:
                case LogType.Payeer:
                case LogType.Blockchain:
                case LogType.Blockio:
                case LogType.CoinPayments:
                case LogType.Coinbase:
                case LogType.Neteller:
                case LogType.AdvCash:
                case LogType.OKPay:
                case LogType.Papara:
                case LogType.MPesa:
                case LogType.LocalBitcoins:
                case LogType.MPesaAgent:
                case LogType.Revolut:
                case LogType.Blocktrail:
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum LogType
    {
        Exceptions = 0, //n
        RefTrack   = 1, //n
        Other      = 2, //n
        CRON       = 3, //n
        Publisher = 4,

        OfferWalls     = 10,
        CPAGPTNetworks = 11,
        HandlerHit     = 12,

        PayPal         = 20,
        PerfectMoney   = 21,
        Payza          = 22,
        SolidTrustPay  = 23,
        Payeer         = 24,
        Blockchain     = 25,
        Blockio        = 26,
        CoinPayments   = 27,
        Coinbase       = 28,
        Neteller       = 29,
        AdvCash        = 30,
        OKPay          = 31,
        Papara         = 32,
        MPesa          = 33,
        LocalBitcoins  = 34,
        MPesaAgent     = 35,
        Revolut        = 36,
        Blocktrail     = 37
    }
}