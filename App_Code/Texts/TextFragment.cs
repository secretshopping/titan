using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Prem.PTC.Texts
{
    /// <summary>
    /// Summary description for TextFragment
    /// </summary>
    public class TextFragment : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return AppSettings.TableNames.TextFragments; } }
        protected override string dbTable { get { return TableName; } }

        [Column("TextFragmentId", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("RelatedTextId")]
        public int RelatedTextId { get { return _relatedTextId; } internal set { _relatedTextId = value; SetUpToDateAsFalse(); } }

        [Column("Ordinal")]
        public int Ordinal { get { return _ordinal; } internal set { _ordinal = value; SetUpToDateAsFalse(); } }

        [Column("TextContent")]
        public string Content { get { return _content; } internal set { _content = value; SetUpToDateAsFalse(); } }

        private int _id, _relatedTextId, _ordinal;
        private string _content;
        #endregion Columns


        #region Constructors

        public TextFragment() : base() { }
        public TextFragment(int id) : base(id) { }
        public TextFragment(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors
    }
}