using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Resources;
using Prem.PTC.Referrals;
using Prem.PTC.Statistics;

public partial class BannersAndTools : System.Web.UI.Page
{
    public string jsSelectAllCode;
    public string GeneralTitle = AppSettings.Site.Name + " | " + AppSettings.Site.Slogan;

    public string RefCodeStr;
    public string RefCodeStr2;
    protected string RefCodeStrNoAnchor;
    public string SplashCodeStr;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.ReferralsBannersEnabled);

        if (!Page.IsPostBack)
        {
            LangAdder.Add(Button1, U5007.LINKS);
            LangAdder.Add(Button2, L1.BANNERS);
            LangAdder.Add(Button3, U5005.SPLASHPAGE);
            LangAdder.Add(Button4, U5007.CUSTOMSPLASH);
            LangAdder.Add(SaveButton, L1.SAVE);

            if (!AppSettings.SplashPage.SplashPageEnabled)
            {
                Button3.Visible = false;
                Button4.Visible = false;
            }
          
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


    protected void links_Activate(object sender, EventArgs e)
    {
        if (Member.CurrentInCache.Membership.CanUseRefTools)
        {
            string RefLinkText = AppSettings.Site.Url + "register.aspx?u=" + Member.CurrentId;

            string RefLink1Text = AppSettings.Site.Url + "default.aspx?u=" + Member.CurrentId;

            RefLink.Text = String.Format("<a href='{0}'>{0}</a>", RefLinkText);
            RefLink2.Text = String.Format("<a href='{0}'>{0}</a>", RefLink1Text);
            RefCodeStrNoAnchor = RefLinkText;
            RefCodeStr = String.Format("<a href='{0}'>{1}</a>", RefLinkText, GeneralTitle);
            RefCodeStr2 = String.Format("<a href='{0}'>{1}</a>", RefLink1Text, GeneralTitle);
        }
        else
        {
            RefLink.Text = RefLink2.Text = RefCodeStrNoAnchor = RefCodeStr = RefCodeStr2 = U6005.UPGRADETOUSEREFTOOLS;
        }
    }

    protected void banners_Activate(object sender, EventArgs e)
    {

        //Show banners
        List<ReferralsBanner> list = TableHelper.GetListFromRawQuery<ReferralsBanner>("SELECT * FROM ReferralsBanners");
        foreach (ReferralsBanner value in list)
        {
            Image BannerImage = new Image();
            BannerImage.ImageUrl = value.BannerUrl;

            var imgUrl = AppSettings.Site.Url + value.BannerUrl.Replace("~/", "");

            string BannerCodeStr;

            if (Member.CurrentInCache.Membership.CanUseRefTools)
            {
                BannerCodeStr = "<a href='" + AppSettings.Site.Url + "default.aspx?u=" + Member.CurrentId + "'><img src='" +
                imgUrl + "' alt='banner' /></a>";
            }
            else
            {
                BannerCodeStr = U6005.UPGRADETOUSEREFTOOLS;
            }

            ReferralsBannerList.Controls.Add(new LiteralControl("<div style='margin-bottom: 100px' class='box'>"));
            ReferralsBannerList.Controls.Add(BannerImage);
            ReferralsBannerList.Controls.Add(new LiteralControl("<br /><br />"));
            ReferralsBannerList.Controls.Add(new LiteralControl("<div class=\"clipboard-wrapper\"><pre id=\"bannerCode-" + value.Id + "\">" + System.Net.WebUtility.HtmlEncode(BannerCodeStr) + "</pre><button type=\"button\" class=\"clipboard btn btn-inverse height-full\" data-click=\"tooltip\" data-placement=\"top\" title=\"Copied!\" data-clipboard-target=\"#bannerCode-" + value.Id + "\">Copy</button></div>"));
            ReferralsBannerList.Controls.Add(new LiteralControl("</div>"));
        }
    }

    protected void splash_Activate(object sender, EventArgs e)
    {
        string SplashLinkText = AppSettings.Site.Url + "splash/welcome.aspx?u=" + Member.CurrentId;

        SplashLink.Text = String.Format("<a href='{0}'>{0}</a>", SplashLinkText);

        SplashCodeStr = String.Format("<a href='{0}'>{1}</a>", SplashLinkText, GeneralTitle);
    }

    protected void customSplash_Activate(object sender, EventArgs e)
    {
        var UserSplashPage = CustomSplashPage.Get(Member.CurrentId);
        CustomSplashPageLink.Text = String.Format("<i>{0}</i>", U5007.NEEDCREATE);

        if (UserSplashPage != null)
        {
            SplashPageCKEditor.Text = UserSplashPage.Text;
            string CustomSplashPageText = AppSettings.Site.Url + "splash/custom.aspx?id=" + UserSplashPage.Id;
            CustomSplashPageLink.Text = String.Format("<a href='{0}'>{0}</a>", CustomSplashPageText);
        }
        else
        {
            string defaultSplashPageText = @"

            <div style=""width:100%;height:100%;background-image: url(" + AppSettings.Site.Url +
            @"splash/SplashPageBackground.jpg);position: absolute;"">

            <div style=""margin:0 auto; width:500px; border:1px solid grey; padding:10px; background:white; margin-top:100px"">
            <h2>This is a sample Splash page.</h2>

            <p><span style=""font-size:10px""><em>Feel free to edit it. You can basically add anything there.</em></span></p>
            </div>
            </div>


            ";
            SplashPageCKEditor.Text = defaultSplashPageText;
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        var content = Request.Form[SplashPageCKEditor.UniqueID];

        var UserSplashPage = CustomSplashPage.Get(Member.CurrentId);

        if (UserSplashPage != null)
        {
            UserSplashPage.Text = content;
            UserSplashPage.Save();
        }
        else
        {
            CustomSplashPage page = new CustomSplashPage();
            page.UserId = Member.CurrentId;
            page.Text = content;
            page.Save();
        }


        //At the end
        customSplash_Activate(this, null);
    }
}

