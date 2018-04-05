using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Data;

namespace SocialNetwork
{
    public class ConversationMessage : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "ConversationMessages"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("ConversationId")]
        public int ConversationId { get { return _ConversationId; } set { _ConversationId = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

        [Column("DateTime")]
        public DateTime DateTime { get { return _DateTime; } set { _DateTime = value; SetUpToDateAsFalse(); } }

        [Column("Text")]
        public string Text { get { return _Text; } set { _Text = value; SetUpToDateAsFalse(); } }

        [Column("MessageType")]
        protected int MessageTypeInt { get { return _MessageTypeInt; } set { _MessageTypeInt = value; SetUpToDateAsFalse(); } }

        [Column("RepresentativeRequestStatus")]
        private int RepresentativeRequestStatusInt { get { return _RepresentativeRequestStatusInt; } set { _RepresentativeRequestStatusInt = value; SetUpToDateAsFalse(); } }

        [Column("RepresentativeTransferAmount")]
        public Money RepresentativeTransferAmount { get { return _RepresentativeTransferAmount; } set { _RepresentativeTransferAmount = value; SetUpToDateAsFalse(); } }

        [Column("IsRead")]
        public bool IsRead { get { return _IsRead; } set { _IsRead = value; SetUpToDateAsFalse(); } }

        private int _id, _UserId, _ConversationId, _MessageTypeInt, _RepresentativeRequestStatusInt;
        private DateTime _DateTime;
        private Money _RepresentativeTransferAmount;
        private Conversation conversation;
        string _Text;
        bool _IsRead;

        public ConversationMessage()
                : base()
        { }

        public ConversationMessage(int id) : base(id) { }

        public ConversationMessage(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate)
        { }

        public MessageType MessageType
        {
            get
            {
                return (MessageType)MessageTypeInt;
            }
            set
            {
                MessageTypeInt = (int)value;
            }
        }

        public Conversation Conversation
        {
            get
            {
                if (conversation == null)
                    conversation = new Conversation(this.ConversationId);
                return conversation;
            }
        }

        public RepresentativeRequestStatus RepresentativeRequestStatus
        {
            get
            {
                return (RepresentativeRequestStatus)RepresentativeRequestStatusInt;
            }
            set
            {
                RepresentativeRequestStatusInt = (int)value;
            }
        }

        #endregion

        public static int GetPendingRequestForRepresentativeCount(int representativeId)
        {
            var query = string.Format("SELECT COUNT(*) FROM ConversationMessages WHERE RepresentativeRequestStatus = {1} AND " +
                "ConversationId IN (SELECT Id FROM Conversations WHERE UserIdOne = {0} OR UserIdTwo = {0}) AND UserId != {0} ",
                    representativeId, (int)RepresentativeRequestStatus.Pending);

            return (int)TableHelper.SelectScalar(query);
        }

        public static int GetPendingRequestForRepresentativeCount(int representativeId, int userId)
        {
            var query = string.Format("SELECT COUNT(*) FROM ConversationMessages WHERE RepresentativeRequestStatus = {1} AND " +
                "ConversationId IN (SELECT Id FROM Conversations WHERE UserIdOne = {0} OR UserIdTwo = {0}) AND UserId = {2} ",
                    representativeId, (int)RepresentativeRequestStatus.Pending, userId);

            return (int)TableHelper.SelectScalar(query);
        }

        public ConversationMessage(int conversationId, int senderId, DateTime dateTime, string text, MessageType messageType,
            RepresentativeRequestStatus representativeRequestStatus, Money representativeTransferAmount)
        {
            ConversationId = conversationId;
            UserId = senderId;
            DateTime = dateTime;
            Text = text;
            MessageType = messageType;
            RepresentativeRequestStatus = representativeRequestStatus;
            RepresentativeTransferAmount = representativeTransferAmount;
            IsRead = false;
        }

        public string TryConfirmTransaction(int userId)
        {
            CheckRepresentativeModificationPermissions(userId);

            RepresentativesTransferManager representativesTransferManager = new RepresentativesTransferManager(
                Conversation.GetOtherUserId(userId), userId);

            string ResultMessage = String.Empty;

            if (this.MessageType == MessageType.RepresentativeDepositRequest)
            {
                ResultMessage = representativesTransferManager.TryConfirmDeposit(this);
            }

            if (this.MessageType == MessageType.RepresentativeWithdrawalRequest)
            {
                ResultMessage = representativesTransferManager.ConfirmWithdrawal(this);
            }

            this.RepresentativeRequestStatus = RepresentativeRequestStatus.Completed;
            this.Save();

            return ResultMessage;
        }

        public void TryDisputeTransaction(int userId)
        {
            CheckRepresentativeModificationPermissions(userId);

            this.RepresentativeRequestStatus = RepresentativeRequestStatus.InDispute;
            this.Save();
        }

        public void TryRejectTransaction(int userId)
        {
            CheckRepresentativeModificationPermissions(userId);

            RepresentativesTransferManager representativesTransferManager = new RepresentativesTransferManager(
                Conversation.GetOtherUserId(userId), userId);

            this.RepresentativeRequestStatus = RepresentativeRequestStatus.Rejected;
            this.Save();

            representativesTransferManager.RejectWithdrawal(this);
        }

        public void TryCancelTransaction(int userId)
        {
            int representativeId = Conversation.GetOtherUserId(userId);

            if (MessageType != MessageType.RepresentativeDepositRequest)
                CheckRepresentativeModificationPermissions(representativeId);
            else
                CheckRepresentativeModificationPermissions(representativeId, false);

            RepresentativesTransferManager representativesTransferManager = new RepresentativesTransferManager(
                userId, representativeId);

            representativesTransferManager.TryCancelDeposit(Member.CurrentInCache.Name, this);
            this.RepresentativeRequestStatus = RepresentativeRequestStatus.Cancelled;
            this.Save();
        }

        private void CheckRepresentativeModificationPermissions(int userId, bool CheckWithCurrentUser = true)
        {
            if (this.MessageType == MessageType.Normal)
                throw new MsgException("This message is not a transaction.");

            if (Conversation.UserIdOne != userId && Conversation.UserIdTwo != userId)
                throw new MsgException("This is not your conversation.");

            if (this.UserId == userId && CheckWithCurrentUser)
                throw new MsgException("You can't confirm your own transaction.");

            if (this.RepresentativeRequestStatus != RepresentativeRequestStatus.Pending && this.RepresentativeRequestStatus != RepresentativeRequestStatus.InDispute)
                throw new MsgException("This transaction cannot be modified.");
        }

        public static int GetNumberOfUnreadMessages(int userId)
        {
            String Query = String.Format(@" SELECT COUNT(*) FROM ConversationMessages WHERE IsRead=0 AND MessageType={0} AND UserId != {1} AND
                                            ConversationId IN (SELECT Id FROM Conversations WHERE UserIdOne={1} OR UserIdTwo={1})", (int)MessageType.Normal, userId);
            return (int)TableHelper.SelectScalar(Query);
        }

        public static void SetConversationMessagesAsRead(int conversationId, int userId)
        {
            String Query = String.Format(@" UPDATE ConversationMessages SET IsRead=1 WHERE IsRead=0 AND MessageType={0} AND UserId!={1} AND
                                            ConversationId={2}",
                                            (int)MessageType.Normal, userId, conversationId);
            TableHelper.ExecuteRawCommandNonQuery(Query);
        }

        public static bool CheckIfThisUserHavePendingActions(int currentUserId)
        {
            String Query = String.Format(@"SELECT COUNT(*) FROM ConversationMessages WHERE 
                                            MessageType IN({0},{1}) AND
                                            RepresentativeRequestStatus={2} AND
                                            ConversationId IN
                                                (SELECT Id FROM Conversations WHERE 
                                                    UserIdOne={3} OR UserIdTwo={3} )",
                                                    (int)MessageType.RepresentativeDepositRequest,
                                                    (int)MessageType.RepresentativeWithdrawalRequest,
                                                    (int)RepresentativeRequestStatus.Pending,
                                                    currentUserId);

            return (Convert.ToInt32(TableHelper.SelectScalar(Query)) > 0) ? true : false;
        }

    }
}
