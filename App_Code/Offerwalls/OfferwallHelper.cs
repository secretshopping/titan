using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan;

/// <summary>
/// Summary description for OfferwallHelper
/// </summary>
public class OfferwallHelper
{
    public static string GetOfferWallsIncludedInPTCContest(string offerwallName = null)
    {
        var whereDict = new Dictionary<string, object>();
        whereDict["IsIncludedInPTCContest"] = true;

        if (!string.IsNullOrEmpty(offerwallName))
            whereDict["DisplayName"] = offerwallName;


        var listOfIncluded = TableHelper.SelectRows<Offerwall>(whereDict);
        string printedList = string.Empty;
        foreach (Offerwall offerwall in listOfIncluded)
        {
            if (!printedList.Contains(offerwall.DisplayName))
                printedList += (offerwall.DisplayName + ", ");
        }
        return printedList;
    }
}