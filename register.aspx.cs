using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using Prem.PTC.Security;
using Prem.PTC;
using System.Security.Cryptography;
using Resources;
using Prem.PTC.Utils.NVP;

public partial class About : TitanPage
{

    public string Description { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        //Page settings
        if (!string.IsNullOrEmpty(AppSettings.Site.Name))
        {
            Title = AppSettings.Site.Name;
        }
        if (!string.IsNullOrEmpty(AppSettings.Site.Description))
        {
            Description = AppSettings.Site.Description;
        }

        if (TitanFeatures.IsAdzbuzz)
            Response.Redirect("~/login.aspx");

        if (TitanFeatures.IsAhmed)
            SpamInfoPlaceHolder.Visible = true;

        //Redirect for one cutomer
        if (TitanFeatures.IsJ5WalterOffersFromHome && CountryManager.LookupCountryCode(IP.Current) != "US")
            Response.Redirect("https://freebiesfromhome.com/user/earn/cpaoffers.aspx");

    }
}
