using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Contests
{
    public enum ContestType
    {
        Null = 0,
        Direct = 1,
        Rented = 2,
        Click = 3,
        Transfer = 4,
        Offerwalls = 5,
        CrowdFlower = 6,
        Forum = 7,
        Offer = 8
    }

    public enum PrizeType
    {
        Null = 0,
        MainBalance = 1,
        PurchaseBalance = 2,
        RentalBalance = 3,
        Points = 4,
        DirectRefLimit = 5,
        RentedReferral = 6,
        CustomPrize = 7
    }

    public enum ContestStatus
    {
        Null = 0,
        Active = 1,
        Paused = 2,
        Finished = 3,
        Deleted = 4
    }
}