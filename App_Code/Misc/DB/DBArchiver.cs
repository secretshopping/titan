using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Prem.PTC;
using ExtensionMethods;

public class DBArchiver
{
    public static string BalanceLogsFileName = "BalanceLogs";
    public static string HistoryFileName = "History";
    public static string IPHistoryLogsFileName = "IPHistoryLogs";
    public static string OfferwallsLogsFileName = "OfferwallsLogs";
    public static string PostbackLogsFileName = "PostbackLogs";

    public static void CRON()
    {
        try
        {
            ArchiveBalanceLogs();
            ArchiveHistoryLogs();
            ArchiveIPHistoryLogs();
            ArchiveOfferwallsLogs();
            ArchivePostbackLogs();
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    protected static void ArchiveBalanceLogs()
    {
        DateTime date = DateTime.Now.AddDays(-AppSettings.DBArchiver.BalanceLogsKeptForDays);
        string wherePart = "WHERE DateOccured < '" + date.ToDBString() + "'";

        //Archive to file
        string archiveQuery = "SELECT * FROM BalanceLogs " + wherePart;
        DBArchiverAPI.ExportToCSV(archiveQuery, date, BalanceLogsFileName);

        //Delete from DB
        string deleteQuery = "DELETE FROM BalanceLogs " + wherePart;

        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            bridge.Instance.ExecuteRawCommandNonQuery(deleteQuery);
        }
    }

    protected static void ArchiveHistoryLogs()
    {
        DateTime date = DateTime.Now.AddDays(-AppSettings.DBArchiver.HistoryLogsKeptForDays);
        string wherePart = "WHERE Date < '" + date.ToDBString() + "'";

        //Archive to file
        string archiveQuery = "SELECT * FROM History " + wherePart;
        DBArchiverAPI.ExportToCSV(archiveQuery, date, HistoryFileName);

        //Delete from DB
        string deleteQuery = "DELETE FROM History " + wherePart;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            bridge.Instance.ExecuteRawCommandNonQuery(deleteQuery);
        }
    }

    protected static void ArchiveIPHistoryLogs()
    {
        DateTime date = DateTime.Now.AddDays(-AppSettings.DBArchiver.IPHistoryLogsKeptForDays);
        string wherePart = "WHERE LoginDate < '" + date.ToDBString() + "'";

        //Archive to file
        string archiveQuery = "SELECT * FROM IPHistoryLogs " + wherePart;


        DBArchiverAPI.ExportToCSV(archiveQuery, date, IPHistoryLogsFileName);

        //Delete from DB
        string deleteQuery = "DELETE FROM IPHistoryLogs " + wherePart;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            bridge.Instance.ExecuteRawCommandNonQuery(deleteQuery);
        }
    }

    protected static void ArchiveOfferwallsLogs()
    {
        DateTime date = DateTime.Now.AddDays(-AppSettings.DBArchiver.OfferwallsLogsKeptForDays);
        string wherePart = "WHERE DateAdded < '" + date.ToDBString() + "'";
        int recordsInEachBatch = 8000;
        int batches = (int)TableHelper.SelectScalar("SELECT COUNT(*) FROM OfferwallsLogs " + wherePart) / recordsInEachBatch;

        string archiveQuery = "SELECT TOP " + recordsInEachBatch + " * FROM OfferwallsLogs " + wherePart;

        string deleteQuery = string.Format(@"WITH CTE AS ({0}) DELETE FROM CTE", archiveQuery);

        for (int i = 0; i < batches; i++)
        {
            //Archive to file
            DBArchiverAPI.ExportToCSV(archiveQuery, date, OfferwallsLogsFileName, i);

            //Delete from DB
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                bridge.Instance.ExecuteRawCommandNonQuery(deleteQuery);
            }
        }
    }

    protected static void ArchivePostbackLogs()
    {
        DateTime date = DateTime.Now.AddDays(-AppSettings.DBArchiver.PostBackLogsKeptForDays);
        string wherePart = "WHERE DateHappened < '" + date.ToDBString() + "'";
        int recordsInEachBatch = 8000;
        int batches = (int)TableHelper.SelectScalar("SELECT COUNT(*) FROM CPAPostbackLogs " + wherePart) / recordsInEachBatch;

        string archiveQuery = "SELECT TOP " + recordsInEachBatch + " * FROM CPAPostbackLogs " + wherePart;

        string deleteQuery = string.Format(@"WITH CTE AS ({0}) DELETE FROM CTE", archiveQuery);

        for (int i = 0; i < batches; i++)
        {
            //Archive to file
            DBArchiverAPI.ExportToCSV(archiveQuery, date, PostbackLogsFileName, i);

            //Delete from DB
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                bridge.Instance.ExecuteRawCommandNonQuery(deleteQuery);
            }
        }
    }
}