using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Advertising;
using Titan.News;

public interface IArticleObjectControl
{
    Article Object { get; set; }
    void DataBind();
}

public interface IStyledArticleObjectControl
{
    Article Object { get; set; }
    ArticleType Type { get; set; }
    void DataBind();
}