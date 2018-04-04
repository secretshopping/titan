using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Resources;

namespace Titan.News
{
    [Serializable]
    public class ArticleView : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "ArticleViews"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("ArticleId")]
        public int ArticleId { get { return _ArticleId; } set { _ArticleId = value; SetUpToDateAsFalse(); } }

        [Column("IP")]
        public string IP { get { return _IP; } set { _IP = value; SetUpToDateAsFalse(); } }

        [Column("ViewDate")]
        public DateTime ViewDate { get { return _ViewDate; } set { _ViewDate = value; SetUpToDateAsFalse(); } }

        [Column("InfluencerUserId")]
        public int InfluencerUserId { get { return _InfluencerUserId; } set { _InfluencerUserId = value; SetUpToDateAsFalse(); } }

        [Column("InfluencerCredited")]
        public bool InfluencerCredited { get { return _InfluencerCredited; } set { _InfluencerCredited = value; SetUpToDateAsFalse(); } }

        [Column("CreatorCredited")]
        public bool CreatorCredited { get { return _CreatorCredited; } set { _CreatorCredited = value; SetUpToDateAsFalse(); } }

        private int _id, _ArticleId, _InfluencerUserId;
        private string _IP;
        private DateTime _ViewDate;
        private bool _InfluencerCredited, _CreatorCredited;

        public ArticleView() : base() { }

        public ArticleView(int id) : base(id) { }

        public ArticleView(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public bool IsFromInfluencer
        {
            get
            {
                return InfluencerUserId != -1;
            }
        }

        #endregion Columns

        public static void TryAdd(int articleId, string IP, int InfluencerUserId)
        {
            TableHelper.ExecuteRawCommandNonQuery(
                String.Format("EXEC TryAddArticleView @ArticleId = {0}, @IP = '{1}', @InfluencerUserId = {2}",
                articleId, IP, InfluencerUserId));
        }
    }
}