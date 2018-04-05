﻿using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;


/// <summary>
/// Handling TrafficGridLatestWinners
/// </summary>
public class TrafficGridTopWinners : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "TrafficGridTopWinners"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("Date")]
    public DateTime Date { get { return _Date; } set { _Date = value; SetUpToDateAsFalse(); } }

    [Column("What")]
    public string What { get { return _What; } set { _What = value; SetUpToDateAsFalse(); } }

    [Column("Username")]
    public string Username { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

    [Column("Value")]
    public Money Value { get { return _Value; } set { _Value = value; SetUpToDateAsFalse(); } }

    private int _id;
    private string name, _What;
    private DateTime _Date;
    private Money _Value;

    #endregion Columns

    public TrafficGridTopWinners()
        : base() { }

    public TrafficGridTopWinners(int id) : base(id) { }

    public TrafficGridTopWinners(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

}
