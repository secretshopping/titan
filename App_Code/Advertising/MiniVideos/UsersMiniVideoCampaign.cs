using System;
using System.Data;
using Prem.PTC;

namespace Titan.MiniVideos
{
    public class UsersMiniVideoCampaign : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "UsersMiniVideoCampaigns"; } }
        protected override string dbTable { get { return TableName; } }

        #region Columns
        public static class Columns
        {
            public const string Id = "Id";
            public const string VideoId = "VideoId";
            public const string Username = "Username";            
            public const string BoughtDate = "BoughtDate";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.VideoId)]
        public int VideoId { get { return _videoId; } set { _videoId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Username)]
        public string Username { get { return _username; } set { _username = value; SetUpToDateAsFalse(); } }

        [Column(Columns.BoughtDate)]
        public DateTime BoughtDate { get { return _boughtDate; } set { _boughtDate = value; SetUpToDateAsFalse(); } }

        private int _id, _videoId;
        private string  _username;
        private DateTime _boughtDate;
        #endregion

        public UsersMiniVideoCampaign() : base() { }

        public UsersMiniVideoCampaign(int id) : base(id) { }

        public UsersMiniVideoCampaign(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }        
    }
}