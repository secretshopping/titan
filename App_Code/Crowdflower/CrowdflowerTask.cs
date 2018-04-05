using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;


    /// <summary>
    /// Handling achievements
    /// </summary>
    public class CrowdflowerTask : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "CrowdflowerTasks"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Points")]
        public int Points { get { return quantity; } set { quantity = value; SetUpToDateAsFalse(); } }

        [Column("Username")]
        public string Username { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

        [Column("Title")]
        public string Title { get { return _Title; } set { _Title = value; SetUpToDateAsFalse(); } }

        [Column("Date")]
        public DateTime Date { get { return _date; } set { _date = value; SetUpToDateAsFalse(); } }


        private int _id, quantity;
        private string name, _Title;
        private DateTime _date;

        #endregion Columns

        public CrowdflowerTask()
            : base() { }

        public CrowdflowerTask(int id) : base(id) { }

        public CrowdflowerTask(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    }
