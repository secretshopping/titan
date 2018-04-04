using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtensionMethods;
using Prem.PTC.Offers;

public class PointsLockingHelper
{
    public static OfferwallsLog FindSimilarLog(string Username, string NetworkName, string TrackingID)
    {
        var result = TableHelper.GetListFromQuery<OfferwallsLog>("WHERE ( OfferStatus = " + (int)OfferwallsLogStatus.CreditedAndPointsLocked 
            + " OR OfferStatus = " + (int)OfferwallsLogStatus.ReversedByAdmin
            + ") AND Username = '" + Username + "' AND NetworkName = '" + NetworkName + "' AND TrackingID = '"
            + TrackingID + "'");

        if (result.Count == 0)
            return null;

        return result[0];
    }

    public static CPAPostbackLog FindSimilarCpaLog(string Username, string NetworkName, string TrackingID)
    {
        var result = TableHelper.GetListFromQuery<CPAPostbackLog>("WHERE ( Statusek = " + (int)CPAPostBackLogStatus.CreditedAndPointsLocked
            + ") AND Username = '" + Username + "' AND NetworkName = '" + NetworkName + "' AND CampaignId = '"
            + TrackingID + "'");

        if (result.Count == 0)
            return null;

        return result[0];
    }

    public static void findLockedCpaLog(OfferRegisterEntry entry , CPAPostBackLogStatus status)
    {
        var query = string.Format("WHERE ( Statusek = {0}) AND Username = '{1}' AND NetworkName = '{2}' AND CampaignId = '{3}'",
            (int)CPAPostBackLogStatus.CreditedAndPointsLocked, entry.Username, entry.Offer.AdvertiserUsername, entry.Offer.NetworkOfferId);

        var lockedLog = TableHelper.GetListFromQuery<CPAPostbackLog>(query);        

        if (lockedLog.Count != 0)
        {
            Prem.PTC.ErrorLogger.Log(query);
            lockedLog[0].Status = status;
            lockedLog[0].Save();
        }
    }
}