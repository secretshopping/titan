using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Cryptocurrencies;

public static class BitcoinIPNManager
{
    public static void AddIPNLog(DateTime operationDate, OperationType operationType, int? confirmations, int userId, string targetAddress, decimal? bitcoinAmount, Money moneyAmount, CryptocurrencyAPIProvider BitcoinAPIProvider, bool isExecuted = false)
    {
        if (operationType == OperationType.Deposit)
        {
            BlockioIPN ipn = new BlockioIPN();
            ipn.OperationDate = operationDate;
            ipn.OperationType = operationType;

            if (confirmations.HasValue)
                ipn.Confirmations = confirmations.Value;
            ipn.UserId = userId;
            ipn.TargetAddress = targetAddress;

            if (bitcoinAmount.HasValue)
                ipn.BitcoinAmount = bitcoinAmount.Value;

            if (moneyAmount != null)
                ipn.MoneyAmount = moneyAmount;

            ipn.IsExecuted = isExecuted;
            ipn.BitcoinAPIProvider = (int)BitcoinAPIProvider;
            ipn.Save();
        }
    }
}