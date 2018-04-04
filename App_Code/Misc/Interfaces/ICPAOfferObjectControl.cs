using Prem.PTC.Advertising;
using Prem.PTC.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Advertising;

public interface ICPAOfferObjectControl
{
    CPAOffer Object { get; set; }
    string Text { get; set; }
    bool ShowReturnButton { get; set; }
    string Sender { get; set; }

    void DataBind();
}