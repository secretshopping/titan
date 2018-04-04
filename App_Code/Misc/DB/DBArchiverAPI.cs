using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Prem.PTC;
using System.Text;

public class DBArchiverAPI
{
    private static string FolderPath = "~/App_Data/";

    public static void ExportToCSV(string dbQuery, DateTime dateOfExport, string whatIsExported, int number = 0)
    {
        try
        {
            string sNumber = number == 0 ? string.Empty : "_" + number;
            string folderpath = HttpContext.Current.Server.MapPath("~/App_Data/" + whatIsExported + "/");
            string filepath = whatIsExported + "_" + dateOfExport.ToString("yyyy_MM_dd") + sNumber + ".csv";

            if (!Directory.Exists(folderpath))
                Directory.CreateDirectory(folderpath);

            string content = ExportToCSVString(dbQuery);

            using (StreamWriter _testData = new StreamWriter(folderpath + filepath))
            {
                _testData.WriteLine(content); // Write the file.
                _testData.Flush();
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log("Error while exporting CSV to file: " + ex.Message);
            throw ex;
        }

    }

    public static string ExportToCSVString(string dbQuery, bool includeHeaders = true)
    {
        string connectionString = ConnectionString.GetConnectionString(Database.Client);

        //Build the CSV file data as a Comma separated string.
        StringBuilder csv = new StringBuilder();

        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(dbQuery))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);

                            if (includeHeaders)
                            {
                                foreach (DataColumn column in dt.Columns)
                                {
                                    //Add the Header row for CSV file.
                                    csv.Append(column.ColumnName);
                                    csv.Append(',');
                                }

                                //Add new line.
                                csv.Append("\r\n");
                            }

                            foreach (DataRow row in dt.Rows)
                            {
                                foreach (DataColumn column in dt.Columns)
                                {
                                    //Add the Data rows.
                                    csv.Append(row[column.ColumnName].ToString().Replace(",", ";"));
                                    csv.Append(',');
                                }

                                //Add new line.
                                csv.Append("\r\n");
                            }

                            //Download the CSV file.
                            //Response.Clear();
                            //Response.Buffer = true;
                            //Response.AddHeader("content-disposition", "attachment;filename=SqlExport.csv");
                            //Response.Charset = "";
                            //Response.ContentType = "application/text";
                            //Response.Output.Write(csv);
                            //Response.Flush();
                            //Response.End();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log("Error while exporting DB to CSV: " + ex.Message);
            ErrorLogger.Log(ex);
        }

        return csv.ToString();
    }

    protected static string GetFileName(string what, DateTime date)
    {
        string temp = what + "DumpFrom " + date.ToShortDateString() + ".csv";
        return temp.Replace(" ", "_").Replace("-", "_").Replace("/", "_");
    }

}