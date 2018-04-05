using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Newtonsoft.Json;
using Prem.PTC.Members;
using System.Text;
using System.Security.Cryptography;
using Titan;
using ExtensionMethods;

public class CrowdflowerHandler
{
    public static void ProcessRequest(HttpContext context)
    {
        try
        {
            string LogMessage = context.Request.Params.ToRawString();
            ErrorLogger.Log(LogMessage, LogType.OfferWalls);

            //Force refresh
            AppSettings.Offerwalls.Reload();

            string payload = context.Request["payload"];
            string signature = context.Request["signature"];

            string myhash = SHA1HashStringForUTF8String(payload + AppSettings.Offerwalls.CrowdFlowerKey);

            if (myhash == signature)
            {
                //All ok
                //Lets check if we have First or second request
                if (payload.Contains("conversion_id"))
                {
                    //2
                    var data = JsonConvert.DeserializeObject<RecData2>(payload);

                    string title = data.job_title;

                    if (title.Length > 80)
                        title = title.Substring(0, 79);

                    string conversionID = data.conversion_id.Substring(1);

                    CrowdflowerTask task = new CrowdflowerTask(Convert.ToInt32(conversionID));
                    task.Title = title;

                    decimal points = data.adjusted_amount;

                    //Not to duplicate the entries
                    if (task.Points == -1)
                    {
                        try
                        {
                            CreditUser(task.Username, points, "CrowdFlower", data.job_title);
                        }
                        catch (Exception ex) { ErrorLogger.Log(ex); }
                    }

                    task.Points = Convert.ToInt32(points);
                    task.Save();

                    context.Response.Write("OK");
                }
                else
                {
                    //1
                    var data = JsonConvert.DeserializeObject<RecData1>(payload);

                    CrowdflowerTask task = new CrowdflowerTask();
                    task.Date = DateTime.Now;
                    task.Points = -1;
                    task.Username = data.uid;
                    task.Save();

                    context.Response.Write("0" + task.Id.ToString());
                }
            }
            else
            {
                context.Response.StatusCode = 403;
                context.Response.Status = "403 Access Denied";
            }
        }
        catch (Exception ex) { Prem.PTC.ErrorLogger.Log(ex); }
    }

    public class RecData1
    {
        public string uid { get; set; }
        public int amount { get; set; }
        public Decimal adjusted_amount { get; set; }

    }

    public class RecData2
    {
        public int amount { get; set; }
        public Decimal adjusted_amount { get; set; }
        public string conversion_id { get; set; }
        public string job_title { get; set; }
    }

    private static void CreditUser(string username, decimal points, string offerwallname, string trackingID = "?")
    {
        Money Sent = new Money(points);
        Money Calculated = new Money(0);

        Member User = new Member(username);

        CrowdflowerCrediter Crediter = new CrowdflowerCrediter(User);
        OfferwallsLogStatus Status = OfferwallsLogStatus.CreditedByOfferwall;

        if (Sent < new Money(0))
        {
            //Reversal
            Status = OfferwallsLogStatus.ReversedByOfferwall;
            Calculated = Crediter.Reverse(Sent, AppSettings.Offerwalls.ConvertCrowdflowerToMainBalance ? CreditAs.MainBalance : CreditAs.Points, offerwallname, AppSettings.Offerwalls.ConvertCrowdflowerToMainBalance);
        }
        else
        {
            //Credit
            Calculated = Crediter.Credit(Sent, AppSettings.Offerwalls.ConvertCrowdflowerToMainBalance ? CreditAs.MainBalance : CreditAs.Points, offerwallname, AppSettings.Offerwalls.ConvertCrowdflowerToMainBalance);
        }

        //Add OfferwallsLog    
        OfferwallsLog.CreateCrowdflower(username, Sent, Calculated, trackingID, Status);
    }

    private static string SHA1HashStringForUTF8String(string s)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(s);

        var sha1 = SHA1.Create();
        byte[] hashBytes = sha1.ComputeHash(bytes);

        return HexStringFromBytes(hashBytes);
    }

    private static string HexStringFromBytes(byte[] bytes)
    {
        var sb = new StringBuilder();
        foreach (byte b in bytes)
        {
            var hex = b.ToString("x2");
            sb.Append(hex);
        }
        return sb.ToString();
    }
}