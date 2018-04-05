using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Resources;
using MarchewkaOne.Titan.Balances;
using Titan.Cryptocurrencies;

public class TransferHelper
{
    static TransferHelperCache cache = new TransferHelperCache();
    static Dictionary<string, List<string>> rulesFrom;

    public static string TryInvokeTransfer(string transferFrom, string transferTo, Money amount, Member user, ref bool htmlResponse)
    {
        var TokenCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);

        //Anti-Injection Fix
        if (amount <= new Money(0))
            throw new MsgException(L1.ERROR);

        if (!TransferHelper.IsAllowed(transferFrom, transferTo))
            throw new MsgException("This transfer is not allowed.");

        if (transferFrom == "MPesa" && String.IsNullOrWhiteSpace(user.GetPaymentAddress(PaymentProcessor.MPesa)))
            throw new MsgException(U6010.ACCOUNTIDREQUIRED);

        AppSettings.DemoCheck();
        htmlResponse = false;

        if (transferFrom == "Main balance")
        {
            // Option 1: From Main Balance (always available)

            if (amount > user.MainBalance)
                throw new MsgException(L1.NOTENOUGHFUNDS);

            user.SubtractFromMainBalance(amount, "Transfer from Main Balance");
            user.MoneyTransferredFromMainBalance += amount;

            if (transferTo == "Traffic balance")
                user.AddToTrafficBalance(amount, "Transfer from Main Balance");

            else if (transferTo == "Purchase balance" && AppSettings.Payments.TransferMainInAdBalanceEnabled)
                user.AddToPurchaseBalance(amount, "Transfer from Main Balance");

            else if (transferTo == "Cash balance")
                user.AddToCashBalance(amount, "Transfer from Main Balance");

            else if (transferTo == TokenCryptocurrency.Code + " Wallet" && AppSettings.Payments.TransferFromMainBalanceToTokenWalletEnabled)
            {              
                //Let's calculate transfer amount using current token rate
                decimal tokenValue = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token).ConvertFromMoney(amount);
                user.AddToCryptocurrencyBalance(CryptocurrencyType.ERC20Token, tokenValue, "Transfer from Main Balance");
            }

            else throw new MsgException("You must select your target account / this transfer is not available.");

            user.Save();
            AddHistoryAndTryAchievement(amount, user, "Main balance", transferTo);
            return U3501.TRANSFEROK;
        }
        else if (transferFrom == "Cash balance")
        {
            if (AppSettings.Payments.CashBalanceEnabled && AppSettings.Payments.CashToAdBalanceEnabled)
            {
                if (amount > user.CashBalance)
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                user.SubtractFromCashBalance(amount, "Transfer from Cash Balance");

                if (transferTo == "Purchase balance")
                {
                    user.AddToPurchaseBalance(amount, "Transfer from Cash Balance");
                }

                user.SaveBalances();
                return U3501.TRANSFEROK;
            }
            else throw new MsgException("You cannot transfer from Cash Balance");
        }
        else if (transferFrom == "Commission Balance")
        {
            if (((AppSettings.Payments.CommissionToMainBalanceEnabled)
                || (TitanFeatures.UserCommissionToMainBalanceEnabled && user.CommissionToMainBalanceEnabled)
                || (AppSettings.Payments.CommissionToAdBalanceEnabled) || TitanFeatures.isAri) && user.CheckAccessCustomizeTradeOwnSystem)
            {
                if (amount > user.CommissionBalance)
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                user.SubtractFromCommissionBalance(amount, "Transfer from Commission Balance");

                if (transferTo == "Main balance")
                {
                    Money amountWithFee = TransferManager.GetAmountWithFee(amount, TransferFeeType.SameUserCommissionToMain, user);
                    user.AddToMainBalance(amountWithFee, "Transfer from Commission Balance");

                    //Pools
                    Money moneyLeft = amount - amountWithFee;
                    PoolDistributionManager.AddProfit(ProfitSource.TransferFees, moneyLeft);
                }
                else if (transferTo == "Purchase balance")
                {
                    user.AddToPurchaseBalance(amount, "Transfer from Commission Balance");
                }
                else if (transferTo == "Cash balance")
                    user.AddToCashBalance(amount, "Transfer from Purchase Balance");

                user.SaveBalances();
                return U3501.TRANSFEROK;
            }
            else throw new MsgException("You cannot transfer from Commission Balance");
        }
        else if (transferFrom == "Purchase balance")
        {
            if (!TitanFeatures.IsRevolca)
                throw new MsgException("You cannot transfer from Purchase Balance");

            if (amount > user.MainBalance)
                throw new MsgException(L1.NOTENOUGHFUNDS);

            user.SubtractFromPurchaseBalance(amount, "Transfer from Purchase Balance");
            user.AddToMainBalance(amount, "Transfer from Purchase Balance");
            user.SaveBalances();
            return U3501.TRANSFEROK;
        }
        else
        {
            htmlResponse = true;
            return TryInvokeProcessorTransfer(transferFrom, transferTo, amount, user);
        }
    }

    protected static void AddHistoryAndTryAchievement(Money amount, Member User, string from, string to)
    {
        //Add history entries
        History.AddTransfer(User.Name, amount, from, to);

        //Achievement trial
        User.TryToAddAchievements(
                    Prem.PTC.Achievements.Achievement.GetProperAchievements(
                    Prem.PTC.Achievements.AchievementType.AfterTransferringOnceAmount, amount.GetTotals()));
    }

    protected static string TryInvokeProcessorTransfer(string transferFrom, string transferTo, Money amount, Member user)
    {
        var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

        if (transferFrom == BtcCryptocurrency.DepositApiProcessor.ToString() && amount is CryptocurrencyMoney)
        {
            //We have BTC direct transfer
            var cryptoAmount = (CryptocurrencyMoney)amount;
            var bg = new DepositToWalletCryptocurrencyButtonGenerator(user, cryptoAmount);
            return GenerateHTMLButtons.GetBtcButton(bg);
        }

        if (transferFrom == BtcCryptocurrency.DepositApiProcessor.ToString())
        {
            var bg = new DepositCryptocurrencyButtonGenerator(user, amount);
            return GenerateHTMLButtons.GetBtcButton(bg);
        }

        if (amount < AppSettings.Payments.MinimumTransferAmount)
            throw new MsgException(U3000.ITSLOWERTHANMINIMUM);

        var thegateway = PaymentAccountDetails.GetFirstGateway(transferFrom, true);

        if (thegateway == null)
            throw new MsgException("No specified gateway installed.");

        amount = thegateway.CalculateAmountWithFee(amount);

        if (transferTo == "Traffic balance")
        {
            var bg = new TransferToRentalBalanceButtonGenerator(user, amount);
            bg.Strategy = thegateway.GetStrategy();
            return bg.Generate();
        }
        else if (transferTo == "Purchase balance")
        {
            var bg = new TransferToAdvertisingBalanceButtonGenerator(user, amount);
            bg.Strategy = thegateway.GetStrategy();
            return bg.Generate();
        }
        else if (transferTo == "Cash balance")
        {
            var bg = new TransferToCashBalanceButtonGenerator(user, amount);
            bg.Strategy = thegateway.GetStrategy();
            return bg.Generate();
        }
        else if (transferTo == "Marketplace balance")
        {
            var bg = new TransferToMarketplaceBalanceButtonGenerator(user, amount);
            bg.Strategy = thegateway.GetStrategy();
            return bg.Generate();
        }
        else if (transferTo == TransferOptionConst.PointsTransfer)
        {
            var bg = new TransferToPointsBalanceButtonGenerator(user, amount);
            bg.Strategy = thegateway.GetStrategy();
            return bg.Generate();
        }

        return String.Empty;
    }

    public static bool IsAllowed(string from, string to)
    {
        rulesFrom = (Dictionary<string, List<string>>)cache.Get();

        if (rulesFrom.ContainsKey(from) && rulesFrom[from].Contains(to))
            return true;

        return false;
    }

    public static ListItem[] GetAvailableListItems(string from)
    {
        rulesFrom = (Dictionary<string, List<string>>)cache.Get();

        return rulesFrom[from].Select(item => new ListItem(item, item)).ToArray();
    }

    public static Dictionary<string, List<string>> GetAvailableOptions()
    {
        Dictionary<string, List<string>> AvailableTransfers = new Dictionary<string, List<string>>();

        var TokenCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);
        var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

        if (AppSettings.Payments.TransferMainInAdBalanceEnabled)
            TryAdd(AvailableTransfers, "Main balance", "Purchase balance");

        if (((AppSettings.Payments.CommissionToMainBalanceEnabled && !TitanFeatures.UserCommissionToMainBalanceEnabled)
            || (TitanFeatures.UserCommissionToMainBalanceEnabled && Member.Current.CommissionToMainBalanceEnabled)) && Member.CurrentInCache.CheckAccessCustomizeTradeOwnSystem)
            TryAdd(AvailableTransfers, "Commission Balance", "Main balance");

        if (AppSettings.Payments.CommissionToAdBalanceEnabled)
            TryAdd(AvailableTransfers, "Commission Balance", "Purchase balance");

        if (TitanFeatures.isAri)
        {
            TryAdd(AvailableTransfers, "Main balance", "Cash balance");
            TryAdd(AvailableTransfers, "Commission Balance", "Cash balance");
        }

        if (TitanFeatures.IsRevolca)
        {
            TryAdd(AvailableTransfers, "Purchase balance", "Main balance");
        }

        foreach (var t in PaymentAccountDetails.PaymentAccountDetailsClasses)
        {
            var instance = Activator.CreateInstance(t);

            if (PaymentAccountDetails.RunStaticMethod(t, "GetFirstIncomeGateway") != null)
            {
                string pp = t.GetProperty("AccountType").GetValue(instance, null).ToString();

                if (pp == "MPesa")  //MPesa only to Cash Balance or Purchase Balance (if Cash Balance disabled)
                {
                    if (AppSettings.Payments.CashBalanceEnabled)
                        TryAdd(AvailableTransfers, pp, "Cash balance");
                    else
                        TryAdd(AvailableTransfers, pp, "Purchase balance");
                }
                else //All other payment processors = no restrictions
                {
                    if (AppSettings.Payments.TransferFromPaymentProcessorsToAdBalanceEnabled)
                        TryAdd(AvailableTransfers, pp, "Purchase balance");

                    if (AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled)
                        TryAdd(AvailableTransfers, pp, "Traffic balance");

                    if (AppSettings.Payments.CashBalanceEnabled)
                        TryAdd(AvailableTransfers, pp, "Cash balance");

                    if (AppSettings.Payments.MarketplaceBalanceDepositEnabled)
                        TryAdd(AvailableTransfers, pp, "Marketplace balance");

                    if (AppSettings.Points.PointsDepositEnable)
                        TryAdd(AvailableTransfers, pp, TransferOptionConst.PointsTransfer);
                }
            }
        }

        if (AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled)
            TryAdd(AvailableTransfers, "Main balance", "Traffic balance");

        if (AppSettings.Payments.CashToAdBalanceEnabled)
            TryAdd(AvailableTransfers, "Cash balance", "Purchase balance");

        if (AppSettings.Payments.MainToMarketplaceBalanceEnabled)
            TryAdd(AvailableTransfers, "Main balance", "Marketplace balance");

        if (AppSettings.Payments.TransferFromBTCWalletToPurchaseBalanceEnabled)
            TryAdd(AvailableTransfers, "BTC Wallet", "Purchase balance");

        if (AppSettings.Payments.TransferFromTokenWalletToPurchaseBalanceEnabled)
            TryAdd(AvailableTransfers, TokenCryptocurrency.Code + " Wallet", "Purchase balance");

        if (AppSettings.Payments.TransferFromMainBalanceToTokenWalletEnabled)
            TryAdd(AvailableTransfers, "Main balance", TokenCryptocurrency.Code + " Wallet");

        if (BtcCryptocurrency.DepositEnabled && CryptocurrencyApiFactory.Get(BtcCryptocurrency.DepositApiProcessor).AllowToUsePaymentButtons())
        {
            string RadioToName = (BtcCryptocurrency.DepositTarget == DepositTargetBalance.Wallet) ? "BTC Wallet" : BtcCryptocurrency.DepositTarget.ToString();
            TryAdd(AvailableTransfers, BtcCryptocurrency.DepositApiProcessor.ToString(), RadioToName);
        }

        return AvailableTransfers;
    }

    private static void TryAdd(Dictionary<string, List<string>> dictionary, string key, string value)
    {
        List<String> list;
        if (dictionary.TryGetValue(key, out list))
        {
            list.Add(value);
            dictionary[key] = list;
        }
        else
        {
            list = new List<string>();
            list.Add(value);
            dictionary.Add(key, list);
        }
    }
}