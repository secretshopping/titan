using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC;
using Resources;
using Titan.MiniVideos;
using Prem.PTC.Utils;

public partial class user_entertainment_minivideo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EntertainmentMiniVideoEnabled);

        if (!IsPostBack)
        {
            BindDataToDDL();
        }

        UserName.Text = Member.CurrentName;
        TitleLiteral.Text = U6008.MINIVIDEO;
        SubLiteral.Text = U6008.MINIVIDEOBOUGHTDESCRIPTION;
        LoadMiniVideos();
        LangAdder.Add(YourVideosButton, U6008.YOURVIDEOS);
        LangAdder.Add(AvaibleVideosButton, U6008.AVAIBLEVIDEOS);
    }

    private void BindDataToDDL()
    {
        SearchCategoryDropDownList.Items.Clear();
        SearchCategoryDropDownList.Items.Add(new ListItem("All", "-1"));
        SearchCategoryDropDownList.Items.AddRange(MiniVideoCategory.ListItems);
    }

    private void LoadMiniVideos()
    {
        var categoryId = 0;
        var withCategories = Convert.ToInt32(SearchCategoryDropDownList.SelectedValue) != -1;

        if (withCategories)
            categoryId = Convert.ToInt32(SearchCategoryDropDownList.SelectedValue);

        List<MiniVideoCampaign> AvailableMiniVideosList = MiniVideoCampaign.GetAllAvaibleVideosForUser(UserName.Text);

        if (AvailableMiniVideosList.Count == 0)
        {
            NoVideosPanelWrapper.Visible = true;
            NoVideosPanel.Visible = true;
            NoVideosPanel.Text = U6008.NOVIDEOS;
            SearchPlaceHolder.Visible = false;
        }
        else
        {
            AvaibleVideosPlaceHolder.Controls.Clear();
            try
            {
                foreach (var video in AvailableMiniVideosList)
                {
                    if (withCategories)
                    {
                        if (video.VideoCategory == categoryId)
                            AvaibleVideosPlaceHolder.Controls.Add(GetAdHTML(video));
                    }
                    else
                        AvaibleVideosPlaceHolder.Controls.Add(GetAdHTML(video));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }
    }

    protected UserControl GetAdHTML(MiniVideoCampaign video)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/Advertisements/MiniVideo.ascx");
        var parsedControl = objControl as IMiniVideoObjectControl;
        parsedControl.Object = video;

        parsedControl.DataBind();

        return (UserControl)parsedControl;
    }

    protected void UsersBoughtMiniVideosGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var title = new MiniVideoCampaign(Convert.ToInt32(e.Row.Cells[1].Text)).Title;
            e.Row.Cells[1].Text = title;
        }
    }

    protected void UsersBoughtMiniVideosGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "goToVideo")
        {
            var index = e.GetSelectedRowIndex() % UsersBoughtMiniVideosGridView.PageSize;
            var row = UsersBoughtMiniVideosGridView.Rows[index];

            var videoId = Convert.ToInt32(row.Cells[0].Text);
            var video = new MiniVideoCampaign(videoId);

            Response.Redirect(video.VideoURL);
        }
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        MenuMultiView.ActiveViewIndex = viewIndex;

        if (viewIndex == 1)
            UsersBoughtMiniVideosGridView.DataBind();

        //Change button style
        foreach (Button b in MenuButtonPlaceHolder.Controls)
        {
            b.CssClass = "";
        }
        TheButton.CssClass = "ViewSelected";
    }

    protected void AvaibleVideosView_Activate(object sender, EventArgs e)
    {
        SearchCategoryDropDownList.SelectedIndex = 0;
    }
    
}