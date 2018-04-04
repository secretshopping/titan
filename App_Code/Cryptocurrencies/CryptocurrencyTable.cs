using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Resources;

namespace Titan.Cryptocurrencies
{
    [Serializable]
    public class CryptocurrencyTable : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "Cryptocurrencies"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("TypeInt", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("TypeInt")]
        protected int TypeInt { get { return _TypeInt; } set { _TypeInt = value; SetUpToDateAsFalse(); } }

        [Column("Name")]
        public string Name { get { return _Name; } set { _Name = value; SetUpToDateAsFalse(); } }

        [Column("Code")]
        public string Code { get { return _Code; } set { _Code = value; SetUpToDateAsFalse(); } }

        [Column("WithdrawalEnabled")]
        public bool WithdrawalEnabled { get { return _WithdrawalEnabled && WithdrawalApiProcessorInt != 0; } set { _WithdrawalEnabled = value; SetUpToDateAsFalse(); } }

        [Column("WithdrawalSource")]
        protected int WithdrawalSourceInt { get { return _WithdrawalSource; } set { _WithdrawalSource = value; SetUpToDateAsFalse(); } }

        [Column("WithdrawalMinimum")]
        public decimal WithdrawalMinimum { get { return _WithdrawalMinimum; } set { _WithdrawalMinimum = value; SetUpToDateAsFalse(); } }

        [Column("WithdrawalFeeFixed")]
        public Money WithdrawalFeeFixed { get { return _WithdrawalFeeFixed; } set { _WithdrawalFeeFixed = value; SetUpToDateAsFalse(); } }

        [Column("WithdrawalFeePercent")]
        public decimal WithdrawalFeePercent { get { return _WithdrawalFeePercent; } set { _WithdrawalFeePercent = value; SetUpToDateAsFalse(); } }

        [Column("WithdrawalFeePolicy")]
        protected int WithdrawalFeePolicyInt { get { return _WithdrawalFeePolicyInt; } set { _WithdrawalFeePolicyInt = value; SetUpToDateAsFalse(); } }

        [Column("WithdrawalMaximumAutomaticAmount")]
        public Money WithdrawalMaximumAutomaticAmount { get { return _WithdrawalMaximumAutomaticAmount; } set { _WithdrawalMaximumAutomaticAmount = value; SetUpToDateAsFalse(); } }

        [Column("WithdrawalApiProcessor")]
        protected int WithdrawalApiProcessorInt { get { return _WithdrawalApiProcessor; } set { _WithdrawalApiProcessor = value; SetUpToDateAsFalse(); } }

        [Column("ActivateUserAddressAfterDays")]
        public int ActivateUserAddressAfterDays { get { return _ActivateUserAddressAfterDays; } set { _ActivateUserAddressAfterDays = value; SetUpToDateAsFalse(); } }

        [Column("MaxValueOfPendingRequests")]
        public Money MaxValueOfPendingRequests { get { return _MaxValueOfPendingRequests; } set { _MaxValueOfPendingRequests = value; SetUpToDateAsFalse(); } }

        [Column("DepositEnabled")]
        public bool DepositEnabled { get { return _DepositEnabled && DepositApiProcessorInt != 0; } set { _DepositEnabled = value; SetUpToDateAsFalse(); } }

        [Column("DepositMinimum")]
        public decimal DepositMinimum { get { return _DepositMinimum; } set { _DepositMinimum = value; SetUpToDateAsFalse(); } }

        [Column("DepositMinimumConfirmations")]
        public int DepositMinimumConfirmations { get { return _DepositMinimumConfirmations; } set { _DepositMinimumConfirmations = value; SetUpToDateAsFalse(); } }

        [Column("DepositTarget")]
        protected int DepositTargetInt { get { return _DepositTargetInt; } set { _DepositTargetInt = value; SetUpToDateAsFalse(); } }

        [Column("DepositApiProcessor")]
        protected int DepositApiProcessorInt { get { return _DepositApiProcessor; } set { _DepositApiProcessor = value; SetUpToDateAsFalse(); } }

        [Column("MarketPrice")]
        public Money MarketPrice { get { return _MarketPrice; } set { _MarketPrice = value; SetUpToDateAsFalse(); } }

        [Column("MarketPriceEnlargedBy")]
        public Money MarketPriceEnlargedBy { get { return _MarketPriceEnlargedBy; } set { _MarketPriceEnlargedBy = value; SetUpToDateAsFalse(); } }

        [Column("MarketPriceMultipliedByPercent")]
        public decimal MarketPriceMultipliedByPercent { get { return _MarketPriceMultipliedByPercent; } set { _MarketPriceMultipliedByPercent = value; SetUpToDateAsFalse(); } }

        [Column("WalletEnabled")]
        public bool WalletEnabled
        {
            get { return _WalletEnabled; }
            set
            {
                _WalletEnabled = value;
                if (Type == CryptocurrencyType.ERC20Token && !value)
                    AppSettings.Ethereum.ERC20TokensFreezeSystemEnabled = false;
                SetUpToDateAsFalse();
            }
        }

        private int _id, _TypeInt, _WithdrawalSource, _WithdrawalFeePolicyInt, _ActivateUserAddressAfterDays, _DepositTargetInt, _DepositApiProcessor,
            _WithdrawalApiProcessor, _DepositMinimumConfirmations;
        private decimal _WithdrawalMinimum, _WithdrawalFeePercent, _DepositMinimum, _MarketPriceMultipliedByPercent;
        private bool _WithdrawalEnabled, _DepositEnabled, _WalletEnabled;
        private Money _WithdrawalFeeFixed, _WithdrawalMaximumAutomaticAmount, _MaxValueOfPendingRequests, _MarketPrice, _MarketPriceEnlargedBy;
        private string _Code, _Name;

        #endregion Columns

        public CryptocurrencyTable(int id, string CoinmarketcapID) : base(id)
        {
            //Everytime the original constructor is called let's try update current Market rate
            //This class is cached for 60 seconds, so the consturcor is called evey 60 seconds
            Money CurrentMarketPrice = CoinmarketcapHelper.GetCurrentExchangeRate(CoinmarketcapID, AppSettings.Site.CurrencyCode);
            if (CurrentMarketPrice != Money.Zero && CurrentMarketPrice != MarketPrice)
            {
                MarketPrice = CurrentMarketPrice;
                Save();
            }
        }

        public CryptocurrencyTable(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public CryptocurrencyType Type
        {
            get
            {
                return (CryptocurrencyType)TypeInt;
            }
            set
            {
                TypeInt = (int)value;
            }
        }

        public WithdrawalSourceBalance WithdrawalSource
        {
            get
            {
                return (WithdrawalSourceBalance)WithdrawalSourceInt;
            }
            set
            {
                WithdrawalSourceInt = (int)value;
            }
        }

        public WithdrawalFeePolicy WithdrawalFeePolicy
        {
            get
            {
                return (WithdrawalFeePolicy)WithdrawalFeePolicyInt;
            }
            set
            {
                WithdrawalFeePolicyInt = (int)value;
            }
        }

        public CryptocurrencyAPIProvider WithdrawalApiProcessor
        {
            get
            {
                return (CryptocurrencyAPIProvider)WithdrawalApiProcessorInt;
            }
            set
            {
                WithdrawalApiProcessorInt = (int)value;
            }
        }

        public CryptocurrencyAPIProvider DepositApiProcessor
        {
            get
            {
                return (CryptocurrencyAPIProvider)DepositApiProcessorInt;
            }
            set
            {
                DepositApiProcessorInt = (int)value;
            }
        }

        public DepositTargetBalance DepositTarget
        {
            get
            {
                return (DepositTargetBalance)DepositTargetInt;
            }
            set
            {
                DepositTargetInt = (int)value;
            }
        }

        public string GetWithdrawalSourceName()
        {
            if (WithdrawalSource == WithdrawalSourceBalance.Wallet)
                return string.Format(U6012.WALLET, this.Code);

            if (WithdrawalSource == WithdrawalSourceBalance.MainBalance)
                return L1.MAINBALANCE;

            return String.Empty;
        }
    }
}