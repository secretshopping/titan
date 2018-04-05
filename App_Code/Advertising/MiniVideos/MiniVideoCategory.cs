using System.Data;
using Prem.PTC;
using System.Web.UI.WebControls;
using System.Linq;
using System.Collections.Generic;

namespace Titan.MiniVideos
{
    public class MiniVideoCategory : BaseTableObject
    {
        public static int DEFAULT_CATEGORY_ID = 100;

        #region Columns
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "MiniVideoCategories"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("CategoryName")]
        public string Name { get { return _name; } set { _name = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int _Status { get { return _status; } set { _status = value; SetUpToDateAsFalse(); } }

        public UniversalStatus Status
        {
            get { return (UniversalStatus)_Status; }
            set { _Status = (int)value; }
        }

        private int _id, _status;
        private string _name;
        #endregion Columns

        #region Constructors

        public MiniVideoCategory() : base() { }

        public MiniVideoCategory(int id)
        {
            Id = id;
            DataRow row;
            Parser bridge = ParserPool.Acquire(Database).Instance;
            if (bridge.Select(dbTable, TableHelper.MakeDictionary("Id", Id)).Rows.Count > 0)
                row = bridge.Select(dbTable, TableHelper.MakeDictionary("Id", Id)).Rows[0];
            else
            {
                Id = DEFAULT_CATEGORY_ID;
                row = bridge.Select(dbTable, TableHelper.MakeDictionary("Id", Id)).Rows[0];
            }
            ParserPool.Release(bridge);

            SetColumnsFromDataRow(row, true);
        }

        public MiniVideoCategory(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public override void Delete()
        {
            Status = UniversalStatus.Deleted;
            Save();
        }

        public static ListItem[] ListItems
        {
            get
            {
                var list = AllActiveCategories;

                var query = from MiniVideoCategory cat in list
                            orderby cat.Name
                            select new ListItem(cat.Name, (int)cat.Id + "");

                return query.ToArray();
            }
        }

        public static List<MiniVideoCategory> AllActiveCategories
        {
            get
            {
                var query = string.Format("SELECT * FROM {0} WHERE [Status] != {1}", TableName, (int)UniversalStatus.Deleted);
                return TableHelper.GetListFromRawQuery<MiniVideoCategory>(query);
            }
        }

        public static bool CanAddThisName(string categoryName)
        {
            return (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM {0} WHERE CategoryName = '{1}' AND [Status] = {2}", TableName, categoryName, (int)UniversalStatus.Active)) == 0;
        }
    }
}