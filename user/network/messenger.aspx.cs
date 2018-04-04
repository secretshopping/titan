using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SocialNetwork;
using MemberExtentionMethods;
using Resources;
using System.Web;

public partial class messenger : System.Web.UI.Page
{
    List<Conversation> conversations;
    Member user;
    public Member Recipient { get; set; }
    public Conversation SelectedConversation
    {
        get
        {
            if (ViewState["SelectedConversation"] != null)
                return (Conversation)ViewState["SelectedConversation"];
            else
                return null;
        }
        set
        {
            ViewState["SelectedConversation"] = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.PeopleMessagesEnabled);
        user = Member.CurrentInCache;
        StatusPlaceHolder.Visible = false;

        if (Request.QueryString["recipientId"] != null)
        {
            int recipientId = Convert.ToInt32(Request.QueryString["recipientId"]);
            if (recipientId == user.Id)
                Response.Redirect("messenger.aspx");
            try
            {
                Recipient = new Member(recipientId);
            }
            catch
            {
                Response.Redirect("messenger.aspx");
            }
        }

        if (Request.QueryString["cid"] != null)
        {
            try
            {
                var conversation = new Conversation(Convert.ToInt32(Request.QueryString["cid"]));

                if (conversation.UserIdOne == Member.CurrentId || conversation.UserIdTwo == Member.CurrentId)
                    SelectedConversation = conversation;

                ConversationMessage.SetConversationMessagesAsRead(Convert.ToInt32(Request.QueryString["cid"]), Member.CurrentId);
            }
            catch (Exception ex) { }
        }

        if (!IsPostBack)
        {
            SendButton.Text = L1.SEND;

            #region Actions

            try
            {
                if (Request.QueryString["action"] != null && Request.QueryString["id"] != null)
                {
                    ErrorMessagePanel.Visible = false;
                    SuccMessagePanel.Visible = false;
                    StatusPlaceHolder.Visible = true;

                    //Perform actions
                    ConversationMessage conversationMessage = new ConversationMessage(Convert.ToInt32(Request.QueryString["id"]));

                    SuccMessage.Text = String.Empty;

                    if (Request.QueryString["action"] == "confirm")
                    {
                        SuccMessage.Text = conversationMessage.TryConfirmTransaction(Member.CurrentId);                      
                    }

                    if (Request.QueryString["action"] == "dispute")
                    {
                        conversationMessage.TryDisputeTransaction(Member.CurrentId);
                        SuccMessage.Text = U6010.DEPOSITDISPUTEINFO;
                    }
                        
                    if (Request.QueryString["action"] == "reject")
                        conversationMessage.TryRejectTransaction(Member.CurrentId);

                    if (Request.QueryString["action"] == "cancel")
                    {
                        conversationMessage.TryCancelTransaction(Member.CurrentId);
                        SuccMessage.Text = String.Format(U6010.DEPOSITCANCELEDINFO);
                    }
                        
                    NotificationManager.Refresh(NotificationType.PendingRepresentativePaymentRequest);
                    SuccMessagePanel.Visible = true;
                }
            }
            catch (MsgException ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            catch (Exception) { }

            #endregion
        }

        BindConversations();
        BindMessages();

        OtherMemberNameLiteral.Text = U5004.MESSAGE;
        MessageTextBox.Attributes.Add("placeholder", U6000.ENTERYOURMESSAGEHERE);
    }


    private void BindConversations()
    {
        conversations = user.GetConversations();

        ConversationsPlaceHolder.Controls.Clear();

        if (conversations.Count == 0)
        {
            var noConversationsLiteral = new Literal();
            noConversationsLiteral.Text = string.Format("<div class='selected'>{0}</div>", U6000.YOUHAVENOCONVERSATIONS);
            ConversationsPlaceHolder.Controls.Add(noConversationsLiteral);
        }
        else
        {
            if (Recipient != null)
            {
                SelectedConversation = Conversation.Get(user.Id, Recipient.Id);
            }
            else if (SelectedConversation == null)
            {
                SelectedConversation = conversations[0];
            }

            for (int i = 0; i < conversations.Count; i++)
            {
                UserControl conversationBox = (UserControl)Page.LoadControl("~/Controls/Network/ConversationBox.ascx");

                PropertyInfo myProp = conversationBox.GetType().GetProperty("Conversation");
                myProp.SetValue(conversationBox, conversations[i], null);

                conversationBox.DataBind();
                var div = new Panel();
                var wraper = new LinkButton();
                wraper.PostBackUrl = "/user/network/messenger.aspx?cid=" + conversations[i].Id;
                wraper.Controls.Add(conversationBox);
                wraper.CommandArgument = conversations[i].Id.ToString();
                div.Controls.Add(wraper);
                ConversationsPlaceHolder.Controls.Add(div);

                if (conversations[i].Id == SelectedConversation.Id)
                    div.CssClass = "selected bg-info";
                else
                    div.CssClass = "";
            }
        }
    }

    private void BindMessages()
    {
        if (SelectedConversation != null)
        {
            var messages = SelectedConversation.AllMessages;
            StringBuilder sb = new StringBuilder();
            int otherId = SelectedConversation.GetOtherUserId(Member.CurrentId);

            var otherMember = new Member(otherId);
            var avatarImg = string.Format("<a href=\"javascript:;\" class=\"image\"><img src='{0}' /></a>", ResolveUrl(otherMember.AvatarUrl));

            int lastMemberId = 0;

            ConversationMessage.SetConversationMessagesAsRead(SelectedConversation.Id, Member.CurrentId);

            foreach (var m in messages)
            {
                var isMyMessage = m.UserId == Member.CurrentId;
                sb.AppendFormat("<li class='{0}'>", isMyMessage ? "right" : "left");

                StringBuilder messageText = new StringBuilder();

                if (m.MessageType == MessageType.RepresentativeDepositRequest || m.MessageType == MessageType.RepresentativeWithdrawalRequest)
                {
                    //If action is pending and escrow time finished, change action status
                    if (ConversationManager.GetEscrowTimeLeft(m.DateTime) <= 0 && m.RepresentativeRequestStatus == RepresentativeRequestStatus.Pending)
                    {
                        m.RepresentativeRequestStatus = RepresentativeRequestStatus.InDispute;
                        m.Save();
                    }

                    string commandText = m.MessageType == MessageType.RepresentativeDepositRequest ? U6010.DEPOSITVIAREPRESENTATIVE : U6010.WITHDRAWVIAREPRESENTATIVE;
                    messageText.AppendFormat("<div class=\"alert alert-warning fade in m-b-15 text-center w-100\">{0}.<br/>{1}: <b>{2}</b> ({5} - {6})<br/>{3}: <b>{4}</b>",
                        commandText, L1.AMOUNT, m.RepresentativeTransferAmount, L1.STATUS, m.RepresentativeRequestStatus.ToString(), U6010.WITHDRAWALFEE, 
                        NumberUtils.FormatPercents(AppSettings.Representatives.RepresentativesHelpWithdrawalFee));

                    if(m.RepresentativeRequestStatus == RepresentativeRequestStatus.Pending)
                        messageText.AppendFormat("<br/><br/>{0}: {1}", U4200.TIMELEFT, ConversationManager.GetHtmlEscrowTimeLeft(m.DateTime));

                    if (m.RepresentativeRequestStatus == RepresentativeRequestStatus.Pending)
                    {
                        if(m.UserId != Member.CurrentId)
                        {
                            messageText.AppendFormat("<br/><br/>");
                            messageText.AppendFormat("<a href=\"user/network/messenger.aspx?action=confirm&id={1}\" class='btn btn-primary btn-xs m-r-5 btn-success'><i class='fa fa-check'></i> {0}</a>", L1.CONFIRM, m.Id);

                            if (m.MessageType == MessageType.RepresentativeDepositRequest)
                                messageText.AppendFormat("<a href=\"user/network/messenger.aspx?action=dispute&id={1}\" class='btn btn-primary btn-xs m-r-5 btn-danger'><i class='fa fa-exclamation'></i> {0}</a>", U6010.DISPUTE, m.Id);

                            if (m.MessageType == MessageType.RepresentativeWithdrawalRequest)
                                messageText.AppendFormat("<a href=\"user/network/messenger.aspx?action=reject&id={1}\" class='btn btn-primary btn-xs m-r-5 btn-danger'><i class='fa fa-exclamation'></i> {0}</a>", U6010.REJECT, m.Id);
                        }
                        else if(m.MessageType == MessageType.RepresentativeDepositRequest)
                            messageText.AppendFormat("<br /><a href=\"user/network/messenger.aspx?action=cancel&id={1}\" class='btn btn-primary btn-xs m-r-5 btn-warning'><i class='fa fa-ban'></i> {0}</a>", U4000.CANCEL, m.Id);
                    }
                        
                    messageText.AppendFormat("</div>");
                }

                messageText.Append(m.Text);

                if (lastMemberId == m.UserId)
                {
                    sb.AppendFormat("<div class=\"row\"><div class=\"col-md-12\"><div class=\"message p-20 no-direction {0}\">{1}</div></div></div>", isMyMessage ? "my-message" : "", messageText);
                }
                else
                {
                    sb.AppendFormat("<div class=\"row\"><div class=\"col-md-12\"><a href=\"javascript:;\" class=\"name\">{0}</a>", isMyMessage ? Member.CurrentName : otherMember.Name);
                    sb.AppendFormat("<span class=\"date-time\">{0}</span></div></div>", m.DateTime.ToLongDateString() + " " + m.DateTime.ToLongTimeString());
                    sb.AppendFormat("<div class=\"row\"><div class=\"col-md-12\">{0}<div class=\"message p-20 {1}\">{2}</div></div></div>", isMyMessage ? "" : avatarImg, isMyMessage ? "my-message" : "", messageText);
                }
                sb.Append("</li>");

                lastMemberId = m.UserId;
            }

            MessagesLiteral.Text = sb.ToString();
            OtherMemberNameLiteral.Text = otherMember.Name;
        }
    }

    protected void SendButton_Click(object sender, EventArgs e)
    {
        var message = InputChecker.HtmlEncode(MessageTextBox.Text, MessageTextBox.MaxLength, "message");

        if (Recipient == null)
            Recipient = new Member(SelectedConversation.GetOtherUserId(Member.CurrentId));

        user.SendMessage(Recipient.Id, message);
        MessageTextBox.Text = string.Empty;
        BindConversations();
        BindMessages();
    }
}