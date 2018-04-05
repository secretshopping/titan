using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for JackpotPrize
/// </summary>
public class JackpotTicketPrize : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "JackpotTicketPrizes"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {
        public const string Id = "Id";
        public const string JackpotId = "JackpotId";
        public const string PrizeType = "PrizeType";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.JackpotId)]
    public int JackpotId { get { return _JackpotId; } set { _JackpotId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.PrizeType)]
    protected int PrizeTypeInt { get { return _PrizeTypeInt; } set { _PrizeTypeInt = value; SetUpToDateAsFalse(); } }
    #endregion

    public JackpotPrize PrizeType
    {
        get
        {
            return (JackpotPrize)PrizeTypeInt;
        }

        set
        {
            PrizeTypeInt = (int)value;
        }
    }

    private int _id, _JackpotId, _PrizeTypeInt;

    public JackpotTicketPrize()
            : base()
    { }

    public JackpotTicketPrize(int id) : base(id) { }

    public JackpotTicketPrize(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    { }
}