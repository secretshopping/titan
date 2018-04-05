using System;
using System.Linq;
using System.Data;
using System.Web.UI.WebControls;

namespace Prem.PTC.Offers
{
    /// <summary>
    /// Summary description for OfferStatus
    /// </summary>
    public enum OfferStatus
    {
        Null = 0,

        Active = 1,
        Completed = 2,
        Denied = 3,
        Ignored = 4,
        Pending = 5,
        Reported = 6,
        UnderReview = 7
    }

    public static class OfferStatusHelper
    {
        /// <summary>
        /// Returns list control source with all statuses' short descriptions as text 
        /// and int status ordinal as value
        /// </summary>
        public static ListItem[] ListItems
        {
            get
            {
                var query = from OfferStatus status in Enum.GetValues(typeof(OfferStatus))
                            where (status != OfferStatus.Null && status != OfferStatus.Reported && status != OfferStatus.Ignored && status != OfferStatus.Active)
                            orderby (int)status
                            select new ListItem(status.ShortDescription(), (int)status + "");

                return query.ToArray();
            }
        }
    }

    public static class OfferStatusExtensions
    {
        /// <summary>
        /// Provides human readable, short description for each status.
        /// </summary>
        public static string ShortDescription(this OfferStatus status)
        {
            if (status == OfferStatus.Null) return "Unknown";

            return Enum.GetName(typeof(OfferStatus), status);
        }
    }
}