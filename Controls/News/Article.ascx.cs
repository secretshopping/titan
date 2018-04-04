using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using Titan.InvestmentPlatform;
using Titan.News;

public partial class Controls_News_Article : System.Web.UI.UserControl, IStyledArticleObjectControl
{
    public Article Object { get; set; }
    public ArticleCategory Category { get; set; }
    public Member Author { get; set; }
    public ArticleType Type { get; set; }

    public override void DataBind()
    {
        base.DataBind();

        Author = Object.GetAuthor();
        Category = Object.GetCategory();

        HeadlinerPlaceHolder.Visible = Type == ArticleType.Headliner;
        MainPlaceHolder.Visible = Type == ArticleType.Main;
        CategoryPlaceHolder.Visible = Type == ArticleType.Category;
        WorldNewsPlaceHolder.Visible = Type == ArticleType.WorldNews;
    }
}