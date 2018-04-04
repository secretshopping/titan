using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Texts;
using Microsoft.AspNet.FriendlyUrls;
using Titan.News;
using Prem.PTC.Members;
using Prem.PTC;
using System.Globalization;
using Resources;

public partial class sites_article : TitanPage
{
    #region Properties

    public string SelectedCountry { get; set; }

    public Article Article
    {
        get
        {
            return (Article)ViewState["ArticleViewstate"];
        }
        set
        {
            ViewState["ArticleViewstate"] = value;
        }
    }
    public Member Author
    {
        get
        {
            return (Member)ViewState["AuthorViewstate"];
        }
        set
        {
            ViewState["AuthorViewstate"] = value;
        }
    }

    #endregion

    public int CreditReadAfterSeconds = 20;

    protected void Page_Load(object sender, EventArgs e)
    {
        SelectedCountry = (new RegionInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name)).TwoLetterISORegionName;

        if (!Page.IsPostBack)
        {
            try
            {
                int articleId = Convert.ToInt32(Page.RouteData.Values["id"]);

                Article = new Article(articleId);
                Author = Article.GetAuthor();
                TextLiteral.Text = Server.HtmlDecode(Server.HtmlDecode(Article.Text));

                InitalizeTreningArticles();
                InitalizeSuggestedArticles();
            }
            catch (Exception ex)
            {
                Response.Redirect("~/sites/defaultnews.aspx");
            }
        }
    }

    protected void InitalizeTreningArticles()
    {
        TreningArticlesPlaceHolder.Controls.Clear();

        ArticleFilter articleFilter = new ArticleFilter(SelectedCountry);
        articleFilter.FirstResults = 5;
        List<Article> articles = ArticlesManager.Get(articleFilter);

        AddArticlesToPlaceholder(articles, TreningArticlesPlaceHolder);
    }

    protected void InitalizeSuggestedArticles()
    {
        SuggestedArticlesPlaceHolder.Controls.Clear();

        ArticleFilter articleFilter = new ArticleFilter(SelectedCountry);
        articleFilter.FirstResults = 5;
        articleFilter.CategoryId = Article.CategoryId;
        List<Article> articles = ArticlesManager.Get(articleFilter);

        AddArticlesToPlaceholder(articles, SuggestedArticlesPlaceHolder);
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
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/News/ArticleLink.ascx");
        var parsedControl = objControl as IArticleObjectControl;
        parsedControl.Object = article;
        parsedControl.DataBind();

        return objControl;
    }

    protected Literal GetNoArticlesLiteral()
    {
        Literal literal = new Literal();
        literal.Text = String.Format("<div class='no-articles'></div>", U6012.NOARTICLES);
        return literal;
    }

    protected void CreditPostback_Click(object sender, EventArgs e)
    {
        int InfluencerId = -1;

        if (Request.QueryString["ref"] != null)
            InfluencerId = InfluencerCodeHelper.ToUserId(Request.QueryString["ref"]);

        ArticleView.TryAdd(Article.Id, IP.Current, InfluencerId);
    }
}