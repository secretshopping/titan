using System;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;

public partial class sites_ticket : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string IsTicket = string.IsNullOrEmpty(Request.QueryString["ticketId"]) ? "" : Request.QueryString["ticketId"];
        if (IsTicket != "")
        {
            TicketLiteral.Text = SupportTicketReply.GetAllTicketRepliesHtml(Convert.ToInt32(IsTicket), Member.CurrentInCache, true, true);
            NotificationManager.Refresh(NotificationType.UnreadSupportTickets);
        }
    }

    protected void SupportTicketButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (Member.IsLogged)
            {
                var ticket = new SupportTicket(Convert.ToInt32(Request.QueryString["ticketId"]));
                var InText = InputChecker.HtmlEncode(SupportTicketTextBox.Text, SupportTicketTextBox.MaxLength, U5004.MESSAGE);

                ticket.ReplyFromMember(InText);
                ticket.IsSolved = false;
                ticket.Date = DateTime.Now;
                ticket.Save();
                SuccMessage.Text = U3501.SUPPSENT;

                TicketLiteral.Text = SupportTicketReply.GetAllTicketRepliesHtml(ticket.Id, Member.Current, true);

                //Clear the fields
                ErrorMessagePanel.Visible = false;
                SupportTicketTextBox.Text = "";
            }
        }
        catch (MsgException ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }
}