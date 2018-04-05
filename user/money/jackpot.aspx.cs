using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using MarchewkaOne.Titan.Balances;
using Titan;
using System.Reflection;

public partial class About : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.MoneyJackpotEnabled);

        var activeJackpotList = JackpotManager.GetJackpots(active: true);
        foreach (var jackpot in activeJackpotList)
        {
            JackpotPlaceHolder.Controls.Add(GetJackpotCode(jackpot, isHistory: false));
        }
        if (activeJackpotList.Count <= 0)
        {
            NoDataPanel1.Visible = true;
            NoDataLiteral1.Text = L1.NODATA;
        }

        var finishedJackpotList = JackpotManager.GetJackpots(active: false).Take(5).ToList();
        foreach (var jackpot in finishedJackpotList)
        {
            JackpotHistoryPlaceHolder.Controls.Add(GetJackpotCode(jackpot, isHistory: true));
        }
        if (finishedJackpotList.Count <= 0)
        {
            NoDataPanel2.Visible = true;
            NoDataLiteral2.Text = L1.NODATA;
        }

        Button1.Text = L1.ACTIVE;
        Button2.Text = L1.HISTORY;

    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
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
    private UserControl GetJackpotCode(Jackpot jackpot, bool isHistory)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Misc/Jackpot.ascx");
        var parsedControl = objControl as ICustomObjectControl;
        parsedControl.ObjectID = jackpot.Id;

        PropertyInfo myProp = parsedControl.GetType().GetProperty("IsHistory");
        myProp.SetValue(parsedControl, isHistory, null);

        parsedControl.DataBind();

        return objControl;
    }

}
