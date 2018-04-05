using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Prem.PTC.Utils;

public partial class user_advert_bannerse : System.Web.UI.Page
{
    string validURL;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertisersRoleEnabled && AppSettings.TitanFeatures.AdvertBannersEnabled && Member.CurrentInCache.IsAdvertiser);

            AddLang();
            BannerUploadByUrlButton.Visible = AppSettings.Site.BannersAddByUrlEnabled;

            if (!AppSettings.TitanFeatures.PublishersRoleEnabled)
                Response.Redirect("~/user/advert/banners.aspx");            
        }
        UrlRegularExpressionValidator.ValidationExpression = RegexExpressions.AdWebsiteUrl;

        Form.Action = Request.RawUrl;
        ErrorMessagePanel.Visible = false;
    }

    private void AddLang()
    {
        MenuButtonBuyBanner.Text = L1.ADDNEW;
        MenuButtonMyBanners.Text = L1.MANAGE;
        LangAdder.Add(UrlRequiredFieldValidator, L1.REQ_URL, true);
        LangAdder.Add(UrlRegularExpressionValidator, L1.ER_BADURL, true);
        HintAdder.Add(BannerUrlTextBox, L1.H_URL);
        LangAdder.Add(ImageUploadedValidator, U6000.PLEASEUPLOADIMAGE, true);
        LangAdder.Add(ImageSubmitValidator, U6000.CHOOSEFILE, true);
        
        BuyBannerButton.Text = L1.BUY;
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccessMessagePanel.Visible = false;

        var TheButton = (Button)sender;
        int viewIndex = Int32.Parse(TheButton.CommandArgument);

        MenuMultiView.ActiveViewIndex = viewIndex;

        foreach (Button b in MenuButtonPlaceHolder.Controls)        
            b.CssClass = "";
        
        TheButton.CssClass = "ViewSelected";
    }

    #region New Banner
    Banner tempImage;

    public Banner TempImage
    {
        get
        {
            if (tempImage == null)
            {
                if (ViewState["TempImage"] == null)
                {
                    ViewState["TempImage"] = Banner.Empty;
                }

                tempImage = ViewState["TempImage"] as Banner;
            }
            return tempImage;
        }
        set
        {

            tempImage = value;
            ViewState["TempImage"] = tempImage;
        }
    }

    protected void ImageUploadButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsValid) return;

            if
            (!((Banners.TryCreateBannerFromUrl(BannerFileUrlTextBox.Text, out tempImage) ||
                Banner.TryFromStream(ImageUpload.PostedFile.InputStream, out tempImage))
               &&
               ExternalBannerAdvert.DoesImageHaveValidDimensions(tempImage,
                   new ExternalBannerAdvertPack(Convert.ToInt32(PacksDDL.SelectedValue)))))
                throw new MsgException(U6000.INVALIDBANNERIMAGEORDIMENSIONS);

            //banner 
            ErrorMessagePanel.Visible = SuccessMessagePanel.Visible = false;

            //DeleteOldImage();
            
            TempImage = TempImage.Save(AppSettings.FolderPaths.BannerAdvertImages);

            ImagePreview.ImageUrl = tempImage.Path;

            ClearUpload();
        }
        catch (Exception ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
            DeleteOldImage();
        }
    }

    private void ClearUpload()
    {
        if(ImageUpload.HasFile)
            ImageUpload.Dispose();
        if (!string.IsNullOrEmpty(BannerFileUrlTextBox.Text))
            BannerFileUrlTextBox.Text = "";
    }

    private void DeleteOldImage()
    {
        if (TempImage != null && TempImage.IsSaved)
            TempImage.Delete();
    }

    private void ClearBanner()
    {
        TempImage = null;
        ImageUpload.Visible = true;
        ImageUploadButton.Visible = true;
        ImagePreview.ImageUrl = string.Empty;
    }

    protected void UrlCheckerUpdatePanel_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.Params.Get("__EVENTARGUMENT")))
        {
            string argument = Request.Params.Get("__EVENTARGUMENT");

            validURL = Encryption.Decrypt(argument);

            if (validURL.StartsWith("http"))
            {
                BannerUrlTextBox.Text = validURL;
                BannerUrlTextBox.Enabled = false;
                CheckURLButton.Visible = false;
            }
        }
    }

    protected void BuyBannerButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                AppSettings.DemoCheck();

                ErrorMessagePanel.Visible = SuccessMessagePanel.Visible = false;
                var pack = new ExternalBannerAdvertPack(Convert.ToInt32(PacksDDL.SelectedValue));
                ExternalBannerAdvert.Buy(Member.Current, BannerUrlTextBox.Text, Convert.ToInt32(CategoriesDDL.SelectedValue), TempImage, pack, TargetBalanceRadioButtonList.TargetBalance);

                SuccessMessagePanel.Visible = true;
                SuccessMessage.Text = U6000.REQUESTSENTPLEASEWAIT;
                ClearNewBannerFields(true);
            }
        }
        catch (Exception ex)
        {
            if (ex is MsgException)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            else
                ErrorLogger.Log(ex);

            DeleteOldImage();
            ClearNewBannerFields(false);
        }
    }

    private void ClearNewBannerFields(bool successful)
    {
        if (successful)
        {
            BannerUrlTextBox.Text = string.Empty;
        }
        ClearBanner();
        BannerUrlTextBox.Enabled = true;
        CheckURLButton.Visible = true;

    }
    #endregion

    protected void BuyBannerView_Activate(object sender, EventArgs e)
    {
        BuyBannersPlaceHolder.Visible = ExternalBannerAdvert.IsBuyingAvaliable();

        if (!BuyBannersPlaceHolder.Visible)
        {
            NewBannerUnavailable.Visible = true;
            NewBannerUnavailable.HeaderText = U6000.NEWBANNERSUNAVAILABLEHEADER;
            NewBannerUnavailable.Reason = U6000.NEWBANNERSUNAVAILABLEREASON;
        }
        else
            NewBannerUnavailable.Visible = false;
    }

    protected void PacksDDL_Init(object sender, EventArgs e)
    {
        var packs = ExternalBannerAdvertPack.GetActive();
        var listItems = new List<ListItem>();
        foreach (var p in packs)
        {
            var dimensions = new ExternalBannerAdvertDimensions(p.ExternalBannerDimensionsId);
            listItems.Add(
                new ListItem
                {
                    Value = p.Id.ToString(),
                    Text = string.Format("{0} {1} ({2} x {3}px) - {4}", p.Clicks, L1.CLICKS, dimensions.Width, dimensions.Height, p.Price)
                });
        }

        PacksDDL.Controls.Clear();
        PacksDDL.Items.AddRange(listItems.ToArray());
    }

    protected void CategoriesDDL_Init(object sender, EventArgs e)
    {
        var categories = PublishersWebsiteCategory.GetActive();
        var listItems = new List<ListItem>();
        foreach (var c in categories)
        {
            listItems.Add(
                new ListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
        }

        CategoriesDDL.Controls.Clear();
        CategoriesDDL.Items.AddRange(listItems.ToArray());
    }

    protected void PacksDDL_SelectedIndexChanged(object sender, EventArgs e)
    {
        ClearBanner();
    }

    protected void MyBannersGridView_DataBound(object sender, EventArgs e)
    {
        MyBannersGridView.Columns[2].HeaderText = L1.IMAGE;
        MyBannersGridView.Columns[3].HeaderText = U5006.CATEGORY;
        MyBannersGridView.Columns[4].HeaderText = U6000.DIMENSIONS;
        MyBannersGridView.Columns[5].HeaderText = L1.CLICKS;
        MyBannersGridView.Columns[6].HeaderText = L1.STATUS;
    }

    protected void MyBannersGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var advert = new ExternalBannerAdvert(Convert.ToInt32(e.Row.Cells[0].Text));

            try
            {
                var bannerImage = advert.Image;
                var imageWebControl = new Image
                {
                    ImageUrl = bannerImage.Path,
                    Width = Unit.Pixel(bannerImage.Width / 10),
                    Height = Unit.Pixel(bannerImage.Height / 10)
                };

                e.Row.Cells[2].Text = string.Empty;
                e.Row.Cells[2].Controls.Add(imageWebControl);

                e.Row.Cells[4].Text = bannerImage.Width + " x " + bannerImage.Height;
                e.Row.Cells[5].Text = HtmlCreator.GenerateCPAAdProgressHTML(advert.ClicksReceived, advert.ClicksBought);
            }
            catch (Exception ex)
            {
                e.Row.Cells[2].Text = string.Empty;
            }            
        }
    }

    protected void MyBannersGridView_DataSource_Init(object sender, EventArgs e)
    {
        MyBannersGridView_DataSource.SelectCommand =
            string.Format(@"SELECT a.Id, a.Url, a.ImagePath, (SELECT Name FROM PublishersWebsiteCategories WHERE Id = a.PublishersWebsiteCategoryId) AS Category,
                            a.ExternalBannerAdvertPackId, a.ClicksReceived, a.Status 
                            FROM ExternalBannerAdverts a
                            WHERE a.UserId = {0}", Member.CurrentId);
    }

    protected void MyBannersView_Activate(object sender, EventArgs e)
    {
        MyBannersGridView.DataBind();
    }

    protected void ImageSubmitValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = ImageUpload.HasFile || !string.IsNullOrEmpty(BannerFileUrlTextBox.Text);
    }

    protected void ImageUploadedValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = TempImage != null && TempImage.IsSaved;
    }
}