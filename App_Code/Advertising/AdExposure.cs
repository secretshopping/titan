using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AdExposure
/// </summary>
public enum AdExposure
{
    Null = 0,

    Micro = 1, //0-5s
    Fixed = 5, //6-10s
    Mini = 2, //11-15s
    Standard = 3, //16-30s
    Extended = 4 //30s+
}