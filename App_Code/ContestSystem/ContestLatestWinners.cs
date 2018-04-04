using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

namespace Prem.PTC.Contests
{

    public class ContestLatestWinners : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "ContestLatestWinners"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Type")]
        protected int IntType { get { return type; } set { type = value; SetUpToDateAsFalse(); } }

        [Column("Winner1")]
        public string Winner1 { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("Winner2")]
        public string Winner2 { get { return aname; } set { aname = value; SetUpToDateAsFalse(); } }

        [Column("Winner3")]
        public string Winner3 { get { return bname; } set { bname = value; SetUpToDateAsFalse(); } }

        private int _id, quantity, type;
        private string name, aname, bname;


        #endregion Columns

        public ContestLatestWinners()
            : base() { }

        public ContestLatestWinners(int id) : base(id) { }

        public ContestLatestWinners(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public ContestType Type
        {
            get
            {
                return (ContestType)IntType;
            }

            set
            {
                IntType = (int)value;
            }
        }

        public static void AddAndClean(ContestType Type, string w1, string w2, string w3)
        {
            //Check if there is something to clean
            int count = TableHelper.CountOf<ContestLatestWinners>(TableHelper.MakeDictionary("Type", (int)Type));
            if (count > 0)
            {
                //Yes
                TableHelper.DeleteRows<ContestLatestWinners>(TableHelper.MakeDictionary("Type", (int)Type));
            }

            ContestLatestWinners temp = new ContestLatestWinners();
            temp.Type = Type;
            temp.Winner1 = w1;
            temp.Winner2 = w2;
            temp.Winner3 = w3;
            temp.Save();
        }
    }
}