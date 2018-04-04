using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using SocialNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MemberExtentionMethods
{
    public static class MemberExtentionMethods
    {
        public static List<Member> GetEverUpgradedReferralsList(this Member user)
        {
            var query = string.Format(@"SELECT * FROM Users WHERE ReferrerId = {0} AND HasEverUpgraded = 1 AND IsRented = 0", user.Id);
            return TableHelper.GetListFromRawQuery<Member>(query);
        }

        public static List<Conversation> GetConversations(this Member user)
        {
            return TableHelper.GetListFromRawQuery<Conversation>(string.Format("SELECT * FROM Conversations WHERE (UserIdOne = {0} OR UserIdTwo = {0}) ORDER BY DateTime DESC", user.Id));
        }

        public static void SendMessage(this Member sender, int recipientId, string text)
        {
            SendMessage(sender, recipientId, text, MessageType.Normal, RepresentativeRequestStatus.NA, Money.Zero);
        }

        public static void SendMessage(this Member sender, int recipientId, string text, MessageType messageType, 
            RepresentativeRequestStatus representativeRequestStatus, Money representativeTransferAmount)
        {
            //if (!sender.IsFriendsWith(recipientId) && messageType == MessageType.Normal)
            //    throw new MsgException("You can only send messages to your friends.");

            if (string.IsNullOrEmpty(text))
                return;

            var sentDate = AppSettings.ServerTime;
            var conversation = Conversation.GetAndUpdate(sender.Id, recipientId, sentDate);
            conversation.Save();

            var message = new ConversationMessage(conversation.Id, sender.Id, sentDate, 
                text, messageType, representativeRequestStatus, representativeTransferAmount);

            message.Save();
        }

        public static bool IsFriendsWith(this Member user, int friendId)
        {
            return Friendship.Get(user.Id, friendId) != null;
        }

        public static bool IsFriendsWith(this Member user, Member friend)
        {
            if (user == friend)
                return true;

            return user.IsFriendsWith(friend.Id);
        }

        public static void AddFriend(this Member user, int friendId, Action onSuccess)
        {
            if(user.Id == friendId)
                throw new MsgException("You should already be friends with yourself. Otherwise, please contact your local support group.");
            if (user.IsFriendsWith(friendId))
                throw new MsgException("You are already friends.");
            if (FriendshipRequest.Exists(user.Id, friendId))
                throw new MsgException("You have already sent a request to this user.");

            FriendshipRequest.Create(user.Id, friendId);

            onSuccess();
        }

        public static void AddFriend(this Member user, Member friend, Action onSuccess)
        {
            user.AddFriend(friend.Id, onSuccess);
        }

        public static List<FriendshipRequest> GetFriendshipRequests(this Member user, FriendshipRequestStatus status, bool byMe)
        {
            return user.GetFriendshipRequests(byMe).Where(r => r.Status == status).ToList();
        }

        public static List<FriendshipRequest> GetFriendshipRequests(this Member user, bool byMe)
        {
            StringBuilder query = new StringBuilder();
            string target = "SenderId";
            if (!byMe)
                target = "RecipientId";

            query.Append("SELECT * FROM FriendshipRequests WHERE ");
            query.Append(target + " = ");
            query.Append(user.Id);
            query.Append(" ORDER BY DateTime DESC;");
            return TableHelper.GetListFromRawQuery<FriendshipRequest>(query.ToString());
        }

        public static List<Post> GetPosts(this Member user)
        {
            string query = string.Format("SELECT * FROM Posts WHERE AuthorId = {0} ORDER BY DateTime DESC", user.Id);
            return TableHelper.GetListFromRawQuery<Post>(query.ToString());
        }

        public static void IncreaseMetrixBonusEarnings(this Member user, Money value)
        {
            user.MatrixBonusMoneyIncome += value;
            user.TotalEarned += new Money(50);
        }
    }
}