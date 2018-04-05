using System;
using System.Collections.Generic;
using System.Web.Caching;
using Prem.PTC.Payments;
using Titan.Cryptocurrencies;
using System.Linq;

public class AvailableForDepositsPaymentProcessors : CacheBase
{
    protected override string Name { get { return "AvailableForDepositsPaymentProcessors"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(1); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.BelowNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override bool CachedInAdminPanel { get { return true; } }  
    protected override object GetDataFromSource()
    {
        var gateways = PaymentAccountDetails.AllGateways;
        var availableGateways = new HashSet<PaymentProcessor>();

        foreach (var gateway in gateways)
            if (gateway.IsActive && (gateway.Cashflow == GatewayCashflowDirection.ToGate || gateway.Cashflow == GatewayCashflowDirection.Both))
                availableGateways.Add(gateway.GetProcessorType());

        var cryptocurrencies = CryptocurrencyFactory.GetAllAvailable();
        foreach (var crypto in cryptocurrencies)        
            if (crypto.DepositApiProcessor != 0 && CryptocurrencyApiFactory.Get(crypto.DepositApiProcessor).AllowToUsePaymentButtons())
            {
                if (crypto.DepositApiProcessor == CryptocurrencyAPIProvider.CoinPayments)
                    availableGateways.Add(PaymentProcessor.CoinPayments);
                if (crypto.DepositApiProcessor == CryptocurrencyAPIProvider.Coinbase)
                    availableGateways.Add(PaymentProcessor.Coinbase);
            }

        return availableGateways.Distinct().ToList();
    }
}
