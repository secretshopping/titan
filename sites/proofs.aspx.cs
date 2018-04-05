using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Resources;

public partial class sites_proofs : TitanPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ProofsPageLoad();

        if (TitanFeatures.IsAhmed)
            PaymentProofsAdditionalInfoPlaceHolder.Visible = false;
    }

    protected void ProofsPageLoad()
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.PaymentProofsEnabled);
        PayoutProofsGridView.EmptyDataText = L1.NOPROOFS;
    }

    protected void PayoutProofsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            PaymentProof proof = new PaymentProof(Convert.ToInt32(e.Row.Cells[0].Text));
            //Country
            int userId = Convert.ToInt32(e.Row.Cells[2].Text);
            Member User = new Member(userId);
            e.Row.Cells[2].Text = "<img src=\"Images/Flags/" + User.CountryCode.ToLower() + ".png\" class=\"imagemiddle\" /> " + User.Country;

            //Type
            PaymentType type = (PaymentType)Convert.ToInt32(e.Row.Cells[3].Text);
            if (type == PaymentType.Instant)
                e.Row.Cells[3].Text = L1.INSTANT;
            else if (type == PaymentType.Manual)
                e.Row.Cells[3].Text = L1.MANUAL;

            //Money
            e.Row.Cells[4].Text = Money.AddCurrencySign(e.Row.Cells[4].Text);

            //Via
            PaymentProcessor proc = (PaymentProcessor)Convert.ToInt32(e.Row.Cells[5].Text);

            if (proc == PaymentProcessor.CustomPayoutProcessor)
                e.Row.Cells[5].Text = proof.ProcessorName;
            else
                e.Row.Cells[5].Text = proc.ToString();
        }
    }
}