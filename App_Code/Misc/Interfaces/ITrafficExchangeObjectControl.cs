using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Advertising;

public interface ITrafficExchangeObjectControl
{
    TrafficExchangeAdvert Object { get; set; }
    void DataBind();
}