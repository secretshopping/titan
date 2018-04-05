using Resources;
using System;
using System.Drawing;
using Titan.ICO;
using Prem.PTC;
using Titan.Cryptocurrencies;

public partial class Controls_ICO_ICOStageInformation : System.Web.UI.UserControl
{
    private ICOStage currentStage;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            InitControls();
    }

    public void InitControls()
    {
        currentStage = ICOStage.GetCurrentStage();
        var TokenCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);

        if (currentStage == null)
        {
            StagePlaceHolder.Visible = false;
            NoStageLiteral.Text = U6012.NOSTAGEINFO;

            var nextStage = ICOStage.GetNextStage();

            if (nextStage == null)
                NextStageLiteral.Visible = false;
            else
                NextStageLiteral.Text = string.Format(U6012.NEXTSTAGEINFO, "<b>" + nextStage.StartDate + "</b>");
        }
        else
        {
            decimal ProgressBarValue = ((decimal)currentStage.GetAvailableTokens() / (decimal)currentStage.TotalAvailableTokens)  * 100;
            int availableTokens = currentStage.GetAvailableTokens();

            NoStagePlaceHolder.Visible = false;
            ProgressBarLiteral.Text = string.Format("<div class='progress-bar' style='width: {0}%'>{1}: <b>{2}</b> {3} ({0}%)</div>", ProgressBarValue.ToString("#.#"),
                U6012.TOKENSLEFT, availableTokens, TokenCryptocurrency.Code);
            NameTextBox.Text = string.Format(U6012.ISLIVE, currentStage.Name);
            
        }
    }
}