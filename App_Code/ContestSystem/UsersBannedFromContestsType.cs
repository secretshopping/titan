using Prem.PTC;
using Prem.PTC.Contests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


public class UsersBannedFromContestsType : BaseTableObject
{

    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "UsersBannedFromContestsTypes"; } }
    protected override string dbTable { get { return TableName; } }

    public static class Columns
    {
        public static string Id = "Id";
        public static string UserId = "UserId";
        public static string Type = "Type";
    }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Type")]
    protected int IntType { get { return _Type; } set { _Type = value; SetUpToDateAsFalse(); } }

    private int _id, _UserId, _Type;

    #endregion

    public UsersBannedFromContestsType()
            : base()
    { }

    public UsersBannedFromContestsType(int id) : base(id) { }

    public UsersBannedFromContestsType(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    { }

    public ContestType Type
    {
        get
        {
            return (ContestType)IntType;
        }

        set
        {
            IntType = (int)value;
        }
    }

    public static List<UsersBannedFromContestsType> Get(int userId)
    {
        string query = string.Format("SELECT * FROM UsersBannedFromContestsTypes WHERE UserId = {0};", userId);
        return TableHelper.GetListFromRawQuery<UsersBannedFromContestsType>(query);
    }

    public static void DeleteUserRecords(int userId)
    {
        string query = string.Format("DELETE FROM UsersBannedFromContestsTypes WHERE UserId = {0};", userId);
        TableHelper.ExecuteRawCommandNonQuery(query);
    }

    public static void SaveBan(int userId, ContestType contestType)
    {
        var contestsTypestable = new UsersBannedFromContestsType();

        contestsTypestable.UserId = userId;
        contestsTypestable.Type = contestType;
        contestsTypestable.Save();
    }

    public static bool IsBannedFromContestType(int userId, ContestType contestType)
    {
        var BlockedContestTypes = UsersBannedFromContestsType.Get(userId);

        foreach (var BlockedContestType in BlockedContestTypes)
        {
            if (BlockedContestType.Type == contestType)
                return true;
        }

        return false;
    }
}