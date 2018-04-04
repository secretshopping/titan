using System;
using System.Collections.Generic;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using Resources;
using Titan;

public partial class About : System.Web.UI.Page
{

    public List<PtcAdvertPack> availableOptions;
    PtcAdvert Ad;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled &&
            (AppSettings.TitanFeatures.AdvertAdsEnabled)
            && Member.CurrentInCache.IsAdvertiser);

        int AdId = Convert.ToInt32(Request.QueryString["id"]);
        Ad = new PtcAdvert(AdId);

        //Lang & Hint
        LangAdder.Add(CreateAdButton, L1.BUY);
        TitleLabel.Text = Ad.Title;
        URLLabel.Text = Ad.TargetUrl;

        //Get Packs
        availableOptions = TableHelper.SelectRows<PtcAdvertPack>(TableHelper.MakeDictionary("IsVisibleByMembers", true));

        if (!IsPostBack)
            BindDataToDDL();
    }

    private void BindDataToDDL()
    {
        var listPacks = availableOptions;

        var list = new Dictionary<string, string>();
        foreach (PtcAdvertPack pack in listPacks)
        {
            if (pack.Ends.EndMode == Ad.Pack.Ends.EndMode)
            {
                string ends = L1.NEVER;
                if (pack.Ends.EndMode == End.Mode.Days)
                    ends = pack.Ends.Value.ToString() + " " + L1.DAYS;
                else if (pack.Ends.EndMode == End.Mode.Clicks)
                    ends = pack.Ends.Value.ToString() + " " + L1.VIEWS;

                if (pack.Ends.EndMode == Ad.Ends.EndMode)
                    list.Add(pack.Id.ToString(), ends + " (" + pack.DisplayTime.Seconds.ToString() + "s) - " + pack.Price.ToString());
            }
        }
        ddlOptions.DataSource = list;
        ddlOptions.DataTextField = "Value";
        ddlOptions.DataValueField = "Key";
        ddlOptions.DataBind();
    }


    protected void CreateAdButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                AppSettings.DemoCheck();

                if (Ad.Status == AdvertStatus.Finished)
                {
                    Member User = Member.Logged(Context);
                    PtcAdvertPack Pack = new PtcAdvertPack(Int32.Parse(ddlOptions.SelectedValue));

                    if (Pack.Price > User.PurchaseBalance)
                        throw new MsgException(L1.NOTENOUGHFUNDS);

                    //Take money and save the user
                    string note = "PTC views";

                    User.SubtractFromPurchaseBalance(Pack.Price, note);
                    User.SaveBalances();

                    PtcCrediter.TryToCreditReferrerAfterPurchase(User, Pack.Price, note);

                    //Modify
                    Ad.Prolong(Pack);
                    Ad.Status = AdvertStatus.Paused;
                    Ad.SaveStatus();
                    Ad.Save();

                    //Add history entry
                    string entryText = "";
                    if (Pack.Ends.EndMode == End.Mode.Clicks)
                        entryText = (Convert.ToInt32(Pack.Ends.Value)).ToString() + " ad clicks";
                    else if (Pack.Ends.EndMode == End.Mode.Days)
                        entryText = (Convert.ToInt32(Pack.Ends.Value)).ToString() + " ad days";
                    History.AddPurchase(User.Name, Pack.Price, entryText);

                    Response.Redirect("ads.aspx");

                }
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
