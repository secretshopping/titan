using System;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Resources;
using System.IO;
using Titan.MiniVideos;

public partial class user_advert_minivideo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.AdvertMiniVideoEnabled && (!Member.IsLogged || Member.CurrentInCache.IsAdvertiser));

        TitleLiteral.Text = U6008.MINIVIDEO;
        SubLiteral.Text = U6008.MINIVIDEODESCRIPTION;
        if (!IsPostBack)
        {
            BindDataToDDL();            
        }

        UserName.Text = Member.CurrentName;

        LangAdder.Add(ManageButton, L1.MANAGE);
        LangAdder.Add(AddNewButton, L1.NEWCAMPAIGN);
        LangAdder.Add(CreateVideoButton, U4200.CREATE);
        LangAdder.Add(TitleRequired, L1.REQ_TITLE);
        LangAdder.Add(DescriptionRequiredFieldValidator, L1.REQ_DESC);
        LangAdder.Add(ImageSubmitCustomValidator, U6000.CHOOSEFILE);
        LangAdder.Add(ImageServerCustomValidator, U5006.MUSTUPLOADIMAGE);
        LangAdder.Add(VideoSubminCustomValidator, U6000.CHOOSEFILE);
        LangAdder.Add(VideoServerCustomValidator, U6008.MUSTUPLOADVIDEO);
    }

    protected void VideoImageButton_Click(object sender, EventArgs e)
    {
        var fileName = HashingManager.GenerateMD5(DateTime.Now + "miniVideoImg");
        const string filePath = "~/Images/b_ads/";

        try
        {
            Banner videoImage;
            var inputStream = ImageFileUpload.PostedFile.InputStream;

            if (!Banner.TryFromStream(inputStream, out videoImage) || videoImage.Width > MiniVideoManager.VideoImageMaxWidth ||
                videoImage.Height > MiniVideoManager.VideoImageImageMaxHeight)
                throw new MsgException(string.Format(U6003.INVALIDIMAGEORDIMENSION, MiniVideoManager.VideoImageMaxWidth, MiniVideoManager.VideoImageImageMaxHeight));

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

    protected void ImageSubmitValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = ImageFileUpload.HasFile;
    }

    protected void ImageValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = VideoImage.ImageUrl != "";
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

    protected void VideoValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = VideoLabel.Text != "";
    }

    protected void AddVideoView_Activate(object sender, EventArgs e)
    {
        lblPrice.Text = Member.CurrentInCache.Membership.MiniVideoUploadPrice.ToString();
    }

    private void BindDataToDDL()
    {
        VideoCategoriesList.Items.Clear();
        VideoCategoriesList.Items.AddRange(MiniVideoCategory.ListItems);        
    }

    protected void CreateVideoButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        if (Page.IsValid)
        {
            try
            {
                var user = Member.Current;
                var price = user.Membership.MiniVideoUploadPrice;

                if (!MiniVideoCampaign.GetTitleAvability(TitleTextBox.Text))
                    throw new MsgException(U6008.VIDEOWITHTITLEEXIST);

                var miniVideoCampaign = new MiniVideoCampaign
                {
                    Title = TitleTextBox.Text,
                    Username = user.Name,
                    Status = MiniVideoStatus.Active,
                    Description = DescriptionTextBox.Text,
                    ImageURL = VideoImage.DescriptionUrl,
                    VideoURL = VideoURLHiddenLabel.Text,
                    VideoCategory = Convert.ToInt32(VideoCategoriesList.SelectedValue),
                    AddedDate = DateTime.Now
                };

                PurchaseOption.ChargeBalance(user, price, TargetBalanceRadioButtonList.Feature, TargetBalanceRadioButtonList.TargetBalance, "Mini Video Upload");

                miniVideoCampaign.Save();

                SuccMessagePanel.Visible = true;
                SuccMessage.Text = U6008.MINIVIDEOCREATED;
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

    protected void UserMiniVideosGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var status = (MiniVideoStatus)Convert.ToInt32(e.Row.Cells[3].Text);
            e.Row.Cells[3].Text = HtmlCreator.GetColoredStatus(status);

            var categoryId = Convert.ToInt32(e.Row.Cells[2].Text);
            e.Row.Cells[2].Text = new MiniVideoCategory(categoryId).Name;
        }
    }

    protected void UserMiniVideosGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "remove")
        {
            ErrorMessagePanel2.Visible = false;

            var index = e.GetSelectedRowIndex() % UserMiniVideosGridView.PageSize;
            var row = UserMiniVideosGridView.Rows[index];

            var videoId = Convert.ToInt32(row.Cells[0].Text);
            var video = new MiniVideoCampaign(videoId);

            video.Delete();

            UserMiniVideosGridView.DataBind();
        }
    }

    protected void manageView_Activate(object sender, EventArgs e)
    {
        UserMiniVideosGridView.DataBind();
    }
}
