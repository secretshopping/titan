using Prem.PTC.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ExternalCpaOfferEventArgs
/// </summary>
public class ExternalCpaOfferEventArgs :EventArgs
{
    public CPAOffer CPAOffer { get; set; }
    public string LoginId { get; set; }
    public string EmailId { get; set; }
    public ExternalCpaOfferEventArgs(CPAOffer cpaOffer, string loginId, string emailId)
    {
        CPAOffer = cpaOffer;
        LoginId = loginId;
        EmailId = emailId;
    }
}