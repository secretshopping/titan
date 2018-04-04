using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

namespace Titan.Cryptocurrencies
{
    [Serializable]
    public class CryptocurrencyApiOperation : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "CryptocurrencyApiOperations"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("TypeInt")]
        protected int TypeInt { get { return _TypeInt; } set { _TypeInt = value; SetUpToDateAsFalse(); } }

        [Column("CryptocurrencyTypeId")]
        public int CryptocurrencyTypeId { get { return _CryptocurrencyTypeId; } set { _CryptocurrencyTypeId = value; SetUpToDateAsFalse(); } }

        [Column("WithdrawalsAvailable")]
        public bool WithdrawalsAvailable { get { return _WithdrawalEnabled; } set { _WithdrawalEnabled = value; SetUpToDateAsFalse(); } }

        [Column("DepositsAvailable")]
        public bool DepositsAvailable { get { return _DepositEnabled; } set { _DepositEnabled = value; SetUpToDateAsFalse(); } }

        private int _id, _TypeInt, _CryptocurrencyTypeId;
        private bool _WithdrawalEnabled, _DepositEnabled;

        #endregion Columns

        public CryptocurrencyApiOperation(int id) : base(id) { }

        public CryptocurrencyApiOperation(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public CryptocurrencyType Cryptocurrency
        {
            get
            {
                return (CryptocurrencyType)CryptocurrencyTypeId;
            }
            set
            {
                CryptocurrencyTypeId = (int)value;
            }
        }

        public CryptocurrencyAPIProvider ApiType
        {
            get
            {
                return (CryptocurrencyAPIProvider)TypeInt;
            }
            set
            {
                TypeInt = (int)value;
            }
        }

        
    }
}