using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Titan;
using Prem.PTC.Members;
using System.Web;

public class IPHistoryLog : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "IPHistoryLogs"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("LoginDate")]
    public DateTime LoginDate { get { return date; } set { date = value; SetUpToDateAsFalse(); } }

    [Column("IP")]
    public string IP { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

    [Column("ISP")]
    public string ISP
    {
        get { return name2; }
        set
        {
            name2 = value;

            if (value != null)
            {
                if (value.Length > 49)
                    name2 = name2.Substring(0, 49);
                else
                    name2 = value;
            }
            SetUpToDateAsFalse();
        }
    }

    [Obsolete]
    [Column("Username")]
    public string Username { get { return name3; } set { name3 = value; SetUpToDateAsFalse(); } }

    [Column("IsMasterLogin")]
    public bool IsMaster { get { return ismaster; } set { ismaster = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }


    private int _id, _UserId;
    private string name, name1, name2, name3;
    private DateTime date;
    private bool ismaster;

    #endregion Columns

    public IPHistoryLog()
        : base() { }

    public IPHistoryLog(int id) : base(id) { }

    public IPHistoryLog(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

    public static void RecordLogin(Member User, bool IsMasterLogin = false)
    {
        IPHistoryLog Log = new IPHistoryLog();
        Log.Username = User.Name;
        Log.UserId = User.Id;
        Log.LoginDate = AppSettings.ServerTime;
        Log.IP = Member.GetCurrentIP(HttpContext.Current.Request);
        Log.ISP = "-";
        Log.IsMaster = IsMasterLogin;
        Log.Save();
    }
}
