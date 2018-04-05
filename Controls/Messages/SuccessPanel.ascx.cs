using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;
using Prem.PTC.Advertising;
using Prem.PTC.Texts;

public partial class Controls_SuccessPanel : System.Web.UI.UserControl
{
    public string SuccessText { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {      
    }
    protected override void DataBind(bool raiseOnDataBinding)
    {
        base.DataBind(raiseOnDataBinding);
        SuccessLabel.Text = SuccessText;

    }

}
