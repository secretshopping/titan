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

    public bool IsCurrentlyWatched
    {
        set { _IsCurrentlyWatched = value; }

        get { return _IsCurrentlyWatched; }
    }

    private bool _IsCurrentlyWatched = true;

    #endregion

    public override void DataBind()
    {
        base.DataBind();

        if (IsCurrentlyWatched)
            CurrentlyWatchedPlaceHolder.Visible = true;
        else
            NotCurrentlyWatchedPlaceHolder.Visible = true;
    }

    #region Usable properties
    public string Title
    {
        get
        {
            return Object.Title;
        }
    }

    public string Time
    {
        get
        {
            return AdPackManager.GetAdDisplayTime(Object) + "s";
        }
    }



    #endregion

}
