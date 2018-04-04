using System;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Resources;
using System.IO;
using Titan.MiniVideos;
using Titan.News;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Prem.PTC.Advertising;
using System.Web;

public partial class user_news_writing : System.Web.UI.Page
{
    public enum RequestType
    {
        Create = 0,
        Edit = 1
    }

    public RequestType PageRequest { get; set; }

    public int MaxKeywords = 15;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.NewsWritingArticlesEnabled && (!Member.IsLogged || Member.CurrentInCache.IsEarner));

        if (ViewState["editid"] == null)
            ViewState["editid"] = Request.QueryString["editid"];
        if (ViewState["editid"] != null)
        {
            PageRequest = RequestType.Edit;
        }
        else
            PageRequest = RequestType.Create;

        TextCKEditor.config.format_tags = "p;h1";

        if (!IsPostBack)
        {
            TitleLiteral.Text = U6012.WRITINGARTICLES;
            SubLiteral.Text = U6012.WRITINGARTICLESINFO;
            TitleTextBox.MaxLength = Article.TitleMaxCharacters;
            TitleRangeValidator.ValidationExpression = "^.{" + Article.TitleMinCharacters + "," + Article.TitleMaxCharacters + "}$";

            LangAdder.Add(ManageButton, L1.MANAGE);
            LangAdder.Add(AddNewButton, L1.ADDNEW);
            LangAdder.Add(CreateButton, U6013.SENDFORAPPROVAL);
            LangAdder.Add(TitleRequired, L1.REQ_TITLE);
            LangAdder.Add(DescriptionRequiredFieldValidator, L1.REQ_DESC);
            LangAdder.Add(ImageSubmitCustomValidator, U6000.CHOOSEFILE);
            LangAdder.Add(VideoSubminCustomValidator, U6000.CHOOSEFILE);
            LangAdder.Add(CustomValidator1, L1.ER_BADCAPTCHA, true);
            LangAdder.Add(TitleRangeValidator, String.Format(U6013.THISFIELDMUSTCOINTAIN, Article.TitleMinCharacters, Article.TitleMaxCharacters));
            LangAdder.Add(SubtitleRangeValidator, String.Format(U6013.THISFIELDMUSTCOINTAIN, 70, 150));

            ArticlesGridView.EmptyDataText = U6012.NOARTICLES;

            if (PageRequest == RequestType.Edit)
                BindEditWindow();
        }

        KeywordsTextBox.Attributes.Add("data-max", MaxKeywords.ToString());
        KeywordsTextBox.Attributes.Add("data-message", string.Format(U6002.TOOMANYTAGS, MaxKeywords));
    }

    public void MenuButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

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

    #region Manage

    protected void ArticlesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var article = new Article(Convert.ToInt32(e.Row.Cells[0].Text));

            e.Row.Cells[1].Text = InputChecker.HtmlPartialDecode(article.Title);
            e.Row.Cells[2].Text = article.GetCategory().Text;
            e.Row.Cells[4].Text = String.Format("<img src='Images/Flags/{0}.png'/> {1}", article.Geolocation.ToLower(),
                CountryManager.GetCountryName(article.Geolocation));

            e.Row.Cells[6].Text = Money.Parse(e.Row.Cells[6].Text).ToString();
            e.Row.Cells[7].Text = HtmlCreator.GetColoredStatus(article.Status);

            if (article.Status != AdvertStatus.Paused)
                e.Row.Cells[8].Text = "&nbsp;";

            if (article.Status != AdvertStatus.Active)
                e.Row.Cells[9].Text = "&nbsp;";

            //Edit button
            ((LinkButton)e.Row.Cells[11].FindControl("ImageButton4")).ToolTip = U5007.EDIT;
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[5].Text = U6012.READS;
            e.Row.Cells[6].Text = U6012.MONEYCREDITED;
        }
    }

    protected void ArticlesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //We want to obey OnSort and OnChart events
        string[] commands = new string[4] { "start", "stop", "remove", "edit"};

        if (commands.Contains(e.CommandName))
        {
            ErrorMessagePanel2.Visible = false;

            var index = e.GetSelectedRowIndex() % ArticlesGridView.PageSize;
            var row = ArticlesGridView.Rows[index];

            var articleId = Convert.ToInt32(row.Cells[0].Text);
            var article = new Article(articleId);

            if (e.CommandName == "remove")
            {
                article.Delete();
            }
            else if (e.CommandName == "start" && article.Status == AdvertStatus.Paused)
            {
                article.Status = AdvertStatus.Active;
                article.Save();
            }
            else if (e.CommandName == "stop" && article.Status == AdvertStatus.Active)
            {
                article.Status = AdvertStatus.Paused;
                article.Save();
            }
            else if (e.CommandName == "edit")
            {
                Response.Redirect("writing.aspx?editid=" + articleId);
            }

            ArticlesGridView.DataBind();
        }
    }

    protected void ArticlesSqlDataSource_Init(object sender, EventArgs e)
    {
        ArticlesSqlDataSource.SelectCommand =
            String.Format("SELECT * FROM [Articles] WHERE [CreatorUserId] = {0} AND [StatusInt] != {1} ORDER BY [CreatedDate] DESC",
            Member.CurrentId, (int)UniversalStatus.Deleted);
    }

    #endregion

    #region Add New

    protected void VideoImageButton_Click(object sender, EventArgs e)
    {
        var fileName = HashingManager.GenerateMD5(DateTime.Now + "articleImg");
        const string filePath = "~/Images/b_ads/";

        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        try
        {
            Banner videoImage;
            var inputStream = ImageFileUpload.PostedFile.InputStream;

            if (!Banner.TryFromStream(inputStream, out videoImage) || videoImage.Width > NewsManager.ImageMaxWidth ||
                videoImage.Height > NewsManager.ImageImageMaxHeight)
                throw new MsgException(string.Format(U6003.INVALIDIMAGEORDIMENSION, NewsManager.ImageMaxWidth, NewsManager.ImageImageMaxHeight));

            if (videoImage.Width < ImagesHelper.News.ImageMinWidth || videoImage.Height < ImagesHelper.News.ImageMinHeight)
                throw new MsgException(string.Format(U6013.IMAGETOOSMALL, ImagesHelper.News.ImageMinWidth, ImagesHelper.News.ImageMinHeight));

            videoImage.Save(filePath, fileName);
            VideoImage.ImageUrl = Banner.GetTemporaryBannerPath(videoImage);
            VideoImage.DescriptionUrl = videoImage.Path;
            ImageFileUpload.Dispose();
        }
        catch (Exception ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }

    protected void VideoSourceButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (VideoFileUpload.PostedFile.ContentLength > MiniVideoManager.MaxFileSize)
                throw new MsgException(string.Format(U6008.SIZECANBEUPTO, MiniVideoManager.FileSizeInMB));

            var phyicalFilePath = Path.GetFileName(VideoFileUpload.PostedFile.FileName);
            var extension = Path.GetExtension(phyicalFilePath);

            if (MiniVideoManager.CheckVideoExtension(extension))
            {
                VideoLabel.Text = phyicalFilePath;
                var fileName = HashingManager.GenerateMD5(DateTime.Now + phyicalFilePath) + extension;
                string folder = Server.MapPath("~/Images/uploaded_videos/");
                Directory.CreateDirectory(folder);
                VideoFileUpload.PostedFile.SaveAs(Path.Combine(folder, fileName));
                VideoURLHiddenLabel.Text = "~/Images/uploaded_videos/" + fileName;
            }
            else
            {
                var extensions = "";
                foreach (var ext in MiniVideoManager.FileExtensions)
                    extensions += ext + " ";

                throw new MsgException(string.Format(U6008.WRONGFILEEXTENSION, extensions));
            }

        }
        catch (Exception ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }

    protected void VideoSubmitValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = VideoFileUpload.HasFile;
    }

    protected void ImageSubmitValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = ImageFileUpload.HasFile;
    }

    protected void manageView_Activate(object sender, EventArgs e)
    {
        ArticlesGridView.DataBind();
    }

    protected void AddNewArticleView_Activate(object sender, EventArgs e)
    {
        CountriesList.Items.Clear();
        CountriesList.Items.AddRange(NewsCountriesHelper.ListItems);

        //Select the user-default country
        var SelectedCountry = (new RegionInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name)).TwoLetterISORegionName;
        CountriesList.SelectedValue = SelectedCountry.ToUpper();

        BindDataToCategoriesDDL();
    }

    private void BindDataToCategoriesDDL()
    {
        CategoriesList.Items.Clear();
        CategoriesList.Items.AddRange(ArticleCategory.GetListItems(CountriesList.SelectedValue));
    }

    protected void CountriesList_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindDataToCategoriesDDL();
    }

    protected void CreateOrEditButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                var user = Member.Current;

                string ImageURL = VideoImage.DescriptionUrl;
                string VideoURL = VideoURLHiddenLabel.Text;

                if (PageRequest == RequestType.Create && String.IsNullOrEmpty(ImageURL) && String.IsNullOrEmpty(VideoURL))
                    throw new MsgException(U6012.MUSTUPLOADIMAGEORVIDEO);

                string title = InputChecker.HtmlEncode(TitleTextBox.Text, Article.TitleMaxCharacters, L1.TITLE);
                string description = InputChecker.HtmlEncode(DescriptionTextBox.Text, 150, U6012.SUBTITLE);
                string text = InputChecker.HtmlEncode(Request.Form[TextCKEditor.UniqueID], 2000000000, L1.TEXT);
                List<string> keywords = KeywordsTextBox.Text.Replace(" ", string.Empty)
                                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(tag => InputChecker.HtmlEncode(tag, 20, U6002.TAG))
                                    .ToList();
                string keywordsString = InputChecker.HtmlEncode(String.Join(",", keywords.ToArray()), 5000, U6012.KEYWORDS);

                if (PageRequest == RequestType.Create)
                {
                    Article.Add(title, text, description, keywordsString, Member.CurrentId, CountriesList.SelectedValue,
                        Convert.ToInt32(CategoriesList.SelectedValue), ImageURL, VideoURL);
                    SuccMessage.Text = U6012.ARTICLECREATED + ". " + L1.YOUWILLREDIRECT;
                }
                else
                {
                    Article.Edit(Convert.ToInt32(ViewState["editid"]), title, text, description, keywordsString, Member.CurrentId, CountriesList.SelectedValue,
                        Convert.ToInt32(CategoriesList.SelectedValue), ImageURL, VideoURL);
                    SuccMessage.Text = U6013.ARTICLESAVEDANDSENT + " " + L1.YOUWILLREDIRECT;
                }

                SuccMessagePanel.Visible = true;

                ViewState["editid"] = null;
                Response.AddHeader("REFRESH", "3;URL=writing.aspx");
            }
            catch (MsgException ex)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }
    }

    protected void Validate_Captcha(object source, ServerValidateEventArgs args)
    {
        if (TitanCaptcha.IsValid)
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
    }

    #endregion

    #region Edit

    private void BindEditWindow()
    {
        MenuMultiView.SetActiveView(AddNewArticleView);
        int articleId = Convert.ToInt32(ViewState["editid"]);
        Article article = new Article(articleId); 

        if (article.CreatorUserId != Member.CurrentId || article.Status == AdvertStatus.Deleted)
            Response.Redirect("~/default.aspx");

        TitleTextBox.Text = Server.HtmlDecode(article.Title);
        DescriptionTextBox.Text = Server.HtmlDecode(article.ShortDescription);
        CategoriesList.SelectedValue = article.CategoryId.ToString();
        CountriesList.SelectedValue = article.Geolocation.ToString();
        TextCKEditor.Text = Server.HtmlDecode(article.Text);
        KeywordsTextBox.Text = Server.HtmlDecode(article.Keywords);
        VideoImage.ImageUrl = article.ImageURL;
        VideoLabel.Text = article.VideoURL;
    }

    #endregion


}
