using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC.Payments;
using Resources;
using System;
using System.Web;
using System.Web.UI.WebControls;
using Titan;
using Titan.Registration;

public partial class sites_activation : System.Web.UI.Page
{
    Member User = Member.CurrentInCache;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AppSettings.Registration.IsAccountActivationFeeEnabled || User.IsAccountActivationFeePaid)
            AccessManager.RedirectIfDisabled(false);

        UpgradeViaPaymentProcessor.Text = U6005.PAYVIAPAYMENTPROCESSOR;
        PriceLiteral.Text = AppSettings.Registration.AccountActivationFee.ToString();

        if (!Page.IsPostBack)
        {
            if (!PaymentAccountDetails.AreIncomingPaymentProcessorsAvailable())
            {
                UpgradeViaPaymentProcessorPlaceHolder.Visible = false;
                NoPaymentProcessorsPlaceHolder.Visible = true;
            }

            if(AppSettings.Payments.CashBalanceEnabled && AppSettings.Registration.AccountActivationFeeViaCashBalanceEnabled && 
               AppSettings.Registration.AccountActivationFeeVia == AccountActivationFeeVia.UserPanel)
            {
                PayViaCashBalancePlaceHolder.Visible = true;
                PayViaCashBalanceButton.Text = String.Format("{0} ({1})", U6005.PAYVIACASHBALANCE, User.CashBalance);
                PayViaCashBalanceButton.Enabled = User.CashBalance >= AppSettings.Registration.AccountActivationFee ? true : false;

                String linkToDeposit = String.Format("<a href=\"user/transfer.aspx\">{0}</a>.", U6011.HERE);
                RedirectToDepositLiteral.Text = String.Format("<p style=\"text-align:center\"> <br /> {0} <br />{1} {2}</p>  ", U6011.YOUCANPAYVIACASHBALANCE, U6011.YOUCANDEPOSITCASH, linkToDeposit);
                RedirectToDepositLiteral.Visible = !PayViaCashBalanceButton.Enabled;
            }
        }

        if (TitanFeatures.IsRofriqueWorkMines)
            UpgradeViaPaymentProcessorPlaceHolder.Visible = false;
    }

    protected void upgradeViaPaymentProcessor_Click(object sender, EventArgs e)
    {
        UpgradeViaPaymentProcessorPlaceHolder.Visible = false;
        PaymentProcessorsButtonPlaceholder.Visible = true;

        PriceLiteral.Text = AppSettings.Registration.AccountActivationFee.ToString();

        // Buy membership directly via Paypal, etc.
        var bg = new ActivateAccountButtonGenerator(User);
        PaymentButtons.Text = GenerateHTMLButtons.GetPaymentButtons(bg);
    }

    protected void payViaCashBalanceButton_Click(object sender, EventArgs e)
    {
        User.SubtractFromCashBalance(AppSettings.Registration.AccountActivationFee, "Account activation fee", BalanceLogType.AccountActivationFee);
        User.IsAccountActivationFeePaid = true;
        User.Save();

        AccountActivationFeeCrediter Crediter = (AccountActivationFeeCrediter)CrediterFactory.Acquire(User, CreditType.AccountActivationFee);
        Crediter.CreditReferer(AppSettings.Registration.AccountActivationFee);

        SuccessInfoLiteral.Text = U6011.ACTIVATIONFEEPAID;
        SuccessInfoLiteralPlaceHolder.Visible = true;
        Response.AddHeader("REFRESH", "5;URL=/user/default.aspx");
    }
    
}