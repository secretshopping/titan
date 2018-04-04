<%@ WebHandler Language="C#" Class="MailerHandler" %>

using System;
using System.Web;
using Prem.PTC;
using System.Collections.Generic;
using Prem.PTC.Misc;

public class MailerHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //Force settings refresh
            AppSettings.Reload();

            string SourceIP = IP.Current;
            string password = context.Request.Form["p"].ToString();

            string emailsEncrypted = HttpUtility.UrlDecode(context.Request.Form["to"].ToString());
            string subtitle = HttpUtility.UrlDecode(context.Request.Form["subtitle"].ToString());
            string text = HttpUtility.UrlDecode(context.Request.Form["text"].ToString());
            int emailId = Convert.ToInt32(HttpUtility.UrlDecode(context.Request.Form["emailId"].ToString()));

            List<string> emails = new List<string>(emailsEncrypted.Split(' '));

            //Verify the source
            if (password == HashingManager.SHA256(AppSettings.Offerwalls.UniversalHandlerPassword)
                && UseTitan.AdminPanelIPAddresses.Contains(SourceIP)) //
            {
                //All OK, proceed

                if (emails.Count == 1)
                {
                    //SingleEmail
                    try
                    {
                        Mailer.SendEmail(subtitle, text, emails[0]);
                        context.Response.Write("1");
                        try
                        {
                            Email emailInDatabase = new Email(emailId);
                            emailInDatabase.Status = EmailStatus.Sent;
                            emailInDatabase.Note = "";
                            emailInDatabase.Save();
                        }
                        catch (Exception)
                        { }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            Email emailInDatabase = new Email(emailId);
                            emailInDatabase.Status = EmailStatus.Error;
                            emailInDatabase.Note = ex.Message;
                            emailInDatabase.Save();
                        }
                        catch (Exception e2)
                        {
                            throw ex;
                        }
                    }
                }
                else
                {
                    //MassEmail => No exceptions
                    context.Response.Write(emails.Count);
                    Mailer.SendMassEmail(emails, subtitle, text, emailId);
                }
            }
            else
            {
                //Fraud request, save in logs
                context.Response.Write("Not authorized Admin Panel Mailer request from: " + SourceIP);
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            context.Response.Write(ex.Message.ToString());
        }
    }

    public bool IsReusable { get { return false; } }
}