using Prem.PTC;
using Resources;
using System;
using System.Drawing;
using Titan.ICO;

public partial class Controls_ICO_ICOStage : System.Web.UI.UserControl, ICustomObjectControl
{
    public int ObjectID { get; set; }

    public DateTime StageStart { get; set; }

    public DateTime StageEnd { get; set; }

    private ICOStage currentStage;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            InitControls();
    }

    public void InitControls()
    {
        currentStage = new ICOStage(ObjectID);

        int TotalPurchasedTokens = currentStage.GetAvailableTokens();
        int AvailableTokens = currentStage.TotalAvailableTokens - TotalPurchasedTokens;
        Decimal PercentOfTokens = ((Decimal)TotalPurchasedTokens / (Decimal)currentStage.TotalAvailableTokens) * 100;

        ICOStageName.Text = currentStage.Name;
        ICOStagePriceLiteral.Text = currentStage.TokenPrice.ToString();
        ICOStageAvailableTokensPercentLiteral.Text = string.Format("<input type='text' class='knob' value='{0}' data-width='125' data-height='125' data-thickness='0.25' data-fgColor='#65a858'>", Math.Round(PercentOfTokens, 0).ToString());
        ICOStageAvailableTokensLiteral.Text = (currentStage.TotalAvailableTokens - AvailableTokens).ToString();

        TokenImage.ImageUrl = AppSettings.Ethereum.ERC20TokenImageUrl;

        StageStart = currentStage.StartDate;
        StageEnd = currentStage.EndDate;

        if (StageEnd < AppSettings.ServerTime)
        {
            ICOStageEventLiteral.Text = L1.FINISHED;
            ICOStageItem.Attributes.Add("class", "ICOStage finished");
        }
        else if (StageStart > AppSettings.ServerTime)
        {
            ICOStageTimestamp.Attributes.Add("data-timestamp", StageStart.ToString());
            ICOStageEventLiteral.Text = String.Format("{0} <span class='countdown-placeholder'></span>", U6012.STARTSIN);
            ICOStageItem.Attributes.Add("class", "ICOStage finished ICOtimer");
        }
        else
        {
            ICOStageTimestamp.Attributes.Add("data-timestamp", StageEnd.ToString());
            ICOStageEventLiteral.Text = String.Format("{0} <span class='countdown-placeholder'></span>", U6012.ENDSIN);
            ICOStageItem.Attributes.Add("class", "ICOStage ICOtimer");
        }
    }
}