using Prem.PTC;
using Prem.PTC.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtensionMethods;

public abstract class PaymentHandler
{
    protected HttpContext context;
    protected PaymentProcessor processor;

    public PaymentHandler(HttpContext context, PaymentProcessor processor)
    {
        this.context = context;
        this.processor = processor;
    }

    public void Process()
    {
        try
        {
            Log(context.Request.ToRawString());
            ProcessRequest();
        }
        catch (Exception ex) { ErrorLogger.Log(ex); }
    }

    public abstract void ProcessRequest();

    protected void Log(string message, string request = "")
    {
        LogType Type = LogType.Other;

        if (this is PayeerHandler)
            Type = LogType.Payeer;
        if (this is SolidTrustPayHandler)
            Type = LogType.SolidTrustPay;
        if (this is NetellerHandler)
            Type = LogType.Neteller;
        if (this is OKPayHandler)
            Type = LogType.OKPay;
        if (this is AdvCashHandler)
            Type = LogType.AdvCash;
        if (this is PaparaHandler)
            Type = LogType.Papara;
        if (this is MPesaHandler)
            Type = LogType.MPesa;
        if (this is LocalBitcoinsHandler)
            Type = LogType.LocalBitcoins;
        if (this is MPesaSapamaHandler)
            Type = LogType.MPesaAgent;
        if (this is RevolutHandler)
            Type = LogType.Revolut;
        if (this is CoinPaymentsHandler)
            Type = LogType.CoinPayments;
        if (this is CoinbaseHandler)
            Type = LogType.Coinbase;
        if (this is BlocktrailHandler)
            Type = LogType.Blocktrail;

        ErrorLogger.Log(message + " (" + request + ')', Type);
    }

    #region Checks

    public static CompletedPaymentLog GetTransaction(PaymentProcessor processor, string id)
    {
        List<CompletedPaymentLog> AllTransations = TableHelper.SelectRows<CompletedPaymentLog>(TableHelper.MakeDictionary("PaymentProcessor", (int)processor));

        foreach (var trans in AllTransations)
            if (trans.TransactionId.Trim() == id.Trim())
                return trans;

        return null;
    }

    protected void CheckIfNotDoneYet(string id)
    {
        List<CompletedPaymentLog> AllTransations = TableHelper.SelectRows<CompletedPaymentLog>(TableHelper.MakeDictionary("PaymentProcessor", (int)processor));

        foreach (var trans in AllTransations)
            if (trans.TransactionId.Trim() == id.Trim())
                throw new MsgException("This transaction has already been proceed.");
    }

    protected void CheckHash(string local, string remote)
    {
        if (local.Trim().ToLower() != remote.Trim().ToLower())
            throw new MsgException("Security hashes don't match.");
    }

    protected void CheckIP(string allowedIPs)
    {
        string RequestIP = IP.Current;

        if (!allowedIPs.Contains(RequestIP))
            throw new MsgException("Request sent from unallowed IP");
    }

    protected void CheckMerchant(bool exists)
    {
        if (!exists)
            throw new MsgException("Payment sent to merchant who doesn't exists.");
    }

    protected void CheckStatus(string input, string expected)
    {
        if (input.Trim().ToLower() != expected.Trim().ToLower())
            throw new MsgException("Bad status: " + input);
    }

    protected void CheckCurrency(string input)
    {
        if (input.Trim().ToLower() != AppSettings.Site.CurrencyCode.Trim().ToLower())
            throw new MsgException("Payment sent in the unsupported currency: " + input);
    }

    public static void CheckIfNotDoneYet(PaymentProcessor processor, string id)
    {
        List<CompletedPaymentLog> AllTransations =
                TableHelper.SelectRows<CompletedPaymentLog>(TableHelper.MakeDictionary("PaymentProcessor", (int)processor));

        foreach (var trans in AllTransations)
            if (trans.TransactionId.Trim() == id.Trim())
                throw new MsgException("This transaction has been already completed.");
    }

    #endregion
}