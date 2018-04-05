using Prem.PTC;
using Resources;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Titan.ICO;

public partial class user_ico_info : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.ICOStagesEnabled);

        if (!IsPostBack)
        {
            List<ICOStage> AllStagesList = new List<ICOStage>();

            AllStagesList = ICOStage.GetAllNoDeleted();

            if (AllStagesList.Count == 0)
            {
                NoStageLiteral.Text = L1.NODATA;
                NoStagePlaceHolder.Visible = true;
            }
            else
            {
                for(int i=0; i<AllStagesList.Count; i++)
                    AllStagesLiteral.Controls.Add(GetStageHTML(AllStagesList[i]));
            }
        }
    }

    protected UserControl GetStageHTML(ICOStage stage)
    {
        UserControl objControl = (UserControl)Page.LoadControl("~/Controls/ICO/ICOStage.ascx");
        var parsedControl = objControl as ICustomObjectControl;

        parsedControl.ObjectID = stage.Id;
        parsedControl.DataBind();

        return objControl;
    }
}