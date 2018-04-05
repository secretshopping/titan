using System;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System.Web;

public partial class Controls_Representative : System.Web.UI.UserControl
{
    public Representative Representative { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        var representant = new Member(Representative.UserId);

        Namelabel.InnerText = Representative.Name;

        UserAvatar.Text = HtmlCreator.CreateAvatarPlusUsername(representant);
            
        Emaillabel.InnerText = Representative.Email;
        Emaillabel.HRef = "mailto:" + Representative.Email;
        Citylabel.InnerText = Representative.City;
        Countrylabel.InnerText = Representative.Country;
        PhoneNumberlabel.InnerText = Representative.PhoneNumber;
        Languageslabel.InnerText = Representative.Languages;

        if (!string.IsNullOrEmpty(Representative.Skype))        
            skypeAnhor.HRef = string.Format("skype:live:{0}?chat", Representative.Skype);        

        if(!string.IsNullOrEmpty(Representative.Facebook))
            FacebookAnhor.HRef = string.Format("https:/facebook.com/{0}", Representative.Facebook);
        
        LastActivitySpan.InnerText = representant.LastActivityTime.HasValue ? representant.LastActivityTime.ToString() : L1.NEVER;

        JoinLinkButton.Visible = (AppSettings.Representatives.Policy == AppSettings.Representatives.RepresentativesPolicy.Automatic ? false : true)
            && !Member.IsLogged;

        JoinLinkButton.Text = string.Format(U6002.JOINTOREPRESENTATION, Representative.Name);
        JoinLinkButton.PostBackUrl = string.Format("/register.aspx?u={0}", Representative.UserId);

        if (Member.IsLogged)
        {
            WithdrawalButton.Visible = AppSettings.Representatives.RepresentativesHelpWithdrawalEnabled;            
            DepositButton.Visible = AppSettings.Representatives.RepresentativesHelpDepositEnabled;
        }
    }
}