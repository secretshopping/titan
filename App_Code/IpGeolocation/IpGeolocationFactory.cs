using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class IpGeolocationFactory
{
    public static T Get<T>() where T : IpGeolocationProviderBase
    {
        return (T)Activator.CreateInstance(typeof(T));
    }
}