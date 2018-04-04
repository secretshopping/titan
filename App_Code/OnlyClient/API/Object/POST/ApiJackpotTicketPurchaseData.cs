using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiJackpotTicketPurchaseData
    {
        public int jackpotId { get; set; }
        public int tickets { get; set; }
        public int balance { get; set; }
    }
}