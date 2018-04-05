using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SocialNetwork
{
    [Serializable]
    public class Conversation : BaseTableObject
    {
        #region Columns
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "Conversations"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("DateTime")]
        public DateTime DateTime { get { return _DateTime; } set { _DateTime = value; SetUpToDateAsFalse(); } }

        [Column("UserIdOne")]
        public int UserIdOne { get { return _UserIdOne; } set { _UserIdOne = value; SetUpToDateAsFalse(); } }

        [Column("UserIdTwo")]
        public int UserIdTwo { get { return _UserIdTwo; } set { _UserIdTwo = value; SetUpToDateAsFalse(); } }

        #endregion

        private int _id, _UserIdOne, _UserIdTwo;
        private DateTime _DateTime;

        public List<ConversationMessage> AllMessages
        {
            get
            {
                return TableHelper.SelectRows<ConversationMessage>(TableHelper.MakeDictionary("ConversationId", this.Id));
            }
        }

        public ConversationMessage LastMessage
        {
            get
            {
                return TableHelper.GetListFromRawQuery<ConversationMessage>
                    (string.Format(@"SELECT TOP 1* FROM ConversationMessages 
                                WHERE ConversationId = {0}
                                ORDER BY DateTime DESC", this.Id)).SingleOrDefault();
            }
        }

        public static Conversation GetAndUpdate(int senderId, int recipientId, DateTime dateTime)
        {
            var conversation = Get(senderId, recipientId);
            conversation.DateTime = dateTime;

            return conversation;
        }

        public static Conversation Get(int senderId, int recipientId)
        {
            var conversation = TableHelper.GetListFromRawQuery<Conversation>
                (string.Format("SELECT TOP 1* FROM Conversations WHERE (UserIdOne = {0} AND UserIdTwo = {1}) OR (UserIdOne = {1} AND UserIdTwo = {0})", senderId, recipientId))
                .SingleOrDefault();

            if (conversation == null)
            {
                conversation = new Conversation(senderId, recipientId);
            }
            return conversation;
        }

        public int GetOtherUserId(int loggedInUserId)
        {
            if (UserIdOne == loggedInUserId)
                return UserIdTwo;
            return UserIdOne;
        }
        public Conversation()
                : base()
        { }

        public Conversation(int id) : base(id) { }

        public Conversation(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate)
        { }

        private Conversation(int userIdOne, int userIdTwo)
        {
            UserIdOne = userIdOne;
            UserIdTwo = userIdTwo;
        }
    }
}
