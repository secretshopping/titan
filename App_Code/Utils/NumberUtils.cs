using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class NumberUtils
{
    public const int UnlimitedValue = 2000000000;

    public static string FormatPercents(int input)
    {
        return FormatPercents(input.ToString());
    }

    public static string FormatPercents(decimal input)
    {
        return FormatPercents(input.ToString());
    }

    public static string FormatPercents(string input)
    {
        if (input.EndsWith(".00"))
            input = input.Substring(0, input.Length - 3);

        if (input.EndsWith("0") && input.Contains("."))
            input = input.Substring(0, input.Length - 1);

        input += "%";

        return input;
    }
}
