using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Contests;
using Resources;
using System.Reflection;

public partial class About : System.Web.UI.Page
{
    bool noContestsAvailable = true;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnContestsEnabled && Member.CurrentInCache.IsEarner);

        if (Request.QueryString["participate"] != null)
        {
            try
            {
                int contestId = Convert.ToInt32(Request.QueryString["participate"]);
                var con = new Contest(contestId);
                con.Participate(Member.Current);
                Response.Redirect("contests.aspx");
            }
            catch (Exception ex)
            {
            }
        }

        LatestLiteralDR.Text = L1.LATESTKINDWINNERS + ":";
        Member User = Member.Current;

        //Check if some contests should be finished
        ContestManager.CheckFinished();

        //Load contests
        LoadContests(ContestType.Direct, Contests_DR, User, Button1, VDR);
        LoadContests(ContestType.Rented, Contests_RR, User, Button2, VRR);
        LoadContests(ContestType.Click, Contests_Click, User, Button6, VC);
        LoadContests(ContestType.Transfer, Contests_Transfer, User, Button5, VT);
        LoadContests(ContestType.Offerwalls, Contests_Offerwalls, User, Button4, VO);
        LoadContests(ContestType.CrowdFlower, Contests_Crowdflower, User, Button3, VCL);
        LoadContests(ContestType.Forum, Contests_Forum, User, Button7, FC);
        LoadContests(ContestType.Offer, Contests_Offer, User, Button8, OC);

        if (!Page.IsPostBack)
        {
            //Load latest winners
            LoadLatestWinners(ContestType.Direct, Latest_DR);
            LoadLatestWinners(ContestType.Rented, Latest_RR);
            LoadLatestWinners(ContestType.Click, Latest_Click);
            LoadLatestWinners(ContestType.Transfer, Latest_Transfer);
            LoadLatestWinners(ContestType.Offerwalls, Latest_Offerwalls);
            LoadLatestWinners(ContestType.CrowdFlower, Latest_Crowdflower);
            LoadLatestWinners(ContestType.Forum, Latest_Forum);
            LoadLatestWinners(ContestType.Offer, Latest_Offer);

            Button8.Text = U5004.OFFERCONTEST;
        }

        if (noContestsAvailable)
        {
            Contests_Info.Text = U3900.NOCONTESTS;
            Latest_DR.Text = "";
            LatestLiteralDR.Text = "";
        }

    }

    private void LoadContests(ContestType Type, PlaceHolder ContestLiteral, Member User, Button button, View view)
    {
        var list = ContestManager.GetActiveContestsForMember(Type, User);

        foreach (var contest in list)
            ContestLiteral.Controls.Add(GetContestCode(contest, User));

        if (list.Count == 0)
        {
            button.Visible = false;
        }
        else if (noContestsAvailable)
        {
            //First of that kind, set the active view
            MenuMultiView.SetActiveView(view);
            noContestsAvailable = false;
        }

    }

    private void LoadLatestWinners(ContestType Type, Literal LatestLiteral)
    {
        var List = ContestManager.GetLastestWinners(Type);
        LatestLiteral.Text += "<li>1st: " + List[0] + "</li>";
        LatestLiteral.Text += "<li>2nd: " + List[1] + "</li>";
        LatestLiteral.Text += "<li>3rd: " + List[2] + "</li>";
    }

    private UserControl GetContestCode(Contest contest, Member User)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Misc/Contest.ascx");
        var parsedControl = objControl as ICustomObjectControl;
        parsedControl.ObjectID = contest.Id;
        parsedControl.DataBind();

        return objControl;
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("contests.aspx");

        if (viewIndex == 2 && !String.IsNullOrWhiteSpace(OfferwallHelper.GetOfferWallsIncludedInPTCContest()))
        {
            OfferWallsLabel.Visible = true;
            OfferWallsLabel.Text = U4200.OFFERWALLSINCLICKCONTEST + ": ";
            OfferWallsLabel.Text += OfferwallHelper.GetOfferWallsIncludedInPTCContest();
        }
        else
            OfferWallsLabel.Visible = false;

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }
}
