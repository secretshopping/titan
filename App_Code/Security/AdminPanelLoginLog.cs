using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ExtensionMethods;

public class AdminPanelLoginLog : BaseTableObject
{
    public static void Create(string IP, string userName, string connectionString)
    {
        SqlConnection sqlConnection = new SqlConnection(connectionString);
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.Connection = sqlConnection;

        try
        {
            cmd.CommandText = @"
                                CREATE TABLE [dbo].AdminPanelLoginLogs
                                (
	                                Id int PRIMARY KEY IDENTITY,
	                                LoginDate datetime NOT NULL,
	                                LoginUsername varchar(100) NOT NULL,
	                                LoginIP varchar(100) NOT NULL
                                );
                              ";

            sqlConnection.Open();
            cmd.ExecuteNonQuery();
        }
        catch(Exception ex) { }
        finally
        {
            sqlConnection.Close();
        }

        try
        {
            cmd.CommandText =
                String.Format("INSERT INTO AdminPanelLoginLogs (LoginDate, LoginUsername, LoginIP) VALUES ('{0}','{1}', '{2}')",
                AppSettings.ServerTime.ToDBString(), userName, IP);

            sqlConnection.Open();
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex) { }
        finally
        {
            sqlConnection.Close();
        }
    }


    #region Constructors

    public AdminPanelLoginLog()
            : base()
    {

    }
    public AdminPanelLoginLog(int id)
            : base(id)
    {

    }
    public AdminPanelLoginLog(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    {

    }
    #endregion

    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "AdminPanelLoginLogs"; } }
    protected override string dbTable { get { return TableName; } }

    private int _id;
    private DateTime _LoginDate;
    private string _LoginUserName, _LoginIP;
    public static class Columns
    {
        public const string Id = "Id";
        public const string LoginDate = "LoginDate";
        public const string LoginUserName = "LoginUserName";
        public const string LoginIP = "LoginIP";
    }
    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.LoginDate)]
    public DateTime LoginDate { get { return _LoginDate; } set { _LoginDate = value; SetUpToDateAsFalse(); } }

    [Column(Columns.LoginUserName)]
    public string LoginUserName { get { return _LoginUserName; } set { _LoginUserName = value; SetUpToDateAsFalse(); } }

    [Column(Columns.LoginIP)]
    public string LoginIP { get { return _LoginIP; } set { _LoginIP = value; SetUpToDateAsFalse(); } }


}
