using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Prem.PTC;
using Prem.PTC.Members;

public class SupportTicketReply : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "SupportTicketReplies"; } }
    protected override string dbTable { get { return TableName; } }

    #region Columns
    public static class Columns
    {
        public const string Id = "Id";
        public const string Body = "Body";
        public const string ReplyDate = "Date";
        public const string TicketId = "TicketId";
        public const string IsMember = "IsMember";
        public const string AdminName = "AdminName";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Body)]
    public string Message { get { return _message; } set { _message = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ReplyDate)]
    public DateTime ReplyDate { get { return _replyDate; } set { _replyDate = value; SetUpToDateAsFalse(); } }

    [Column(Columns.TicketId)]
    public int TicketId { get { return _ticketId; } set { _ticketId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.IsMember)]
    public bool IsMember { get { return _isMember; } set { _isMember = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AdminName)]
    public string AdminName { get { return _adminName; } set { _adminName = value; SetUpToDateAsFalse(); } }

    private int _id, _ticketId;
    private string _message, _adminName;
    private DateTime _replyDate;
    private bool _isMember;
    #endregion

    public SupportTicketReply() : base() { }
    public SupportTicketReply(int id) : base(id) { }
    public SupportTicketReply(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

    public static string GetAllTicketRepliesHtml(int ticketID, Member User, bool checkIdentity = false, bool markAsRead = false)
    {
        //Get the ticket
        SupportTicket ticket = new SupportTicket(ticketID);

        if (checkIdentity && ticket.From.Id != User.Id)
            return "";

        if (markAsRead)
            ticket.MarkAsRead();

        List<SupportTicketReply> ListReplies = TableHelper.GetListFromQuery<SupportTicketReply>(
        "WHERE TicketId = " + ticket.Id
                + " ORDER BY [Date] ASC");


        string avatarUrl = User.AvatarUrl;

        StringBuilder Text = new StringBuilder(GenerateTicketHTML(ticket.Date, ticket.From.Name, ticket.Body, true));


        for (int i = 0; i < ListReplies.Count; i++)
        {
            Text.Append(GenerateTicketHTML(ListReplies[i].ReplyDate,
                ListReplies[i].IsMember ? User.FormattedName : ListReplies[i].AdminName, ListReplies[i].Message,
                ListReplies[i].IsMember));
        }
        return Mailer.ReplaceNewLines(Text.ToString());
    }

    public static string GenerateTicketHTML(DateTime date, string author, string text2, bool IsMember)
    {
        StringBuilder user = new StringBuilder(IsMember ? Member.GetAdminUsernameLink(author) : string.Format("<span class=\"supportTicket-admin\">{0}</span>", author));
        StringBuilder text = new StringBuilder();
        text.Append("<div class='row-fluid'><div class='col-md-12 span12'><strong>");
        text.Append(user);
        //text.Append("</strong><br/><span class=\"supportTicket-date\"><strong>");
        //text.Append(date);
        text.Append("</strong></span><br /><br /></div><div class='col-md-12 span12'>");
        text.Append(text2);
        text.Append("</div></div><hr />");
        return text.ToString();
    }
}