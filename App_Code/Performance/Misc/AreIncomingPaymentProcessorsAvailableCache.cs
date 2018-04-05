using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Prem.PTC.Payments;
using Titan.Cryptocurrencies;

public class AreIncomingPaymentProcessorsAvailableCache : CacheBase
{
    protected override string Name { get { return "AreIncomingPaymentProcessorsAvailableCache"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(1); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.BelowNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override object GetDataFromSource()
    {
        var gateways = PaymentAccountDetails.AllGateways;

        foreach (var gateway in gateways)
        {
            if (gateway.IsActive && (gateway.Cashflow == GatewayCashflowDirection.ToGate ||
                gateway.Cashflow == GatewayCashflowDirection.Both))
                return true;
        }

        var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

        if (BtcCryptocurrency.DepositEnabled && CryptocurrencyApiFactory.Get(BtcCryptocurrency.DepositApiProcessor).AllowToUsePaymentButtons())
            return true;

        return false;
    }
}
