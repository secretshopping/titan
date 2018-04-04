using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Titan.InternalExchange
{
    public enum ExchangeOfferStatus
    {
        None = 0,

        Open = 1,
        Filled = 2,
        Canceled = 3,

        //Technical status. Shouldn't be save to database
        Partial = 10
    }

    public static class ExchangeOfferStatusHelper
    {
        /// <summary>
        /// Returns list control source with all statuses' short descriptions as text 
        /// and int status ordinal as value
        /// </summary>
        public static ListItem[] ListItems
        {
            get
            {
                var query = from ExchangeOfferStatus status in Enum.GetValues(typeof(ExchangeOfferStatus))
                            where status != ExchangeOfferStatus.None
                            orderby (int)status
                            select new ListItem(status.ToString(), (int)status + "");

                return query.ToArray();
            }
        }

        public static Dictionary<ExchangeOfferStatus, string> ColorDictionary = new Dictionary<ExchangeOfferStatus, string>
        {
            { ExchangeOfferStatus.Open, "bg-red" },
            { ExchangeOfferStatus.Canceled, "bg-orange" },
            { ExchangeOfferStatus.Filled, "bg-green" },

            { ExchangeOfferStatus.Partial, "bg-blue" }
        };

        public static Dictionary<ExchangeOfferStatus, Color> ForeColorDictionary = new Dictionary<ExchangeOfferStatus, Color>
        {
            { ExchangeOfferStatus.Open, Color.Red },
            { ExchangeOfferStatus.Canceled, Color.Orange },
            { ExchangeOfferStatus.Filled, Color.Green },

            { ExchangeOfferStatus.Partial, Color.Blue }
        };
    }
}