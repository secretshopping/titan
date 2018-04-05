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

public partial class sites_defaultnews : TitanPage
{
    string[] languages;
    public string SelectedCountry { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        #region Languages
        languages = AppSettings.Site.ChoosedLanguages.Split('#');

        LangPanel.Controls.Add(GetButton(System.Threading.Thread.CurrentThread.CurrentCulture.Name, true));
        SelectedCountry = (new RegionInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name)).TwoLetterISORegionName;

        for (int i = 0; i < languages.Length; i++)
            if (languages[i] != System.Threading.Thread.CurrentThread.CurrentCulture.Name)
                LangPanel.Controls.Add(GetButton(languages[i]));
        #endregion

        if (!Page.IsPostBack)
        {
            InitalizeHeadliner();
            InitalizeMainArticles();
            InitalizeCategoryArticles();
            InitalizeWorldNewsArticles();
        }
    }

    #region Languages

    protected LinkButton GetButton(string cultureCode, bool selected = false)
    {
        RegionInfo regionInfo = new RegionInfo(cultureCode);
        LinkButton button = new LinkButton
        {
            Text = String.Format("<img src='Images/Flags/{0}.png' /> {1}", cultureCode.Substring(3).ToLower(), regionInfo.NativeName),
            CommandArgument = cultureCode,
            CommandName = "changelang",
            CssClass = selected ? "btn btn-inverse language-menu-item selected" : "btn btn-inverse language-menu-item"
        };
        button.Command += changelang_Command;

        return button;
    }

    protected void changelang_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "changelang")
        {
            string selectedLanguage = e.CommandArgument.ToString();
            try
            {
                CultureInfo.CreateSpecificCulture(selectedLanguage);
                HttpCookie cookie = new HttpCookie("CultureInfo")
                {
                    Value = selectedLanguage
                };
                Response.Cookies.Add(cookie);
                Response.Redirect(Request.Url.ToString());
            }
            catch (CultureNotFoundException)
            {
                throw new Exception("ERROR: Invalid culture string");
            }
        }
    }

    #endregion

    #region Articles initalization

    protected void InitalizeHeadliner()
    {
        ArticleHeadlinerPlaceHolder.Controls.Clear();

        ArticleFilter articleFilter = new ArticleFilter(SelectedCountry);
        articleFilter.FirstResults = 1;
        List<Article> articles = ArticlesManager.Get(articleFilter);

        AddArticlesToPlaceholder(articles, ArticleHeadlinerPlaceHolder, ArticleType.Headliner);
    }

    protected void InitalizeMainArticles()
    {
        ArticlesPlaceHolder.Controls.Clear();

        ArticleFilter articleFilter = new ArticleFilter(SelectedCountry);
        articleFilter.SkipFirst = 1; //We want to skip headliner
        articleFilter.FirstResults = 30;
        List<Article> articles = ArticlesManager.Get(articleFilter);

        AddArticlesToPlaceholder(articles, ArticlesPlaceHolder, ArticleType.Main);
    }

    protected void InitalizeCategoryArticles()
    {
        ArticlesByCategoryPlaceHolder.Controls.Clear();

        var categories = ArticleCategory.GetAllActiveCategories(SelectedCountry);

        ArticleFilter articleFilter = new ArticleFilter(SelectedCountry);
        articleFilter.FirstResults = 3;

        if (categories.Count == 0)
        {
            ArticlesByCategoryPlaceHolder.Controls.Add(GetNoArticlesLiteral());
            return;
        }

        foreach (var category in categories)
        {
            articleFilter.CategoryId = category.Id;
            List<Article> articles = ArticlesManager.Get(articleFilter);

            Literal literal = new Literal();
            literal.Text = String.Format("<h5>{0}</h5>", category.Text);
            ArticlesByCategoryPlaceHolder.Controls.Add(literal);

            AddArticlesToPlaceholder(articles, ArticlesByCategoryPlaceHolder, ArticleType.Category);
        }
    }

    protected void InitalizeWorldNewsArticles()
    {
        ArticlesWorldNewsPlaceHolder.Controls.Clear();

        var countries = NewsCountriesHelper.GetAvailableCountryCodes().Where(elem => elem != SelectedCountry);

        foreach (var country in countries)
        {
            ArticleFilter articleFilter = new ArticleFilter(country);
            articleFilter.FirstResults = 1;
            List<Article> articles = ArticlesManager.Get(articleFilter);

            RegionInfo regionInfo = new RegionInfo(country);
            Literal literal = new Literal();
            literal.Text = String.Format("<div>{0}</div>", regionInfo.NativeName);
            ArticlesWorldNewsPlaceHolder.Controls.Add(literal);

            AddArticlesToPlaceholder(articles, ArticlesWorldNewsPlaceHolder, ArticleType.WorldNews);
        }
    }

    protected void AddArticlesToPlaceholder(List<Article> articles, PlaceHolder placeHolder, ArticleType articleType)
    {
        if (articles.Count == 0)
            placeHolder.Controls.Add(GetNoArticlesLiteral());

        for (int i = 0; i < articles.Count; i++)
            placeHolder.Controls.Add(GetArticleHTML(articles[i], articleType));
    }

    protected UserControl GetArticleHTML(Article article, ArticleType articleType)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/News/Article.ascx");
        var parsedControl = objControl as IStyledArticleObjectControl;
        parsedControl.Object = article;
        parsedControl.Type = articleType;
        parsedControl.DataBind();

        return objControl;
    }

    protected Literal GetNoArticlesLiteral()
    {
        Literal literal = new Literal();
        literal.Text = String.Format("<div class='no-articles'></div>", U6012.NOARTICLES);
        return literal;
    }

    #endregion
}