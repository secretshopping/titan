using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC.Utils.NVP;
using System;
using System.Web;
using Titan.Marketplace;
using Prem.PTC.Utils;
using Titan.InvestmentPlatform;
using Titan.Cryptocurrencies;

namespace Prem.PTC.Payments
{
    public interface IIpnHandler
    {
        void HandlePayPal(string ipnResponse);
        void HandlePayza(string ipnResponse);
        void HandlePerfectMoney(HttpRequest ipnResponse);
        void HandleSolidTrustPay(string Args, string TransactionID, string Amount);
        void HandlePayeer(string Args, string TransactionID, string Amount);
        void HandleNeteller(string Args, string TransactionID, string Amount);
        void HandleOKPay(string Args, string TransactionID, string Amount);
        void HandleEgoPay(string ipnResponse);
        void HandleAdvCash(string ipnResponse, string TransactionID, string Amount);
        void HandlePapara(string Args, string TransactionID, string Amount);
        void HandleMPesa(string username, string TransactionID, string Amount);
        void HandleLocalBitcoins(string Args, string TransactionID, string Amount);
        void HandleCoinPayments(string Args, string TransactionID, string Amount, string cryptoCurrencyInfo);
        void HandleCoinbase(string Args, string TransactionID, string Amount, string cryptoCurrencyInfo);
    }

    public abstract class BaseIpnHandler : IIpnHandler
    {
        public void HandlePayPal(string ipnResponse)
        {
            var data = NameValuePairs.Parse(ipnResponse);
            string[] args = data["invoice"].Split(PayPalButtonGenerationStrategy.ArgsDelimeter);

            if (AppSettings.Site.CurrencyCode.Equals(data["mc_currency"], StringComparison.OrdinalIgnoreCase) &&
                "Completed".Equals(data["payment_status"], StringComparison.OrdinalIgnoreCase))
            {
                PaymentHandler.CheckIfNotDoneYet(PaymentProcessor.PayPal, data["txn_id"]);
                handle(args, data["txn_id"], data["mc_gross"], "PayPal");
            }
            else ErrorLogger.Log("PayPal IPN: Problems with response: " + ipnResponse, LogType.PayPal);
        }

        public void HandlePayza(string ipnResponse)
        {
            var data = NameValuePairs.Parse(ipnResponse);
            string[] args = new string[] { data["apc_2"], data["apc_3"], data["apc_4"], data["apc_5"], data["apc_6"] };

            if (AppSettings.Site.CurrencyCode.Equals(data["ap_currency"], StringComparison.OrdinalIgnoreCase) &&
                "Success".Equals(data["ap_status"], StringComparison.OrdinalIgnoreCase))
            {
                PaymentHandler.CheckIfNotDoneYet(PaymentProcessor.Payza, data["ap_referencenumber"]);
                handle(args, data["ap_referencenumber"], data["ap_totalamount"], "Payza");
            }
            else ErrorLogger.Log("Payza IPN: Problems with response: " + ipnResponse, LogType.Payza);
        }

        public void HandlePerfectMoney(HttpRequest ipnResponse)
        {
            //var data = NameValuePairs.Parse(ipnResponse);
            var data = ipnResponse;

            string[] args = data["ITEM_ARGS"].Split(PerfectMoneyButtonGenerationStrategy.ArgsDelimeter);

            if (AppSettings.Site.CurrencyCode.Equals(data["PAYMENT_UNITS"], StringComparison.OrdinalIgnoreCase))
            {
                handle(args, data["PAYMENT_BATCH_NUM"], data["PAYMENT_AMOUNT"], "PerfectMoney");
            }
            else ErrorLogger.Log("PerfectMoney IPN: Problems with response: " + ipnResponse, LogType.PerfectMoney);
        }

        public void HandleSolidTrustPay(string Args, string TransactionID, string Amount)
        {
            string[] args = Args.Split(SolidTrustPayButtonGenerationStrategy.ArgsDelimeter);
            handle(args, TransactionID, Amount, "SolidTrustPay");
        }

        public void HandlePayeer(string Args, string TransactionID, string Amount)
        {
            string[] args = Args.Split(PayeerButtonGenerationStrategy.ArgsDelimeter);
            handle(args, TransactionID, Amount, "Payeer");
        }

        public void HandleNeteller(string Args, string TransactionID, string Amount)
        {
            string[] args = Args.Split(NetellerButtonGenerationStrategy.ArgsDelimeter);
            handle(args, TransactionID, Amount, "Neteller");
        }

        public void HandleAdvCash(string argsDelimited, string transactionID, string amount)
        {
            string[] args = argsDelimited.Split(ButtonGenerationStrategy.ArgsDelimeter);
            handle(args, transactionID, amount, "AdvCash");
        }

        public void HandleOKPay(string argsDelimited, string transactionID, string amount)
        {
            string[] args = argsDelimited.Split(ButtonGenerationStrategy.ArgsDelimeter);
            handle(args, transactionID, amount, "OKPay");
        }

        public void HandlePapara(string argsDelimited, string transactionID, string amount)
        {
            string[] args = argsDelimited.Split(ButtonGenerationStrategy.ArgsDelimeter);
            handle(args, transactionID, amount, "Papara");
        }

        public void HandleMPesa(string argsDelimited, string transactionID, string amount)
        {
            string[] args = argsDelimited.Split(ButtonGenerationStrategy.ArgsDelimeter);
            handle(args, transactionID, amount, "MPesa");
        }

        public void HandleEgoPay(string ipnResponse)
        {

        }

        public void HandleLocalBitcoins(string argsDelimited, string transactionID, string amount)
        {
            string[] args = argsDelimited.Split(ButtonGenerationStrategy.ArgsDelimeter);
            handle(args, transactionID, amount, "LocalBitcoins");
        }

        public void HandleCoinPayments(string argsDelimited, string transactionID, string amount, string cryptoCurrencyInfo)
        {
            string[] args = argsDelimited.Split(ButtonGenerationStrategy.ArgsDelimeter);
            handle(args, transactionID, amount, "CoinPayments", cryptoCurrencyInfo);
        }

        public void HandleCoinbase(string argsDelimited, string transactionID, string amount, string cryptoCurrencyInfo)
        {
            string[] args = argsDelimited.Split(ButtonGenerationStrategy.ArgsDelimeter);
            handle(args, transactionID, amount, "Coinbase", cryptoCurrencyInfo);
        }

        protected abstract void handle(string[] args, string TransactionId, string AmountSent, string From, string cryptoCurrencyInfo = "");

        protected void TransferToBalance(string username, string amount, string from, string transId, string targetBalance, string cryptoCurrencyInfo)
        {
            Money money = Money.Parse(amount);
            DepositHelper.TransferToBalance(username, money, from, transId, targetBalance, false, cryptoCurrencyInfo);
        }

        protected void BuyMarketplaceProduct(string username, Money amount, string from, string transId,
            int productId, int quantity, string deliveryAddress, string email, int? promotorId, string cryptoCurrencyInfo)
        {
            MarketplaceProduct product = new MarketplaceProduct(productId);

            bool successful = false;

            try
            {
                product.BuyViaProcessor(username, quantity, email, deliveryAddress, amount, promotorId);
                successful = true;
            }
            catch
            {
                successful = false;
            }

            PaymentProcessor PP = PaymentAccountDetails.GetFromStringType(from);
            CompletedPaymentLog.Create(PP, "Marketplace Purchase", transId, false, username, amount, Money.Zero, successful, cryptoCurrencyInfo);
        }

        protected void UpgradeMembership(Member user, Money amount, MembershipPack pack, string from, string transId, string cryptoCurrencyInfo)
        {
            bool successful = false;

            try
            {
                Membership.AddPack(user, pack, amount);
                successful = true;
            }
            catch (Exception ex)
            {
                successful = false;
                ErrorLogger.Log(ex);
            }

            PaymentProcessor PP = PaymentAccountDetails.GetFromStringType(from);
            CompletedPaymentLog.Create(PP, "Upgrade", transId, false, user.Name, amount, Money.Zero, successful, cryptoCurrencyInfo);
        }

        protected void PayAccountActivationFee(Member user, Money amount, string from, string transId, string cryptoCurrencyInfo)
        {
            try
            {
                ActivationFeeManager.TryMarkAccountActivationFeeAsPaid(user, amount, from, transId, cryptoCurrencyInfo);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }

        protected void DepositCryptocurrencyUsingBTCProvider(Member user, Money moneyAmount, string from, string transId, string cryptoCurrencyInfo)
        {
            var Cryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);
            Cryptocurrency.TryDepositCryptocurrency(user, moneyAmount, transId, cryptoCurrencyInfo);
        }

        protected void BuyInvestmentPlatformPlan(Member user, Money amount, InvestmentPlatformPlan plan, string from, string transId, string cryptoCurrencyInfo)
        {
            bool successful = false;
            
            try
            {
                if (plan.MaxPrice > Money.Zero)
                {
                    if (!plan.CheckPlanPrice(amount))
                        throw new MsgException("Not enough money sent!");

                    InvestmentPlatformManager.BuyOrUpgradePlan(user, PurchaseBalances.PaymentProcessor, plan, amount);
                    successful = true;
                }
                else
                {
                    var price = plan.Price;

                    if (AppSettings.InvestmentPlatform.LevelsEnabled)
                        price += plan.LevelFee;

                    if (amount < price)
                        throw new MsgException("Not enough money sent!");

                    InvestmentPlatformManager.BuyOrUpgradePlan(user, PurchaseBalances.PaymentProcessor, plan);
                    successful = true;
                }
            }
            catch (Exception ex)
            {
                successful = false;
                ErrorLogger.Log(ex);
            }

            PaymentProcessor PP = PaymentAccountDetails.GetFromStringType(from);
            CompletedPaymentLog.Create(PP, "Investment Plan", transId, false, user.Name, amount, Money.Zero, successful, cryptoCurrencyInfo);
        }

        protected void BuyJackpotTickets(Member user, Money amount, int tickets, Jackpot jackpot, string from, string transId, string cryptoCurrencyInfo)
        {
            bool successful = false;

            try
            {
                if (amount < (jackpot.TicketPrice * tickets))
                    throw new MsgException("Not enough money sent!");

                JackpotManager.GiveTickets(jackpot, user, tickets);

                successful = true;
            }
            catch (Exception ex)
            {
                successful = false;
                ErrorLogger.Log(ex);
            }

            PaymentProcessor PP = PaymentAccountDetails.GetFromStringType(from);
            CompletedPaymentLog.Create(PP, "Jackpot Tickets", transId, false, user.Name, amount, Money.Zero, successful, cryptoCurrencyInfo);
        }
    }

    public class BuyAdvertIpnHandler : BaseIpnHandler
    {
        private void buyAdvert(string advertTypeName, int advertId, Money money, string from, string transId, string cryptoCurrencyInfo)
        {
            var advert = (Advert)Activator.CreateInstance(Type.GetType(advertTypeName), new object[] { advertId });

            //Prevent fraud transactions (GET price modifications)c
            //NOTE: {X}.XXX cost = {X}.XX received (1.051 = 1.05, 1.059 = 1.05)
            //That is why we add 0.009

            bool successful = false;

            if ((money + new Money(0.009)) >= advert.Price)
            {
                successful = true;
                advert.Status = AdvertStatus.WaitingForAcceptance;
                advert.SaveStatus();
            }

            //Earnings stats
            if (advert is BannerAdvert)
            {
                BannerAdvert thisAdvert = (BannerAdvert)advert;
                EarningsStatsManager.Add(EarningsStatsType.Banner, money);
                PoolDistributionManager.AddProfit(ProfitSource.Banners, money);
            }

            //AddLog
            PaymentProcessor PP = PaymentAccountDetails.GetFromStringType(from);
            CompletedPaymentLog.Create(PP, "Purchase Advertisement pack", transId, advert.Advertiser.CreatedBy == Advertiser.Creator.Stranger, advert.Advertiser.MemberUsername, money, Money.Zero, successful, cryptoCurrencyInfo);
        }

        protected override void handle(string[] args, string TransactionId, string AmountSent, string From, string cryptoCurrencyInfo = "")
        {
            buyAdvert(args[0], Convert.ToInt32(args[1]), Money.Parse(AmountSent), From, TransactionId, cryptoCurrencyInfo);
        }
    }

    public class TransferToRentalBalanceIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string TransactionId, string AmountSent, string From, string cryptoCurrencyInfo = "")
        {
            TransferToBalance(args[0], AmountSent, From, TransactionId, "Traffic Balance", cryptoCurrencyInfo);
        }
    }

    public class TransferToAdvertisingBalanceIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string TransactionId, string AmountSent, string From, string cryptoCurrencyInfo = "")
        {
            TransferToBalance(args[0], AmountSent, From, TransactionId, "Purchase Balance", cryptoCurrencyInfo);
        }
    }

    public class TransferToCashBalanceIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string TransactionId, string AmountSent, string From, string cryptoCurrencyInfo = "")
        {
            TransferToBalance(args[0], AmountSent, From, TransactionId, "Cash Balance", cryptoCurrencyInfo);
        }
    }

    public class TransferToMarketplaceBalanceIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string TransactionId, string AmountSent, string From, string cryptoCurrencyInfo = "")
        {
            TransferToBalance(args[0], AmountSent, From, TransactionId, "Marketplace Balance", cryptoCurrencyInfo);
        }
    }

    public class TransferToPointsBalanceIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string TransactionId, string AmountSent, string From, string cryptoCurrencyInfo = "")
        {
            TransferToBalance(args[0], AmountSent, From, TransactionId, "Points Balance", cryptoCurrencyInfo);
        }
    }

    public class BuyMarketplaceProductIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string transactionId, string amountSent, string from, string cryptoCurrencyInfo = "")
        {
            string username = args[0];
            int productId = Convert.ToInt32(args[1]);
            int quantity = Convert.ToInt32(args[3]);
            string deliveryAddress = args[4];


            var values = args[5].Split('#');
            string email = HashingManager.Base64Decode(values[0]);
            int? promotorId = Convert.ToInt32(HashingManager.Base64Decode(values[1]));

            Money amount = Money.Parse(amountSent);
            BuyMarketplaceProduct(username, amount, from, transactionId, productId, quantity, deliveryAddress, email, promotorId, cryptoCurrencyInfo);
        }
    }

    public class BuyMembershipIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string transactionId, string amountSent, string from, string cryptoCurrencyInfo = "")
        {
            string username = args[0];
            int packId = Convert.ToInt32(args[1]);
            Money amount = Money.Parse(amountSent);
            UpgradeMembership(new Member(username), amount, new MembershipPack(packId), from, transactionId, cryptoCurrencyInfo);
        }
    }

    public class BuyInvestmentPlanIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string transactionId, string amountSent, string from, string cryptoCurrencyInfo = "")
        {
            var user = new Member(args[0]);
            var plan = new InvestmentPlatformPlan(Convert.ToInt32(args[1]));
            Money amount = Money.Parse(amountSent);
            BuyInvestmentPlatformPlan(user, amount, plan, from, transactionId, cryptoCurrencyInfo);
        }
    }

    public class ActivateAccountIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string transactionId, string amountSent, string from, string cryptoCurrencyInfo = "")
        {
            PayAccountActivationFee(new Member(Convert.ToInt32(args[0])), Money.Parse(amountSent), from, transactionId, cryptoCurrencyInfo);
        }
    }

    /// <summary>
    /// Deposit to website global currency. All deposits are made using BTC API Provider at the moment (See Transfer Helper).
    /// </summary>
    public class DepositCryptocurrencyIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string transactionId, string amountSent, string from, string cryptoCurrencyInfo = "")
        {
            DepositCryptocurrencyUsingBTCProvider(new Member(args[0]), Money.Parse(amountSent), from, transactionId, cryptoCurrencyInfo);
        }
    }

    /// <summary>
    /// Deposit to wallet. All deposits are made using BTC API Provider at the moment (See Transfer Helper).
    /// </summary>
    public class WalletDepositCryptocurrencyIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string transactionId, string amountSent, string from, string cryptoCurrencyInfo = "")
        {
            //This method is never fired
        }
    }

    public class BuyJuckpotTicketsIpnHandler : BaseIpnHandler
    {
        protected override void handle(string[] args, string transactionId, string amountSent, string from, string cryptoCurrencyInfo = "")
        {
            var user = new Member(args[0]);
            var jackpot = new Jackpot(Convert.ToInt32(args[1]));
            var tickets = Int32.Parse(args[3]);
            Money amount = Money.Parse(amountSent);
            BuyJackpotTickets(user, amount, tickets, jackpot, from, transactionId, cryptoCurrencyInfo);
        }
    }
}
