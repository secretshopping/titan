using Prem.PTC;
using Resources;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Controls_Representatives_RepresentativePaymentMethod : System.Web.UI.UserControl
{
    private RepresentativesPaymentProcessor _representativePaymentMethod;
    public RepresentativesPaymentProcessor RepresentativePaymentMethod
    {
        set
        {
            _representativePaymentMethod = value;
            ProcessorName = _representativePaymentMethod.Name;
            LogoImagePath = _representativePaymentMethod.LogoPath;
            DepositInfo = _representativePaymentMethod.DepositInfo;
            WithdrawalInfo = _representativePaymentMethod.WithdrawalInfo;
        }
    }

    public string ProcessorName
    {
        get
        {
            return ProcessorNameTextBox.Text;
        }
        set
        {
            ProcessorNameTextBox.Text = value;
        }
    }

    public string LogoImagePath
    {
        get
        {
            return ProcessorLogoImage.DescriptionUrl;
        }
        set
        {
            ProcessorLogoImage.DescriptionUrl = value;
            ProcessorLogoImage.ImageUrl = value;
        }

    }

    public string DepositInfo
    {
        get
        {
            return DepositInformationTextBox.Text;
        }
        set
        {
            DepositInformationTextBox.Text = value;
        }
    }

    public string WithdrawalInfo
    {
        get
        {
            return WithdrawalInformationTextBox.Text;
        }
        set
        {
            WithdrawalInformationTextBox.Text = value;
        }
    }    

    public void ClearFields()
    {
        WithdrawalInfo = string.Empty;
        DepositInfo = string.Empty;
        LogoImagePath = string.Empty;
        ProcessorName = string.Empty;
        ProcessorLogoImage_Upload.Dispose();
        ProcessorLogoImage.ImageUrl = string.Empty;
        ProcessorLogoImage.DescriptionUrl = string.Empty;
        ProcessorLogoImage.Dispose();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        DepositInformationPlaceHolder.Visible = AppSettings.Representatives.RepresentativesHelpDepositEnabled;
        WithdrawalInformationPlaceHolder.Visible = AppSettings.Representatives.RepresentativesHelpWithdrawalEnabled;

        LangAdder.Add(ProcessorNameRequiredFieldValidator, L1.NAME + " " + U3900.FIELDISREQUIRED, true);
        //LangAdder.Add(DepositInformationRequiredFieldValidator, U6010.DEPOSITINFO + " " + U3900.FIELDISREQUIRED, true);
        //LangAdder.Add(WithdrawalInformationRequiredFieldValidator, U6010.WITHDRAWALINFO + " " + U3900.FIELDISREQUIRED, true);
    }

    protected void ProcessorLogoImage_UploadSubmit_Click(object sender, EventArgs e)
    {
        var fileName = HashingManager.GenerateMD5(DateTime.Now + "paymentLogoImg");
        const string filePath = "~/Images/RepPaymentsLogos/";

        ErrorMessagePanel.Visible = false;
        try
        {
            if (!Page.IsValid) return;

            Banner logoImage;
            var inputStream = ProcessorLogoImage_Upload.PostedFile.InputStream;

            if (!Banner.TryFromStream(inputStream, out logoImage) || logoImage.Width > ImagesHelper.RepresentativesPaymentsLogo.LogoImageMaxWidth ||
                logoImage.Height > ImagesHelper.RepresentativesPaymentsLogo.LogoImageMaxHeight)
                throw new MsgException(string.Format(U6003.INVALIDIMAGEORDIMENSION, ImagesHelper.RepresentativesPaymentsLogo.LogoImageMaxWidth, ImagesHelper.RepresentativesPaymentsLogo.LogoImageMaxHeight));

            logoImage.Save(filePath, fileName);
            ProcessorLogoImage.ImageUrl = Banner.GetTemporaryBannerPath(logoImage);
            ProcessorLogoImage.DescriptionUrl = logoImage.Path;
            ProcessorLogoImage.Dispose();
        }
        catch (Exception ex)
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = ex.Message;
        }
    }

    protected void PTCImageSubmitValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = ProcessorLogoImage_Upload.HasFile;
    }

    protected void ProcessorLogoImageValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = ProcessorLogoImage.ImageUrl != "";
    }
}