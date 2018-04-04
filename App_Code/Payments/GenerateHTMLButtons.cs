using System;
using System.Collections.Generic;
using Prem.PTC;
using Prem.PTC.Payments;
using System.Web.UI.WebControls;
using System.Text;
using Prem.PTC.Members;
using Resources;
using Titan.Cryptocurrencies;

public class GenerateHTMLButtons
{
    public static string GetPaymentButton(BaseButtonGenerator bg, Type t)
    {
        var sb = new StringBuilder();       
        var instance = Activator.CreateInstance(t);
        var gateway = (PaymentAccountDetails)PaymentAccountDetails.RunStaticMethod(t, "GetFirstIncomeGateway");

        if (gateway != null && gateway.AccountType != "MPesaAgent") //MPesaAgent do not support payment buttons
        {
            bg.Strategy = gateway.GetStrategy();
            sb.Append(bg.Generate());
        }

        return string.IsNullOrEmpty(sb.ToString()) ? U6011.NOACTIVEPAYMENTPROCESSORS : sb.ToString();
    }

    public static string GetPaymentButton(BaseButtonGenerator bg, CryptocurrencyType t)
    {
        var cryptocurrency = CryptocurrencyFactory.Get(t);
        var sb = new StringBuilder(); 

        if (cryptocurrency.DepositEnabled && CryptocurrencyApiFactory.Get(cryptocurrency.DepositApiProcessor).AllowToUsePaymentButtons())        
            sb.Append(GetBtcButton(bg));

        return string.IsNullOrEmpty(sb.ToString()) ? U6011.NOACTIVEPAYMENTPROCESSORS : sb.ToString();
    }

    public static string GetPaymentButtons(BaseButtonGenerator bg)
    {
        var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

        int buttonsPresent = 0;
        StringBuilder sb = new StringBuilder();

        foreach (var t in PaymentAccountDetails.PaymentAccountDetailsClasses)
        {
            var instance = Activator.CreateInstance(t);

            var gateway = (PaymentAccountDetails)PaymentAccountDetails.RunStaticMethod(t, "GetFirstIncomeGateway");

            if (gateway != null && gateway.AccountType != "MPesaAgent") //MPesaAgent do not support payment buttons
            {
                bg.Strategy = gateway.GetStrategy();
                sb.Append(bg.Generate());
                buttonsPresent++;
            }
        }

        if (BtcCryptocurrency.DepositEnabled && CryptocurrencyApiFactory.Get(BtcCryptocurrency.DepositApiProcessor).AllowToUsePaymentButtons())
        {
            sb.Append(GetBtcButton(bg));
            buttonsPresent++;
        }

        if (buttonsPresent == 0)
            return U6011.NOACTIVEPAYMENTPROCESSORS;

        return sb.ToString();
    }

    public static string GetBtcButton(BaseButtonGenerator bg)
    {
        StringBuilder sb = new StringBuilder();
        var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

        if (BtcCryptocurrency.DepositEnabled && CryptocurrencyApiFactory.Get(BtcCryptocurrency.DepositApiProcessor).AllowToUsePaymentButtons())
        {        
            bg.Strategy = CryptocurrencyApiFactory.Get(BtcCryptocurrency.DepositApiProcessor).GetStrategy();
            sb.Append(bg.Generate());
        }

        return sb.ToString();
    }


    public static ListItem[] TransferFromItems
    {
        get
        {
            List<ListItem> list = new List<ListItem>();
            var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);
            var TokenCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);

            if (AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled || AppSettings.Payments.MainToMarketplaceBalanceEnabled 
                || AppSettings.Payments.TransferMainInAdBalanceEnabled || AppSettings.Payments.TransferFromMainBalanceToTokenWalletEnabled)
                list.Add(new ListItem("Main balance", "Main balance"));

            if (((AppSettings.Payments.CommissionToMainBalanceEnabled && !TitanFeatures.UserCommissionToMainBalanceEnabled)
                || (TitanFeatures.UserCommissionToMainBalanceEnabled && Member.Current.CommissionToMainBalanceEnabled)
                || AppSettings.Payments.CommissionToAdBalanceEnabled || TitanFeatures.isAri) && Member.Current.CheckAccessCustomizeTradeOwnSystem)
                list.Add(new ListItem("Commission Balance", "Commission Balance"));

            if (!TitanFeatures.isAri && AppSettings.Payments.CashBalanceEnabled && AppSettings.Payments.CashToAdBalanceEnabled)
                list.Add(new ListItem("Cash balance", "Cash balance"));

            if (TitanFeatures.IsRevolca)
                list.Add(new ListItem("Purchase balance", "Purchase balance"));

            foreach (var t in PaymentAccountDetails.PaymentAccountDetailsClasses)
            {
                var instance = Activator.CreateInstance(t);

                if (PaymentAccountDetails.RunStaticMethod(t, "GetFirstIncomeGateway") != null)
                {
                    string pp = t.GetProperty("AccountType").GetValue(instance, null).ToString();
                    list.Add(new ListItem(pp, pp));
                }
            }

            if (BtcCryptocurrency.DepositEnabled && CryptocurrencyApiFactory.Get(BtcCryptocurrency.DepositApiProcessor).AllowToUsePaymentButtons())
            {
                var btc = BtcCryptocurrency.DepositApiProcessor.ToString();
                list.Add(new ListItem(btc, btc));
            }

            if (AppSettings.Payments.TransferFromBTCWalletToPurchaseBalanceEnabled)
                list.Add(new ListItem("BTC Wallet", "BTC Wallet"));

            if (AppSettings.Payments.TransferFromTokenWalletToPurchaseBalanceEnabled)
                list.Add(new ListItem(String.Format("{0} Wallet", TokenCryptocurrency.Code), String.Format("{0} Wallet", TokenCryptocurrency.Code)));

            return list.ToArray();
        }
    }

    public static ListItem[] TransferBalanceFromItems
    {
        get
        {
            List<ListItem> list = new List<ListItem>();
            var TokenCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);

            if (AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled || AppSettings.Payments.MainToMarketplaceBalanceEnabled
                || AppSettings.Payments.TransferMainInAdBalanceEnabled || AppSettings.Payments.TransferFromMainBalanceToTokenWalletEnabled)
                list.Add(new ListItem("Main balance", "Main balance"));

            if (((AppSettings.Payments.CommissionToMainBalanceEnabled && !TitanFeatures.UserCommissionToMainBalanceEnabled)
                || (TitanFeatures.UserCommissionToMainBalanceEnabled && Member.Current.CommissionToMainBalanceEnabled)
                || AppSettings.Payments.CommissionToAdBalanceEnabled || TitanFeatures.isAri) && Member.Current.CheckAccessCustomizeTradeOwnSystem)
                list.Add(new ListItem("Commission Balance", "Commission Balance"));

            if (!TitanFeatures.isAri && AppSettings.Payments.CashBalanceEnabled && AppSettings.Payments.CashToAdBalanceEnabled)
                list.Add(new ListItem("Cash balance", "Cash balance"));

            if (TitanFeatures.IsRevolca)
                list.Add(new ListItem("Purchase balance", "Purchase balance"));
            
            if (AppSettings.Payments.TransferFromBTCWalletToPurchaseBalanceEnabled)
                list.Add(new ListItem("BTC Wallet", "BTC Wallet"));

            if (AppSettings.Payments.TransferFromTokenWalletToPurchaseBalanceEnabled)
                list.Add(new ListItem(String.Format("{0} Wallet", TokenCryptocurrency.Code), String.Format("{0} Wallet", TokenCryptocurrency.Code)));

            return list.ToArray();
        }
    }

    public static ListItem[] DepositBalanceFromItems
    {
        get
        {
            List<ListItem> list = new List<ListItem>();
            var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

            foreach (var t in PaymentAccountDetails.PaymentAccountDetailsClasses)
            {
                var instance = Activator.CreateInstance(t);

                if (PaymentAccountDetails.RunStaticMethod(t, "GetFirstIncomeGateway") != null)
                {
                    string pp = t.GetProperty("AccountType").GetValue(instance, null).ToString();
                    list.Add(new ListItem(pp, pp));
                }
            }

            if (BtcCryptocurrency.DepositEnabled && CryptocurrencyApiFactory.Get(BtcCryptocurrency.DepositApiProcessor).AllowToUsePaymentButtons())
            {
                var btc = BtcCryptocurrency.DepositApiProcessor.ToString();
                list.Add(new ListItem(btc, btc));
            }
            return list.ToArray();
        }
    }


    public static ListItem[] CashoutFromItems
    {
        get
        {
            List<ListItem> list = new List<ListItem>();

            foreach (var t in PaymentAccountDetails.PaymentAccountDetailsClasses)
            {
                var instance = Activator.CreateInstance(t);

                if ((bool)PaymentAccountDetails.RunStaticMethod(t, "HasCashoutGateway"))
                {
                    string pp = t.GetProperty("AccountType").GetValue(instance, null).ToString();
                    list.Add(new ListItem(
                        String.Format("<img src=\"../Images/OneSite/TransferMoney/{0}.png\" class=\"imagemiddle\" style=\"padding:0 3px\" /> {0}", pp), pp));
                }

            }

            //Custom Payment Processors
            //Use IDs = Int32
            var customPP = TableHelper.SelectRows<CustomPayoutProcessor>(TableHelper.MakeDictionary("StatusInt", (int)UniversalStatus.Active));
            foreach (var element in customPP)
            {
                list.Add(new ListItem("<img src=\"" + element.ImageURL + "\" class=\"imagemiddle\" style=\"padding:0 3px\" />" + element.Name, element.Id.ToString()));
            }
            
            return list.ToArray();
        }
    }

    public static ListItem[] BTCFromItems
    {
        get
        {
            List<ListItem> list = new List<ListItem>();
            var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

            if (BtcCryptocurrency.DepositEnabled)
                list.Add(new ListItem("<img src=\'../Images/OneSite/TransferMoney/GoCoin.png\' class=\'imagemiddle\' style=\'padding:0 3px\' />", "Wallet"));

            return list.ToArray();
        }
    }

    public static ListItem[] BTCToItems
    {
        get
        {
            List<ListItem> list = new List<ListItem>();
            var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

            if (BtcCryptocurrency.DepositEnabled)
            {

                if (BtcCryptocurrency.DepositTarget == DepositTargetBalance.PurchaseBalance)
                    list.Add(new ListItem("<img src='../Images/OneSite/TransferMoney/Purchase Balance.png' class='imagemiddle' style='padding:0 3px' />", "Purchase Balance"));

                if (BtcCryptocurrency.DepositTarget == DepositTargetBalance.CashBalance)
                    list.Add(new ListItem("<img src='../Images/OneSite/TransferMoney/Cash Balance.png' class='imagemiddle' style='padding:0 3px' />", "Cash Balance"));

                if (BtcCryptocurrency.DepositTarget == DepositTargetBalance.Wallet)
                    list.Add(new ListItem("<img src='../Images/OneSite/TransferMoney/Cash Balance.png' class='imagemiddle' style='padding:0 3px' />", "BTC Wallet"));

            }

            return list.ToArray();
        }
    }

}