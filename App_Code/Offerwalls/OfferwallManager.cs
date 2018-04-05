using MarchewkaOne.Titan.Offerwalls;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan;


public class OfferwallManager
{
    public static List<Offerwall> GetOfferwallsForMember(Member user)
    {
        var offerwallsQuery = string.Format("SELECT * FROM {0} WHERE [Status] = {1} ORDER BY {2}", Offerwall.TableName, (int)OfferwallStatus.Active, Offerwall.Columns.OrderNumber);
        var ActiveOfferwalls = TableHelper.GetListFromRawQuery<Offerwall>(offerwallsQuery);
        var BlockedOfferwallsForMember = TableHelper.SelectRows<OfferwallsBlocked>(TableHelper.MakeDictionary("UserId", user.Id));
        var NewList = new List<Offerwall>();

        foreach (var active in ActiveOfferwalls)
        {
            bool IsOk = true;

            foreach (var blocked in BlockedOfferwallsForMember)
            {            
                if (blocked.OfferwallId == active.Id)
                    IsOk = false;
            }

            if (TitanFeatures.IsRewardStacker && (active.DisplayName == "RadioLoyalty" || active.DisplayName == "Jun Group"))
                IsOk = false;

            if (IsOk)
                NewList.Add(active);
        }

        return NewList;
    }
}