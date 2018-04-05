using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class InvestmenPlatformSQLHelper
{
    private Parser parser;

    private static string MaxMultiplier
    {
        get
        {
            return Math.Pow(10, CoreSettings.GetMaxDecimalPlaces()).
                ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"))
                .Replace(",", "");
        }
    }

    public InvestmenPlatformSQLHelper(Parser bridge)
    {
        this.parser = bridge;
    }
}