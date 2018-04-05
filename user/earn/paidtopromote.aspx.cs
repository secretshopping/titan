using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Web.UI.WebControls;
using Titan.PaidToPromote;

public partial class user_earn_paidtopromote : System.Web.UI.Page
{
    private int UserId
    {
        get
        {
            return Member.CurrentId;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnPaidToPromoteEnabled);

        if (!IsPostBack)
        {
            Button1.Text = L1.MANAGE;
            GetLinkButton.Text = U6009.GETYOURLINK;
            SetContent();
        }
    }

    protected void GetLinkButton_Click(object sender, EventArgs e)
    {
        PaidToPromoteManager.AddUserToPool(UserId);
        SetContent();
    }

    private void SetContent()
    {
        if (PaidToPromoteManager.IsInPool(UserId))
        {
            InactiveUserPlaceHolder.Visible = false;
            ActiveUserPlaceHolder.Visible = true;
            
            LinkOpensLabel.Text = PaidToPromoteManager.GetUserClicks(UserId);
            RotatorLinkLiteral.Text = String.Format("<a href='{0}'>{0}</a>", PaidToPromoteManager.GetUserLink(UserId));
        }
        else
        {
            InactiveUserPlaceHolder.Visible = true;
            ActiveUserPlaceHolder.Visible = false;
        }     
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        SPanel.Visible = false;
        EPanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }
}