using System;
using System.Data;
using Prem.PTC;
using System.Collections.Generic;

namespace Titan.MiniVideos
{

    public class MiniVideoCampaign : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "MiniVideoCampaigns"; } }
        protected override string dbTable { get { return TableName; } }

        #region Columns
        public static class Columns
        {
            public const string Id = "Id";
            public const string Username = "Username";
            public const string Title = "Title";
            public const string Status = "Status";
            public const string Description = "Description";
            public const string ImageURL = "ImageURL";
            public const string VideoURL = "VideoURL";
            public const string VideoCategory = "VideoCategory";
            public const string AddedDate = "AddedDate";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Username)]
        public string Username { get { return _username; } set { _username = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Title)]
        public string Title { get { return _title; } set { _title = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Status)]
        protected int _Status { get { return _status; } set { _status = value; SetUpToDateAsFalse(); } }

        public MiniVideoStatus Status
        {
            get { return (MiniVideoStatus)_Status; }
            set { _Status = (int)value; }
        }

        [Column(Columns.Description)]
        public string Description { get { return _description; } set { _description = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ImageURL)]
        public string ImageURL { get { return _imageURL; } set { _imageURL = value; SetUpToDateAsFalse(); } }

        [Column(Columns.VideoURL)]
        public string VideoURL { get { return _videoURL; } set { _videoURL = value; SetUpToDateAsFalse(); } }

        [Column(Columns.VideoCategory)]
        public int VideoCategory { get { return _videoCategory; } set { _videoCategory = value; SetUpToDateAsFalse(); } }

        [Column(Columns.AddedDate)]
        public DateTime AddedDate { get { return _addedDate; } set { _addedDate = value; SetUpToDateAsFalse(); } }

        private int _id, _status, _videoCategory;
        private string _title, _description, _imageURL, _videoURL, _username;
        private DateTime _addedDate;
        #endregion

        public MiniVideoCampaign() : base() { }

        public MiniVideoCampaign(int id) : base(id) { }

        public MiniVideoCampaign(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public void SetActive()
        {
            Status = MiniVideoStatus.Active;
            Save();
        }

        public void SetPaused()
        {
            Status = MiniVideoStatus.Paused;
            Save();
        }

        public override void Delete()
        {
            Status = MiniVideoStatus.Deleted;
            Save();
        }

        public static List<MiniVideoCampaign> GetAllAvaibleVideosForUser(string username)
        {
            var getVideoTime = DateTime.Now.AddDays(-AppSettings.MiniVideo.MiniVideoKeepDays);
            var query = string.Format(@"SELECT * FROM {0} WHERE [Username] != '{1}' AND [Status] != {2} AND [Status] != {3} AND [Status] != {4} AND [AddedDate] > '{5}'
                                       AND ID NOT IN (SELECT VideoId FROM UsersMiniVideoCampaigns WHERE [Username] = '{1}')",
                                      TableName, username, (int)MiniVideoStatus.Deleted, (int)MiniVideoStatus.Finished, (int)MiniVideoStatus.Paused, getVideoTime);

            return TableHelper.GetListFromRawQuery<MiniVideoCampaign>(query);
        }

        public static bool GetTitleAvability(string title)
        {
            var query = string.Format("SELECT COUNT(*) FROM {0} WHERE {1} = '{2}'",
                                      TableName, Columns.Title, title);

            return (int)TableHelper.SelectScalar(query) == 0;
        }
    }
}