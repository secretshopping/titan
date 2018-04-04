using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;

public class LevelMemberships
{
    public static IMembership Get(int xp, bool getNext = false)
    {
        var cache = new MembershipsCache();
        List<Membership> list = (List<Membership>)cache.Get();

        IMembership result = null;

        for (int i=0; i<list.Count; i++)
        {
            if (list[i].MinPointsToHaveThisLevel <= xp)
                result = list[i];
            else
            {
                if (getNext)
                    result = list[i];
                break;
            }
        }

        return result;
    }

    public static IMembership GetNext(int xp)
    {
        return Get(xp, true);
    }
}