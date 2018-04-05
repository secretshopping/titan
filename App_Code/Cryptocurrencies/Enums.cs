using Prem.PTC;
using Prem.PTC.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Titan.Cryptocurrencies
{
    public enum CryptocurrencyType
    {
        BTC = 0,
        ETH = 1,
        XRP = 2,

        ERC20Token = 99,
        ERCFreezed = 100
    }

    public enum DepositTargetBalance
    {
        [Description("Purchase Balance")]
        PurchaseBalance = 2,
        [Description("Cash Balance")]
        CashBalance = 3,
        [Description("Wallet")]
        Wallet = 4
    }

    public enum WithdrawalSourceBalance
    {
        [Description("Main Balance")]
        MainBalance = 0,
        [Description("Wallet")]
        Wallet = 1
    }

    public enum CryptocurrencyAPIProvider
    {
        CoinPayments = 3,
        Coinbase = 4,
        Blocktrail = 5,
        Parity = 6
    }

    public enum OperationType
    {
        Null = 0,
        Deposit = 1,
        Withdrawal = 2
    }

    public enum CoinbaseAddressesPolicy
    {
        CoinbaseEmailOrBTCWallet = 0,
        CoinbaseEmail = 1,
        BTCWallet = 2,
    }

    public enum WithdrawalFeePolicy
    {
        Constant = 0,
        Packs = 1
    }

}