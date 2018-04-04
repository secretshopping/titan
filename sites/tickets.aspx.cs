using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using Resources;

public partial class sites_tickets : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Member.IsLogged)
            Response.Redirect("~/login.aspx");
        
        LangAdder.Add(Button3, L1.SENDSUPPORTTICKET);
        LangAdder.Add(Button4, U3501.VIEWTICKETS);

        TicketUserName.Text = Member.CurrentName;
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("~/sites/contact.aspx");
        else
            Response.Redirect("~/sites/tickets.aspx");

    }

    protected void MessageGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Get the ticket
            SupportTicket ticket = new SupportTicket(Convert.ToInt32(e.Row.Cells[0].Text));

            if (!AppSettings.SupportTickets.TicketDepartmentsEnabled)
                MessageGridView.Columns[2].Visible = false;
            else
            {
                var allDepartments = SupportTicketDepartment.AllDepartments;
                foreach (ListItem item in allDepartments)
                {
                    if (item.Value == e.Row.Cells[2].Text)
                        e.Row.Cells[2].Text = item.Text;
                }
            }

            //Bold not solved
            var check = (CheckBox)e.Row.Cells[5].Controls[0];
            if (!check.Checked)
            {
                //e.Row.BackColor = System.Drawing.Color.FromArgb(254, 255, 223);
                e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#f0f0f0");
            }
        }
    }

    protected void MessageGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Read")
        {

            int rowIndex = e.GetSelectedRowIndex() % MessageGridView.PageSize;
            var dataKey = MessageGridView.DataKeys[rowIndex].Values["SupportTicketId"].ToString();


            Response.Redirect(String.Format("~/sites/ticket.aspx?ticketId={0}", dataKey));
            
        }
    }

    protected void MessageGridView_DataBound(object sender, EventArgs e)
    {

    }
}