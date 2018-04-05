using Prem.PTC;
using System;
using System.Text;

public partial class Controls_Misc_CustomHeader : System.Web.UI.UserControl
{
    public string Content
    {
        get
        {
            return AppSettings.Site.CustomHeaderContent;
        }
    }

    protected void ScriptsLiteral_Init(object sender, EventArgs e)
    {
        //Could use bundles instead
        int version = 6004;
        StringBuilder scripts = new StringBuilder();
        do
        {
            scripts.AppendFormat("<link href='Scripts/default/assets/css/update/U{0}.min.css' rel='stylesheet' />", version);
            version = VersionManager.GetNextVersion(version);
        }
        while (version <= VersionManager.CurrentVersion);

        ScriptsLiteral.Text = scripts.ToString();
    }
}