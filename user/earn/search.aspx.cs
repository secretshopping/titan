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
using System.Text;
using Titan;

public partial class EarnSearch : System.Web.UI.Page
{
    //Viewstate
    protected static string PageViewState = "Viewstate_SearchCurrentPage";
    protected static string QueryViewState = "Viewstate_Query2";

    protected void Page_Load(object sender, EventArgs e)
    {
        AppSettings.SearchAndVideo.Reload();

        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnersRoleEnabled && 
            AppSettings.TitanFeatures.EarnSearchEnabled 
            && (Member.IsLogged || Member.CurrentInCache.IsEarner));

        if (Request.QueryString["q"] != null)
        {
            Query = HttpUtility.UrlDecode(Request.QueryString["q"]);
            SearchBox.Text = Query;
        }
        else
            Query = String.Empty;

        if (!Page.IsPostBack)
        {
            try
            {             
                    DisplayResultPage();
                    CreditMember();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                QueryResultsVideosLiteral.Text = "<i>" + U4000.ERRORAPI + "Yahoo Seach API</i>";
            }
            ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "EndRequestHandlerForSearch", "EndRequestHandlerForSearch();", true);
        }


    }

    public Int32 CurrentPage
    {
        get
        {
            if (ViewState[PageViewState] == null)
                CurrentPage = 1;

            return (int)ViewState[PageViewState];
        }
        set
        {
            ViewState[PageViewState] = value;
        }
    }

    public String Query
    {
        get
        {
            if (ViewState[QueryViewState] == null)
                Query = "";
            return (string)ViewState[QueryViewState];
        }
        set
        {
            ViewState[QueryViewState] = value;
        }
    }

    protected void DisplayResultPage()
    {
        PageNumberLiteral.Text = CurrentPage.ToString();

        //Nav buttons
        if (CurrentPage == 1)
            PreviousPageButton.Visible = false;
        else
            PreviousPageButton.Visible = true;

        //if (page.HasMorePages)
        //    NextPageButton.Visible = true;
        //else
        //    NextPageButton.Visible = false;
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        string text = SearchBox.Text.Trim();
        if (!string.IsNullOrWhiteSpace(text))
        {
            text = HttpUtility.UrlEncode(text);
            Response.Redirect("~/user/earn/search.aspx?q=" + text);
        }
    }

    protected void PreviousPageButton_Click(object sender, EventArgs e)
    {
        CurrentPage = CurrentPage - 1;
        DisplayResultPage();
    }

    protected void NextPageButton_Click(object sender, EventArgs e)
    {
        CurrentPage = CurrentPage + 1;
        DisplayResultPage();
    }

    protected void CreditMember()
    {
        //We should credit the member and refresh balance

        try
        {
            if (!Member.IsLogged)
                throw new MsgException(U4000.YOUMUSTBELOGGED.Replace("%p%", AppSettings.PointsName));

            Member User = Member.CurrentInCache;
            SearchCrediter Crediter = new SearchCrediter(User);
            bool HasCredited = Crediter.CreditSearch();

            //Refresh Points panel
            UpdatedPointsTextBox.Text = User.PointsBalance.ToString();
            UpdatePanel1.Update();

            if (HasCredited)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "SearchCredit", "animatePrice();", true);
            }
        }
        catch (MsgException ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }

    }
}
