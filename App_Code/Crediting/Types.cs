using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace Titan
{
    public enum CreditType
    {
        Null = 0,

        PTC = 1,
        CPAGPTOffer = 2,
        Crowdflower = 3,
        Offerwall = 4,

        FacebookLike = 5,
        RefBack = 6,
        TweeterFollow = 7, //Not implemented
        YoutubeView = 8, //Not implemented

        TrafficGrid = 9,
        Contest = 10,

        TrafficExchange = 11,

        Upgrade = 12,

        PurchaseRSACrediter = 13,
        Forum = 14,
        AdPackCrediter = 15,

        CashBalanceDeposit = 16,

        AccountActivationFee = 17
    }

    public enum CreditAs
    {
        Null = 0,

        Points = 1, // As Int32
        MainBalance = 2 // As Money
    }

    public static class CrediterTypeHelper
    {
        /// <summary>
        /// Returns list control source with all statuses' short descriptions as text 
        /// and int status ordinal as value
        /// </summary>
        public static ListItem[] CreditAsListItems
        {
            get
            {
                var query = from CreditAs status in Enum.GetValues(typeof(CreditAs))
                            where status != CreditAs.Null
                            orderby (int)status
                            select new ListItem(status.ToString(), (int)status + "");

                return query.ToArray();
            }
        }
    }
}