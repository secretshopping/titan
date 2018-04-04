using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;
using Prem.PTC.Advertising;
using Prem.PTC.Texts;

public partial class Controls_AccountActivationPopUp : System.Web.UI.UserControl
{
    #region Parameters
    public Money AccountActivationFee
    {
        get { return _Fee; }
        set { _Fee = value; }
    }

    public String PopUpTitle
    {
        get { return U6011.ACCOUNTACTIVATION; }
    }

    public String PopUpHtmlBody
    {
        get { return _HtmlBody; }
        set { _HtmlBody = value; }
    }

    public String PopUpButtonTextConfirm
    {
        get { return U6011.ACTIVATE; }
    }

    public String PopUpButtonTextCancel
    {
        get { return U4000.CANCEL; }
    }

    private Money _Fee;
    private String _HtmlBody;
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        if (AppSettings.Registration.IsAccountActivationFeeEnabled && !Member.CurrentInCache.IsAccountActivationFeePaid)
            this.Visible = true;
        else
            this.Visible = false;

        AccountActivationFee = AppSettings.Registration.AccountActivationFee;

        String MainText = String.Empty;
        MainText += String.Format(U6011.ACTIVATIONINFO, AppSettings.Site.Name);
        MainText += String.Format("<br /><br /><b> {0}: {1} </b>", L1.PRICE, AccountActivationFee);

        String linkToDeposit = String.Format("<a href=\"user/transfer.aspx\">{0}</a>.", U6011.HERE);

        if (AppSettings.Payments.CashBalanceEnabled && AppSettings.Registration.AccountActivationFeeViaCashBalanceEnabled && (Member.CurrentInCache.CashBalance < AccountActivationFee))
            MainText += String.Format("<span style=\"color:green\"><br /><br /> {0} <br />{1} {2}</span>", U6011.YOUCANPAYVIACASHBALANCE, U6011.YOUCANDEPOSITCASH, linkToDeposit);




        if (TitanFeatures.IsRofriqueWorkMines)
            MainText += String.Format("<span style=\"text-align: left\"><b>{0}</b></span>", GetAdditionalInfo()); 
        

        _HtmlBody = String.Format("<div><br/> {0} </div><br/>", MainText);
    }

    private static String GetAdditionalInfo()
    {
        StringBuilder additionalInfo = new StringBuilder();
        additionalInfo.Append("<br /><br />The people you refer directly are your level 1 refferals and you earn 20% of the account activation fee as a token from us.");
        additionalInfo.Append("<br />You still earn 5% more from the people your level 1 refferals refer as per the below chart:");
        additionalInfo.Append("<br />Level 2 refferals earn you ---- 1.25$/day");
        additionalInfo.Append("<br />Level 3 refferals earn you ---- 6.25$/day");
        additionalInfo.Append("<br />Level 4 refferals earn you ---- 31.25$/day");
        additionalInfo.Append("<br />Level 5 refferal earn you ---- 156.25$week");
        additionalInfo.Append("<br />Level 6 refferals earn you ---- 781.25$/month");
        additionalInfo.Append("<br />Level 7 refferals earn you ---- 3906.25$/month");
        additionalInfo.Append("<br />Level 8 refferals earn you ---- 19531.25$/month");
        additionalInfo.Append("<br />Level 9 refferals earn you ---- 97656.25$/quarter");
        return additionalInfo.ToString();
    }
}
