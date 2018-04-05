using System;
using Prem.PTC.Members;
using SocialNetwork;

public partial class ConversationBox : System.Web.UI.UserControl
{
    public Conversation Conversation { get; set; }
    public Member OtherUser { get; private set; }
    public Member User { get; private set; }
    public string MessageText { get; private set; }

    public override void DataBind()
    {
        base.DataBind();      
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        User = Member.CurrentInCache;
        OtherUser = new Member(Conversation.GetOtherUserId(User.Id));

        if (NotificationManager.Get(NotificationType.PendingRepresentativePaymentRequest) > 0)
        {
            int requestsFromThatUser = ConversationMessage.GetPendingRequestForRepresentativeCount(User.Id, OtherUser.Id);
            UnreadMessagesLiteral.Text = requestsFromThatUser == 0 ? String.Empty : String.Format("({0})", requestsFromThatUser);
        }

        MessageText = Conversation.LastMessage != null ? Conversation.LastMessage.Text : String.Empty;
    }
}
