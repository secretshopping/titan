using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Web;
using SelectPdf;
using System.IO;
using Titan.InvestmentPlatform;
using System.Drawing;
using System.Windows.Forms;

namespace Prem.PTC
{
    public class HtmlInvestmentProofGenerator
    {
        private string TemplateLocation
        {
            get
            {
                if(AppSettings.InvestmentPlatform.LevelsEnabled)
                    return "~/Plugins/Receipt/LevelsTicketTemplate.html";

                return "~/Plugins/Receipt/PlansTicketTemplate.html";
            }
        }
        private readonly InvestmentTicket ticket;
        private readonly InvestmentUsersPlans userPlan;
        private readonly InvestmentPlatformPlan platformPlan;
        private readonly Member user;
        private readonly string fileName;

        public HtmlInvestmentProofGenerator(InvestmentTicket ticket)
        {
            this.ticket = ticket;
            user = new Member(ticket.UserId);
            fileName = string.Format("{0}.pdf", ticket.GenerateTicketNumber());
            platformPlan = new InvestmentPlatformPlan(new InvestmentUsersPlans(ticket.UserPlanId).PlanId);
        }

        public HtmlInvestmentProofGenerator(InvestmentUsersPlans userPlan)
        {
            this.userPlan = userPlan;
            user = new Member(userPlan.UserId);
            fileName = string.Format("{0}.pdf", userPlan.GeneratePlanNumber());
            platformPlan = new InvestmentPlatformPlan(userPlan.PlanId);
        }

        /// <summary>
        /// Can't call this method from UpdatePanel
        /// </summary>
        public void DownloadPdf()
        {
            var doc = GetPdf();

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/pdf";
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            byte[] pdfData;
            using (var ms = new MemoryStream())
            {
                doc.Save(ms);
                pdfData = ms.ToArray();
            }

            HttpContext.Current.Response.BinaryWrite(pdfData);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        public void SendPdfViaEmail()
        {
            try
            {
                var doc = GetPdf();
                using (MemoryStream memoryStream = new MemoryStream(doc.Save()))
                {
                    System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(memoryStream, ct);
                    attachment.ContentDisposition.FileName = fileName;
                    
                    var title = string.Format(U6012.DEPOSITONLEVEL, platformPlan.Name);
                    var message = string.Format(U6012.DEPOSITONLEVELMESSAGE, platformPlan.Name);

                    Mailer.SendEmail(title, message, user.Email, null, null, attachments: new System.Net.Mail.Attachment[1] { attachment });
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }

        private PdfDocument GetPdf()
        {
            var htmlString = GetHtml();
            var converter = new HtmlToPdf();

            converter.Options.PdfPageSize = PdfPageSize.Custom;
            converter.Options.PdfPageCustomSize = new SizeF(300, 499);
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.WebPageWidth = 300;
            converter.Options.WebPageHeight = 499;
            converter.Options.MarginTop = converter.Options.MarginLeft = converter.Options.MarginRight = 5;

            var doc = converter.ConvertHtmlString(htmlString);

            return doc;
        }

        string GetHtml()
        {
            var defaultColor = platformPlan.Color;
            var myColor = ColorTranslator.FromHtml(defaultColor);
            var myBrighterColor = HexConverter(ControlPaint.Light(myColor));
            var myDarkerColor = HexConverter(ControlPaint.Dark(myColor));
            var watermark = AppSettings.InvestmentPlatform.ProofsWatermarkEnabled ? string.Format("{0}/Images/Misc/WATERMARK.png", AppSettings.Site.Url) : "";

            var htmlFromFile = File.ReadAllText(HttpContext.Current.Request.MapPath(TemplateLocation));

            string htmlString;

            if (AppSettings.InvestmentPlatform.LevelsEnabled)
            {
                htmlString = htmlFromFile
                                .Replace("[FIRST_COLOR]", defaultColor)
                                .Replace("[SECOND_COLOR]", myDarkerColor)
                                .Replace("[THIRD_COLOR]", myBrighterColor)
                                .Replace("[TICKET_NAME]", platformPlan.Name)
                                .Replace("[DATE_TIME]", ticket.Date.ToString())
                                .Replace("[TICKET_NO_TEXT]", string.Format("{0}:", U6012.TICKETNO))
                                .Replace("[TICKET_NO]", ticket.GenerateTicketNumber())
                                .Replace("[INVESTMENT_TEXT]", string.Format("{0}:", U6006.INVESTMENT))
                                .Replace("[DEPOSITED_AND_FEE]", string.Format("{0}: {1} + {2}", U6012.TICKETDEPOSITEDFEE, ticket.LevelPrice.ToString(), ticket.LevelFee.ToString()))
                                .Replace("[RECEIVE]", string.Format("{0}: {1}", U6012.TICKETRECEIVE, ticket.LevelEarnings.ToString()))
                                .Replace("[NOTE]", AppSettings.InvestmentPlatform.ProofsNote)
                                .Replace("[IMAGE_PATH]", watermark)
                                .Replace("[SITE_URL]", AppSettings.Site.Url);
            }
            else
            {
                htmlString = htmlFromFile
                                .Replace("[FIRST_COLOR]", defaultColor)
                                .Replace("[SECOND_COLOR]", myDarkerColor)
                                .Replace("[THIRD_COLOR]", myBrighterColor)
                                .Replace("[TICKET_NAME]", platformPlan.Name)
                                .Replace("[DATE_TIME]", userPlan.PurchaseDate.ToString())
                                .Replace("[TICKET_NO_TEXT]", string.Format("{0}:", U6012.TICKETNO))
                                .Replace("[TICKET_NO]", userPlan.GeneratePlanNumber())
                                .Replace("[INVESTMENT_TEXT]", string.Format("{0}:", U6006.INVESTMENT))
                                .Replace("[DEPOSITED_AND_FEE]", string.Format("{0}: {1}", L1.PRICE, platformPlan.Price.ToString()))
                                .Replace("[RECEIVE]", string.Format("{0}: {1}", U6012.TICKETRECEIVE, userPlan.MoneyToReturn.ToString()))
                                .Replace("[NOTE]", AppSettings.InvestmentPlatform.ProofsNote)
                                .Replace("[TIME]", string.Format("{0}: {1} {2}", U6006.REPURCHASETIME, platformPlan.Time.ToString(), L1.DAYS))
                                .Replace("[IMAGE_PATH]", watermark)
                                .Replace("[SITE_URL]", AppSettings.Site.Url);
            }

            return htmlString;
        }

        private static String HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}