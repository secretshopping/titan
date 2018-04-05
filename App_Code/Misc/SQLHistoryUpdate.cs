using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SQLHistoryUpdate
/// </summary>
public class SQLHistoryUpdate : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "SQLUpdateHistory"; } }
    protected override string dbTable { get { return TableName; } }

    #region Columns
    public static class Columns
    {
        public const string Id = "Id";
        public const string DatePerformed = "DatePerformed";
        public const string UserName = "UserName";
        public const string SQLQuery = "SQLQuery";
    }
    #endregion

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.DatePerformed)]
    public DateTime DatePerformed { get { return _datePerformed; } set { _datePerformed = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UserName)]
    public string UserName { get { return _userName; } set { _userName = value; SetUpToDateAsFalse(); } }

    [Column(Columns.SQLQuery)]
    public string SQLQuery { get { return _sqlQuery; } set { _sqlQuery = value; SetUpToDateAsFalse(); } }

    private DateTime _datePerformed;
    private int _id;
    private string _sqlQuery, _userName;

    public SQLHistoryUpdate(int id) : base(id) { }

    public SQLHistoryUpdate(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    public SQLHistoryUpdate(string Username, string SQLQuery)
    {
        this.UserName = Username;
        this.SQLQuery = SQLQuery;
        DatePerformed = AppSettings.ServerTime;
    }
}