using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Advertising;

public interface IAdPackObjectControl
{
    AdPacksAdvert Object { get; set; }
    void DataBind();
}