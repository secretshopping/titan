using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
/// <summary>
/// Summary description for UniqueAdClick
/// </summary>
public class UniqueAdClick : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "UniqueAdClicks"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("UniqueAdId")]
    public int UniqueAdId { get { return _UniqueAdId; } set { _UniqueAdId = value; SetUpToDateAsFalse(); } }

    int _Id, _UserId, _UniqueAdId;

    #endregion

    #region Constructors

    public UniqueAdClick()
            : base()
        {

    }
    public UniqueAdClick(int id)
            : base(id)
        {

    }
    public UniqueAdClick(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {

    }
    #endregion
}