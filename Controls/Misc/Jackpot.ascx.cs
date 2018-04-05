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
using Prem.PTC.Contests;
using Prem.PTC.Memberships;
using Prem.PTC.Payments;

public partial class Controls_Jackpot : System.Web.UI.UserControl, ICustomObjectControl
{
    public int ObjectID { get; set; }
    public bool IsHistory { get; set; }
    public Jackpot Jackpot;
    public List<JackpotTicket> TicketList;
    Member user;

    protected void Page_Load(object sender, EventArgs e)
    {
        var purchaseOption = PurchaseOption.Get(PurchaseOption.Features.Jackpot);
        
        BuyTicketsFromAdBalancePlaceHolder.Visible = purchaseOption.PurchaseBalanceEnabled;
        BuyTicketsFromCashBalancePlaceHolder.Visible = purchaseOption.CashBalanceEnabled;
        BuyTicketsViaPaymentProcessorPlaceHolder.Visible = purchaseOption.PaymentProcessorEnabled;
        
    }

    public override void DataBind()
    {
        base.DataBind();
        user = Member.CurrentInCache;
        Jackpot = new Jackpot(ObjectID);

        if (IsHistory)
        {
            ParticipantsLiteral.Text = Jackpot.NumberOfParticipants.ToString();
            TicketsLiteral.Text = Jackpot.NumberOfTickets.ToString();
            BuyTicketsPlaceholder.Visible = false;
            UsersTicketsPlaceholder.Visible = false;
            HistoryPlaceholder.Visible = true;

            var winners = Jackpot.GetDistinctUserWinnerIds();
            var winningTickets = Jackpot.GetWinningTicketNumbers();

            if (winners.Count > 0 && winningTickets.Count > 0)
            {
                var sb = new StringBuilder();

                for (int i = 0; i < winners.Count; i++)
                {
                    string name = new Member((int)winners.ElementAt(i)).Name;
                    sb.Append(name);
                    
                    if (i != winners.Count - 1)
                        sb.Append(", ");
                }

                WinnerLiteral.Text = sb.ToString();
                sb.Clear();

                for (int i = 0; i < winningTickets.Count; i++)
                {
                    string ticket = "#" + winningTickets.ElementAt(i).ToString();
                    sb.Append(ticket);

                    if (i != winningTickets.Count - 1)
                        sb.Append(", ");
                }

                WinningTicketLiteral.Text = sb.ToString();
            }
            else
            {
                WinnerLiteral.Text = "-";
                WinningTicketLiteral.Text = "-";
            }
        }
        else
        {
            UsersTicketsPlaceholder.Visible = true;
            ParticipantsLiteral.Text = JackpotManager.GetNumberOfParticipants(Jackpot.Id).ToString();
            TicketsLiteral.Text = JackpotManager.GetNumberOfTickets(Jackpot.Id).ToString();
            BuyTicketsPlaceholder.Visible = true;
            HistoryPlaceholder.Visible = false;
            TicketList = JackpotManager.GetUsersTickets(Jackpot.Id, user.Id);
            UsersTicketsLiteral.Text = string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (var ticket in TicketList)
            {
                sb.Append(string.Format("#{0}, ", ticket.Id.ToString()));
            }
            //remove comma
            if (sb.Length > 2)
                sb.Remove(sb.Length - 2, 2);
            else
                sb.Append("-");
            UsersTicketsLiteral.Text = sb.ToString();

            LangAdder.Add(BuyTicketsFromAdBalanceButton, U6012.PAYVIAPURCHASEBALANCE);
            LangAdder.Add(BuyTicketsFromCashBalanceButton, U6005.PAYVIACASHBALANCE);
            LangAdder.Add(BuyTicketsViaPaymentProcessor, U6005.PAYVIAPAYMENTPROCESSOR);

            NumberOfTicketsLiteral.Text = TicketList.Count.ToString();
        }

        HidePrize();
    }

    private void HidePrize()
    {
        if (IsHistory)
        {
            adBalancePrize.Visible = mainBalancePrize.Visible = loginAdsCreditsPrize.Visible = upgradePrize.Visible = false;
            JackpotTicketPrize ticketPrize = TableHelper.GetListFromRawQuery<JackpotTicketPrize>(string.Format("SELECT * FROM JackpotTicketPrizes WHERE JackpotId = {0}", Jackpot.Id)).FirstOrDefault();

            if (ticketPrize != null)
            {
                switch (ticketPrize.PrizeType)
                {
                    case JackpotPrize.AdBalance:
                        adBalancePrize.Visible = true;
                        break;
                    case JackpotPrize.MainBalance:
                        mainBalancePrize.Visible = true;
                        break;

                    case JackpotPrize.LoginAdsCredits:
                        loginAdsCreditsPrize.Visible = true;
                        break;
                    case JackpotPrize.Upgrade:
                        upgradePrize.Visible = true;
                        break;
                }
            }
        }
        else
        {
            adBalancePrize.Visible = Jackpot.AdBalancePrizeEnabled;
            mainBalancePrize.Visible = Jackpot.MainBalancePrizeEnabled;
            loginAdsCreditsPrize.Visible = Jackpot.LoginAdsCreditsPrizeEnabled;
            bool upgradeVisible = false;
            if (Jackpot.UpgradePrizeEnabled)
            {
                var membership = new Membership(Jackpot.UpgradeIdPrize);
                upgradeVisible = true;
                UpgradeDetails.Text = membership.Name + " (" + Jackpot.UpgradeDaysPrize + " " + L1.DAYS + ")";

            }
            upgradePrize.Visible = upgradeVisible;

        }
    }

    public void BuyTicketsViaAdBalance_Click(object sender, EventArgs e)
    {
        BuyTickets(BalanceType.PurchaseBalance);
    }

    public void BuyTicketsViaCashBalance_Click(object sender, EventArgs e)
    {
        BuyTickets(BalanceType.CashBalance);
    }

    protected void BuyTicketsViaPaymentProcessor_Click(object sender, EventArgs e)
    {
        EPanel.Visible = SPanel.Visible = false;
        try
        {
            int numberOfTickets;
            if (!Int32.TryParse(NumberOfTicketsTextBox.Text, out numberOfTickets) || numberOfTickets <= 0)
                throw new MsgException(U5003.INVALIDNUMBEROFTICKETS);
            
            Member User = Member.Current;

            PaymentProcessorsButtonPlaceholder.Visible = true;
            BuyTicketsPlaceholder.Visible = false;

            Money PackPrice = Jackpot.TicketPrice * numberOfTickets;

            // Buy tickets directly via Paypal, etc.
            var bg = new BuyJackpotTicketsButtonGenerator(User, Jackpot, PackPrice, numberOfTickets);
            PaymentButtons.Text = GenerateHTMLButtons.GetPaymentButtons(bg);
        }
        catch (Exception ex)
        {
            if (ex is MsgException)
            {
                EPanel.Visible = true;
                EText.Text = ex.Message;
            }
            else
                ErrorLogger.Log(ex);
        }
    }

    public void BuyTickets(BalanceType purchaseBalanceType)
    {
        EPanel.Visible = SPanel.Visible = false;

        try
        {
            int numberOfTickets;
            if (!Int32.TryParse(NumberOfTicketsTextBox.Text, out numberOfTickets) || numberOfTickets <= 0)
                throw new MsgException(U5003.INVALIDNUMBEROFTICKETS);

            user = Member.Current;

            JackpotManager.BuyTickets(Jackpot, user, numberOfTickets, purchaseBalanceType);
            NumberOfTicketsTextBox.Text = string.Empty;
            SPanel.Visible = true;
            SText.Text = U5003.TICKETPURCHASESUCCESS.Replace("%n%", numberOfTickets.ToString());
            this.DataBind();
        }
        catch (Exception ex)
        {
            if (ex is MsgException)
            {
                EPanel.Visible = true;
                EText.Text = ex.Message;
            }
            else
                ErrorLogger.Log(ex);
        }
    }
}
