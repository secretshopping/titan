using System;
using Prem.PTC;
using System.Data;
using Resources;
using Titan.Cryptocurrencies;

namespace Titan.Balances
{

    public class UserCryptocurrencyBalance : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "UserCryptocurrencyBalances"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

        [Column("Balance")]
        public CryptocurrencyMoney Balance { get { return amount; } set { amount = value; SetUpToDateAsFalse(); } }

        [Column("CurrencyCode")]
        private string CurrencyCode { get { return code; } set { code = value; SetUpToDateAsFalse(); } }

        public CryptocurrencyType Cryptocurrency
        {
            get { return (CryptocurrencyType)Enum.Parse(typeof(CryptocurrencyType), CurrencyCode); }
            set { CurrencyCode = value.ToString(); }
        }

        private int _id, _UserId;
        private CryptocurrencyMoney amount;
        private string code;

        #endregion Columns

        public UserCryptocurrencyBalance() : base() { }
        public UserCryptocurrencyBalance(int id) : base(id) { }
        public UserCryptocurrencyBalance(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }

        public static CryptocurrencyMoney Get(int userId, CryptocurrencyType code)
        {
            var balance = GetObject(userId, code).Balance;
            return new CryptocurrencyMoney(code, balance.ToDecimal());
        }

        public static Money GetMoneyValue(int userId, CryptocurrencyType code)
        {
            var balance = GetObject(userId, code).Balance;
            return new CryptocurrencyMoney(code, balance.ToDecimal());
        }

        public static void Set(int userId, CryptocurrencyType code, CryptocurrencyMoney value)
        {
            var userBalance = GetObject(userId, code);
            userBalance.Balance = value;
            userBalance.Save();
        }

        public static void Add(int userId, CryptocurrencyMoney amount, CryptocurrencyType code)
        {
            var userBalance = GetObject(userId, code);
            userBalance.Balance += amount;
            userBalance.Save();
        }

        public static void Remove(int userId, CryptocurrencyMoney amount, CryptocurrencyType code)
        {
            var userBalance = GetObject(userId, code);
            if (userBalance.Balance - amount < new CryptocurrencyMoney(code, 0))
                throw new Exception(String.Format(U6010.ERRORREMOVECRYPTOCURRENCYBALANCE, amount, userId, userBalance.Balance));

            userBalance.Balance -= amount;
            userBalance.Save(); 
        }

        private static UserCryptocurrencyBalance GetObject(int userId, CryptocurrencyType code)
        {
            var result = TableHelper.GetListFromRawQuery<UserCryptocurrencyBalance>
                (String.Format("SELECT * FROM UserCryptocurrencyBalances WHERE UserId = {0} AND CurrencyCode = '{1}'", userId, code));

            if (result.Count > 0)
            {
                result[0].Balance.cryptocurrencyType = code; //CryptocurrenyMoney class fix
                return result[0];
            }

            UserCryptocurrencyBalance userCryptocurrencyBalance = new UserCryptocurrencyBalance();
            userCryptocurrencyBalance.Balance = new CryptocurrencyMoney(code, 0);
            userCryptocurrencyBalance.CurrencyCode = code.ToString();
            userCryptocurrencyBalance.UserId = userId;
            userCryptocurrencyBalance.Save();

            return userCryptocurrencyBalance;
        }
    }
}
