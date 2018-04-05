using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for JackpotTicket
/// </summary>
public class JackpotTicket : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "JackpotTickets"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {
        public const string Id = "Id";
        public const string JackpotId = "JackpotId";
        public const string UserId = "UserId";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.JackpotId)]
    public int JackpotId { get { return _JackpotId; } set { _JackpotId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UserId)]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }
    #endregion

    private int _id, _JackpotId, _UserId;

    public JackpotTicket()
            : base()
    { }

    public JackpotTicket(int id) : base(id) { }

    public JackpotTicket(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    { }
}