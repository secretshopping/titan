using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using System.Web.UI.WebControls;
using Resources;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using Prem.PTC.Payments;
using Titan.Leadership;

public partial class About : TitanPage
{
    //?type=ok ?type=fail   [REQUIRED]
    //&id=...               [OPTIONAL]

    public string IconClass = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["PAYEE_ACCOUNT"] != null)
        {
            //PerfectMoney
            Title.Text = "";
            IconClass = "";
            Literal1.Text = PerfectMoneyButtonGenerationStrategy.GenerateInputLiteral(Request.QueryString);
            Description.Text = L1.YOUWILLREDIRECT;
        }
        else if (Request.QueryString["sci_name"] != null)
        {
            //SolidTrustPay
            Title.Text = "";
            IconClass = "";
            Literal1.Text = SolidTrustPayButtonGenerationStrategy.GenerateInputLiteral(Request.QueryString);
            Description.Text = L1.YOUWILLREDIRECT;
        }
        else if (Request.QueryString["netellerItemName"] != null)
        {
            //Neteller
            Title.Text = "";
            IconClass = "";

            NetellerButtonGenerationStrategy.TryInvokeTransfer(Request.QueryString);

            Description.Text = L1.YOUWILLREDIRECT;
        }
        else if (Request.QueryString["type"] != null)
        {
            IconClass = "";

            string type = Request.QueryString["type"].ToString();

            if (type == "ok")
            {
                Title.Text = L1.OP_SUCCESS;

            }
            else if (type == "fail")
            {
                Title.Text = L1.OP_FAIL;
                IconClass = "fa fa-times";
            }
            else if (type == "paymentok")
            {
                Title.Text = U3501.PAYMENTOK;
                Response.AddHeader("REFRESH", "5;URL=user/default.aspx");
            }
            else if (type == "logoutok")
            {
                Title.Text = U3501.LOGOUTOK;

            }
            else if (type == "logoutsuspended")
            {
                Title.Text = U4000.FRAUDINFO;
            }
            else if (type == "registerok")
            {
                Title.Text = U3501.REGISTEROK;
            }
            else if (type == "transferok")
            {
                Title.Text = U3501.TRANSFEROK;
                Response.AddHeader("REFRESH", "5;URL=user/default.aspx");
            }
            else if (type == "profilerok")
            {
                Title.Text = U3900.PROFILEOK;
                Response.AddHeader("REFRESH", "4;URL=user/default.aspx?afterlogin=1");
            }
            else if (type == "friendok")
            {
                Title.Text = U3501.FRIENDOK;
            }
            else if (type == "upgradeok")
            {
                Title.Text = U3501.UPGRADEOK;
                Response.AddHeader("REFRESH", "4;URL=user/default.aspx");
            }

            //Lets check the description error
            if (Request.QueryString["id"] != null)
            {
                string id = Request.QueryString["id"].ToString();

                if (id == "register1")
                {
                    //Instantly
                    Description.Text = L1.ST_REGISTER1 + ". " + L1.YOUWILLREDIRECT;
                    Response.AddHeader("REFRESH", "7;URL=login.aspx");
                }
                else if (id == "register2")
                {
                    //Activation email
                    Description.Text = L1.ST_REGISTER2 + ". " + L1.YOUWILLREDIRECT;
                    Response.AddHeader("REFRESH", "7;URL=login.aspx?afterregister=1");
                }
                else if (id == "activated")
                    Description.Text = L1.ST_ACTIVATED;
                else if (id == "reset")
                    Description.Text = L1.ST_RESET;
                else if (id == "reset2")
                    Description.Text = U4000.ST_RESET;
                else if (id == "logout")
                {
                    Description.Text = L1.SEEYOU + ". " + L1.YOUWILLREDIRECT;
                    Response.AddHeader("REFRESH", "3;URL=default.aspx");
                    IconClass = "fa fa-check";
                }
                else if (id == "logoutsus")
                {
                    Description.Text = L1.YOUWILLREDIRECT;
                    Response.AddHeader("REFRESH", "3;URL=default.aspx");
                }
            }
        }
        else if (Request.QueryString["confirm"] != null)
        {
            try
            {
                string hash = Request.QueryString["confirm"];
                string username = Encryption.Decrypt(hash);
                Member User = new Member(username);



                if (User.Status == MemberStatus.Inactive)
                {
                    User.Activate();

                    if (AppSettings.Proxy.SMSType == ProxySMSType.EveryRegistration)
                    {
                        User.RequireSMSPin();
                        User.SaveStatus();
                    }

                    if (User.HasReferer)
                    {
                        var list = new List<RestrictionKind>();
                        list.Add(RestrictionKind.DirectReferralsCount);
                        LeadershipSystem.CheckSystem(list, new Member(User.ReferrerId));
                    }

                    Title.Text = L1.OP_SUCCESS;
                    Description.Text = L1.YOUWILLREDIRECT;

                    IconClass = "fa fa-check";

                    //Try contests [activated]
                    if (User.HasReferer && AppSettings.Misc.AreContestsEnabled)
                        Prem.PTC.Contests.ContestManager.IMadeAnAction(Prem.PTC.Contests.ContestType.Direct, User.Referer, null, 1);

                    Response.AddHeader("REFRESH", "2;URL=login.aspx");
                }
                else if (User.Status == MemberStatus.Active)
                {
                    //But first check if this address hasn't been taken in the meantime
                    if (Member.ExistsWithEmail(User.Temp))
                        throw new MsgException(L1.ER_DUPLICATEEMAIL);

                    //Email change
                    User.Email = User.Temp;
                    User.Save();

                    //Show info
                    Title.Text = L1.OP_SUCCESS;
                    Description.Text = L1.YOUWILLREDIRECT;
                    IconClass = "fa fa-check";
                    Response.AddHeader("REFRESH", "2;URL=login.aspx");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                Title.Text = L1.OP_FAIL;
                IconClass = "fa fa-times";
            }
        }
        else
        {
            //Error page
            Title.Text = L1.ERROR;
            Description.Text = L1.ERROR_INFO;
            IconClass = "fa fa-times";
        }
    }
}
