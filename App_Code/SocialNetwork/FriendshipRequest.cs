using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace SocialNetwork
{
    public class FriendshipRequest : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "FriendshipRequests"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("SenderId")]
        public int SenderId { get { return _SenderId; } set { _SenderId = value; SetUpToDateAsFalse(); } }

        [Column("RecipientId")]
        public int RecipientId { get { return _RecipientId; } set { _RecipientId = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

        [Column("DateTime")]
        public DateTime DateTime { get { return _DateTime; } set { _DateTime = value; SetUpToDateAsFalse(); } }

        public FriendshipRequestStatus Status
        {
            get { return (FriendshipRequestStatus)StatusInt; }
            set { StatusInt = (int)value; }
        }

        int _id, _SenderId, _RecipientId, _StatusInt;
        DateTime _DateTime;
        #endregion Columns

        public FriendshipRequest()
                : base() { }

        public FriendshipRequest(int id) : base(id) { }

        public FriendshipRequest(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

        public static bool Exists(int senderId, int recipientId)
        {
            var whereDict = TableHelper.MakeDictionary("SenderId", senderId);
            whereDict.Add("RecipientId", recipientId);
            return TableHelper.RowExists(FriendshipRequest.TableName, whereDict);
        }

        public static bool IsPending(int senderId, int recipientId)
        {
            var whereDict = TableHelper.MakeDictionary("SenderId", senderId);
            whereDict.Add("RecipientId", recipientId);
            whereDict.Add("Status", (int)FriendshipRequestStatus.Pending);
            return TableHelper.RowExists(FriendshipRequest.TableName, whereDict);
        }

        public static FriendshipRequest Get(int userIdOne, int userIdTwo)
        {
            string query = string.Format(@"SELECT TOP 1* FROM FriendshipRequests 
            WHERE (SenderId = {0} AND RecipientId = {1}) OR (SenderId = {1} AND RecipientId = {0})",
            userIdOne, userIdTwo);
           
            return TableHelper.GetListFromRawQuery<FriendshipRequest>(query).FirstOrDefault();
        }

        private FriendshipRequest(int senderId, int recipientId)
        {
            SenderId = senderId;
            RecipientId = recipientId;
            Status = FriendshipRequestStatus.Pending;
            DateTime = AppSettings.ServerTime;
        }
        public void Accept()
        {
            Status = FriendshipRequestStatus.Accepted;
            this.Save();
            Friendship.Create(this.SenderId, this.RecipientId);
        }

        public void Reject()
        {
            Status = FriendshipRequestStatus.Rejected;
            this.Save();
        }

        public static void Create(int senderId, int recipientId)
        {
            var friendshipRequest = new FriendshipRequest(senderId, recipientId);
            friendshipRequest.Save();
        }
    }
    public enum FriendshipRequestStatus
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3,
    }
}