using Prem.PTC.Advertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class BannerDimensionsDictionary
{
    private Dictionary<int, List<BannerAdvert>> contents = new Dictionary<int, List<BannerAdvert>>();

    public BannerDimensionsDictionary(Dictionary<int, List<BannerAdvert>> contents)
    {
        this.contents = contents;
    }

    public List<BannerAdvert> GetAdverts(int dimensionsId)
    {
        return contents[dimensionsId];
    }

    public static BannerDimensionsDictionary Initialize()
    {
        var result = new Dictionary<int, List<BannerAdvert>>();
        var dimensions = TableHelper.SelectAllRows<BannerAdvertDimensions>();

        foreach (var dimension in dimensions)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("Status", (int)AdvertStatus.Active);
            dict.Add("BannerAdvertDimensionId", dimension.Id);
            List<BannerAdvert> adverts = TableHelper.SelectRows<BannerAdvert>(dict);

            result.Add(dimension.Id, adverts);
        }

        return new BannerDimensionsDictionary(result);
    }
}