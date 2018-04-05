using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC;

public partial class sites_help : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TestimonialsPlaceHolder.Visible = Member.IsLogged;

        if (!AppSettings.Site.ForumEnabled)
        {
            forumDiv.Visible = false;
            faqDiv.Attributes["class"] = contactDiv.Attributes["class"] = "col-md-6 col-sm-6";            
        }
    }
}