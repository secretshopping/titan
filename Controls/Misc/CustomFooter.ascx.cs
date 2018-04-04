using Prem.PTC;

public partial class Controls_Misc_CustomFooter : System.Web.UI.UserControl
{
    public string Content
    {
        get
        {
            return AppSettings.Site.CustomFooterContent;
        }
    }
}