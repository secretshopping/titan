using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Prem.PTC.Utils;

namespace Prem.PTC.Texts
{
    public enum WebsiteTextType
    {
        Null = 0,
        NewsNote = 1,
        FAQ = 2,
        ToS = 3,
        PrivacyPolicy = 4,
        WelcomeText = 5,
        SupportTicketReplyTemplate = 6,
        EmailReplyTemplate = 7,
        Custom1 = 8
    }

    /// <summary>
    /// Summary description for WebsiteText
    /// </summary>
    public class WebsiteText : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return AppSettings.TableNames.Texts; } }
        protected override string dbTable { get { return TableName; } }

        [Column("TextId", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("TextType")]
        protected int TextType { get { return _textType; } set { _textType = value; SetUpToDateAsFalse(); } }

        [Column("IsVisible")]
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; SetUpToDateAsFalse(); } }

        [Column("Title")]
        public string Title { get { return _title; } set { _title = value; SetUpToDateAsFalse(); } }

        [Column("CreatedDate")]
        public DateTime Created { get { return _created; } protected set { _created = value; SetUpToDateAsFalse(); } }

        [Column("LastModifiedDate")]
        public DateTime? LastModified { get { return _lastModified; } protected set { _lastModified = value; SetUpToDateAsFalse(); } }

        private int _id, _textType;
        private bool _isVisible;
        private string _title;
        private DateTime _created;
        private DateTime? _lastModified;

        #endregion Columns


        public WebsiteTextType Type { get { return (WebsiteTextType)TextType; } protected set { TextType = (int)value; } }

        private List<TextFragment> fragments;

        protected bool isContentLoaded = false;
        private string _content;
        public string Content
        {
            get
            {
                if (!isContentLoaded) LoadContent();

                return _content;
            }
            set
            {
                _content = value;
                isContentLoaded = true;
                LastModified = DateTime.Now;
            }
        }

        private void LoadContent()
        {
            _content = string.Empty;

            var text = from fragment in fragments
                       orderby fragment.Ordinal ascending
                       select fragment.Content;

            foreach (var fragment in text) _content += fragment;

            isContentLoaded = true;
        }


        #region Constructors

        public WebsiteText(WebsiteTextType type)
            : base()
        {
            Type = type;
            Created = DateTime.Now;
            fragments = new List<TextFragment>();
        }

        public WebsiteText(WebsiteTextType type, string title)
            : this(type)
        {
            Title = title;
        }

        public WebsiteText(WebsiteTextType type, string title, string content)
            : this(type, title)
        {
            Content = content;
        }

        public WebsiteText(int id)
            : base()
        {
            Init();
        }

        public WebsiteText(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {
            Init();
        }

        private void Init()
        {
            var whereId = TableHelper.MakeDictionary("RelatedTextId", Id);
            fragments = TableHelper.SelectRows<TextFragment>(whereId);
        }

        #endregion Constructors


        public static WebsiteText Select(WebsiteTextType type)
        {
            var where = TableHelper.MakeDictionary("TextType", (int)type);
            var list = TableHelper.SelectRows<WebsiteText>(where);

            return list.Count > 0 ? list[0] : new WebsiteText(type);
        }


        private const int NVARCHAR_MAX_LENGTH = 200000000;
        private TextSplitter splitter = new TextSplitter(NVARCHAR_MAX_LENGTH);

        public override void Save(bool forceSave = false)
        {
            base.Save(forceSave);
            SaveTextFragments(forceSave);
        }

        // NOTE: method reuses TextFragmentsObjects
        private void SaveTextFragments(bool forceSave)
        {
            splitter.Text = Content;

            if (splitter.BlocksCount > fragments.Count)
                AddNeededTextFragments();

            SetTextFragments();

            DeleteNotNeededTextFragments();

            foreach (var fragment in fragments) fragment.Save();
        }

        private void AddNeededTextFragments()
        {
            int maxOrdinal = GetMaxOrdinal();

            for (int i = 1; i <= splitter.BlocksCount - fragments.Count; ++i)
            {
                var fragment = new TextFragment()
                {
                    Ordinal = maxOrdinal + i,
                    RelatedTextId = Id
                };
                fragments.Add(fragment);
            }
        }

        private int GetMaxOrdinal()
        {
            int maxOrdinal = 0;
            foreach (var fragment in fragments)
                maxOrdinal = Math.Max(maxOrdinal, fragment.Ordinal);
            return maxOrdinal;
        }

        private void SetTextFragments()
        {
            List<string> newText = splitter.SplitText;
            for (int i = 0; i < newText.Count; ++i) fragments[i].Content = newText[i];
        }

        private void DeleteNotNeededTextFragments()
        {
            for (int i = splitter.BlocksCount; i < fragments.Count; ++i)
            {
                var fragment = fragments[i];
                fragments.RemoveAt(i);
                fragment.Delete();
            }
        }

        public override void Delete()
        {
            base.Delete();
            foreach (var fragment in fragments) fragment.Delete();
            fragments = null;
        }
    }
}