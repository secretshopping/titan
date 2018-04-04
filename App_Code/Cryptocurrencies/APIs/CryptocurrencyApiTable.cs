using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using System.Text;
using System.Web;

namespace Titan.Cryptocurrencies
{
    [Serializable]
    public abstract class CryptocurrencyApiTable : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "CryptocurrencyApis"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("TypeInt", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("TypeInt")]
        protected int TypeInt { get { return _TypeInt; } set { _TypeInt = value; SetUpToDateAsFalse(); } }

        [Column("Name")]
        public string Name { get { return _Name; } set { _Name = value; SetUpToDateAsFalse(); } }

        [Column("Enabled")]
        public bool Enabled { get { return _Enabled; } set { _Enabled = value; SetUpToDateAsFalse(); } }

        [Column("Field1")]
        public string Field1 { get { return _Field1; } set { _Field1 = value; SetUpToDateAsFalse(); } }

        [Column("Field2")]
        public string Field2 { get { return _Field2; } set { _Field2 = value; SetUpToDateAsFalse(); } }

        [Column("Field3")]
        public string Field3 { get { return _Field3; } set { _Field3 = value; SetUpToDateAsFalse(); } }

        [Column("Field4")]
        public string Field4 { get { return _Field4; } set { _Field4 = value; SetUpToDateAsFalse(); } }

        [Column("Field5")]
        public string Field5 { get { return _Field5; } set { _Field5 = value; SetUpToDateAsFalse(); } }

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

        private int _id, _TypeInt;
        private bool _Enabled;
        private string _Name, _Field5, _Field4, _Field3, _Field2, _Field1;

        #endregion Columns

        public CryptocurrencyApiTable(int id) : base(id) { }

        public CryptocurrencyApiTable(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public List<string> GetSupportedCryptocurrencyForDeposts()
        {
            return GetSupportedCryptocurrency("DepositsAvailable");
        }

        public List<string> GetSupportedCryptocurrencyForWithdrawals()
        {
            return GetSupportedCryptocurrency("WithdrawalsAvailable");
        }

        private List<string> GetSupportedCryptocurrency(string columnToCheck)
        {
            var query = String.Format("SELECT Code FROM Cryptocurrencies WHERE TypeInt IN " +
               "(SELECT CryptocurrencyTypeId FROM CryptocurrencyApiOperations WHERE TypeInt = {0} AND {1} = 1)",
               this.TypeInt, columnToCheck);

            return TableHelper.GetStringListFromRawQuery(query);
        }
    }
}