using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

public class UserUrl : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "UserUrls"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } protected set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Url")]
    public string Url { get { return _Url; } protected set { _Url = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }


    #endregion Columns
    public AdvertStatus Status
    {
        get { return (AdvertStatus)StatusInt; }
        protected set { StatusInt = (int)value; }
    }

    int _Id, _UserId, _StatusInt;
    string _Url;

    public UserUrl(int id) : base(id) { }

    public UserUrl(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate) { }

    private UserUrl(int userId, string url)
    {
        UserId = userId;
        Url = url;
        Status = AdvertStatus.WaitingForAcceptance;
    }
    public void Accept()
    {
        this.Activate();
    }
    public void Activate()
    {
        Status = AdvertStatus.Active;
        this.Save();
    }

    public void Reject()
    {
        Status = AdvertStatus.Rejected;
        this.Save();
    }

    public void Pause()
    {
        Status = AdvertStatus.Paused;
        Save();
    }

    public override void Delete()
    {
        Status = AdvertStatus.Deleted;
        Save();
    }

    public static void Create(int userId, string url)
    {
        var userUrl = Get(userId, url);
        if (userUrl != null)
        {
            if (userUrl.Status == AdvertStatus.Rejected)
            {
                throw new MsgException(U6002.THISURLHASBEENREJECTED);
            }
            else
            {
                userUrl.Status = AdvertStatus.WaitingForAcceptance;
            }
        }
        else
        {
            userUrl = new UserUrl(userId, url);
        }

        userUrl.Save();
    }

    public static void ChangeAllStatuses(IEnumerable<int> ids, AdvertStatus status)
    {
        foreach (var id in ids)
        {
            var url = new UserUrl(id);

            if (status == AdvertStatus.Active)
                url.Accept();
            else if (status == AdvertStatus.Rejected)
                url.Reject();
        }
    }

    private static UserUrl Get(int userId, string url)
    {
        var query = string.Format("SELECT * FROM UserUrls WHERE UserId = {0} AND Url = '{1}'", userId, url);
        return TableHelper.GetListFromRawQuery<UserUrl>(query).FirstOrDefault();
    }

    public static List<UserUrl> GetActive(int userId)
    {
        var query = string.Format("SELECT * FROM UserUrls WHERE UserId = {0} AND Status = {1}", userId, (int)AdvertStatus.Active);

        return TableHelper.GetListFromRawQuery<UserUrl>(query);
    }

    public static bool UserHasActive(int userId)
    {
        var query = string.Format("SELECT COUNT(*) FROM UserUrls WHERE UserId = {0} AND Status = {1}", userId, (int)AdvertStatus.Active);

        return (int)TableHelper.SelectScalar(query) > 0;
    }
}