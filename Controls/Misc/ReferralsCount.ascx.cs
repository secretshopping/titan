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

public partial class Controls_ReferralsCount : System.Web.UI.UserControl
{

    public Member User { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            DataBind();
    }

    public override void DataBind()
    {
        User = Member.CurrentInCache;
        string limit = U4200.UNLIMITED.ToLower();
        string count = User.GetDirectReferralsCount().ToString(); ;
        if (User != null)
        {
            if (User.DirectReferralLimit < 1000000000)
                limit = User.DirectReferralLimit.ToString();

            if (!TitanFeatures.IsTrafficThunder)
                CurrentRefsLiteral.Text = string.Format(U5007.CURRENTREFS, count, limit);
            else
                CurrentRefsLiteral.Text = String.Empty;

            base.DataBind();
        }
    }

}
