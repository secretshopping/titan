using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
public partial class user_webinars : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.WebinarsEnabled);            
        }
    }

    protected void WebinarsGridView_DataBound(object sender, EventArgs e)
    {
        WebinarsGridView.EmptyDataText = U6003.NOWEBINARS;
        WebinarsGridView.Columns[0].HeaderText = L1.TITLE;
        WebinarsGridView.Columns[1].HeaderText = L1.DESCRIPTION;
        WebinarsGridView.Columns[2].HeaderText = L1.LANGUAGE;
        WebinarsGridView.Columns[3].HeaderText = L1.TIME;
    }

    protected void WebinarsGridViewDataSource_Init(object sender, EventArgs e)
    {
        WebinarsGridViewDataSource.SelectCommand = string.Format("SELECT * FROM Webinars WHERE Status = {0} AND DateTime > GETDATE()", (int)UniversalStatus.Active);
    }
}