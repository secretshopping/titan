using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Data;
using Prem.PTC.Memberships;

/// <summary>
/// Summary description for FamobiGame
/// </summary>
public class FamobiGame : BaseTableObject
{

    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "FamobiGames"; } }
    protected override string dbTable { get { return TableName; } }

    #region Columns
    public static class Columns
    {
        [Obsolete]
        public const string Id = "Id";
        public const string Name = "Name";
        public const string Url = "Url";
        public const string ThumbUrl = "ThumbUrl";
        public const string Description = "Description";
        public const string AspectRatio = "AspectRatio";
        public const string RequiredMembershipId = "RequiredMembershipId";
    }

    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Name)]
    public string Name { get { return _name; } set { _name = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Url)]
    public string Url { get { return _url; } set { _url = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ThumbUrl)]
    public string ThumbUrl { get { return _thumbUrl; } set { _thumbUrl = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Description)]
    public string Description { get { return _Description; } set { _Description = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AspectRatio)]
    public string AspectRatio { get { return _AspectRatio; } set { _AspectRatio = value; SetUpToDateAsFalse(); } }

    [Column(Columns.RequiredMembershipId)]
    public int RequiredMembershipId { get { return _requiredMembershipId; } set { _requiredMembershipId = value; SetUpToDateAsFalse(); } }


    private int _id, _requiredMembershipId;
    private string _url, _thumbUrl, _Description, _nameThum, _name, _AspectRatio;

    #endregion
    

    public FamobiGame()
        : base() { }

    public FamobiGame(int id) : base(id) { }

    public FamobiGame(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }

    public static void UpdateTableAfterDeletingMembership(int membershipId)
    {
        var querry = string.Format(@"UPDATE FamobiGames SET {0} = {1} WHERE {0} = {2}", Columns.RequiredMembershipId, Membership.Standard.Id, membershipId);
        TableHelper.ExecuteRawCommandNonQuery(querry);
    }
}
