using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SocialNetwork
{
    [Serializable]
    public class Post : BaseTableObject
    {
        public static int MaxImageWidth = 500;
        public static int MaxImageHeight = 500;

        #region Columns
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "Posts"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("AuthorId")]
        public int AuthorId { get { return _AuthorId; } set { _AuthorId = value; SetUpToDateAsFalse(); } }

        [Column("DateTime")]
        public DateTime DateTime { get { return _DateTime; } set { _DateTime = value; SetUpToDateAsFalse(); } }       

        [Column("ImagePath")]
        public string ImagePath { get { return _ImagePath; } set { _ImagePath = value; SetUpToDateAsFalse(); } }

        [Column("Text")]
        public string Text { get { return _Text; } set { _Text = value; SetUpToDateAsFalse(); } }

        #endregion

        private int _id, _AuthorId;
        private DateTime _DateTime;
        private Banner _Image;
        string _Text, _ImagePath;
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
        public List<Comment> Comments
        {
            get
            {
                string query = string.Format("SELECT * FROM Comments WHERE PostId = {0} ORDER BY DateTime ASC;", this.Id);
                return TableHelper.GetListFromRawQuery<Comment>(query);
            }
        }

        private Post()
            : base()
        { }

        public Post(int id) : base(id) { }

        public Post(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }

        private Post(int authorId, string text, Banner image)
        {
            if (string.IsNullOrEmpty(text) && !IsImageValid(image))
                throw new MsgException("Post must include text or image.");

            AuthorId = authorId;
            Text = text;
            Image = image;
            DateTime = AppSettings.ServerTime;
        }

        public override void Delete()
        {
            TableHelper.ExecuteRawCommandNonQuery(string.Format("DELETE FROM Comments WHERE PostId = {0}", this.Id));
            base.Delete();         
        }

        public void AddComment(int userId, string text, Banner image)
        {
            var comment = Comment.Create(userId, this.Id, text, image);
            comment.Save();
        }

        public static Post Create(int authorId, string text, Banner image)
        {
            return new Post(authorId, text, image);
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
