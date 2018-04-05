using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Resources;
using Prem.PTC.Members;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.UI;
using Prem.PTC.Advertising;
using HtmlAgilityPack;

namespace Titan.News
{
    [Serializable]
    public class Article : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "Articles"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Title")]
        public string Title { get { return _Title; } set { _Title = value; SetUpToDateAsFalse(); } }

        [Column("ShortDescription")]
        public string ShortDescription { get { return _ShortDescription; } set { _ShortDescription = value; SetUpToDateAsFalse(); } }

        [Column("Text")]
        public string Text { get { return _Text; } set { _Text = value; SetUpToDateAsFalse(); } }

        [Column("VideoURL")]
        public string VideoURL { get { return _VideoURL; } set { _VideoURL = value; SetUpToDateAsFalse(); } }

        [Column("ImageURL")]
        public string ImageURL { get { return _ImageURL; } set { _ImageURL = value; SetUpToDateAsFalse(); } }

        [Column("Geolocation")]
        public string Geolocation { get { return _Geolocation; } set { _Geolocation = value; SetUpToDateAsFalse(); } }

        [Column("CategoryId")]
        public int CategoryId { get { return _CategoryId; } set { _CategoryId = value; SetUpToDateAsFalse(); } }

        [Column("StatusInt")]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

        [Column("CreatorUserId")]
        public int CreatorUserId { get { return _CreatorUserId; } set { _CreatorUserId = value; SetUpToDateAsFalse(); } }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get { return _CreatedDate; } set { _CreatedDate = value; SetUpToDateAsFalse(); } }

        [Column("LastUpdatedDate")]
        public DateTime LastUpdatedDate { get { return _LastUpdatedDate; } set { _LastUpdatedDate = value; SetUpToDateAsFalse(); } }

        [Column("Keywords")]
        public string Keywords { get { return _Keywords; } set { _Keywords = value; SetUpToDateAsFalse(); } }

        [Column("OriginalTitleLink")]
        public string OriginalTitleLink { get { return _OriginalTitleLink; } set { _OriginalTitleLink = value; SetUpToDateAsFalse(); } }

        [Column("Clicks")]
        public int Reads { get { return _Clicks; } set { _Clicks = value; SetUpToDateAsFalse(); } }

        [Column("CreatorMoneyEarned")]
        public Money CreatorMoneyEarned { get { return _CreatorMoneyEarned; } set { _CreatorMoneyEarned = value; SetUpToDateAsFalse(); } }

        private int _id, _CategoryId, _StatusInt, _CreatorUserId, _Clicks;
        private string _Title, _ShortDescription, _Text, _VideoURL, _ImageURL, _Geolocation, _Keywords, _OriginalTitleLink;
        private DateTime _CreatedDate, _LastUpdatedDate;
        private Money _CreatorMoneyEarned;

        public Article() : base() { }

        public Article(int id) : base(id) { }

        public Article(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public AdvertStatus Status
        {
            get
            {
                return (AdvertStatus)StatusInt;
            }
            set
            {
                StatusInt = (int)value;
            }
        }

        #endregion Columns

        public ArticleCategory GetCategory()
        {
            return new ArticleCategory(CategoryId);
        }

        public string GetSharingLink(int userId)
        {
            return GetLink() + String.Format("?ref={0}", HttpUtility.UrlEncode(InfluencerCodeHelper.ToCode(userId)));
        }

        public string GetLink()
        {
            return String.Format("{0}article/{1}/{2}", AppSettings.Site.Url, this.Id, this.OriginalTitleLink);
        }

        public string GetImageURL()
        {
            return (HttpContext.Current.Handler as Page).ResolveClientUrl(ImageURL);
        }

        public string GetCovertImageURL()
        {
            return GetImageURL();
        }

        public string GetCoverImageURLAbsolutePath()
        {
            return string.Format("{0}{1}", AppSettings.Site.Url.TrimEnd('/'), (HttpContext.Current.Handler as Page).ResolveUrl(ImageURL));
        }

        public Member GetAuthor()
        {
            return new Member(CreatorUserId);
        }

        public string GetRawText()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(Text)));

            return doc.DocumentNode.InnerText;
        }

        public static int TitleMinCharacters { get { return 10; } }
        public static int TitleMaxCharacters { get { return 500; } } //It's MAX 500 on the database

        public static Article Add(string title, string text, string shortDescription, string keywords,
            int userId, string countryCode, int articleCategoryId, string imageUrl = null, string videoUrl = null)
        {
            Article NewArticle = new Article();
            NewArticle.CategoryId = articleCategoryId;
            NewArticle.CreatedDate = AppSettings.ServerTime;
            NewArticle.CreatorUserId = userId;
            NewArticle.Geolocation = countryCode;
            NewArticle.ImageURL = imageUrl;
            NewArticle.VideoURL = videoUrl;
            NewArticle.Keywords = keywords;
            NewArticle.LastUpdatedDate = AppSettings.ServerTime;
            NewArticle.ShortDescription = shortDescription;
            NewArticle.Status = AdvertStatus.WaitingForAcceptance;
            NewArticle.Text = text;
            NewArticle.Title = title;
            NewArticle.OriginalTitleLink = Regex.Replace(HttpUtility.UrlEncode(HttpUtility.HtmlDecode(title).ToLower()), "%..", "-").Replace("+", "-").Replace(".","");
            NewArticle.OriginalTitleLink = Regex.Replace(NewArticle.OriginalTitleLink, "-{2,}", "-").Trim('-');

            if (userId==1005)
                NewArticle.Status = AdvertStatus.Active;

            NewArticle.Save();

            return NewArticle;
        }

        public static Article Edit(int articleId, string title, string text, string shortDescription, string keywords,
            int userId, string countryCode, int articleCategoryId, string imageUrl = null, string videoUrl = null)
        {
            Article NewArticle = new Article(articleId);
            NewArticle.CategoryId = articleCategoryId;
            NewArticle.Geolocation = countryCode;

            if (!String.IsNullOrEmpty(imageUrl))
                NewArticle.ImageURL = imageUrl;

            if (!String.IsNullOrEmpty(videoUrl))
                NewArticle.VideoURL = videoUrl;

            NewArticle.Keywords = keywords;
            NewArticle.LastUpdatedDate = AppSettings.ServerTime;
            NewArticle.ShortDescription = shortDescription;
            NewArticle.Text = text;
            NewArticle.Title = title;
            NewArticle.Status = AdvertStatus.WaitingForAcceptance;
            NewArticle.Save();

            return NewArticle;
        }
    }
}