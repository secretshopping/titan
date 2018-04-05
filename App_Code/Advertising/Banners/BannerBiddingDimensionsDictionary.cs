using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Advertising;

public class BannerBiddingDimensionsDictionary
{
    private Dictionary<int, Dictionary<int, List<BannerAdvert>>> contents = new Dictionary<int, Dictionary<int, List<BannerAdvert>>>();

    public BannerBiddingDimensionsDictionary(Dictionary<int, Dictionary<int, List<BannerAdvert>>> contents)
    {
        this.contents = contents;
    }

    public List<BannerAdvert> GetAdverts(int dimensionsId, int position)
    {
        return contents[dimensionsId][position];
    }

    public static BannerBiddingDimensionsDictionary Initialize()
    {
        var result = new Dictionary<int, Dictionary<int, List<BannerAdvert>>>();
        var dimensions = TableHelper.SelectAllRows<BannerAdvertDimensions>();

        foreach (var dimension in dimensions)
        {
            var positionDictionary = new Dictionary<int, List<BannerAdvert>>();

            List<BannerAdvert> adverts = new List<BannerAdvert>();
            BannerAdvert current = BannerAuctionManager.GetCurrentBanner(dimension, 1);

            if (current != null)
                adverts.Add(current);

            if (adverts.Count == 0)
            {
                //Show Administrator banners
                var dict = new Dictionary<string, object>();
                dict.Add("Status", (int)AdvertStatus.Active);
                dict.Add("BannerAdvertDimensionId", dimension.Id);
                dict.Add("CreatedBy", (int)Advertiser.Creator.Admin);
                adverts = TableHelper.SelectRows<BannerAdvert>(dict);
            }

            positionDictionary[1] = adverts;

            result.Add(dimension.Id, positionDictionary);
        }

        return new BannerBiddingDimensionsDictionary(result);
    }
}