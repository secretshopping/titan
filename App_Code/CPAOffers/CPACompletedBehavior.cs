using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;


public enum CPACompletedBehavior
{
    PopupOnScreen = 1,
    SendEmail = 2,
    PopupAndSendEmail = 3
}

public class CPACompletedBehaviorHelper
{
    public static ListItem[] ListItems
    {
        get
        {
            var query = from CPACompletedBehavior status in Enum.GetValues(typeof(CPACompletedBehavior))
                        orderby (int)status
                        select new ListItem(GetResourceText(status), (int)status + "");

            return query.ToArray();
        }
    }

    private static string GetResourceText(CPACompletedBehavior perm)
    {
        switch (perm)
        {
            case CPACompletedBehavior.PopupAndSendEmail:
                return Resources.U3500.CP_3;
            case CPACompletedBehavior.PopupOnScreen:
                return Resources.U3500.CP_1;
            case CPACompletedBehavior.SendEmail:
                return Resources.U3500.CP_2;
            default:
                return "";
        }
    }
}