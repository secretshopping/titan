using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

public class JackpotWinningTicket : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "JackpotWinningTickets"; } }
    protected override string dbTable { get { return TableName; } }

    public static class Columns
    {
        public const string Id = "Id";
        public const string JackpotId = "JackpotId";
        public const string UserWinnerId = "UserWinnerId";
        public const string WinningTicketNumber = "WinningTicketNumber";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.JackpotId)]
    public int JackpotId { get { return _JackpotId; } set { _JackpotId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.UserWinnerId)]
    public int? UserWinnerId { get { return _UserWinnerId; } set { _UserWinnerId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.WinningTicketNumber)]
    public int? WinningTicketNumber { get { return _WinningTicketNumber; } set { _WinningTicketNumber = value; SetUpToDateAsFalse(); } }

    private int _id, _JackpotId;
    private int? _UserWinnerId, _WinningTicketNumber;

    public JackpotWinningTicket()
        : base()
    { }

    public JackpotWinningTicket(int id) 
        : base(id)
    { }

    public JackpotWinningTicket(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate)
    { }
}