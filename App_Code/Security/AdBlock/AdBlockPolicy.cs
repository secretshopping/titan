using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public enum AdBlockPolicy
{
    AllowAll = 0, 
    DenyAccessForLoggedIn = 1, //default
    DenyAccessForAll = 2 
}