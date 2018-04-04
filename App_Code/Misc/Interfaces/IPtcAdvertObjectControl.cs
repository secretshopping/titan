using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Advertising;

public interface IPtcAdvertObjectControl
{
    PtcAdvert Object { get; set; }
    bool IsPreview { get; set; }
    void DataBind();
}