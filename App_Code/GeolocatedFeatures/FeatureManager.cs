using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Used t omanage Geolocated Features
/// </summary>
public class FeatureManager
{
    private GeolocatedFeatureType Feature;
    private String CC;

    public FeatureManager(Member user, GeolocatedFeatureType feature)
    {
        this.CC = user.CountryCode;
        this.Feature = feature;
    }

    public FeatureManager(GeolocatedFeatureType feature)
    {
        string ip = Member.GetCurrentIP(HttpContext.Current.Request);
        var ci = new CountryInformation(ip);

        this.CC = ci.CountryCode;
        this.Feature = feature;
    }


    /// <summary>
    /// Gets the boolean indicating if the feature is allowed for member or not
    /// </summary>
    public bool IsAllowed
    {
        get
        {
            int price = GeolocatedFeature.GetPrice(CC, Feature);
            if (price == -1)
                return false;
            return true;
        }
    }

    /// <summary>
    /// Gets reward in Points for completing this feature. Returns -1 when
    /// feature is not allowed for this member
    /// </summary>
    public int Reward
    {
        get
        {
            return GeolocatedFeature.GetPrice(CC, Feature);
        }
    }
}