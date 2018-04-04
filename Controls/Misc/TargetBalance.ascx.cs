using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Web.UI.WebControls;

public partial class Controls_TargetBalance : System.Web.UI.UserControl
{
    private string feature;
    public string Feature
    {
        get { return feature; }
        set
        {
            if (!Enum.IsDefined(typeof(PurchaseOption.Features), value))
                throw new ArgumentException("Feature is not present in the Features enum");
            feature = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Visible = Member.IsLogged;
    }

    public PurchaseBalances TargetBalance
    {
        get { return (PurchaseBalances)Enum.Parse(typeof(PurchaseBalances), TargetBalances.SelectedValue, true); }
    }

    protected void TargetBalances_Init(object sender, EventArgs e)
    {
        PurchaseOption.Features targetFeature = (PurchaseOption.Features)Enum.Parse(typeof(PurchaseOption.Features), Feature, true);
        var purchaseOption = PurchaseOption.Get(targetFeature);

        if (purchaseOption.PurchaseBalanceEnabled)
            TargetBalances.Items.Add(new ListItem(U6012.PURCHASEBALANCE, PurchaseBalances.Purchase.ToString()));
        if (AppSettings.Payments.CashBalanceEnabled && purchaseOption.CashBalanceEnabled)
            TargetBalances.Items.Add(new ListItem(U5008.CASHBALANCE, PurchaseBalances.Cash.ToString()));
        TargetBalances.SelectedIndex = 0;
    }
}