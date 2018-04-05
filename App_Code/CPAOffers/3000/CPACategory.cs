using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Data;
using System.Web.UI.WebControls;

namespace Titan
{
    public class CPACategory : BaseTableObject
    {
        public static int DEFAULT_CATEGORY_ID = 12;

        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "CPACategories"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("CategoryName")]
        public string Name { get { return _ClientHTML; } set { _ClientHTML = value; SetUpToDateAsFalse(); } }

        private int _id;
        private string _ClientHTML;

        #endregion Columns

        #region Constructors

        public CPACategory()
            : base() { }

        public CPACategory(int id)
        {

            Id = id;
            DataRow row;
            Parser bridge = ParserPool.Acquire(Database).Instance;
            if (bridge.Select(dbTable, TableHelper.MakeDictionary("Id", Id)).Rows.Count > 0)
                row = bridge.Select(dbTable, TableHelper.MakeDictionary("Id", Id)).Rows[0];
            else
            {
                Id = DEFAULT_CATEGORY_ID;
                row = bridge.Select(dbTable, TableHelper.MakeDictionary("Id",Id)).Rows[0];
            }
            ParserPool.Release(bridge);

            SetColumnsFromDataRow(row, true);

        }

        public CPACategory(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        #endregion Constructors

        /// <summary>
        /// Returns list control source with all statuses' short descriptions as text 
        /// and int status ordinal as value
        /// </summary>
        public static ListItem[] ListItems
        {
            get
            {
                var list = TableHelper.SelectAllRows<CPACategory>();

                var query = from CPACategory cat in list
                            orderby (int)cat.Id
                            select new ListItem(cat.Name, (int)cat.Id + "");

                return query.ToArray();
            }
        }

        public static List<CPACategory> AllCategories
        {
            get
            {
                return TableHelper.SelectAllRows<CPACategory>();
            }
        }

    }
}