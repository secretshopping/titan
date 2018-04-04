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
using Prem.PTC.Memberships;

public partial class Controls_AdPacksAdvert : System.Web.UI.UserControl, IAdPackObjectControl
{
    public AdPacksAdvert Object { get; set; }

    #region Control specifics

    public bool IsPreview { get; set; }
    public string AdColor { get; set; }

    public bool IsActive
    {
        set { _IsActive = value; }

        get { return _IsActive; }
    }

    private bool _IsActive = true;


    #endregion

    public override void DataBind()
    {
        base.DataBind();

        if (IsActive)
            ActiveAdvertPlaceHolder.Visible = true;
        else
            InactiveAdvertPlaceHolder.Visible = true;

    }

    #region Usable properties

    public string HoverHintText
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            return sb.ToString();
        }
    }

    public Money UserEarnedMoney { get; set; }
    public Money EarningsDR { get; set; }
    public Money EarningsRR { get; set; }
    public int EarningsPoints { get; set; }

    public string Title
    {
        get
        {
            return Object.Title;
        }
    }

    public string Description
    {
        get
        {
            return Object.Description;
        }
    }

    public string Flags
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            return sb.ToString();
        }
    }


    #endregion

}
