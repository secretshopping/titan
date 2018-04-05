using System;
using System.Collections.Generic;
using System.Web;
using System.Net.Mail;
using Prem.PTC;
using System.Text;
using System.Net;
using Prem.PTC.Members;
using System.Threading.Tasks;
using Prem.PTC.Misc;
using System.Threading;
using Resources;

public class Mailer
{
    public Mailer()
    {
    }

    public static void SendEmail(string Subtitle, string Text, string To, List<string> list = null, string from = null, IEnumerable<Attachment> attachments = null)
    {
        if (!AppSettings.IsDemo)
        {
            AppSettings.Email.Reload();

            MailMessage message = new MailMessage();

            //Blank NoReply email fix
            try
            {
                var testEmail = new MailAddress(AppSettings.Email.NoReply);
            }
            catch (Exception ex)
            {
                throw new MsgException("The administrator e-mail address is not set up correctly. Contact the administrator for more details.");
            }

            //So weird...
            if (from == null)
                message.From = new MailAddress(AppSettings.Email.NoReply, AppSettings.Site.Name);
            else
                message.From = new MailAddress(from, AppSettings.Site.Name);

            if (To != null)
                message.To.Add(new MailAddress(To));
            else
            {
                //MassEmail
                foreach (var elem in list)
                    message.Bcc.Add(new MailAddress(elem));
            }

            message.Subject = Subtitle;
            message.Body = Text;
            message.IsBodyHtml = true;

            if(attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }
            }
            

            SmtpClient client = new SmtpClient();

            client.Port = AppSettings.Email.Port;
            client.Host = AppSettings.Email.Host;
            client.Credentials = new NetworkCredential(AppSettings.Email.Username, AppSettings.Email.Password);
            client.EnableSsl = AppSettings.Email.IsSecureMail;

            client.Send(message);
        }
    }

    public static void SendActivationLink(string username, string email)
    {
        string hash = Encryption.Encrypt(username);
        string link = AppSettings.Site.Url + "status.aspx?confirm=" + HttpUtility.UrlEncode(hash);

        SendEmail("Activation Link", ReplaceNewLines(AppSettings.Emails.AccountActivation.Replace("%Sitename%", AppSettings.Site.Name)
            .Replace("%Username%", username).Replace("%ActivationLink%", "<a href=\"" + link + "\">" + link + "</a>")), email);
    }

    public static void SendAdvertRejectionMessage(string body, string email, string targetURL, string reason)
    {
        SendEmail("Campaign Rejected", ReplaceNewLines(AppSettings.Emails.AdvertRejection.Replace("%Sitename%", AppSettings.Site.Name)
            .Replace("%TargetURL%", targetURL).Replace("%Reason%", reason)), email);
    }

    public static void SendGiftCodeEmail(string email, string username, string codeSent, string giftCode, string giftCard)
    {
        SendEmail("GiftCode received", ReplaceNewLines(AppSettings.GiftCards.EmailDraft.Replace("%Sitename%", AppSettings.Site.Name)
            .Replace("%Username%", username).Replace("%CodeSent%", codeSent).Replace("%GiftCode%", giftCode).Replace("%GiftCard%", giftCard)), email);
    }

    public static void SendAdvertStartedMessage(string body, string email, string targetURL)
    {
        SendEmail("Campaign Started", ReplaceNewLines(AppSettings.Emails.AdvertStart.Replace("%Sitename%", AppSettings.Site.Name).Replace("%TargetURL%", targetURL)), email);
    }

    public static void SendResetPasswordInformation(string username, string email, string newPassword)
    {
        SendEmail("Password Reset", ReplaceNewLines(AppSettings.Emails.ResetPassword.Replace("%Sitename%", AppSettings.Site.Name)
            .Replace("%Username%", username).Replace("%NewPassword%", newPassword)), email);
    }

    public static void SendResetPasswordLink(string username, string email, string secretCode, int userId)
    {
        SendEmail("Password Reset", ReplaceNewLines(AppSettings.Emails.ResetPasswordLink.Replace("%Sitename%", AppSettings.Site.Name)
            .Replace("%Username%", username).Replace("%PasswordResetLink%", AppSettings.Site.Url + "resetpwd.aspx?from=" + userId + "&s=" + HttpUtility.UrlEncode(secretCode))), email);
    }

    public static void SendContactMessage(string fromEmail, string text)
    {
        SendEmail("Support request", "[Support request from non-logged member]<br/>FROM EMAIL: " + fromEmail + "<br/>TEXT:<br/>" + ReplaceNewLines(text), AppSettings.Email.Forward);
    }

    public static void SendCRONFailureMessage()
    {
        SendEmail("CRON Run Failed", "Today's CRON run was unsuccessful. It may mean that user statistics haven't been recalculated, ads haven't been " +
        " cleared and many many other issues haven't been handled. Check error log for details.", AppSettings.Email.Forward);
    }
    public static void SendNewReferralMessage(string username, string referralUsername, string email)
    {
        try
        {
            SendEmail("New referral", ReplaceNewLines(AppSettings.Emails.NewReferral.Replace("%Sitename%", AppSettings.Site.Name)
                .Replace("%Username%", username).Replace("%ReferralUsername%", referralUsername)), email);
        }
        catch(Exception e)
        {
            ErrorLogger.Log(e);
        }
    }

    public static void SendPayoutConfirmationMessage(string username, Money amount, string email)
    {
        SendEmail("Payout", ReplaceNewLines(AppSettings.Emails.PayoutEmailMessage.Replace("%Sitename%", AppSettings.Site.Name)
            .Replace("%Username%", username).Replace("%Amount%", amount.ToString())), email);
    }

    public static void SendNewReferralCommisionMessage(string username, string referralUsername, Money commision, string email, string note, int points = 0)
    {
        var allCommision = new StringBuilder();

        if (commision > Money.Zero)
            allCommision.Append(commision.ToString());

        if (points > 0)
            allCommision.Append(string.Format(" + {0} {1}", points, AppSettings.PointsName));


        SendEmail("New commision (" + note + ")", ReplaceNewLines(AppSettings.Emails.NewReferralCommision.Replace("%Sitename%", AppSettings.Site.Name)
            .Replace("%Username%", username).Replace("%ReferralUsername%", referralUsername).Replace("%Commision%", allCommision.ToString())), email);
    }

    public static void SendNewMarketplaceMessage(string emailAddress, string productName, string hash)
    {
        string confirmationUrl = string.Format("{0}user/advert/marketplace.aspx?h={1}", AppSettings.Site.Url, hash);
        string team = AppSettings.Site.Name;
        string message = string.Format(Resources.U5006.MARKETPLACEEMAILTOBUYER, productName, confirmationUrl, team);

        SendEmail("Marketplace purchase", message, emailAddress);
    }

    public static void SendNewCreditLineMessage(string emailAddress, Money amount)
    {
        string message = string.Format(U6008.CREDITLINEACCEPTEDEMAIL, amount, AppSettings.Site.Name);

        SendEmail(U6008.CREDITLINEMAILTITLE, message, emailAddress);
    }

    public static void SendCreditLineRemainderEmail(string message, string userName)
    {
        SendEmail(string.Format(U5006.CREDITLINEREMINDER, AppSettings.Site.Name), message, userName);
    }


    public static void TryToSendCPACreditedMessage(Member User, string text)
    {
        try
        {
            if (User.CPAOfferCompletedBehavior == CPACompletedBehavior.PopupAndSendEmail ||
                User.CPAOfferCompletedBehavior == CPACompletedBehavior.SendEmail)
            {
                SendEmail(Resources.U3500.YOURCPA, "Hello " + User.Name + "!<br/><br/>" + text, User.Email);
            }
        }
        catch (Exception ex) { ErrorLogger.Log(ex); }
    }

    public static int SendMassEmail(List<string> To, string subject, string body, int emailId)
    {
        int sentTo = 0;
        int count = 0;

        if (!AppSettings.IsDemo)
        {
            try
            {
                //Optimalization
                string messageBody = ReplaceNewLines(body);
                string messageTitle = subject;

                MailAddress from = new MailAddress(AppSettings.Email.NoReply, AppSettings.Site.Name);
                string finalSubject =  messageTitle;
                NetworkCredential credential = new NetworkCredential(AppSettings.Email.Username, AppSettings.Email.Password);

                Parallel.ForEach(To, () => 0, (email, loop, subtotal) =>
                {
                    try
                    {
                        MailMessage message = new MailMessage();
                        message.From = from;
                        message.Subject = finalSubject;
                        message.Body = messageBody;
                        message.IsBodyHtml = true;
                        message.To.Add(new MailAddress(email));

                        SmtpClient client = new SmtpClient();
                        client.Port = AppSettings.Email.Port;
                        client.Host = AppSettings.Email.Host;
                        client.Credentials = credential;
                        client.EnableSsl = AppSettings.Email.IsSecureMail;
                        client.Send(message);

                        subtotal += 1;

                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex);
                    }
                    return subtotal;
                },
                finalResult => Interlocked.Add(ref count, finalResult));
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
            finally
            {
                try
                {
                    ErrorLogger.Log(sentTo + "/" + To.Count + "/" + count + "/" + Thread.VolatileRead(ref sentTo) + "/");


                    sentTo = count;


                    ErrorLogger.Log("Mailer: Mass email ('" + subject + "') successfully sent to " + sentTo + " members.");

                    int plannedSentTo = To.Count;
                    int percent = Convert.ToInt32(((new Decimal(sentTo)) * (Decimal)100) / (new Decimal(plannedSentTo)));

                    Email emailInDatabase = new Email(emailId);

                    if (sentTo == 0)
                        emailInDatabase.Status = EmailStatus.Error;
                    else
                        emailInDatabase.Status = EmailStatus.Sent;

                    emailInDatabase.Note = "Success rate " + sentTo + "/" + plannedSentTo + " (" + percent + "%)";
                    emailInDatabase.Save();
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                }
            }
        }

        return sentTo;
    }

    public static int SendEmailToUser(string ToEmail, string subject, string body)
    {
        SendEmail(subject, ReplaceNewLines(body), ToEmail);
        return 1;
    }

    public static string ReplaceNewLines(string blockOfText, string replaceWith = "<br/>")
    {
        return blockOfText.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
    }

    public static int SendCodeVerificationEmail(string emailAddress)
    {
        var rand = new Random();
        int code = rand.Next(101, 99998);

        try
        {
            var message = string.Format(U6000.SECURITYCODEEMAIL, code);

            SendEmail(string.Format(U6000.WITHDRAWALVERIFICATIONEMAILTITLE, AppSettings.Site.Name), message, emailAddress);

            ErrorLogger.Log("Requested the following code: " + code + " via email");
        }
        catch (Exception ex)
        {
            var errorMessage = U6000.FAILEDSENDINGEMAIL;
            ErrorLogger.Log(errorMessage + " " + ex.Message);
            throw new MsgException(errorMessage);
        }
        return code;
    }

}