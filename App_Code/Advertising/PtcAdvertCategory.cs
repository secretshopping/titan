using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace Prem.PTC.Advertising
{
    public sealed class PtcAdvertCategory : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return AppSettings.TableNames.PtcAdvertCategories; } }
        protected override string dbTable { get { return TableName; } }

        [Column("PtcAdvertCategoryId", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Name")]
        public string Name { get { return _name; } set { _name = value; SetUpToDateAsFalse(); } }

        [Column("IsActive")]
        public bool IsActive { get { return _isActive; } set { _isActive = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        public int StatusInt { get { return _status; } set { _status = value; SetUpToDateAsFalse(); } }

        public UniversalStatus Status
        {
            get { return (UniversalStatus)StatusInt; }
            set { StatusInt = (int)value; }
        }

        private int _id, _status;
        private string _name;
        private bool _isActive;

        #endregion


        #region Constructors

        public PtcAdvertCategory() : base() { }
        public PtcAdvertCategory(int id) : base(id) { }
        public PtcAdvertCategory(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion

        /// <exception cref="DbException"/>
        public static ListItem[] ListControlSource
        {
            get
            {
                DataTable ptcAdvertCategoriesTable;
                using(var bridge = ParserPool.Acquire(Database.Client))
                {
                    ptcAdvertCategoriesTable = bridge.Instance.Select(AppSettings.TableNames.PtcAdvertCategories, null);
                }

                var ptcAdvertCategories = new List<PtcAdvertCategory>(ptcAdvertCategoriesTable.Rows.Count);
                foreach (DataRow row in ptcAdvertCategoriesTable.Rows) ptcAdvertCategories.Add(new PtcAdvertCategory(row));

                var sort = from category in ptcAdvertCategories
                           orderby category.Name ascending
                           select new ListItem(category.Name, Convert.ToString(category.Id));

                return sort.ToArray();
            }
        }

        public static List<PtcAdvertCategory> GetActiveCategories()
        {
            return TableHelper.SelectRows<PtcAdvertCategory>(TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
        }
    }
}