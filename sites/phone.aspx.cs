using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;

public partial class sites_phone : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LangAdder.Add(ConfirmTheCode, L1.CONFIRM);
        LangAdder.Add(SendSMS, L1.SENDSMSBUTTON);

        //?s=phone&username=[uname]&code=[userid]
        string username = Request.QueryString["username"].Replace("'", "");
        string code = Request.QueryString["code"].Replace("'", "");


        Member User = new Member(username);

        if (User.Status == MemberStatus.AwaitingSMSPIN && User.Id.ToString() == code)
        {
            if (User.IsPhoneVerified)
            {
                //He previously verified the phone
                CC.Text = User.CountryCode;
                CC.Enabled = false;
                PHONE.Text = User.PhoneNumber;
                PHONE.Enabled = false;
            }
        }
    }

    protected void SendSMS_Click(object sender, EventArgs e)
    {
        try
        {
            AppSettings.Proxy.Reload();
            Member User = new Member(Request.QueryString["username"].Replace("'", ""));

            if (User.UnconfirmedSMSSent > 0)
            {
                ErrorMessagePanel2.Visible = true;
                ErrorMessage2.Text = L1.ALREADYSENT + "(+" + User.PhoneCountryCode + " " + User.PhoneNumber + ")";
            }
            else if (User.Status == MemberStatus.AwaitingSMSPIN)
            {
                string pin = ProxStop.SendSMSWithPIN(CC.Text, PHONE.Text);
                Session["HASHEDCODE"] = Prem.PTC.Members.MemberAuthenticationService.ComputeHash(pin.ToString());

                SMSPanel1.Visible = false;
                SMSPanel2.Visible = true;

                User.UnconfirmedSMSSent++;
                User.PhoneCountryCode = CC.Text.Trim();
                User.PhoneNumber = PHONE.Text.Trim();
                User.Save();
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    protected void ConfirmTheCode_Click(object sender, EventArgs e)
    {
        try
        {
            string hash = Prem.PTC.Members.MemberAuthenticationService.ComputeHash(TheCode.Text.Trim());
            if (hash == Session["HASHEDCODE"].ToString())
            {
                //PIN OK
                Member User = new Member(Request.QueryString["username"].Replace("'", ""));
                User.IsPhoneVerified = true;
                User.IsPhoneVerifiedBeforeCashout = true;
                User.MakeActive();
                User.UnconfirmedSMSSent--;
                User.Save();

                SuccMessagePanel2.Visible = true;
                SuccMessage2.Text = U3501.PHONESUCC;

                ConfirmTheCode.Visible = false;
            }
            else
            {
                ErrorMessagePanel2.Visible = true;
                ErrorMessage2.Text = L1.WRONGCODE;
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            throw ex;
        }
    }
}