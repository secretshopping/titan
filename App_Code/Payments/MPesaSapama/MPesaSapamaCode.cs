using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{

    public class MPesaSapamaCode : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public new static string TableName { get { return "MPesaSapamaCodes"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Amount")]
        public Money Amount { get { return _Amount; } set { _Amount = value; SetUpToDateAsFalse(); } }

        [Column("Code")]
        public string Code { get { return _apiKey; } set { _apiKey = value; SetUpToDateAsFalse(); } }

        [Column("Phone")]
        public string Phone { get { return _walletNumber; } set { _walletNumber = value; SetUpToDateAsFalse(); } }

        [Column("IsProceed")]
        public bool IsProceed { get { return _IsPaid; } set { _IsPaid = value; SetUpToDateAsFalse(); } }


        private int _id, _UserId;
        private string _apiName, _apiKey, _walletNumber, _secretKey;
        private bool _IsPaid;
        private DateTime _DateAdded;
        private Money _Amount;

        #endregion

        #region Constructors

        public MPesaSapamaCode() : base() { }
        public MPesaSapamaCode(int id) : base(id) { }
        public MPesaSapamaCode(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public static void Create(Money amount, string code, string phone)
        {
            MPesaSapamaCode order = new MPesaSapamaCode();
            order.Amount = amount;
            order.Code = code;
            order.Phone = phone;
            order.IsProceed = false;
            order.Save();
        }

        public static bool TryValidate(string username, string code, string phone)
        {
            var where = TableHelper.MakeDictionary("Phone", phone.Trim());
            where.Add("Code", code.Trim());
            where.Add("IsProceed", false);
            var list = TableHelper.SelectRows<MPesaSapamaCode>(where);

            if(Member.CurrentName == "admin")
            {
                DepositHelper.TransferToBalance(username, Money.Parse("1"), "MPesaAgent", "TEST", "Cash Balance");
                return true;
            }

            if (list.Count == 0)
                throw new MsgException(L1.WRONGCODE);

            var mpesaCode = list[0];

            //Mark as used
            mpesaCode.IsProceed = true;
            mpesaCode.Save();

            //Transfer money to member
            string TargetBalance = "Purchase Balance";

            if (AppSettings.Payments.CashBalanceEnabled)
                TargetBalance = "Cash Balance";

            DepositHelper.TransferToBalance(username, mpesaCode.Amount, "MPesaAgent", mpesaCode.Code, TargetBalance);


            return true;
        }
    }
}