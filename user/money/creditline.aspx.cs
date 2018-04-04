using System;
using System.Linq;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Resources;

public partial class About : System.Web.UI.Page
{
    private Member user;
    private string validURL;

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.MoneyCreditLineEnabled);
        user = Member.Current;

        DataBind();
        CreditLineManager.CRON();
    }

    public override void DataBind()
    {
        Button1.Text = U5007.BORROW;
        base.DataBind();
        CreditLineLoan loan = CreditLineManager.GetUsersLoans(user.Id, false).FirstOrDefault();
        if (loan == null)
        {
            RepayBorrowDescriptionLiteral.Text = string.Format(U5007.MAXBORROW, CreditLineManager.GetMaxPossibleRequest(user).ToString());
            BorrowButton.Visible = true;
            BorrowButton.Text = U5007.BORROW;
            RepayButton.Visible = false;
            RepayPlaceHolder.Visible = false;
        }
        else
        {
            RepayPlaceHolder.Visible = true;
            
            RepayBorrowDescriptionLiteral.Text = string.Format(U5007.CANTBORROWUNTIL, loan.Loaned - loan.Repaid);
            FirstDateLiteral.Text = loan.FirstDeadline.ToShortDateString();
            SecondDateLiteral.Text = loan.SecondDeadline.ToShortDateString();
            FinalDateLiteral.Text = loan.FinalDeadline.ToShortDateString();

            FirstRepayLiteral.Text = loan.AmounBeforeFirstDeadline.ToString();
            SecondRepayLiteral.Text = loan.AmounBeforeSecondDeadline.ToString();
            FinalRepayLiteral.Text = loan.Loaned.ToString();
            RepayButton.Visible = true;
            RepayButton.Text = U5007.PAYBACK;
            BorrowButton.Visible = false;
            RepayPlaceHolder.Visible = true;
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

    protected void RepayBorrow(object sender, EventArgs e)
    {
        try
        {
            SuccMessagePanel.Visible = false;
            ErrorMessagePanel.Visible = false; 

            Money amount = Money.Parse(RepayBorrowAmountTextBox.Text);
            if (amount <= Money.Zero)
                throw new MsgException("Amount must be greater than 0.");

            int commandArgument = Int32.Parse(((Button)sender).CommandArgument);

            if (commandArgument == 1)
            {
                CreditLineManager.TrySendRequest(user, amount);
                SuccMessagePanel.Visible = true;
                SuccMessage.Text = string.Format(U5007.MONEYAFTERAPPROVAL, U6012.PURCHASEBALANCE);
            }
            else if (commandArgument == 2)
            {
                var realAmount = Money.Zero;
                CreditLineManager.TryRepayDebt(amount, user, out realAmount);
                SuccMessagePanel.Visible = true;
                SuccMessage.Text = string.Format(U5007.SUCCREPAID, realAmount.ToString());
            }

            this.DataBind();
        }
        catch(Exception ex)
        {
            if (ex is MsgException)
            {
                ErrorMessagePanel.Visible = true;
                ErrorMessage.Text = ex.Message;
            }
            else
            {
                ErrorLogger.Log(ex);
            }
        }
    }
}

   


 
