using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.Advertising
{
    public enum BannerAuctionStatus
    {
        Opened = 0,
        Closed = 1,
        OnDisplay = 2,
        Ended = 3,
        EndedToRemove = 4
    }
}