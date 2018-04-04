using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public class ExposuresServedYesterdayCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "ExposuresServedYesterday"; } }
    protected override object GetDataFromSource()
    {
        return TableHelper.GetIntListFromString((new Prem.PTC.Statistics.Statistics(Prem.PTC.Statistics.StatisticsType.PTCClicks)).Data1)[1] +
                                TableHelper.GetIntListFromString((new Prem.PTC.Statistics.Statistics(Prem.PTC.Statistics.StatisticsType.BannerClicks)).Data1)[1];

    }
}