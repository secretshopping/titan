using System;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC;
using Resources;

public partial class About : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnersRoleEnabled && AppSettings.TitanFeatures.EarnCrowdFlowerEnabled && Member.CurrentInCache.IsEarner);

        AppSettings.Offerwalls.Reload();

        LangAdder.Add(Button2, L1.COMPLETEDTASKS);
        DirectRefsGridView.EmptyDataText = L1.NODATA;

        if (!AppSettings.IsDemo)
        {
            CrowdFlowerPlaceHolder.Visible = false;
            DemoCrowdFlowerPlaceHolder.Visible = true;
            CrowdFlowerLogo.ImageUrl = "Images/Offerwall/crowdFlower.png";
            DemoCrowdFlowerLabel.Text = "This is demo view of Crowdflower platform. On your original website you will see there list of tasks.";
        }
        else
        {
            //CrowdFlower
            if (string.IsNullOrEmpty(AppSettings.Offerwalls.CrowdFlowerName))
                cfOff.Visible = true;
            else
                cfOn.Visible = true;

            UserName.Text = Member.CurrentName;
        }
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        if (viewIndex == 0)
            Response.Redirect("crowdflower.aspx");

        MenuMultiView.ActiveViewIndex = viewIndex;

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
        
    }
}
