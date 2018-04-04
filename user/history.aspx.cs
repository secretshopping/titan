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

public partial class About : System.Web.UI.Page
{
    public static int MaxDisplayedLogs = 200;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            HistoryGridView.DataBind();
        }
    }

    protected void HistorySqlDataSource_Init(object sender, EventArgs e)
    {
        HistorySqlDataSource.SelectCommand = string.Format("SELECT TOP {0} * FROM History WHERE AssignedUsername = '{1}' ORDER BY Date DESC", MaxDisplayedLogs, Member.CurrentName);
    }

}
