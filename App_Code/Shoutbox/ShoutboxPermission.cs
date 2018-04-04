using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;


public enum ShoutboxPermission
{
    NoRestrictions = 1,

    HideUsername = 2,
    DoNotPublish = 3
}

public class ShoutboxPermissionHelper
{
    public static ListItem[] ListItems
    {
        get
        {
            var query = from ShoutboxPermission status in Enum.GetValues(typeof(ShoutboxPermission))
                        orderby (int)status
                        select new ListItem(GetResourceText(status), (int)status + "");

            return query.ToArray();
        }
    }

    private static string GetResourceText(ShoutboxPermission perm)
    {
        switch (perm)
        {
            case ShoutboxPermission.DoNotPublish:
                return Resources.U3500.SP_DONOT;
            case ShoutboxPermission.HideUsername:
                return Resources.U3500.SP_HIDE;
            case ShoutboxPermission.NoRestrictions:
                return Resources.U3500.SP_NOREST;
            default:
                return "";
        }
    }
}