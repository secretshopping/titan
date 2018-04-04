using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.IO;
using System.Text;

public class EmailCSVExporter
{
    public static void GenerateAndReturn()
    {
        string fileName = "EmailExport_" + AppSettings.ServerTime.ToString("yyyy_MM_dd") + ".csv";

        try
        {
            string content = DBArchiverAPI.ExportToCSVString("SELECT firstname, secondname, username, email, country FROM users", false);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            HttpContext.Current.Response.Charset = "";
            HttpContext.Current.Response.ContentType = "application/text";
            HttpContext.Current.Response.Output.Write(content);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        catch (Exception ex)
        {
            ErrorLogger.Log("Error while exporting CSV to file: " + ex.Message);
            throw new MsgException(ex.Message);
        }
    }

}