using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Texts;
using Prem.PTC;
using System.Globalization;
using Titan.News;
using Resources;

public partial class sites_searchnews : TitanPage
{
    public string SelectedCountry { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    { 
        SelectedCountry = (new RegionInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name)).TwoLetterISORegionName;

        if (!Page.IsPostBack)
        {
            ArticlesPlaceHolder.Controls.Clear();

            if (Request.QueryString["q"] != null)
            {
                string Query = HttpUtility.UrlDecode(Request.QueryString["q"]);

                ArticleFilter articleFilter = new ArticleFilter(SelectedCountry);
                articleFilter.SearchText = Query;
                List<Article> articles = ArticlesManager.Get(articleFilter);

                AddArticlesToPlaceholder(articles, ArticlesPlaceHolder);

                ScriptManager.RegisterStartupScript(Page, GetType(), "setSearchTextBox", "setSearchTextBox('" + Query + "');", true);
            }
        }
    }

    protected void AddArticlesToPlaceholder(List<Article> articles, PlaceHolder placeHolder)
    {
        if (articles.Count == 0)
            placeHolder.Controls.Add(GetNoArticlesLiteral());

        for (int i = 0; i < articles.Count; i++)
            placeHolder.Controls.Add(GetArticleHTML(articles[i]));
    }

    protected UserControl GetArticleHTML(Article article)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/News/SearchResultArticle.ascx");
        var parsedControl = objControl as IArticleObjectControl;
        parsedControl.Object = article;
        parsedControl.DataBind();

        return objControl;
    }

    protected Literal GetNoArticlesLiteral()
    {
        Literal literal = new Literal();
        literal.Text = String.Format("<div>{0}</div>", U6003.NORESULTS);
        return literal;
    }
}