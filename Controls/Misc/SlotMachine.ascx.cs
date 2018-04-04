using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.SlotMachine;

public partial class Controls_Misc_SlotMachine : System.Web.UI.UserControl
{
    public int UserChances
    {
        get
        {
            if (Member.IsLogged)
                return Member.CurrentInCache.SlotMachineChances;
            else if(Session["anonymousSlotChances"] == null)            
                Session["anonymousSlotChances"] = 1;

            return (int)Session["anonymousSlotChances"];
        }
    }

    private int points;

    protected void MainButton_Click(object sender, EventArgs e)
    {
        ErrorMessagePanel.Visible = false;
        SuccMessagePanel.Visible = false;

        if (SlotMachine.PullTheLever(out points))
        {
            SuccMessagePanel.Visible = true;
            SuccMessage.Text = L1.YOUWON + " " + points.ToString() + " " + AppSettings.Payments.CurrencyPointsName + ".";

            if (!Member.IsLogged)
            {
                SuccMessage.Text += " " + U6006.SLOTMACHINEREGISTER;
                MachinePanel.Visible = false;
            }
        }
        else
        {
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = U6006.SLOTMACHINEERROR; 
        }
    }
}