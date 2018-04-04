using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Text;
using System.Web.UI.WebControls;

public partial class LeadershipRewards : System.Web.UI.Page
{
    public Member user; 
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.ReferralsLeadershipEnabled);

        if (!IsPostBack)
        {
            user = Member.Current;

            AddTexts();
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


    private void AddTexts()
    {
        Button1.Text = U5006.YOURLEVEL;
        Button2.Text = U5006.LEADERSHIPLEVELS;

        var currentLeadership = user.CurrentLeadership;
        var nextLeadership = user.NextLeadership;
        if (currentLeadership != null)
            CurrentLevelLiteral.Text = currentLeadership.Name;
        else
            CurrentLevelPlaceHolder.Visible = false;

            NextLevelLiteral.Text = user.NextLeadership.Name;
            DirectReferralsLiteral.Text = user.DirectReferralsWithHighestMembership.Count.ToString();
            IndirectReferralsLiteral.Text = user.InDirectReferralsWithHighestMembership.ToString();
            TeamPartnersLiteral.Text = user.NumberOfTeamPartners.ToString();
            TotalFreshFundsLiteral.Text = user.TotalTeamDeposits.ToString();
            TotalDateSpanLiteral.Text = user.LeadershipResetTime.ToShortDateString() + " - " + user.LeadershipDeadline.ToShortDateString();

        StringBuilder subtotals = new StringBuilder();
        DateTime startDate = user.LeadershipResetTime;
        int daySpan = 0;
        for (int i = 0; i < user.TeamSubDeposits.Count; i++)
        {
            subtotals.AppendFormat(@"<tr>
                                <td>{0} - {1}</td>
                                         <td>{2}</td>
                                     </tr>", user.TeamSubDeposits[i].StartDate.ToShortDateString(), user.TeamSubDeposits[i].EndDate.ToShortDateString(), user.TeamSubDeposits[i].TeamDeposits.ToString());
            startDate = startDate.AddDays(nextLeadership.SubTimeConstraint);
        }
        SubTotalFunds.Text = subtotals.ToString();

    }

    protected void LeadershipLevelsGridViewDataSource_Init(object sender, EventArgs e)
    {
        LeadershipLevelsGridViewDataSource.SelectCommand = "SELECT * FROM ReferralLeadershipLevels";
    }

    protected void LeadershipLevelsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[6].Text = new Money(Convert.ToDecimal(e.Row.Cells[6].Text)).ToString() + "/" + e.Row.Cells[8].Text + " " +  L1.DAYS;
            e.Row.Cells[7].Text = new Money(Convert.ToDecimal(e.Row.Cells[7].Text)).ToString() + "/" + Convert.ToInt32(e.Row.Cells[8].Text)/ Convert.ToInt32(e.Row.Cells[9].Text) + " " + L1.DAYS;

        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Text = L1.NAME;
            e.Row.Cells[2].Text = U4000.REWARD;
            e.Row.Cells[3].Text = U3000.INDIRECTREFERRALS;
            e.Row.Cells[4].Text = L1.DIRECT + " " + L1.REFERRALS;
            e.Row.Cells[5].Text = U5006.TEAMPARTNERS;
            e.Row.Cells[6].Text = string.Format(U5006.FRESHFUNDS);
            e.Row.Cells[7].Text = string.Format(U5006.FRESHFUNDS);
        }
    }

    protected void View2_Activate(object sender, EventArgs e)
    {
        LeadershipLevelsGridView.DataBind();
    }
}