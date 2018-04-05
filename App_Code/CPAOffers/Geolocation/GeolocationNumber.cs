using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class GeolocationNumber
{
    //String: Min#Max

    public int Min { get; set; }
    public int Max { get; set; }

    public GeolocationNumber(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public override string ToString()
    {
        return Min + "#" + Max;
    }

    public static GeolocationNumber Parse(string input)
    {
        try
        {
            string[] arr = input.Split('#');
            return new GeolocationNumber(Convert.ToInt32(arr[0]), Convert.ToInt32(arr[1]));
        }
        catch (Exception ex) { }

        return new GeolocationNumber(0, 0);
    }

    public static bool IsGeolocationNumber(string input)
    {
        if (input.Contains('#'))
            return true;
        return false;
    }

    /// <summary>
    /// Checks whether the value is in the range of GeolocationNumber. Inclusive.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public bool IsBetweenMinAndMax(int value)
    {
        if (value >= Min && value <= Max)
            return true;
        return false;
    }
}