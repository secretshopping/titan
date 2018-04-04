using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Types of SMS confirmations
/// </summary>
public enum ProxySMSType
{
    Null = 0,


    None = 1,

    EveryCashout = 2,

    EveryRegistration = 3,

    FirstCashout = 4
}


public enum IPVerificationPolicy
{
    Null = 0,


    None = 1,

    EveryRegistration = 2,

    EveryLogin = 3
}

public enum ProxyProviderType
{
    Null = 0,


    None = 1,

    ProxStop = 2,

    BlockedCom = 3,

    IpQualityScore = 4
}