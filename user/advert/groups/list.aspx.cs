using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using System.Data;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Titan.Shares;
using Prem.PTC.Advertising;

public partial class About : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertAdPacksEnabled &&
              (AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.CustomGroups || AppSettings.RevShare.AdPack.GroupPolicy == GroupPolicy.AutomaticAndCustomGroups));

        if (!IsPostBack)
        {
            DataBind();
        }       
    }

    public override void DataBind()
    {
        base.DataBind();
        CreateGroupButton.Text = U4200.OPENGROUP;
        Button1.Text = U5003.INPROGRESS;
        Button2.Text = U5003.ACTIVE;
        Button3.Text = U5003.EXPIRED;
        CreateGroupButton.Visible = Member.IsLogged 
            && Member.CurrentInCache.IsEarner
            && Convert.ToInt32(TableHelper.SelectScalar("SELECT COUNT(Number) FROM CustomGroups")) > 0;
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

    protected void CreateGroupButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/user/advert/groups/createcustomgroup.aspx");
    }

    #region OPEN GROUPS GRIDVIEW
    protected void OpenGroupsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int userId = Convert.ToInt32(e.Row.Cells[0].Text);
            int userCustomGroupId = Convert.ToInt32(e.Row.Cells[7].Text);

            //userId
            e.Row.Cells[0].Text = new Member(userId).Name;

            //Name
            e.Row.Cells[1].BackColor = System.Drawing.Color.FromName(e.Row.Cells[2].Text);

            //insert link
            e.Row.Cells[1].Text = string.Format("<span style='font-weight:bold'><a style='color:white;' href='{0}user/advert/groups/customgroup.aspx?g={1}'>{2}<a/><span/>", AppSettings.Site.Url, userCustomGroupId, e.Row.Cells[1].Text);

            //Color
            e.Row.Cells[2].Visible = false;

            //UserGroupId - > number of AdPacks to complete
            e.Row.Cells[8].Text = (Convert.ToInt32(e.Row.Cells[4].Text) - Convert.ToInt32(e.Row.Cells[3].Text)).ToString();

            //AdPacksAdded
            e.Row.Cells[3].Text = HtmlCreator.GenerateCPAAdProgressHTML(Convert.ToInt32(e.Row.Cells[3].Text), Convert.ToInt32(e.Row.Cells[4].Text),
                AppSettings.RevShare.AdPack.AdPackNamePlural);

            //AdPacksLimit
            e.Row.Cells[4].Visible = false;

            //Percentage
            e.Row.Cells[5].Visible = false;

            //Daily profit
            if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase)
                e.Row.Cells[6].Text = "+" + Convert.ToInt32(e.Row.Cells[6].Text).ToString();

            e.Row.Cells[6].Text += "%";


            //UserGroupId - > Number of participants
            e.Row.Cells[7].Text = AdPackManager.GetNumberOfParticipantsInGroup(userCustomGroupId).ToString();
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[2].Visible = false;

            e.Row.Cells[4].Visible = false;
            e.Row.Cells[5].Visible = false;

            if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Accelerate)
                OpenGroupsGridView.Columns[6].HeaderText = U4200.DAILYPROFITRAISEDBY;
            else if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase)
                OpenGroupsGridView.Columns[6].HeaderText = U5001.PROFITRAISEDBY;
        }
    }

    protected void OpenGroupsGridView_PreRender(object sender, EventArgs e)
    {
        for (int i = 0; i < OpenGroupsGridView.Rows.Count; i++)
        {
            OpenGroupsGridView.Rows[i].Cells[1].BackColor = System.Drawing.Color.FromName(OpenGroupsGridView.Rows[i].Cells[2].Text);
        }
    }

    protected void View1_Activate(object sender, EventArgs e)
    {
        OpenGroupsGridView.DataBind();
    }

    protected void OpenGroupsGridView_Init(object sender, EventArgs e)
    {
        OpenGridViewDataSource.SelectCommand = CustomGroupManager.GroupSqlCommand(CustomGroupStatus.InProgress);
    }

    #endregion

    #region ClOSEDGROUPSGRIDVIEW
    protected void ClosedGroupsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int userId = Convert.ToInt32(e.Row.Cells[0].Text);
            int userCustomGroupId = Convert.ToInt32(e.Row.Cells[7].Text);

            //userId
            e.Row.Cells[0].Text = new Member(userId).Name;

            //Name
            e.Row.Cells[1].BackColor = System.Drawing.Color.FromName(e.Row.Cells[2].Text);

            //insert link
            e.Row.Cells[1].Text = string.Format("<span style='font-weight:bold'><a style='color:white;' href='{0}user/advert/groups/customgroup.aspx?g={1}'>{2}<a/><span/>", AppSettings.Site.Url, userCustomGroupId, e.Row.Cells[1].Text);

            //Color
            e.Row.Cells[2].Visible = false;

            //AdPacksAdded
            e.Row.Cells[3].Text = HtmlCreator.GenerateCPAAdProgressHTML(Convert.ToInt32(e.Row.Cells[3].Text), Convert.ToInt32(e.Row.Cells[4].Text),
                AppSettings.RevShare.AdPack.AdPackNamePlural);

            //AdPacksLimit
            e.Row.Cells[4].Visible = false;

            //Percentage
            e.Row.Cells[5].Visible = false;

            //Daily profit
            if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase)
                e.Row.Cells[6].Text = "+" + Convert.ToInt32(e.Row.Cells[6].Text).ToString();

            e.Row.Cells[6].Text += "%";

            //UserGroupId - > Number of participants
            e.Row.Cells[7].Text = AdPackManager.GetNumberOfParticipantsInGroup(userCustomGroupId).ToString();
            // e.Row.Cells[8].Visible = false;
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[2].Visible = false;

            e.Row.Cells[4].Visible = false;
            e.Row.Cells[5].Visible = false;

            if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Accelerate)
                ClosedGroupsGridView.Columns[6].HeaderText = U4200.DAILYPROFITRAISEDBY;
            else if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase)
                ClosedGroupsGridView.Columns[6].HeaderText = U5001.PROFITRAISEDBY;
        }
    }

    protected void ClosedGroupsGridView_PreRender(object sender, EventArgs e)
    {
        for (int i = 0; i < ClosedGroupsGridView.Rows.Count; i++)
        {
            ClosedGroupsGridView.Rows[i].Cells[1].BackColor = System.Drawing.Color.FromName(ClosedGroupsGridView.Rows[i].Cells[2].Text);
        }
    }

    protected void View2_Activate(object sender, EventArgs e)
    {
        ClosedGroupsGridView.DataBind();
    }

    protected void ClosedGroupsGridViewDataSource_Init(object sender, EventArgs e)
    {
        ClosedGroupsGridViewDataSource.SelectCommand = CustomGroupManager.GroupSqlCommand(CustomGroupStatus.Active);
    }

    #endregion

    #region CLOSEDGROUPSGRIDVIEW2

    protected void ClosedGroupsGridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int userId = Convert.ToInt32(e.Row.Cells[0].Text);
            int userCustomGroupId = Convert.ToInt32(e.Row.Cells[7].Text);

            //userId
            e.Row.Cells[0].Text = new Member(userId).Name;

            //Name
            e.Row.Cells[1].BackColor = System.Drawing.Color.FromName(e.Row.Cells[2].Text);

            //insert link
            e.Row.Cells[1].Text = string.Format("<span style='font-weight:bold'><a style='color:white;' href='{0}user/advert/groups/customgroup.aspx?g={1}'>{2}<a/><span/>", AppSettings.Site.Url, userCustomGroupId, e.Row.Cells[1].Text);

            //Color
            e.Row.Cells[2].Visible = false;

            //AdPacksAdded
            e.Row.Cells[3].Text = HtmlCreator.GenerateCPAAdProgressHTML(Convert.ToInt32(e.Row.Cells[3].Text), Convert.ToInt32(e.Row.Cells[4].Text),
                AppSettings.RevShare.AdPack.AdPackNamePlural);

            //AdPacksLimit
            e.Row.Cells[4].Visible = false;

            //Percentage
            e.Row.Cells[5].Visible = false;

            //Daily profit
            if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase)
                e.Row.Cells[6].Text = "+" + Convert.ToInt32(e.Row.Cells[6].Text).ToString();

            e.Row.Cells[6].Text += "%";

            //UserGroupId - > Number of participants
            e.Row.Cells[7].Text = AdPackManager.GetNumberOfParticipantsInGroup(userCustomGroupId).ToString();
            // e.Row.Cells[8].Visible = false;
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[2].Visible = false;

            e.Row.Cells[4].Visible = false;
            e.Row.Cells[5].Visible = false;

            if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Accelerate)
                ClosedGroupsGridView2.Columns[6].HeaderText = U4200.DAILYPROFITRAISEDBY;
            else if (AppSettings.RevShare.AdPack.CustomReturnOption == CustomReturnOption.Increase)
                ClosedGroupsGridView2.Columns[6].HeaderText = U5001.PROFITRAISEDBY;
        }
    }

    protected void ClosedGroupsGridView2_PreRender(object sender, EventArgs e)
    {
        for (int i = 0; i < ClosedGroupsGridView.Rows.Count; i++)
        {
            ClosedGroupsGridView2.Rows[i].Cells[1].BackColor = System.Drawing.Color.FromName(ClosedGroupsGridView2.Rows[i].Cells[2].Text);
        }
    }

    protected void View3_Activate(object sender, EventArgs e)
    {
        ClosedGroupsGridView2.DataBind();
    }

    protected void ClosedGroupsGridView2DataSource_Init(object sender, EventArgs e)
    {
        ClosedGroupsGridView2DataSource.SelectCommand = CustomGroupManager.GroupSqlCommand(CustomGroupStatus.Expired);
    }
    #endregion

}
