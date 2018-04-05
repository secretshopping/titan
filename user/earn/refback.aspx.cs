using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using Resources;

public partial class About : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.EarnersRoleEnabled && 
            AppSettings.TitanFeatures.EarnRefBackEnabled && Member.CurrentInCache.IsEarner);

        DirectRefsGridView.EmptyDataText = L1.NOREFBACKDATA;
       
        UserName.Text = Member.CurrentName;
    }

    protected void DirectRefsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Create an instance of the datarow
            var rowData = e.Row.Cells[2].Text;
            e.Row.Cells[2].Text = "<a href=\"" + rowData + "\">Click</a>";


            //Check if user declared already
            int refid = Int32.Parse(e.Row.Cells[0].Text);
            var where = TableHelper.MakeDictionary("RefbackSiteId", refid);
            where.Add("Username", Member.CurrentName);

            var list = TableHelper.SelectRows<RefBackDeclarations>(where);

            if (list.Count > 0)
            {
                //Already declared
                e.Row.Cells[3].Text = L1.REFDECLARED;
            }
        }
    }

    protected void DirectRefsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "comSend")
        {
            try
            {
                AppSettings.DemoCheck();

                int index = e.GetSelectedRowIndex() % DirectRefsGridView.PageSize;

                GridViewRow row = DirectRefsGridView.Rows[index];

                var declaration = new RefBackDeclarations();
                declaration.Username = Member.CurrentName;
                declaration.RefbackSiteId = Int32.Parse(row.Cells[0].Text);
                declaration.Save();

                var site = new RefBackSites(declaration.RefbackSiteId);
                site.UsersDeclared++;
                site.Save();

                DirectRefsGridView.DataBind();
            }
            catch (MsgException ex)
            { }
        }

    }
}
