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

public partial class Controls_CryptocurrencyBalancesInfo : System.Web.UI.UserControl
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Member.IsLogged)
        {
            CryptocurrencyPanelControl.Visible = true;
            ControlTextLiteral.Text = String.Format("You have <b>{1}</b> in your {0} wallet",
                AppSettings.CryptocurrencyTrading.CryptocurrencyCode, Member.CurrentInCache.GetCryptocurrencyBalance(CryptocurrencyType.BTC));
        }
    }

}
