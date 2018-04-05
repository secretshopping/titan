using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for OfferwallsLogStatus
/// </summary>
public enum OfferwallsLogStatus
{
    Null = 0,

    CreditedByOfferwall = 1,
    ReversedByAdmin = 3,
    ReversedByOfferwall = 4,

    WrongSignature = 5,
    SentFromUnallowedIP = 6,
    OfferwallInactive = 7,

    //4000
    CreditedAndPointsLocked = 8,
    CreditedByOfferwallPointsUnlocked = 9,

    MemberNotFoundByUsername = 10,

    WrongCreditVariable = 11
}

public static class OfferwallsLogStatusHelper
{
    /// <summary>
    /// Returns list control source with all statuses' short descriptions as text 
    /// and int status ordinal as value
    /// </summary>
    public static ListItem[] ListItems
    {
        get
        {
            var query = from OfferwallsLogStatus status in Enum.GetValues(typeof(OfferwallsLogStatus))
                        where status != OfferwallsLogStatus.Null
                        orderby (int)status
                        select new ListItem(status.ShortDescription(), (int)status + "");

            var array = query.ToArray();

            foreach (var item in array)
            {
                if (item.Value == "1" || item.Value == "9")
                    item.Attributes.Add("style", "color:green");

                else if (item.Value == "3" || item.Value == "4")
                    item.Attributes.Add("class", "textRed");

                else if (item.Value == "8")
                    item.Attributes.Add("style", "color: purple;");

                else
                    item.Attributes.Add("class", "textOrange");
            }

            return array;
        }
    }
}

public static class OfferwallsLogExtensions
{
    /// <summary>
    /// Provides human readable, short description for each status.
    /// </summary>
    public static string ShortDescription(this OfferwallsLogStatus status)
    {
        if (status == OfferwallsLogStatus.Null) return "Unknown";

        return Enum.GetName(typeof(OfferwallsLogStatus), status);
    }
}