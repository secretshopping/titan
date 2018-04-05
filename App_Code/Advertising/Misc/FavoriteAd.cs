using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Text;

public class FavoriteAd : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "FavouriteAds"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("AdvertType")]
    protected int AdvertType { get { return _AdvertType; } set { _AdvertType = value; SetUpToDateAsFalse(); } }

    [Column("AdvertId")]
    public int AdvertId { get { return _AdvertId; } set { _AdvertId = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    private int _id, _AdvertId, _AdvertType, _UserId;

    #endregion Columns

    public FavoriteAd()
        : base()
    { }

    public FavoriteAd(int id) : base(id) { }

    public FavoriteAd(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate)
    { }

    public FavoriteAdType Type
    {
        get
        {
            return (FavoriteAdType)AdvertType;
        }

        set
        {
            AdvertType = (int)value;
        }
    }

    public static bool IsFavorite(int userId, int adId, FavoriteAdType type)
    {
        var count = (int)TableHelper.SelectScalar(
            String.Format("SELECT COUNT(*) FROM FavouriteAds WHERE UserId = {0} AND AdvertId = {1} AND AdvertType = {2}",
            userId, adId, (int)type));

        if (count > 0)
            return true;

        return false;
    }

    public static void AddToFavorites(int userId, int adId, FavoriteAdType type)
    {
        FavoriteAd ad = new FavoriteAd();
        ad.UserId = userId;
        ad.AdvertId = adId;
        ad.Type = type;
        ad.Save();
    }
    public static void RemoveFromFavorites(int userId, int adId, FavoriteAdType type)
    {
        TableHelper.ExecuteRawCommandNonQuery(String.Format("DELETE FROM FavouriteAds WHERE UserId = {0} AND AdvertId = {1} AND AdvertType = {2}",
            userId, adId, (int)type));
    }

    public static List<int> GetUserFavorites(int userId, FavoriteAdType type)
    {
        List<int> result = new List<int>();

        var list = TableHelper.GetListFromRawQuery<FavoriteAd>(
            String.Format("SELECT AdvertId FROM FavouriteAds WHERE UserId = {0} AND AdvertType = {1}", userId, (int)type));

        foreach (var elem in list)
            result.Add(elem.AdvertId);

        return result;
    }

    public static string GetUserFavoritesCommaDelimited(int userId, FavoriteAdType type)
    {
        var result = GetUserFavorites(userId, type);
        if (result.Count == 0)
            return "-1";

        StringBuilder sb = new StringBuilder();
        foreach (var elem in result)
        {
            sb.Append(elem);
            sb.Append(",");
        }

        return sb.ToString().TrimEnd(',');
    }

}
