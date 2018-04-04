using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SelectPdf;
using System.IO;

public class HtmlInvoiceGenerator
{
    private static readonly string templateLocation = TitanFeatures.isAri ? "~/Plugins/Receipt/ReceiptTemplateAri.html" : "~/Plugins/Receipt/ReceiptTemplate.html";
    private readonly PurchasedItem item;
    private readonly Member user;
    private readonly string fileName;


    public HtmlInvoiceGenerator(PurchasedItem item)
    {
        this.item = item;
        this.user = new Member(item.UserId);
        this.fileName = "Invoice_" + item.Id + ".pdf";
}

    public void DownloadPdf()
    {
        PdfDocument doc = GetPdf();
        doc.Save(HttpContext.Current.Response, false, fileName);
    }

    public void SendPdfViaEmail()
    {
        try
        {
            PdfDocument doc = GetPdf();
            using (MemoryStream memoryStream = new MemoryStream(doc.Save()))
            {
                System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(memoryStream, ct);
                attachment.ContentDisposition.FileName = fileName;

                Mailer.SendEmail("Invoice", "Please find invoice attached", user.Email, null, null, attachments: new System.Net.Mail.Attachment[1] { attachment });
            }
        }
        catch(Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    private PdfDocument GetPdf()
    {
        var htmlString = GetHtml();
        HtmlToPdf converter = new HtmlToPdf();
        converter.Options.PdfPageSize = PdfPageSize.A4;
        converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
        converter.Options.WebPageWidth = 1024;

        PdfDocument doc = converter.ConvertHtmlString(htmlString);
        return doc;
    }



    string GetHtml()
    {
        string fullUserName = user.FirstName + " " + user.SecondName;
        if (string.IsNullOrEmpty(fullUserName))
            fullUserName = user.Name;

        string htmlFromFile = File.ReadAllText(HttpContext.Current.Request.MapPath(templateLocation));
        string htmlString = htmlFromFile
                            .Replace("[SITE_LOGO]", AppSettings.Site.LogoImageURL)
                            .Replace("[SHOW_SITE_NAME]", AppSettings.Site.ShowSiteName ? "inline" : "none")
                            .Replace("[SITE_NAME]", AppSettings.Site.Name)
                            .Replace("[NAME_ADDRESS]", AppSettings.Payments.YourInvoiceName)
                            .Replace("[SITE_URL]", AppSettings.Site.Url)
                            .Replace("[RESX_INVOICE]", TitanFeatures.isAri ? U6005.INVOICE : U6005.RECEIPT)
                            .Replace("[RESX_DATE]", L1.DATE)
                            .Replace("[ITEM_DATE]", item.DateAdded.ToShortDateString())
                            .Replace("[ITEM_ID]", item.Id.ToString())
                            .Replace("[RESX_CUSTOMERID]", U6005.CUSTOMERID)
                            .Replace("[USER_ID]", user.Id.ToString())
                            .Replace("[RESX_BILLTO]", U6005.BILLTO)
                            .Replace("[USER_FULLNAME]", fullUserName)
                            .Replace("[USER_EMAIL]", user.Email)
                            .Replace("[USER_MEMBERSHIP]", user.MembershipName)
                            .Replace("[USER_COUNTRY]", user.Country)
                            .Replace("[RESX_DESCRIPTION]", L1.DESCRIPTION)
                            .Replace("[RESX_QUANTITY]", U6005.QUANTITY)
                            .Replace("[RESX_UNITPRICE]", U5006.UNITPRICE)
                            .Replace("[RESX_TAX]", U6005.TAX)
                            .Replace("[RESX_TOTAL]", U5001.TOTAL)
                            .Replace("[ITEM_DESCRIPTION]", item.Description)
                            .Replace("[ITEM_QUANTITY]", item.Quantity.ToString())
                            .Replace("[ITEM_UNITPRICE]", item.UnitPrice.ToString())
                            .Replace("[ITEM_TAX]", item.Tax.ToString())
                            .Replace("[ITEM_TOTALVALUE]", item.GetTotalValue().ToString())
                            .Replace("[ADMIN_EMAIL]", AppSettings.Email.Forward);

        return htmlString;
    }
}