using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Web.UI.WebControls;
using Titan.Cryptocurrencies;
using Titan.ICO;

public partial class user_ico_history : System.Web.UI.Page
{
    public Cryptocurrency TokenCryptocurrency { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.ICOHistoryEnabled);
        TokenCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);

        if (!IsPostBack)
        {
            MainDescriptionLiteral.Text = String.Format(U6012.PURCHASEHISTORY, TokenCryptocurrency.Code);
            PurchasesGridView.EmptyDataText = L1.NODATA;
            PurchasesGridView.DataBind();
        }
    }

    protected void PurchasesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var stageId = Convert.ToInt32(e.Row.Cells[1].Text);
            var stage = new ICOStage(stageId);
            e.Row.Cells[1].Text = stage.Name;

            var tokens = Convert.ToInt32(e.Row.Cells[3].Text);
            e.Row.Cells[3].Text = string.Format("{0} {1}", tokens, TokenCryptocurrency.Code);

        }
    }

    protected void PurchasesGridViewSqlDataSource_Init(object sender, EventArgs e)
    {
        string command = string.Format("SELECT * FROM {0} WHERE UserId = {1} ORDER BY {2} DESC",
            ICOPurchase.TableName, Member.CurrentInCache.Id, ICOPurchase.Columns.PurchaseTime);

        PurchasesGridViewSqlDataSource.SelectCommand = command;
    }   
}