using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetwork
{
    public enum RepresentativeRequestStatus
    {
        NA = 0,

        Pending = 1,
        Completed = 2,
        InDispute = 3,
        Closed =4,

        Rejected = 5,   //For withdrawals
        Cancelled = 6
    }
}