using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CustomGroupManager
/// </summary>
public static class AutomaticGroupManager
{
    public static AutomaticGroup GetUsersAutomaticGroup(Member user, ref int numberOfAdPacksToNextGroup, ref AutomaticGroup nextGroup)
    {
        try {
            var numberOfActiveAdpacks = AdPackManager.GetUsersActiveAdPacks(user.Id).Count;

            var allAutomaticGroups = GetAllAutomaticGroups();

            int minNumberOfAdPacks = 0;
            AutomaticGroup currentGroup = null;
            foreach (var group in allAutomaticGroups)
            {
                if (numberOfActiveAdpacks >= minNumberOfAdPacks && numberOfActiveAdpacks <= group.AdPacksLimit)
                {
                    currentGroup = group;
                    minNumberOfAdPacks = group.AdPacksLimit + 1;
                    var nextGroups = TableHelper.GetListFromRawQuery<AutomaticGroup>(string.Format("SELECT * FROM AutomaticGroups WHERE Number = {0}", (group.Number + 1).ToString()));
                    if (nextGroups.Count > 0)
                        nextGroup = nextGroups[0];
                }
            }

            if (currentGroup == null)
            {
                var maxNumber = allAutomaticGroups.Max(x => x.Number);
                currentGroup = (from groups in allAutomaticGroups.Where(x => x.Number == maxNumber) select groups).ToList()[0];
            }
            var maxGroupNumber = Convert.ToInt32(TableHelper.SelectScalar("SELECT MAX(Number) FROM AutomaticGroups"));
            if (currentGroup.Number >= maxGroupNumber)
                nextGroup = null;

            if (nextGroup != null)
                numberOfAdPacksToNextGroup = currentGroup.AdPacksLimit + 1 - numberOfActiveAdpacks;

            return currentGroup;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public static List<AutomaticGroup> GetAllAutomaticGroups()
    {
        return TableHelper.GetListFromRawQuery<AutomaticGroup>("SELECT * FROM AutomaticGroups ORDER BY Number");
    }

    public static void UpdateHighestGroup()
    {
        var query1 = "UPDATE AutomaticGroups SET IsHighestGroup = 'false'; ";
        var query2 = "UPDATE AutomaticGroups SET IsHighestGroup = 'true' WHERE ((select max(Number) from automaticgroups) = Number);";

        TableHelper.ExecuteRawCommandNonQuery(query1 + query2);
    }

    public static void UpdateGroupNumbers()
    {
        var automaticGroups = GetAllAutomaticGroups().OrderBy(x => x.AdPacksLimit);

        var number = 1;
        foreach(var group in automaticGroups)
        {
            group.Number = number;
            number++;
            group.Save();
        }
    }


}