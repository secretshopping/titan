using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC;
using Resources;
using Prem.PTC.Advertising;

public partial class About : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnersRoleEnabled && 
            AppSettings.TitanFeatures.EarnTrafficGridEnabled && Member.CurrentInCache.IsEarner);

        if (!Page.IsPostBack)
        {
            Form.Action = Request.RawUrl;

            LangAdder.Add(Button2, L1.LATESTWINNERS);
            LangAdder.Add(Button3, L1.TOPWINNERS);

            Member User = Member.Current;

            L_Chances.Text = User.Membership.TrafficGridChances.ToString();
            L_Clicked.Text = User.TrafficGridHitsToday.ToString();
            L_Duration.Text = User.Membership.TrafficGridShorterAd.ToString();
            L_Max.Text = User.Membership.TrafficGridTrials.ToString();

            TheGrid.Text = TrafficGridManager.GenerateGridHTML();

            if (!TrafficGridManager.IsOn)
            {
                GridOff.Visible = true;
                GridOn.Visible = false;
            }

            GridView1.EmptyDataText = L1.NODATA;
            GridView2.EmptyDataText = L1.NODATA;

            if ((!AppSettings.TitanFeatures.EarnTrafficExchangeEnabled && AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled) ||
                (AppSettings.TrafficGrid.P_RentalBalance == 0))
                trafficBalance.Visible = false;

            if ((!AppSettings.TitanFeatures.ReferralsRentedEnabled) ||
                (AppSettings.TrafficGrid.P_RentedReferral == 0))
                rentedReferrals.Visible = false;

            if (AppSettings.TrafficGrid.P_Points == 0)
                points.Visible = false;

            if (AppSettings.TrafficGrid.P_MainBalance == 0)
                mainBalance.Visible = false;

            if (AppSettings.TrafficGrid.P_AdBalance == 0)
                adBalance.Visible = false;

            if (AppSettings.TrafficGrid.P_DirectReferralLimit == 0)
                drLimit.Visible = false;

        }
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("trafficgrid.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";

    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Create an instance of the datarow
            var rowData = e.Row.Cells[2].Text;

            //Lets generate avatar + colored login 
            Member User = new Member(rowData);
            e.Row.Cells[2].Text = HtmlCreator.CreateAvatarPlusUsername(User);
            e.Row.Cells[4].Text = "<img src=\"Images/Flags/" + User.CountryCode.ToLower() + ".png\" />";

        }
    }
    protected void TrafficRefreshUpdatePanel_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.Params.Get("__EVENTARGUMENT")))
        {
            string agrument = Request.Params.Get("__EVENTARGUMENT");
            if (PTCSercurityManager.IsTrafficAdvertFound(agrument))
            {
                PTCSercurityManager.Release();
                L_Clicked.Text = Member.Current.TrafficGridHitsToday.ToString();
            }
        }
    }

    public string ImageUrl
    {
        get
        {
            int count = 4;
            if (string.IsNullOrEmpty(AppSettings.TrafficGrid.Image4))
                count = 3;
            if (string.IsNullOrEmpty(AppSettings.TrafficGrid.Image3))
                count = 2;
            if (string.IsNullOrEmpty(AppSettings.TrafficGrid.Image2))
                count = 1;

            Random rand = new Random();
            int result = rand.Next(1, count + 1);

            if (result == 1)
                return AppSettings.Site.Url + AppSettings.TrafficGrid.Image1;

            if (result == 2)
                return AppSettings.Site.Url + AppSettings.TrafficGrid.Image2;

            if (result == 3)
                return AppSettings.Site.Url + AppSettings.TrafficGrid.Image3;

            if (result == 4)
                return AppSettings.Site.Url + AppSettings.TrafficGrid.Image4;

            return "";
        }
    }
}
