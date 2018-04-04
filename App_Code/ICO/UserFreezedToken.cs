using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Titan.Cryptocurrencies;

namespace Titan.ICO
{
    [Serializable]
    public class UserFreezedToken : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "UserFreezedTokens"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string UserId = "UserId";
            public const string NumberOfTokens = "NumberOfTokens";
            public const string DateOfAction = "DateOfAction";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return id; } protected set { id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.UserId)]
        public int UserId { get { return userId; } set { userId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.NumberOfTokens)]
        public decimal NumberOfTokens { get { return numberOfTokens; } set { numberOfTokens = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DateOfAction)]
        public DateTime DateOfAction { get { return dateOfAction; } set { dateOfAction = value; SetUpToDateAsFalse(); } }

        private int id, userId;
        private decimal numberOfTokens;
        private DateTime dateOfAction;

        public UserFreezedToken() : base() { }
        public UserFreezedToken(int id) : base(id) { }
        public UserFreezedToken(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public static void Add(int userId, decimal numberOfTokens)
        {
            var NewAction = new UserFreezedToken();

            NewAction.UserId = userId;
            NewAction.NumberOfTokens = numberOfTokens;
            NewAction.DateOfAction = AppSettings.ServerTime;

            NewAction.Save();
        }
    }
}

