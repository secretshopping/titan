using Prem.PTC.Advertising;
using Prem.PTC.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class ExternalOfferWallsManager
{
    public PublishersWebsite PublishersWebsite { get; private set; }
    public List<CPAOffer> CpaOffers { get; private set; }
    public ExternalOfferWallsManager(string host, int publishersWebsiteId, string countryCode, bool isPreview = false)
    {
        PublishersWebsite = PublishersWebsite.GetActiveWebsite(host, publishersWebsiteId, isPreview);
        CpaOffers = GetTitanOffers(countryCode, isPreview);
    }

    private List<CPAOffer> GetTitanOffers(string countryCode, bool isPreview = false)
    {
        if (isPreview)
            return TableHelper.GetListFromRawQuery<CPAOffer>(string.Format("SELECT TOP 30 * FROM CPAOffers WHERE Status = {0}", (int)AdvertStatus.Active, countryCode));

        return TableHelper.GetListFromRawQuery<CPAOffer>(string.Format("SELECT TOP 30 * FROM CPAOffers WHERE Status = {0} AND (GeolocatedCC = '' OR GeolocatedCC LIKE '%{1}%')", (int)AdvertStatus.Active, countryCode));
    }
}