using Newtonsoft.Json;
using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;


public class FixerExchangeRates : ExchangeRates
{
    public FixerExchangeRates()
    {
    }

    public override Money GetRate(string from, string to)
    {
        using (WebClient client = new MyWebClient())
        {
            string response = client.DownloadString(
                String.Format("http://api.fixer.io/latest?base={0}&symbols={1}", from, to));

            var results = JsonConvert.DeserializeObject<dynamic>(response);
            return Money.Parse(results.rates[to].ToString());
        }
    }
}