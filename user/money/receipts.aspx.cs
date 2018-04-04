using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using System.Data;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using System.Text;
using System.Threading;
using MarchewkaOne.Titan.Balances;

public partial class Receipts : System.Web.UI.Page
{
 
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.MoneyReceiptsEnabled);

        if (!IsPostBack)
        {
            HistoryGridView.EmptyDataText = L1.NODATA;
        }
    }


    protected void HistorySqlDataSource_Init(object sender, EventArgs e)
    {
        var itemType = PurchasedItemType.Transfer;
        if (TitanFeatures.isAri)
            itemType = PurchasedItemType.AdPack;

        HistorySqlDataSource.SelectCommand = string.Format(@"SELECT * FROM PurchasedItems 
                                                            WHERE UserId = {0} 
                                                            AND ItemType = {1}
                                                            ORDER BY DateAdded DESC", Member.CurrentId, (int)itemType);
    }

    protected void HistoryGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "download")
        {
            int index = Convert.ToInt32(e.CommandArgument) % HistoryGridView.PageSize;
            GridViewRow row = HistoryGridView.Rows[index];
            int itemId = Convert.ToInt32((row.Cells[0].Text));

            var item = new PurchasedItem(itemId);
            HtmlInvoiceGenerator generator = new HtmlInvoiceGenerator(item);
            generator.DownloadPdf();
        }
    }

    protected void HistoryGridView_DataBound(object sender, EventArgs e)
    {
        HistoryGridView.Columns[0].HeaderText = U6005.INVOICE + "#";
        HistoryGridView.Columns[3].HeaderText = U6005.QUANTITY;
        HistoryGridView.Columns[4].HeaderText = U5006.UNITPRICE;
        HistoryGridView.Columns[5].HeaderText = U6005.TAX;
        HistoryGridView.Columns[6].HeaderText = U5001.TOTAL;
    }
}
