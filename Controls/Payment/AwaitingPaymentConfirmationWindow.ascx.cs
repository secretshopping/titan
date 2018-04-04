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
using Titan.Cryptocurrencies;

public partial class Controls_AwaitingPaymentConfirmationWindow : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Visible = false;
        var BtcCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.BTC);

        if (Member.IsLogged && BtcCryptocurrency.DepositEnabled)
        {
            var pendingBtcDeposit = CompletedPaymentLog.GetPendingBTCDeposits(Member.CurrentId);

            if (pendingBtcDeposit != null)
            {
                this.Visible = true;
                MessageLiteral.Text =
                    String.Format(U6011.AWAITINGBTCCONFIRMATION, "<b>" + BtcCryptocurrency.DepositMinimumConfirmations + "</b> ",
                    "<b>" + pendingBtcDeposit.TransactionId + "</b>");
            }
        }
    }
}
