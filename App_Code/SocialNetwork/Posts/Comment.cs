using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SocialNetwork
{
    [Serializable]
    public class Comment : BaseTableObject
    {
        public static int MaxImageWidth = 125;
        public static int MaxImageHeight = 125;

        #region Columns
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "Comments"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("DateTime")]
        public DateTime DateTime { get { return _DateTime; } set { _DateTime = value; SetUpToDateAsFalse(); } }

        [Column("AuthorId")]
        public int AuthorId { get { return _AuthorId; } set { _AuthorId = value; SetUpToDateAsFalse(); } }

        [Column("PostId")]
        public int PostId { get { return _PostId; } set { _PostId = value; SetUpToDateAsFalse(); } }

        [Column("Text")]
        public string Text { get { return _Text; } set { _Text = value; SetUpToDateAsFalse(); } }

        [Column("ImagePath")]
        public string ImagePath { get { return _ImagePath; } set { _ImagePath = value; SetUpToDateAsFalse(); } }
        #endregion

        int _id, _AuthorId, _PostId;
        DateTime _DateTime;
        string _Text, _ImagePath;
        private Banner _Image;

        public Banner Image
        {
            get { return _Image; }
            set
            {
                if (value != null && value.IsSaved && value.HasValidDimensions(MaxImageWidth, MaxImageHeight))
                {
                    _Image = value;
                    _ImagePath = value.Path;
                    SetUpToDateAsFalse();
                }
                else _Image = null;
            }
        }
        public Post Post
        {
            get
            {
                return TableHelper.SelectRows<Post>(TableHelper.MakeDictionary("Id", PostId)).Single();
            }
        }

        private Comment()
            : base()
        { }

        public Comment(int id) : base(id) { }

        public Comment(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }

        private Comment(int authorId, int postId, string text, Banner image)
        {
            if (string.IsNullOrEmpty(text) && !IsImageValid(image))
                throw new MsgException("Post must include text or image.");

            AuthorId = authorId;
            PostId = postId;
            Text = text;
            Image = image;
            DateTime = AppSettings.ServerTime;
        }

        public void Delete(int userId)
        {
            if (userId != AuthorId && userId != Post.AuthorId)
                throw new MsgException("Unauthorized");

            this.Delete();
        }

        public static Comment Create(int authorId, int postId, string text, Banner image)
        {
            return new Comment(authorId, postId, text, image);
        }

        public void Save()
        {
            base.Save();
        }

        private bool IsImageValid(Banner image)
        {
            return image != null && image.IsSaved && image.HasValidDimensions(MaxImageWidth, MaxImageHeight);
        }

        public static void ValidateImage(int width, int height)
        {
            if (width > MaxImageWidth)
                throw new MsgException(string.Format("Select an image with smaller width (max {0} px)", MaxImageWidth));
            if (height > MaxImageHeight)
                throw new MsgException(string.Format("Select an image with smaller height (max {0} px)", MaxImageHeight));
        }
    }
}
