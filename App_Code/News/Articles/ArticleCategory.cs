using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Resources;
using System.Web.UI.WebControls;
using System.Linq;

namespace Titan.News
{
    [Serializable]
    public class ArticleCategory : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "ArticleCategories"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Text")]
        public string Text { get { return _Text; } set { _Text = value; SetUpToDateAsFalse(); } }

        [Column("StatusInt")]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

        [Column("CountryCode")]
        public string CountryCode { get { return _CountryCode; } set { _CountryCode = value; SetUpToDateAsFalse(); } }


        private int _id, _StatusInt;
        private string  _Text, _CountryCode;

        public ArticleCategory() : base() { }

        public ArticleCategory(int id) : base(id) { }

        public ArticleCategory(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public UniversalStatus Status
        {
            get
            {
                return (UniversalStatus)StatusInt;
            }
            set
            {
                StatusInt = (int)value;
            }
        }

        #endregion Columns

        public static ListItem[] GetListItems(string countryCode)
        {
                var query = from ArticleCategory cat in GetAllActiveCategories(countryCode)
                            orderby cat.Text
                            select new ListItem(cat.Text, (int)cat.Id + "");

                return query.ToArray();
        }

        public static List<ArticleCategory> GetAllActiveCategories(string countryCode)
        {
                var query = string.Format("SELECT * FROM {0} WHERE [StatusInt] = {1} AND CountryCode = '{2}'", TableName, (int)UniversalStatus.Active,
                    countryCode);
                return TableHelper.GetListFromRawQuery<ArticleCategory>(query);
        }
    }
}