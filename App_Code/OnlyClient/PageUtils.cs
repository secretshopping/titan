using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public static class PageUtils
{
    public static void BaseGridView_PreRender(this Page page, object sender, EventArgs args)
    {
        GridView BaseGridView = (GridView)sender;

        if (BaseGridView.HeaderRow != null)
            BaseGridView.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

    public static void ValidateMoneyNonNegative(this Page page, object sender, ServerValidateEventArgs args)
    {
        Money value;
        args.IsValid = Money.TryParse(args.Value, out value) && value >= Money.Zero;
    }

    public static void ValidateMoneyWithNegative(this Page page, object sender, ServerValidateEventArgs args)
    {
        Money value;
        args.IsValid = Money.TryParse(args.Value, out value);
    }
}