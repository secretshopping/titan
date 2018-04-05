using System;
using System.Collections.Generic;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using Resources;
using System.Net;

public partial class About : System.Web.UI.Page
{
    public List<TrafficExchangeAdvertPack> availableOptions;
    TrafficExchangeAdvert Ad;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled
            && Member.CurrentInCache.IsAdvertiser);
        
        if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
        {
            int AdId = Convert.ToInt32(Request.QueryString["id"]);
            Ad = new TrafficExchangeAdvert(AdId);

            if (Ad.Advertiser.MemberUsername != Member.CurrentName)
                Response.Redirect("~/user/default.aspx");

            //Lang & Hint
            TitleLabel.Text = Ad.Title;
            URLLabel.Text = Ad.TargetUrl;

            ViewState["TrafficExchangeCampaignId"] = AdId;
        }
    }

    protected void AddNewAdWithURLCheck_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.Params.Get("__EVENTARGUMENT")))
        {
            string argument = Request.Params.Get("__EVENTARGUMENT");
            string validURL = Encryption.Decrypt(argument);
            URLLabel.Visible = false;
            URLTextBox.Text = validURL;
            URLTextBox.Enabled = false;
            CheckURLButton.Visible = false;           
        }
    }

    protected void CreateAdButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                AppSettings.DemoCheck();

                if (URLTextBox.Enabled)
                    throw new MsgException(U4200.CHECKURL);

                int AdId = Convert.ToInt32((int)ViewState["TrafficExchangeCampaignId"]);
                Ad = new TrafficExchangeAdvert(AdId);

                if (Ad.Advertiser.MemberUsername != Member.CurrentName)
                    throw new MsgException("This is not your campaign.");

                TrafficExchangeSubpage newsp = new TrafficExchangeSubpage();
                newsp.AdId = AdId;
                newsp.SubPage = URLTextBox.Text;
                newsp.Save();

                Response.Redirect("trafficexchange.aspx");

            }
            catch (MsgException ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }
    }


}

class MyClient : WebClient
{
    public bool HeadOnly { get; set; }
    protected override WebRequest GetWebRequest(Uri address)
    {
        WebRequest req = base.GetWebRequest(address);
        if (HeadOnly && req.Method == "GET")
        {
            req.Method = "HEAD";
        }
        return req;
    }
}