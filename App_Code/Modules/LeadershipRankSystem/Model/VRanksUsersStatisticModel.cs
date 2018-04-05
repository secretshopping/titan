using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;

/// <summary>
/// Summary description for VRanksUsersStatistic
/// </summary>
public partial class VRanksUsersStatistic : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "VRanksUsersStatistic"; } }

    protected override string dbTable { get { return TableName; } }

    public static class Columns
    {
        public const string Id = "Id";
        public const string Username = "Username";
        public const string CurrentRankName = "CurrentRankName";
        public const string AcquiredRank = "AcquiredRank";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Username)]
    public string Username { get { return _username; } protected set { _username = value; SetUpToDateAsFalse(); } }

    [Column(Columns.CurrentRankName)]
    public string CurrentRankName { get { return _currentRankName; } protected set { _currentRankName = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AcquiredRank)]
    public string AcquiredRank { get { return _acquiredRank; } protected set { _acquiredRank = value; SetUpToDateAsFalse(); } }
    
    private int _id;
    private string _acquiredRank, _currentRankName, _username;
}