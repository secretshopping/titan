using Prem.PTC;
using Prem.PTC.Contests;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Prem.PTC.Utils;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Titan.ICO;

namespace Titan.Cryptocurrencies
{
    [Serializable]
    public abstract class Cryptocurrency : CryptocurrencyTable
    {
        public abstract CryptocurrencyType CryptocurrencyType { get; }
        public abstract bool Available { get; }
        protected string CoinmarketcapID { get; set; }

        #region Constructors

        public Cryptocurrency(int tableObjectId, string CoinmarketcapID) : base(tableObjectId, CoinmarketcapID)
        {
            this.CoinmarketcapID = CoinmarketcapID;
        }

        public Cryptocurrency(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public static Cryptocurrency Get(CryptocurrencyType cryptocurrencyType)
        {
            return CryptocurrencyFactory.Get(cryptocurrencyType);
        }

        #endregion

        #region Rates

        public Money GetValue()
        {
            return GetOriginalValue() * (MarketPriceMultipliedByPercent / 100) + MarketPriceEnlargedBy;
        }

        public Money GetOriginalValue()
        {
            return GetOriginalValue(AppSettings.Site.CurrencyCode);
        }

        public decimal GetRate(string currencyCode)
        {
            return (decimal.One / GetOriginalValue(currencyCode).ToDecimal()).TruncateDecimals(DecimalPlaces);
        }

        public decimal ConvertFromMoney(Money value)
        {
            return (value.ToDecimal() / GetValue().ToDecimal()).TruncateDecimals(DecimalPlaces);
        }

        public decimal ConvertFromMoney(Money value, string currencyCode)
        {
            return (value.ToDecimal() / GetOriginalValue(currencyCode).ToDecimal()).TruncateDecimals(DecimalPlaces);
        }

        public Money ConvertToMoney(decimal value)
        {
            return new Money(value * GetValue().ToDecimal());
        }

        public Money GetOriginalValue(string currencyCode)
        {
            if (currencyCode == AppSettings.Site.CurrencyCode)
                return MarketPrice;

            return GetOriginalValueFromAPI(currencyCode);
        }

        private Money GetOriginalValueFromAPI(string currencyCode)
        {
            return CoinmarketcapHelper.GetCurrentExchangeRate(CoinmarketcapID, currencyCode);
        }

        #endregion

        #region Withdrawals

        public Money GetMinimumWithdrawalAmount(Member user, WithdrawalSourceBalance withdrawalSource)
        {
            if (withdrawalSource == WithdrawalSourceBalance.MainBalance)
            {
                var withdrawLimitInMembership = PayoutLimit.GetGlobalLimitValue(user);
                var minimumCryptocurrencyWithdraw = WithdrawalMinimum * GetValue();

                var list = new List<Money>
                {
                    minimumCryptocurrencyWithdraw,
                    withdrawLimitInMembership
                };

                return list.Max();
            }
            else //Wallet
            {
                return new CryptocurrencyMoney(Type, WithdrawalMinimum);
            }
        }

        public Money GetMaximumWithdrawalAmount(Member user, WithdrawalSourceBalance withdrawalSource, out string errorMessage)
        {
            errorMessage = U6012.PAYOUTREQUESTTOOHIGH;

            if (withdrawalSource == WithdrawalSourceBalance.MainBalance)
            {
                return PayoutManager.GetMaxPossiblePayout(CryptocurrencyTypeHelper.ConvertToPaymentProcessor(this.CryptocurrencyType), user, out errorMessage);
            }
            else //Wallet
            {
                return user.GetCryptocurrencyBalance(this.Type); //No limit
            }
        }

        public string TryMakeWithdrawal(int userId, string address, decimal amount, WithdrawalSourceBalance withdrawalSource)
        {
            var successMessage = U6000.REQUESTSENTPLEASEWAIT;
            var user = new Member(userId);

            Money amountInMoney;
            Money amountInCorrectType;
            decimal amountInCryptocurrency;
            CryptocurrencyWithdrawRequest request = null;

            if (withdrawalSource == WithdrawalSourceBalance.MainBalance)
            {
                amountInCorrectType = new Money(amount);
                amountInMoney = new Money(amount);
                amountInCryptocurrency = ConvertFromMoney(amountInCorrectType);

            }
            else //Wallets
            {
                amountInCorrectType = new CryptocurrencyMoney(CryptocurrencyType, amount);
                amountInMoney = ConvertToMoney(amount);
                amountInCryptocurrency = amount;
            }

            //Full Validation
            ValidateWithdrawal(user, address, amount, withdrawalSource);

            var amountAfterFee = amountInCorrectType - EstimatedWithdrawalFee(amountInCryptocurrency, address, userId, withdrawalSource);
            if (amountAfterFee <= Money.Zero)
                throw new MsgException("You can't withdraw a negative value.");

            if (!IsAutomaticWithdrawal(user, amountInMoney))
                CryptocurrencyWithdrawRequest.ValidatePendingRequests(userId, amountInMoney, this);

            user.CashoutsProceed++;

            if (withdrawalSource == WithdrawalSourceBalance.MainBalance)
            {
                user.MoneyCashout += amountAfterFee;
                user.SubtractFromMainBalance(amountInMoney, String.Format("Requested {0} withdrawal", Type));
                user.Save();

                request = new CryptocurrencyWithdrawRequest(userId, address, amountAfterFee, amountInMoney, Type, withdrawalSource);
            }
            else //Wallets
            {
                user.MoneyCashout += ConvertToMoney(amountAfterFee.ToDecimal());
                user.SubtractFromCryptocurrencyBalance(Type, amount, String.Format("Requested {0} withdrawal", Type));
                user.Save();

                request = new CryptocurrencyWithdrawRequest(userId, address, amountAfterFee, new Money(amount), Type, withdrawalSource);
            }

            request.Save();

            if (IsAutomaticWithdrawal(user, amountInMoney))
            {
                request.Accept();
                successMessage = U5003.BTCWITHDRAWSUCCESS.Replace("%n%", amount.ToString() + ".");
            }

            return successMessage;
        }

        public void MakeWithdrawal(Money amount, string address, Member user, WithdrawalSourceBalance withdrawalSource)
        {
            decimal amountInCryptocurrency = Decimal.Zero;
            Money amountInMoney = Money.Zero;

            if (withdrawalSource == WithdrawalSourceBalance.MainBalance)
            {
                amountInCryptocurrency = ConvertFromMoney(amount);
                amountInMoney = amount;
            }
            else //Wallets
            {
                amountInCryptocurrency = amount.ToDecimal();
                amountInMoney = ConvertToMoney(amountInCryptocurrency);
            }

            CryptocurrencyApiFactory.GetWithdrawalApi(this).TryWithDrawCryptocurrencyFromWallet(amountInCryptocurrency, address, Type);
            BitcoinIPNManager.AddIPNLog(AppSettings.ServerTime, OperationType.Withdrawal, null, user.Id, address, amountInCryptocurrency, amountInMoney, this.WithdrawalApiProcessor, true);

            if (withdrawalSource == WithdrawalSourceBalance.MainBalance)
                PaymentProportionsManager.MemberPaidOut(amountInMoney, CryptocurrencyTypeHelper.ConvertToPaymentProcessor(Type), user);

            AddPaymentProof(amount, user);
        }

        public Money GetAdminWithdrawalFee(Money amount)
        {
            //Final Fee = Processor Fee + Admin Fee (this)
            return this.WithdrawalFeeFixed + Money.MultiplyPercent(amount, this.WithdrawalFeePercent);
        }

        /// <summary>
        /// Final Fee = Processor Fee + Admin Fee 
        /// </summary>
        /// <param name="cryptocurrencyAmount"></param>
        /// <param name="userAddress"></param>
        /// <param name="userId"></param>
        /// <param name="withdrawalSource"></param>
        /// <returns></returns>
        public Money EstimatedWithdrawalFee(decimal cryptocurrencyAmount, string userAddress, int userId, WithdrawalSourceBalance withdrawalSource)
        {
            decimal FinalFee = EstimatedWithdrawalFee(cryptocurrencyAmount, userAddress, userId);

            if (withdrawalSource == WithdrawalSourceBalance.MainBalance)
                return ConvertToMoney(FinalFee);
            else //Wallets
                return new CryptocurrencyMoney(Type, FinalFee);
        }

        public void AddPaymentProof(Money amountInMoney, Member user)
        {
            var paymentType = IsAutomaticWithdrawal(user, amountInMoney) ? PaymentType.Instant : PaymentType.Manual;
            PaymentProof.Add(user.Id, amountInMoney, paymentType, CryptocurrencyTypeHelper.ConvertToPaymentProcessor(Type));
        }

        private void ValidateWithdrawal(Member user, string userAddress, decimal amount, WithdrawalSourceBalance withdrawalSource)
        {
            if (!WithdrawalEnabled)
                throw new MsgException("Withdrawal is currently disabled by the administrator");

            if (CryptocurrencyApi.IsAdministratorAddress(userAddress, WithdrawalApiProcessor))
                throw new MsgException("You can't withdraw to administrator-generated address.");

            Money amountInCorrectType = Money.Zero;
            string errorMessage = String.Empty;

            //General validation
            PayoutManager.ValidatePayoutNotConnectedToAmount(user);

            //Amounts & Balances
            if (withdrawalSource == WithdrawalSourceBalance.MainBalance)
            {
                amountInCorrectType = new Money(amount);

                //Check the balance
                if (amountInCorrectType > user.MainBalance)
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                PayoutManager.ValidatePayout(user, amountInCorrectType);
                PayoutManager.CheckMaxPayout(CryptocurrencyTypeHelper.ConvertToPaymentProcessor(CryptocurrencyType), user, amountInCorrectType);
            }
            else //Wallets
            {
                amountInCorrectType = new CryptocurrencyMoney(this.Type, amount);

                //Check the balance
                if (amountInCorrectType > user.GetCryptocurrencyBalance(CryptocurrencyType))
                    throw new MsgException(string.Format(U6012.NOFUNDSINWALLET, CryptocurrencyType.ToString()));
            }

            //Check MIN withdrawal 
            if (amountInCorrectType < GetMinimumWithdrawalAmount(user, withdrawalSource))
                throw new MsgException(U5003.WITHDRAWALMUSTBEHIGHER);

            //Check MAX withdrawals
            if (amountInCorrectType > GetMaximumWithdrawalAmount(user, withdrawalSource, out errorMessage))
                throw new MsgException(errorMessage);
        }

        private bool IsAutomaticWithdrawal(Member user, Money valueInMoney)
        {
            if (PayoutManager.IsManualCryptocurrencyPayout(user))
                return false;

            if (valueInMoney > WithdrawalMaximumAutomaticAmount)
                return false;

            return true;
        }

        private decimal EstimatedWithdrawalFee(decimal cryptocurrencyAmount, string userAddress, int userId)
        {
            decimal ProcessorFee = CryptocurrencyApiFactory.GetWithdrawalApi(this).GetEstimatedWithdrawalFee(cryptocurrencyAmount, userAddress);

            if (this.WithdrawalFeePolicy == WithdrawalFeePolicy.Packs)
            {
                var fee = BitcoinWithdrawalFeePacks.GetFeePackForUser(userId).Fee;
                return ProcessorFee + this.ConvertFromMoney(Money.MultiplyPercent(this.ConvertToMoney(cryptocurrencyAmount), fee));
            }

            return ProcessorFee + this.ConvertFromMoney(GetAdminWithdrawalFee(this.ConvertToMoney(cryptocurrencyAmount)));
        }

        #endregion

        #region Deposits

        /// <summary>
        /// Use if you don't have Member object (need to get it from DB by address)
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="address"></param>
        /// <param name="amountInCryptocurrency"></param>
        /// <param name="transactionHash"></param>
        /// <param name="cryptoCurrencyInfo"></param>
        /// <param name="confirmations"></param>
        public void TryDepositCryptocurrency(CryptocurrencyAPIProvider provider, string address, decimal amountInCryptocurrency,
             string transactionHash, string cryptoCurrencyInfo, int confirmations = 100)
        {
            Member User = new Member(BitcoinAddress.GetUserId(address, provider));

            TryDepositCryptocurrency(User, amountInCryptocurrency, transactionHash, cryptoCurrencyInfo, confirmations);

            //Blank the address
            BitcoinAddress.MakeBlank(User.Id, provider);
        }

        /// <summary>
        /// Use if you already have a Member object
        /// </summary>
        /// <param name="user"></param>
        /// <param name="amountInCryptocurrency"></param>
        /// <param name="transactionHash"></param>
        /// <param name="cryptoCurrencyInfo"></param>
        /// <param name="confirmations"></param>
        public void TryDepositCryptocurrency(Member user, decimal amountInCryptocurrency, string transactionHash,
                string cryptoCurrencyInfo, int confirmations = 100)
        {
            //Get transaction
            var transaction = CompletedPaymentLog.GetTransactionIfExistsCreateOtherwise(CryptocurrencyTypeHelper.ConvertToPaymentProcessor(Type),
                transactionHash, "Deposit " + Code, user.Name, ConvertToMoney(amountInCryptocurrency), Money.Zero, cryptoCurrencyInfo);

            if (transaction.Successful)
                throw new MsgException("This transaction has already been proceed.");

            ValidateDeposit(amountInCryptocurrency, confirmations);
            DepositCryptocurrency(user, amountInCryptocurrency);

            transaction.Successful = true;
            transaction.Save();
        }

        /// <summary>
        /// Use if this is deposit to website global currency
        /// </summary>
        /// <param name="user"></param>
        /// <param name="amountInCryptocurrency"></param>
        /// <param name="transactionHash"></param>
        /// <param name="cryptoCurrencyInfo"></param>
        /// <param name="confirmations"></param>
        public void TryDepositCryptocurrency(Member user, Money amountInWebsiteGlobalCurrency, string transactionHash,
                string cryptoCurrencyInfo, int confirmations = 100)
        {
            decimal amountInCryptocurrency = ConvertFromMoney(amountInWebsiteGlobalCurrency);
            TryDepositCryptocurrency(user, amountInCryptocurrency, transactionHash, cryptoCurrencyInfo, confirmations);
        }

        private void ValidateDeposit(decimal amountInCryptocurrency, int confirmations)
        {
            if (DepositEnabled == false)
                throw new MsgException("Cryptocurrency deposit is disabled.");

            if (amountInCryptocurrency < DepositMinimum)
                throw new MsgException("Amount is lower then minimum deposit: " + DepositMinimum);

            if (confirmations < DepositMinimumConfirmations)
                throw new MsgException("Not enough confirmations.");
        }

        private void DepositCryptocurrency(Member user, decimal amountInCryptocurrency)
        {
            Money moneyAmount = ConvertToMoney(amountInCryptocurrency);
            LogType LogType = ErrorLoggerHelper.GetTypeFromProcessor(DepositApiProcessor);

            user.Reload();

            if (DepositTarget == DepositTargetBalance.PurchaseBalance)
            {
                user.AddToPurchaseBalance(moneyAmount, Code + " deposit", BalanceLogType.Other);
                user.SaveBalances();
            }
            if (DepositTarget == DepositTargetBalance.CashBalance)
            {
                user.AddToCashBalance(moneyAmount, Code + " deposit", BalanceLogType.Other);
                user.SaveBalances();

                //Referral commission for sponsors when user does Cash Balance deposit
                CashBalanceCrediter Crediter = (CashBalanceCrediter)CrediterFactory.Acquire(user, CreditType.CashBalanceDeposit);
                Crediter.TryCreditReferer(moneyAmount);
            }
            if (DepositTarget == DepositTargetBalance.Wallet)
            {
                user.AddToCryptocurrencyBalance(Type, amountInCryptocurrency, Code + " Wallet deposit", BalanceLogType.WalletDeposit);
            }

            PaymentProportionsManager.MemberPaidIn(moneyAmount, CryptocurrencyTypeHelper.ConvertToPaymentProcessor(Type), user);
            History.AddTransfer(user.Name, moneyAmount, Code + " deposit", DepositTarget.ToString());

            ContestManager.IMadeAnAction(ContestType.Transfer, user.Name, moneyAmount, 0);
        }

        #endregion

        #region Display

        public int DecimalPlaces { get { return CryptocurrencyTypeHelper.GetDecimalPlaces(CryptocurrencyType); } }
        public virtual string CurrencySign { get { return this.Code; } }
        public virtual string CurrencyDisplaySignBefore { get { return String.Empty; } }
        public virtual string CurrencyDisplaySignAfter { get { return this.Code; } }

        public virtual string GetImageUrl()
        {
            return string.Format("~/Images/OneSite/TransferMoney/{0}.png", CryptocurrencyType);
        }

        #endregion

    }
}