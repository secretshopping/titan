using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Controls_FeatureUnavailable : System.Web.UI.UserControl
{
    public string HeaderText { get; set; }
    public string Reason { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
    }
}