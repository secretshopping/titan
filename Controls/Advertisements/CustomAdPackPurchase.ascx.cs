using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.Cryptocurrencies;

public partial class Controls_Advertisements_CustomAdPackPurchase : System.Web.UI.UserControl
{
    public Cryptocurrency TokenCryptocurrency { get; set; }
    public Cryptocurrency ERC20TokenCryptocurrency { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        TokenCryptocurrency = CryptocurrencyFactory.Get<BitcoinCryptocurrency>();
        ERC20TokenCryptocurrency = CryptocurrencyFactory.Get<ERC20TokenCryptocurrency>();


        if (!IsPostBack)
        {
            HideMessages();

            var PurchaseOptions = PurchaseOption.Get(PurchaseOption.Features.AdPack);

            BindDataToTypesDDL();
            CustomCampaignsDropDownManagement();

            CustomPurchaseViaTokenPlaceHolder.Visible = false;
            CustomPurchaseViaMainBalancePlaceHolder.Visible = TitanFeatures.IsTrafficThunder;
            CustomPurchaseViaCommissionBalancePlaceHolder.Visible = AppSettings.RevShare.AdPackPurchasesViaCommissionBalanceEnabled;
            CustomPurchaseViaPurchaseBalancePlaceHolder.Visible = PurchaseOptions.PurchaseBalanceEnabled && !TitanFeatures.PurchaseBalanceDisabled;
            CustomPurchaseViaCashBalancePlaceHolder.Visible = PurchaseOptions.CashBalanceEnabled;
            CustomPurchaseForReferralPlaceHolder.Visible = AppSettings.RevShare.AdPack.BuyAdPacksForReferralsEnabled;

            CustomPurchaseViaERC20TokensButton.Enabled = false;
            CustomPurchaseViaCommissionBalanceButton.Enabled = false;
            CustomPurchaseViaCashBalanceButton.Enabled = false;
            CustomPurchaseViaPurchaseBalanceButton.Enabled = false;
            CustomPurchaseViaMainBalanceButton.Enabled = false;

            TOSAgreement.Checked = false;

            if (TitanFeatures.IsTrafficThunder)
            {
                CustomTypesDDLPlaceHolder.Visible = false;
            }

            LangAdders();
        }

        var adPackType = new AdPackType(Convert.ToInt32(CustomTypesDropDown.SelectedValue));
        CustomPackPriceLabel.Text = adPackType.Price.ToClearString();

        ScriptManager.RegisterStartupScript(CustomAdPackPurchaseUpdatePanel, GetType(), "AdPacksTBChanged", "AdPacksTBChanged();", true);
    }

    private void LangAdders()
    {
        AdPacksToBuyTextBox.Text = "1";
        CustomTypesLabel.Text = "AdPack types";

        LangAdder.Add(CustomPurchaseViaPurchaseBalanceButton, U6012.PAYVIAPURCHASEBALANCE);
        LangAdder.Add(CustomPurchaseViaERC20TokensButton, String.Format(U6012.PAYVIAWALLET, TokenCryptocurrency.Code));
        LangAdder.Add(TOSAgreement, U6012.UNDERSTANDANDAGREE);
        LangAdder.Add(CustomAdPacksBuyTitle, String.Format("{0} {1}", L1.BUY, AppSettings.RevShare.AdPack.AdPackName).ToUpper());
        LangAdder.Add(CustomPurchaseViaCommissionBalanceButton, U6012.PAYVIACOMMISSIONBALANCE);
        LangAdder.Add(CustomPurchaseViaCashBalanceButton, U6005.PAYVIACASHBALANCE);
        LangAdder.Add(CustomBuyForReferralCheckBox, U5008.BUYFORREFERRAL);

        if (TitanFeatures.IsTrafficThunder)
            LangAdder.Add(CustomAdPacksBuyTitle, "Qualify for Airdrop reward");
    }

    protected void CustomTypesDropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
        CustomPackPriceLabel.Text = new AdPackType(Convert.ToInt32(CustomTypesDropDown.SelectedValue)).Price.ToClearString();
    }

    protected void CustomCampaignsDropDownManagement()
    {
        int availableAdverts = AdPackManager.GetUsersAdverts(Member.CurrentId).Count;

        if (availableAdverts > 0)
        {
            SpaceDivPlaceHolder.Visible = false;
            CustomCampaignDDLPlaceHolder.Visible = true;
            CustomCampaignLabel.Text = "Campaigns";

            var list = new Dictionary<string, string>();
            var campaigns = AdPackManager.GetUsersAdverts(Member.CurrentId);

            for (int i = 0; i < campaigns.Count; i++)
            {
                list.Add(campaigns[i].Id.ToString(), campaigns[i].Title);
            }

            CustomCampaignsDropDown.DataSource = list;
            CustomCampaignsDropDown.DataTextField = "Value";
            CustomCampaignsDropDown.DataValueField = "Key";
            CustomCampaignsDropDown.DataBind();
        }
        else
        {
            SpaceDivPlaceHolder.Visible = true;
            CustomCampaignDDLPlaceHolder.Visible = false;
        }
    }

    private void BindDataToTypesDDL()
    {
        CustomTypesDropDown.Items.Clear();

        var availableTypes = AdPackTypeManager.GetAllActiveTypesForUser(Member.CurrentInCache);

        for (int i = 0; i < availableTypes.Count; i++)
        {
            string itemValue = availableTypes[i].Id.ToString();
            string itemString = availableTypes[i].Name;

            ListItem item = new ListItem(itemString, itemValue);

            CustomTypesDropDown.Items.Insert(i, item);
            CustomTypesDropDown.SelectedIndex = 0;
        }

        if (availableTypes.Count > 0)
            CustomTypesDropDown.Attributes.Add("style", string.Format("background-color:{0};color:white", availableTypes[0].Color));
    }

    protected void TOSAgreement_CheckedChanged(object sender, EventArgs e)
    {
        CustomPurchaseViaERC20TokensButton.Enabled = TOSAgreement.Checked;
        CustomPurchaseViaPurchaseBalanceButton.Enabled = TOSAgreement.Checked;
        CustomPurchaseViaCommissionBalanceButton.Enabled = TOSAgreement.Checked;
        CustomPurchaseViaCashBalanceButton.Enabled = TOSAgreement.Checked;
        CustomPurchaseViaMainBalanceButton.Enabled = TOSAgreement.Checked;
    }

    private int AdvertId;
    private int NumberOfPacks;
    private AdPackType AdPackType;
    private Member AdPackOwner;
    private int? AdPackOwnerId;
    private void TryPrepareDataForAdPackPurchase()
    {
        AdvertId = -1;
        NumberOfPacks = 0;
        AdPackType = null;
        AdPackOwner = null;
        AdPackOwnerId = null;

        //Get Advert Id
        try
        {
            AdvertId = Convert.ToInt32(CustomCampaignsDropDown.SelectedValue);
        }
        catch (Exception ex) { }

        //Get number of AdPacks 
        try
        {
            Money CostOfPack = Money.Parse(CustomPackPriceLabel.Text);
            NumberOfPacks = (int)(Decimal.Parse((MoneyToInvestTextBox.Text)) / CostOfPack.ToDecimal());
        }
        catch (Exception ex)
        {
            throw new MsgException(U4000.BADFORMAT);
        }

        //Get AdPack type
        AdPackType = new AdPackType(Convert.ToInt32(CustomTypesDropDown.SelectedValue));


        //Get referral for AdPack
        if (AppSettings.RevShare.AdPack.BuyAdPacksForReferralsEnabled && CustomBuyForReferralCheckBox.Checked && CustomReferralsDropDownList.Items.Count > 0)
            AdPackOwnerId = Convert.ToInt32(CustomReferralsDropDownList.SelectedValue);

        if (AdPackOwnerId.HasValue)
        {
            ValidateAdPacksForReferrals();
            AdPackOwner = new Member(AdPackOwnerId.Value);
        }
    }

    protected void PurchaseButtonViaPurchaseBalance_Click(object sender, EventArgs e)
    {
        ProceedAdPackPurchase(BalanceType.PurchaseBalance);
    }

    protected void PurchaseButtonViaERC20Tokens_Click(object sender, EventArgs e)
    {
        ProceedAdPackPurchase(BalanceType.BTC);
    }

    protected void PurchaseButtonViaCommissionBalance_Click(object sender, EventArgs e)
    {
        ProceedAdPackPurchase(BalanceType.CommissionBalance);
    }

    protected void PurchaseButtonViaCashBalance_Click(object sender, EventArgs e)
    {
        ProceedAdPackPurchase(BalanceType.CashBalance);
    }

    protected void CustomPurchaseViaMainBalanceButton_Click(object sender, EventArgs e)
    {
        ProceedAdPackPurchase(BalanceType.MainBalance);
    }

    protected void CancelLendingButton_Click(object sender, EventArgs e)
    {
        HideMessages();
        CheckIfRedirectAfterAction();
        Hide();
    }

    private void CheckIfRedirectAfterAction()
    {
        if (Request.QueryString["BackAfter"] != null)
            Response.Redirect(ViewState["PreviousPageUrl"].ToString());
    }

    protected void CustomBuyForReferralCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (CustomBuyForReferralCheckBox.Checked)
            BindDataToReferralsDDL();

        CustomReferralsPlaceHolder.Visible = CustomBuyForReferralCheckBox.Checked;
    }

    private void BindDataToReferralsDDL()
    {
        CustomReferralsDropDownList.Items.Clear();

        var referrals = Member.CurrentInCache.GetDirectReferralsList();
        var boughtAdPacks = AdPacksForOtherUsers.GetListByBuyer(Member.CurrentId);

        for (int i = 0; i < referrals.Count; i++)
        {
            string itemValue = referrals[i].Id.ToString();

            int boughtForUser = 0;
            var adPacksForUser = boughtAdPacks.FirstOrDefault(x => x.OwnerId == referrals[i].Id);
            if (adPacksForUser != null)
                boughtForUser = adPacksForUser.Count;
            string itemString = string.Format("{0} (max {1})", referrals[i].Name, AppSettings.RevShare.AdPack.MaxAdPacksForOtherUser - boughtForUser);

            ListItem item = new ListItem(itemString, itemValue);
            CustomReferralsDropDownList.Items.Insert(i, item);
        }
        CustomReferralsDropDownList.DataBind();
    }

    private void ValidateAdPacksForReferrals()
    {
        var boughtAdPacks = AdPacksForOtherUsers.GetListByBuyer(Member.CurrentId);
        var targetUserId = Convert.ToInt32(CustomReferralsDropDownList.SelectedValue);
        var adPacksForUser = boughtAdPacks.FirstOrDefault(x => x.OwnerId == targetUserId);
        var maxAdPacks = AppSettings.RevShare.AdPack.MaxAdPacksForOtherUser;

        if (adPacksForUser != null)
            maxAdPacks = maxAdPacks - adPacksForUser.Count;

        if (NumberOfPacks > maxAdPacks)
            throw new MsgException(string.Format(U5008.CANTBUYADPACKFORTHISUSER, maxAdPacks, AppSettings.RevShare.AdPack.AdPackNamePlural));
    }

    protected void ProceedAdPackPurchase(BalanceType fromBalance)
    {
        HideMessages();

        try
        {
            if (TokenCryptocurrency.WalletEnabled && TOSAgreement.Checked == false)
                throw new MsgException("Terms of Service are not agreed");

            TryPrepareDataForAdPackPurchase();
            AdPackManager.TryBuyAdPack(fromBalance, AdvertId, AdPackType, NumberOfPacks, adPackOwner: AdPackOwner);

            if (NumberOfPacks == 1)
                CustomSuccessLiteral.Text = U4200.BUYADPACKSUCCESS.Replace("%n%", NumberOfPacks.ToString()).Replace("%p%", AppSettings.RevShare.AdPack.AdPackName) + ".";
            else
                CustomSuccessLiteral.Text = U4200.BUYADPACKSUCCESS.Replace("%n%", NumberOfPacks.ToString()).Replace("%p%", AppSettings.RevShare.AdPack.AdPackNamePlural) + ".";

            CustomSuccessPanel.Visible = true;
            CustomMessagesPlaceHolder.Visible = true;

            //Set up date when buy adpack to count number of active adpack (Customization)
            Member.CurrentInCache.FirstActiveDayOfAdPacks = DateTime.Now;

            BindDataToTypesDDL();
            BindDataToReferralsDDL();
        }
        catch (MsgException ex)
        {
            CustomErrorLiteral.Text = ex.Message;
            CustomErrorPanel.Visible = true;
            CustomMessagesPlaceHolder.Visible = true;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    protected void HideMessages()
    {
        CustomSuccessPanel.Visible = false;
        CustomErrorPanel.Visible = false;
        CustomMessagesPlaceHolder.Visible = false;
    }

    public void Show()
    {
        MainPopUpContent.Visible = true;
    }

    public void Hide()
    {
        MainPopUpContent.Visible = false;
    }
}