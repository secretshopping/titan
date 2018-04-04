using Prem.PTC.Advertising;
using Prem.PTC.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Advertising;

public interface IExternalCpaOfferControl
{
    event CpaEventHandler SubmitButtonClicked;
}

public delegate void CpaEventHandler(object sender, ExternalCpaOfferEventArgs e);