using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using Titan.InvestmentPlatform;
using Titan.News;

public partial class Controls_News_ArticleLink : System.Web.UI.UserControl, IArticleObjectControl
{
    public Article Object { get; set; }

    public override void DataBind()
    {
        base.DataBind();
    }
}