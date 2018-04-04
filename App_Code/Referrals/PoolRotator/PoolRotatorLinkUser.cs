using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

public class PoolRotatorLinkUser : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "PoolRotatorLinkUsers"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Expires")]
    public DateTime Expires { get { return _Expires; } set { _Expires = value; SetUpToDateAsFalse(); } }

    [Column("ClicksDelivered")]
    public int ClicksDelivered { get { return _ClicksDelivered; } set { _ClicksDelivered = value; SetUpToDateAsFalse(); } }

    [Column("ReferralsDelivered")]
    public int ReferralsDelivered { get { return _ReferralsDelivered; } set { _ReferralsDelivered = value; SetUpToDateAsFalse(); } }

    [Column("IsActive")]
    public bool IsActive { get { return _IsActive; } set { _IsActive = value; SetUpToDateAsFalse(); } }

    private int _id, _ReferralsDelivered, _UserId, _ClicksDelivered;
    private DateTime _Expires;
    private bool _IsActive;

    #endregion Columns

    public PoolRotatorLinkUser()
            : base() { }

    public PoolRotatorLinkUser(int id) : base(id) { }

    public PoolRotatorLinkUser(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }


    public string GetLink()
    {
        return String.Format("{0}default.aspx?u=*{1}", AppSettings.Site.Url, this.Id);
    }

}
