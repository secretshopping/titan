using Newtonsoft.Json;
using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;


public class OpenExchangeRates : ExchangeRates
{
    private string appId;

    public OpenExchangeRates(string appID)
    {
        appId = appID;
    }

    public override Money GetRate(string from, string to)
    {
        using (WebClient client = new MyWebClient())
        {
            string response = client.DownloadString(
                String.Format("https://openexchangerates.org/api/latest.json?app_id={0}&base={1}", appId, from));

            var results = JsonConvert.DeserializeObject<dynamic>(response);
            return Money.Parse(results.rates[to].ToString());
        }
    }

    public static bool VerifyConnection(string appID)
    {
        try
        {
            OpenExchangeRates validation = new OpenExchangeRates(appID);
            validation.GetRate("GBP", "USD");
        }
        catch (Exception e)
        {
            throw new MsgException("Your OpenExchangeRates App ID is invalid.");
        }
        return true;
    }
}