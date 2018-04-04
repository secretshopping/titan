using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;
using Prem.PTC.Memberships;

public partial class Controls_EarningsCalculator : System.Web.UI.UserControl
{
    /// <summary>
    /// Number of Points per Facebook like
    /// </summary>
    public int PointsPerFacebookLike { get; set; }

    private int[] numbers = new int[13] { 0, 1, 3, 5, 10, 25, 50, 75, 100, 150, 200, 250, 300 };

    protected void Page_Load(object sender, EventArgs e)
    {
        if (AppSettings.Site.PureGPTMode ||
            AppSettings.Site.TrafficExchangeMod)
        {
            MainPanel.Visible = false;
        }
        else
        {
            //Show it
            GenerateConstants();
            
            //Default DDL values
            DropDownList1.SelectedIndex = 3;
            DropDownList1.DataBind();

            DropDownList2.SelectedIndex = 6;
            DropDownList2.DataBind();

            DropDownList3.SelectedIndex = 1;
            DropDownList3.DataBind();
        }
    }

    protected void GenerateConstants()
    {
        var style = "style=\"display:none;\"";

        var Memberships = TableHelper.SelectRows<Membership>(TableHelper.MakeDictionary("Status", (int)MembershipStatus.Active));
        var ItemsList = new List<ListItem>();

        foreach (var elem in Memberships)
        {
            ConstantsLiteral.Text += "<span " + style + " id=\"perclick_" + elem.Id + "\">" + elem.AdvertClickEarnings.ToClearString() + "</span>";
            ConstantsLiteral.Text += "<span " + style + " id=\"perrefclick_" + elem.Id + "\">" + elem.DirectReferralAdvertClickEarnings.ToClearString() + "</span>";

            ItemsList.Add(new ListItem(elem.Name, elem.Id.ToString()));         
        }

        ConstantsLiteral.Text += "<span " + style + " id=\"pointsperlike\">" + PointsPerFacebookLike + "</span>";

        MembershipsDropDownList.Items.AddRange(ItemsList.ToArray());
        MembershipsDropDownList.DataBind();
    }

    protected void DropDownList_Init(object sender, EventArgs e)
    {
        var TargetList = (DropDownList)sender;
        var ItemsList = new List<ListItem>();

        foreach (var value in numbers)
            ItemsList.Add(new ListItem(value.ToString(), value.ToString()));

        TargetList.Items.AddRange(ItemsList.ToArray());
        TargetList.DataBind();
    }
}
