using System;
using System.Collections.Generic;
using Titan.Balances;
using Titan.Marketplace;
using Titan.Leadership;
using Titan.Cryptocurrencies;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        #region Columns

        [Column(Columns.MainBalance)]
        public Money MainBalance { get { return _mainBalance; } set { _mainBalance = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TrafficBalance)]
        public Money TrafficBalance { get { return _trafficBalance; } set { _trafficBalance = value; SetUpToDateAsFalse(); } }

        [Obsolete]
        public Money RentalBalance { get { return _trafficBalance; } set { _trafficBalance = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AdvertisingBalance)]
        public Money PurchaseBalance { get { return _advertisingBalance; } set { _advertisingBalance = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CashBalance)]
        public Money CashBalance { get { return _CashBalance; } set { _CashBalance = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CommissionBalance)]
        public Money CommissionBalance { get { return _CommissionBalance; } set { _CommissionBalance = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PointsBalance)]
        public int PointsBalance { get { return _pointsBalance; } set { _pointsBalance = value; SetUpToDateAsFalse(); } }

        [Column(Columns.InvestmentBalance)]
        public Money InvestmentBalance { get { return _InvestmentBalance; } set { _InvestmentBalance = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MarketplaceBalance)]
        public Money MarketplaceBalance { get { return _MarketplaceBalance; } set { _MarketplaceBalance = value; SetUpToDateAsFalse(); } }

        [Column("PTCCredits")]
        public decimal PTCCredits { get { return _PTCCredits; } set { _PTCCredits = value; SetUpToDateAsFalse(); } }

        [Column("LoginAdsCredits")]
        public int LoginAdsCredits { get { return _LoginAdsCredits; } set { _LoginAdsCredits = value; SetUpToDateAsFalse(); } }

        [Obsolete]
        //for backwards compability
        public Money AdvertisingBalance
        {
            get
            {
                return PurchaseBalance;
            }
        }
        #endregion

        public void AddToBalance(BalanceType balanceType, Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            switch(balanceType)
            {
                case BalanceType.MainBalance:
                    AddToMainBalance(value, note, balanceLogType);
                    break;
                case BalanceType.PurchaseBalance:
                    AddToPurchaseBalance(value, note, balanceLogType);
                    break;
                case BalanceType.TrafficBalance:
                    AddToTrafficBalance(value, note, balanceLogType);
                    break;
                case BalanceType.PointsBalance:
                    AddToPointsBalance((int)value.ToDecimal(), note, balanceLogType);
                    break;
                case BalanceType.CommissionBalance:
                    AddToCommissionBalance(value, note, balanceLogType);
                    break;
                case BalanceType.PTCCredits:
                    AddToPTCCredits(value.ToDecimal(), note);
                    break;
                case BalanceType.CashBalance:
                    AddToCashBalance(value, note, balanceLogType);
                    break;
                case BalanceType.InvestmentBalance:
                    AddToInvestmentBalance(value, note, balanceLogType);
                    break;
                case BalanceType.MarketplaceBalance:
                    AddToMarketplaceBalance(value, note, balanceLogType);
                    break;

                case BalanceType.BTC:
                    AddToCryptocurrencyBalance(CryptocurrencyType.BTC, value.ToDecimal(), note, balanceLogType);
                    break;
                case BalanceType.ETH:
                    AddToCryptocurrencyBalance(CryptocurrencyType.ETH, value.ToDecimal(), note, balanceLogType);
                    break;
                case BalanceType.XRP:
                    AddToCryptocurrencyBalance(CryptocurrencyType.XRP, value.ToDecimal(), note, balanceLogType);
                    break;
                case BalanceType.Token:
                    AddToCryptocurrencyBalance(CryptocurrencyType.ERC20Token, value.ToDecimal(), note, balanceLogType);
                    break;
                case BalanceType.FreezedToken:
                    AddToCryptocurrencyBalance(CryptocurrencyType.ERCFreezed, value.ToDecimal(), note, balanceLogType);
                    break;

                case BalanceType.LoginAdsCredits:
                    AddToLoginAdsCredits((int)value.ToDecimal(), note, balanceLogType);
                    break;
            }
        }

        #region Standard balances

        public void AddToMainBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            MainBalance += value;
            BalanceLog.Add(this, BalanceType.MainBalance, value.ToDecimal(), note, balanceLogType);
        }

        public void AddToTrafficBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            TrafficBalance += value;
            BalanceLog.Add(this, BalanceType.TrafficBalance, value.ToDecimal(), note, balanceLogType);
        }

        [Obsolete]
        public void AddToRentalBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            AddToTrafficBalance(value, note, balanceLogType);
        }

        public void AddToPurchaseBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            PurchaseBalance += value;
            BalanceLog.Add(this, BalanceType.PurchaseBalance, value.ToDecimal(), note, balanceLogType);
        }

        public void AddToCommissionBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            CommissionBalance += value;
            BalanceLog.Add(this, BalanceType.CommissionBalance, value.ToDecimal(), note, balanceLogType);
        }

        public void AddToPointsBalance(Int32 value, String note, BalanceLogType balanceLogType = BalanceLogType.Other, bool triggerActions = true, bool checkLeadershipSystem = true)
        {
            if (value != 0)
            {
                PointsBalance += value;
                BalanceLog.Add(this, BalanceType.PointsBalance, value, note, balanceLogType);
                LevelManager.PointsChanged(this, true, triggerActions);
                if (checkLeadershipSystem)
                {
                    var list = new List<RestrictionKind>();
                    list.Add(RestrictionKind.Points);
                    LeadershipSystem.CheckSystem(list, this);
                }
            }
        }

        public void AddToCashBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            CashBalance += value;
            BalanceLog.Add(this, BalanceType.CashBalance, value.ToDecimal(), note, balanceLogType);
        }

        public void AddToInvestmentBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            InvestmentBalance += value;
            BalanceLog.Add(this, BalanceType.InvestmentBalance, value.ToDecimal(), note, balanceLogType);
        }

        public void AddToMarketplaceBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            MarketplaceBalance += value;
            BalanceLog.Add(this, BalanceType.MarketplaceBalance, value.ToDecimal(), note, balanceLogType);
            MarketplaceBalanceLog.Add(this.Id, value);
        }


        public void SubtractFromMainBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            value = value.Positify();
            MainBalance -= value;
            BalanceLog.Add(this, BalanceType.MainBalance, value.Negatify().ToDecimal(), note, balanceLogType);
        }

        public void SubtractFromTrafficBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            value = value.Positify();
            TrafficBalance -= value;
            BalanceLog.Add(this, BalanceType.TrafficBalance, value.Negatify().ToDecimal(), note, balanceLogType);
        }

        [Obsolete]
        public void SubtractFromRentalBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            SubtractFromTrafficBalance(value, note, balanceLogType);
        }

        public void SubtractFromPurchaseBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            value = value.Positify();
            PurchaseBalance -= value;
            BalanceLog.Add(this, BalanceType.PurchaseBalance, value.Negatify().ToDecimal(), note, balanceLogType);
        }

        public void SubtractFromCashBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            value = value.Positify();
            CashBalance -= value;
            BalanceLog.Add(this, BalanceType.CashBalance, value.Negatify().ToDecimal(), note, balanceLogType);
        }

        public void SubtractFromCommissionBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            value = value.Positify();
            CommissionBalance -= value;
            BalanceLog.Add(this, BalanceType.CommissionBalance, value.Negatify().ToDecimal(), note, balanceLogType);
        }

        public void SubtractFromPointsBalance(Int32 value, String note, BalanceLogType balanceLogType = BalanceLogType.Other, bool setMinPoints = true)
        {
            if (value < 0)
                value = -value;

            if (value != 0)
            {
                PointsBalance -= value;
                BalanceLog.Add(this, BalanceType.PointsBalance, -value, note, balanceLogType);
                LevelManager.PointsChanged(this, setMinPoints);
            }
        }

        public void SubtractFromMarketplaceBalance(Money value, String note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            value = value.Positify();
            MarketplaceBalance -= value;
            BalanceLog.Add(this, BalanceType.MarketplaceBalance, value.Negatify().ToDecimal(), note, balanceLogType);
            MarketplaceBalanceLogManager.MoneySpent(this.Id, value);
        }

        #endregion

        #region Crypto balances

        public CryptocurrencyMoney GetCryptocurrencyBalance(CryptocurrencyType cryptocurrencyType)
        {
            return UserCryptocurrencyBalance.Get(this.Id, cryptocurrencyType);
        }

        public Money GetCryptocurrencyBalanceInMoney(CryptocurrencyType cryptocurrencyType)
        {
            return UserCryptocurrencyBalance.GetMoneyValue(Id, cryptocurrencyType);
        }

        /// <summary>
        /// Do NOT require Save() on member
        /// </summary>
        /// <param name="cryptocurrencyType"></param>
        /// <param name="value"></param>
        /// <param name="note"></param>
        /// <param name="logType"></param>
        public void AddToCryptocurrencyBalance(CryptocurrencyType cryptocurrencyType, decimal value, string note, BalanceLogType logType = BalanceLogType.Other)
        {
            AddToCryptocurrencyBalance(cryptocurrencyType, new CryptocurrencyMoney(cryptocurrencyType, value), note, logType);
        }

        /// <summary>
        /// Do NOT require Save() on member
        /// </summary>
        /// <param name="cryptocurrencyType"></param>
        /// <param name="value"></param>
        /// <param name="note"></param>
        /// <param name="logType"></param>
        public void SubtractFromCryptocurrencyBalance(CryptocurrencyType cryptocurrencyType, decimal value, string note, BalanceLogType logType = BalanceLogType.Other)
        {
            SubtractFromCryptocurrencyBalance(cryptocurrencyType, new CryptocurrencyMoney(cryptocurrencyType, value), note, logType);
        }

        private void AddToCryptocurrencyBalance(CryptocurrencyType cryptocurrencyType, CryptocurrencyMoney value, string note, BalanceLogType logType = BalanceLogType.Other)
        {
            UserCryptocurrencyBalance.Add(this.Id, value, cryptocurrencyType);
            //Works on ONE crypto balance at the moment
            BalanceLog.Add(this, CryptocurrencyTypeHelper.ConvertToBalanceType(cryptocurrencyType), value.ToDecimal(), note, logType);
        }

        private void SubtractFromCryptocurrencyBalance(CryptocurrencyType cryptocurrencyType, CryptocurrencyMoney value, string note, BalanceLogType logType = BalanceLogType.Other)
        {
            UserCryptocurrencyBalance.Remove(this.Id, value, cryptocurrencyType);
            //Works on ONE crypto balance at the moment
            BalanceLog.Add(this, CryptocurrencyTypeHelper.ConvertToBalanceType(cryptocurrencyType), value.ToDecimal() * -1, note, logType);
        }

        #endregion

        #region Additional balances
        public void AddToPTCCredits(decimal value, string note)
        {
            if (value < 0)
                throw new MsgException("You cannot add negative value.");

            PTCCredits += value;
            BalanceLog.AddPtcCreditLog(this, value, note);
        }

        public void SubstractFromPTCCredits(decimal value, string note)
        {
            if (value < 0)
                throw new MsgException("You cannot add negative value.");

            PTCCredits -= value;
            BalanceLog.AddPtcCreditLog(this, -value, note);
        }

        public void AddToLoginAdsCredits(int value, string note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            if (value < 0)
                throw new MsgException("You cannot add negative value.");

            LoginAdsCredits += value;

            BalanceLog.Add(this, BalanceType.LoginAdsCredits, value, note, balanceLogType);
        }

        public void SubstractFromLoginAdsCredits(int value, string note, BalanceLogType balanceLogType = BalanceLogType.Other)
        {
            if (value < 0)
                throw new MsgException("You cannot add negative value.");

            LoginAdsCredits -= value;

            BalanceLog.Add(this, BalanceType.LoginAdsCredits, -value, note, balanceLogType);
        }
        #endregion

        public decimal GetDecimalBalance(BalanceType type)
        {
            if (BalanceTypeHelper.IsCryptoBalance(type))
                return UserCryptocurrencyBalance.Get(Member.CurrentId, CryptocurrencyTypeHelper.ConvertToCryptocurrencyType(type)).ToDecimal();
            else
                return GetBasicBalance(type).ToDecimal();
        }

        public String GetStringBalance(BalanceType type)
        {
            if (BalanceTypeHelper.IsCryptoBalance(type))
                return UserCryptocurrencyBalance.Get(Member.CurrentId, CryptocurrencyTypeHelper.ConvertToCryptocurrencyType(type)).ToString().Trim();
            else
            {
                if (type == BalanceType.PTCCredits || type == BalanceType.PointsBalance)
                    return GetBasicBalance(type).ToClearString().Trim();

                return GetBasicBalance(type).ToString().Trim();
            }     
        }

        public String GetClearStringBalance(BalanceType type)
        {
            if (BalanceTypeHelper.IsCryptoBalance(type))
                return UserCryptocurrencyBalance.Get(Member.CurrentId, CryptocurrencyTypeHelper.ConvertToCryptocurrencyType(type)).ToClearString();
            return GetBasicBalance(type).ToClearString();
        }

        private Money GetBasicBalance(BalanceType type)
        {
            //NO CRYPTOCYURRENCY
            switch (type)
            {
                case BalanceType.MainBalance:
                    return MainBalance;

                case BalanceType.PurchaseBalance:
                    return PurchaseBalance;

                case BalanceType.TrafficBalance:
                    return TrafficBalance;

                case BalanceType.PointsBalance:
                    return new Money(PointsBalance);

                case BalanceType.CommissionBalance:
                    return CommissionBalance;

                case BalanceType.PTCCredits:
                    return new Money(PTCCredits);

                case BalanceType.CashBalance:
                    return CashBalance;

                case BalanceType.InvestmentBalance:
                    return InvestmentBalance;

                case BalanceType.MarketplaceBalance:
                    return MarketplaceBalance;

                default:
                    return Money.Zero;
            }
        }
    }
}