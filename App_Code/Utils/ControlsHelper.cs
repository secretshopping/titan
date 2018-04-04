using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public static class ControlsHelper
{
    public static Control FindControlRecursive(Control rootControl, string controlID)
    {
        if (rootControl.ID == controlID) return rootControl;

        foreach (Control controlToSearch in rootControl.Controls)
        {
            Control controlToReturn =
                FindControlRecursive(controlToSearch, controlID);
            if (controlToReturn != null) return controlToReturn;
        }
        return null;
    }
}