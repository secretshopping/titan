using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan.Publish.PTCOfferWalls;

public partial class Controls_Advertisements_PtcOfferWall : System.Web.UI.UserControl
{
    public PTCOfferWall OfferWall { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public string HoverHintText
    {
        get
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(L1.THISADLASTS + " ");
            sb.Append(OfferWall.DisplayTime * OfferWall.Adverts);
            sb.Append(" " + L1.SECONDS);
            return sb.ToString();
        }
    }
}