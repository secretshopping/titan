using System;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Resources;
using System.IO;
using Titan.MiniVideos;
using Titan.News;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI;
using ExtensionMethods;
using System.Data;

public partial class user_news_sharing : System.Web.UI.Page
{
    public Money EarningsPer1000Shares { get; set;}

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.NewsSharingArticlesEnabled && (!Member.IsLogged || Member.CurrentInCache.IsEarner));

        EarningsPer1000Shares = Member.CurrentInCache.Membership.ArticleInfluencerCPM;
        SearchTextBox.Attributes.Add("placeholder", L1.SEARCH);
        if (!IsPostBack)
        {
            TitleLiteral.Text = U6012.SHARINGARTICLES;
            SubLiteral.Text = U6012.SHARINGARTICLESINFO;
            ArticlesButton.Text = U6012.ARTICLES;
            StatisticsButton.Text = L1.STATISTICS;
            ArticlesGridView.EmptyDataText = L1.NODATA;
        }
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


    #region Articles

    protected void ArticlesView_Activate(object sender, EventArgs e)
    {
        CountriesDropDownList.Items.Clear();
        CountriesDropDownList.Items.AddRange(NewsCountriesHelper.ListItems);
        BindDataToCategoriesDDL();
        LoadArticles();
    }

    protected void CategoriesDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadArticles();
    }

    protected void CountriesDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindDataToCategoriesDDL();
        LoadArticles();
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        LoadArticles();
    }

    private void BindDataToCategoriesDDL()
    {
        CategoriesDropDownList.Items.Clear();
        CategoriesDropDownList.Items.AddRange(ArticleCategory.GetListItems(CountriesDropDownList.SelectedValue));
    }

    protected void LoadArticles()
    {
        string country = CountriesDropDownList.SelectedValue.Replace("'", "''");
        string searchText = SearchTextBox.Text.Replace("'", "''");

        ArticleFilter articleFilter = new ArticleFilter(CountriesDropDownList.SelectedValue, Convert.ToInt32(CategoriesDropDownList.SelectedValue));

        if (!String.IsNullOrWhiteSpace(searchText))
            articleFilter.SearchText = searchText;

        List<Article> articles = ArticlesManager.Get(articleFilter);

        ArticlesPlaceHolder.Controls.Clear();
        for (int i = 0; i < articles.Count; i++)
            ArticlesPlaceHolder.Controls.Add(GetArticleHTML(articles[i]));
    }

    protected UserControl GetArticleHTML(Article article)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/News/ShareableArticle.ascx");
        var parsedControl = objControl as IArticleObjectControl;
        parsedControl.Object = article;
        parsedControl.DataBind();

        return objControl;
    }

    #endregion

    #region Statistics

    protected void ArticlesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[2].Text = String.Format("<img src='Images/Flags/{0}.png'/> {1}", e.Row.Cells[2].Text.ToLower(),
                CountryManager.GetCountryName(e.Row.Cells[2].Text));

            int mySharedReads = Convert.ToInt32(e.Row.Cells[3].Text) / 1000;

            e.Row.Cells[4].Text = new Money(mySharedReads * EarningsPer1000Shares.ToDecimal()).ToString();
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[3].Text = U6012.READS;
            e.Row.Cells[4].Text = U6012.ESTIMATEDMONEY;
        }
    }

    protected void ArticlesSqlDataSource_Init(object sender, EventArgs e)
    {
        ArticlesSqlDataSource.SelectCommand =
            String.Format(@"SELECT TOP 15 a.Title, ac.Text, a.Geolocation, ac.Text AS Category, COUNT(av.Id) AS Reads FROM ArticleViews  av
                            JOIN Articles a ON a.Id = av.ArticleId
                            JOIN ArticleCategories ac ON ac.Id = a.CategoryId
                            WHERE av.InfluencerUserId = {0}
                            GROUP BY av.ArticleId, a.Title, ac.Text, a.Geolocation
                            ORDER BY Reads DESC", Member.CurrentId);
    }

    #endregion
}
