using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class UnpaidPayoutRequests
{
    public string text { get; set; }
    public bool exists { get; set; }

    public UnpaidPayoutRequests(bool exists, string message)
    {
        this.exists = exists;
        this.text = message;
    }
}
