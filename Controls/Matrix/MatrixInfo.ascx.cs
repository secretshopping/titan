using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.Matrix;
using Titan.MemerModels;

public partial class Controls_Matrix_MatrixInfo : System.Web.UI.UserControl
{
    protected TimeSpan timeToShuffleMatrix;
    protected KeyValuePair<int, int> maxLevelWithChildrenCount;
    protected void Page_Load(object sender, EventArgs e)
    {

        this.Visible = AppSettings.TitanFeatures.ReferralMatrixEnabled && AppSettings.Matrix.Type == MatrixType.Campaing && !AppSettings.IsDemo;

        if (!Page.IsPostBack && AppSettings.TitanFeatures.ReferralMatrixEnabled && !AppSettings.IsDemo)
        {
            timeToShuffleMatrix = AppSettings.Matrix.LastMatrixRebuild.AddDays(AppSettings.Matrix.DaysBetweenMatrixRebuild) - AppSettings.ServerTime;
            maxLevelWithChildrenCount = MatrixBase.GetLastLevelWithChildrenCount();

            var availableCampaigns = MatrixBase.GetAvailableAdvertTypes();
            if (availableCampaigns.Count > 0)
            {
                MatrixRestrictionPlaceHolder.Visible = true;
                MatrixRestrictionLiteral.Text = string.Format(U6003.MATRIXRESTRICTION, string.Join(", ", availableCampaigns));
            }

            if (Member.IsLogged)
            {
                MyMatrixLevelPlaceHolder.Visible = true;

                if (Member.CurrentInCache.MatrixId.IsNull)
                {
                    if (availableCampaigns.Count > 0)
                    {

                        MyMatrixLevelLiteral.Text = U6003.BUYCAMPAIGNSTOJOINMATRIX + " " + string.Join(", ", availableCampaigns);
                    }
                    else
                        MyMatrixLevelPlaceHolder.Visible = false;
                }
                else
                    MyMatrixLevelLiteral.Text = U6003.MYMATRIXLEVEL + ": " + Member.CurrentInCache.MatrixId.GetLevel().ToString();

            }
        }
    }
}