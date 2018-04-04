using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Titan;
using Prem.PTC;
using Resources;
using Prem.PTC.Members;
using System.Text;
using Prem.PTC.Advertising;
using System.Globalization;
using UseTitanHelpers;

public partial class Controls_UseTitanDemoHeader : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (AppSettings.IsDemo && (UseTitanDemoHelper.IsProductsSet() || UseTitanDemoHelper.IsThemeSet()))
        {
            GlobalPlaceHolder.Visible = true;
            StringBuilder builder = new StringBuilder();

            if (UseTitanDemoHelper.IsProductsSet())
            {
                //We have product(s)
                var products = UseTitanDemoHelper.GetProducts().Split(',').ToList();
                products.Remove("1"); //We remove Titan Core

                if (products.Count() == 1)
                    builder.Append(UseTitanProducts.GetHTML((TitanProduct)Convert.ToInt32(products[0])));
                else
                {
                    for(int i=0; i<products.Count(); i++)
                    {
                        builder.Append(UseTitanProducts.GetHTML((TitanProduct)Convert.ToInt32(products[i])));
                        if (i < products.Count() - 1)
                            builder.Append(" <br> ");
                    }
                }
            }

            if (UseTitanDemoHelper.IsThemeSet())
            {
                //We have theme preset

                if (UseTitanDemoHelper.IsProductsSet())
                    builder.Append(" <br>with the theme ");
                else
                    builder.Append(" the theme ");

                builder.AppendFormat("<b class='theme-name'>{0}</b>", UseTitanDemoHelper.GetTheme());
            }

            MainTextLiteral.Text = builder.ToString();
        }
    }
}
