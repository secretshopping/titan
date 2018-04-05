using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public enum GroupPolicy
{
    Classic = 1,
    AutomaticGroups = 2,
    CustomGroups = 3,
    AutomaticAndCustomGroups = 4
}

public enum CustomReturnOption
{
    Accelerate = 1,
    Increase = 2
}

public enum DistributionPolicy
{
    Pools = 1,
    Fixed = 2
}

public enum CustomGroupStatus
{
    InProgress = 1, //AdPacksAdded < AdPackLimit
    Active = 2, //AdPacksAdded = AdPacksLimit AND MoneyToReturn > MoneyReturned
    Expired = 3 //AdPacksAdded = AdPacksLimit AND MoneyToReturn = MoneyReturned
}