using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using Titan.InvestmentPlatform;
using Titan.News;

public partial class Controls_SearchResultNews_Article : System.Web.UI.UserControl, IArticleObjectControl
{
    public Article Object { get; set; }
    public ArticleCategory Category { get; set; }
    public Member Author { get; set; }

    public override void DataBind()
    {
        base.DataBind();

        Author = Object.GetAuthor();
        Category = Object.GetCategory();
    }
}