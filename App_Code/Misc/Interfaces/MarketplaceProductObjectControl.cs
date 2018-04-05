using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Advertising;

namespace Titan.Marketplace
{
    public interface MarketplaceProductObjectControl
    {
        MarketplaceProduct Object { get; set; }
        bool IsPreview { get; set; }
        void DataBind();
    }
}