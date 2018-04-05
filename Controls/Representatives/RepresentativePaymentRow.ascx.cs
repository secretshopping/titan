public partial class Controls_Representatives_RepresentativePaymentRow : System.Web.UI.UserControl, IRepresentativePaymentRow
{
    public string ProcessorName
    {
        get
        {
            return ProcessorNameLiteral.Text;
        }
        set
        {
            ProcessorNameLiteral.Text = value;
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

    public string Info
    {
        get
        {
            return InformationLiteral.Text;
        }
        set
        {
            InformationLiteral.Text = value;
        }
    }
}