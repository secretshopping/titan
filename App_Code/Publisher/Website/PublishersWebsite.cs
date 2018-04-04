using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Resources;
using Prem.PTC.Advertising;

public class PublishersWebsite : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "PublishersWebsites"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Url")]
    public string Url { get { return _Url; } set { _Url = value; SetUpToDateAsFalse(); } }

    [Column("Host")]
    public string Host { get { return _Host; } set { _Host = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

    [Column("PublishersWebsiteCategoryId")]
    public int PublishersWebsiteCategoryId { get { return _PublishersWebsiteCategoryId; } set { _PublishersWebsiteCategoryId = value; SetUpToDateAsFalse(); } }

    [Column("PostbackUrl")]
    public string PostbackUrl { get { return _PostbackUrl; } set { _PostbackUrl = value; SetUpToDateAsFalse(); } }

    public PublishersWebsiteStatus Status
    {
        get { return (PublishersWebsiteStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    int _Id, _UserId, _PublishersWebsiteCategoryId, _StatusInt;
    string _Url, _Host, _PostbackUrl;
    #endregion Columns

    public PublishersWebsite(int id) : base(id) { }

    public PublishersWebsite(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    private PublishersWebsite(int userId, string url, int categoryId)
    {
        Uri uri = new Uri(url, UriKind.Absolute);
        Host = uri.Host.Replace("www.", "").Trim();
        UserId = userId;
        Url = url;
        Status = PublishersWebsiteStatus.Pending;
        PublishersWebsiteCategoryId = categoryId;
    }

    public void Accept()
    {
        Status = PublishersWebsiteStatus.Accepted;
        this.Save();
    }

    public void Reject()
    {
        Status = PublishersWebsiteStatus.Rejected;
        this.Save();
    }

    public static PublishersWebsite GetActiveWebsite(string host, int publishersWebsiteId, bool skipHostVerification = false)
    {
        host = host.Replace("www.", "").Trim();
        var query = string.Format("SELECT TOP 1 * FROM PublishersWebsites WHERE Status = {0} AND Host = '{1}' AND Id = {2}",
            (int)PublishersWebsiteStatus.Accepted, host, publishersWebsiteId);

        if (skipHostVerification)
            query = string.Format("SELECT TOP 1 * FROM PublishersWebsites WHERE Status = {0} AND Id = {2}",
            (int)PublishersWebsiteStatus.Accepted, host, publishersWebsiteId);

        return TableHelper.GetListFromRawQuery<PublishersWebsite>(query).FirstOrDefault();
    }

    public void AddPostbackUrl(string postbackUrl)
    {
        PostbackUrl = postbackUrl;
        Save();
    }

    public List<ExternalBannerAdvert> GetActiveBanners(int dimensionsId)
    {
        var query = string.Format(@"SELECT * FROM ExternalBannerAdverts 
                                    WHERE Status = {0} 
                                    AND PublishersWebsiteCategoryId = {1}
                                    AND ExternalBannerAdvertPackId IN 
                                        (SELECT Id FROM ExternalBannerAdvertPacks WHERE ExternalBannerDimensionsId = {2})",
            (int)AdvertStatus.Active, this.PublishersWebsiteCategoryId, dimensionsId);
        return TableHelper.GetListFromRawQuery<ExternalBannerAdvert>(query);
    }

    public static void Create(int userId, string url, int categoryId)
    {
        var website = GetUsersWebsite(userId, url);

        if (website == null)
        {
            website = new PublishersWebsite(userId, url, categoryId);
            website.Save();
        }
        else
        {
            website.UserUpdate(url, categoryId);
        }
    }

    private void UserUpdate(string url, int categoryId)
    {
        Url = url;
        PublishersWebsiteCategoryId = categoryId;
        Status = PublishersWebsiteStatus.Pending;
        Save();
    }

    private static PublishersWebsite GetUsersWebsite(int userId, string url)
    {
        string host = new Uri(url, UriKind.Absolute).Host.Replace("www.", "").Trim();
        string query = string.Format("SELECT * FROM PublishersWebsites WHERE UserId = {0} AND Host = '{1}'",
            userId, host);

        return TableHelper.GetListFromRawQuery<PublishersWebsite>(query).FirstOrDefault();
    }

    public static List<PublishersWebsite> GetWebsites(int userId)
    {
        var query = string.Format("SELECT * FROM PublishersWebsites WHERE UserId = {0}",
            userId);
        return TableHelper.GetListFromRawQuery<PublishersWebsite>(query);
    }

    public static List<PublishersWebsite> GetActiveAndPendingWebsites(int userId)
    {
        var query = string.Format("SELECT * FROM PublishersWebsites WHERE UserId = {0} AND (Status = {1} OR Status = {2});",
            userId, (int)PublishersWebsiteStatus.Accepted, (int)PublishersWebsiteStatus.Pending);
        return TableHelper.GetListFromRawQuery<PublishersWebsite>(query);
    }

    public static bool ActiveOrPendingWebsiteExists(int userId)
    {
        var query = string.Format("SELECT COUNT(*) FROM PublishersWebsites WHERE UserId = {0} AND (Status = {1} OR Status = {2});",
            userId, (int)PublishersWebsiteStatus.Accepted, (int)PublishersWebsiteStatus.Pending);

        return (int)TableHelper.SelectScalar(query) > 0;
    }

    public static void ChangeAllStatuses(IEnumerable<int> websitesIds, PublishersWebsiteStatus status)
    {
        foreach (var websiteId in websitesIds)
        {
            var website = new PublishersWebsite(websiteId);

            if (status == PublishersWebsiteStatus.Accepted)
                website.Accept();           
            else if (status == PublishersWebsiteStatus.Rejected)
                website.Reject();           
        }
    }

    public override void Delete()
    {
        Status = PublishersWebsiteStatus.Deleted;
        Save();
    }
    public static List<PublishersWebsite> GetActiveWithPostbackUrls(int userId)
    {
        var query = string.Format("SELECT * FROM PublishersWebsites WHERE UserId = {0} AND Status = {1} AND PostbackUrl IS NOT NULL;",
            userId, (int)PublishersWebsiteStatus.Accepted);
        return TableHelper.GetListFromRawQuery<PublishersWebsite>(query);
    }

    public static bool AreAnyActiveWithPostbackUrls(int userId)
    {
        var query = string.Format("SELECT COUNT(*) FROM PublishersWebsites WHERE UserId = {0} AND Status = {1} AND PostbackUrl IS NOT NULL;",
            userId, (int)PublishersWebsiteStatus.Accepted);

        return (int)TableHelper.SelectScalar(query) > 0;
    }
}

public enum PublishersWebsiteStatus
{
    Pending = 1,
    Accepted = 2,
    Rejected = 3,
    Deleted = 4
}