using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Resources;
/// <summary>
/// Summary description for UniqueAdClick
/// </summary>
public class AdPacksForOtherUsers : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "AdPacksForOtherUsers"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("BuyerId")]
    public int BuyerId { get { return _BuyerId; } set { _BuyerId = value; SetUpToDateAsFalse(); } }

    [Column("OwnerId")]
    public int OwnerId { get { return _OwnerId; } set { _OwnerId = value; SetUpToDateAsFalse(); } }

    [Column("Count")]
    public int Count { get { return _Count; } set { _Count = value; SetUpToDateAsFalse(); } }

    int _Id, _BuyerId, _OwnerId, _Count;

    #endregion

    #region Constructors

    public AdPacksForOtherUsers()
            : base()
    {

    }
    public AdPacksForOtherUsers(int id)
            : base(id)
    {

    }
    public AdPacksForOtherUsers(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    {

    }
    #endregion

    public static List<AdPacksForOtherUsers> Get()
    {
        return TableHelper.SelectAllRows<AdPacksForOtherUsers>();
    }

    public static List<AdPacksForOtherUsers> Get(int buyerId, int ownerId)
    {
        return TableHelper.GetListFromRawQuery<AdPacksForOtherUsers>(string.Format("SELECT * FROM AdPacksForOtherUsers WHERE BuyerId = {0} AND OwnerId = {1}", buyerId, ownerId));
    }

    public static List<AdPacksForOtherUsers> GetListByBuyer(int buyerId)
    {
        return TableHelper.GetListFromRawQuery<AdPacksForOtherUsers>(string.Format("SELECT * FROM AdPacksForOtherUsers WHERE BuyerId = {0}", buyerId));
    }

    public static List<AdPacksForOtherUsers> GetListByOwner(int ownerId)
    {
        return TableHelper.GetListFromRawQuery<AdPacksForOtherUsers>(string.Format("SELECT * FROM AdPacksForOtherUsers WHERE OwnerId = {0}", ownerId));
    }

    public static void AddOrUpdate(AdPacksForOtherUsers record, int buyerId, int ownerId, int count)
    {
        if(record != null)
            record.Count += count;
        else
            record = new AdPacksForOtherUsers()
            {
                BuyerId = buyerId,
                OwnerId = ownerId,
                Count = count
            };

        record.Save();
    }

    public static void Validate(int buyerId, int ownerId, int count, out AdPacksForOtherUsers record)
    {
        record = Get(buyerId, ownerId).FirstOrDefault();
        if (record != null)
        {
            if (record.Count + count > AppSettings.RevShare.AdPack.MaxAdPacksForOtherUser)
                throw new MsgException(string.Format(U5008.CANTBUYADPACKFORTHISUSER, AppSettings.RevShare.AdPack.MaxAdPacksForOtherUser - record.Count,
                    AppSettings.RevShare.AdPack.AdPackNamePlural));
        }
        else
        {
            if (count > AppSettings.RevShare.AdPack.MaxAdPacksForOtherUser)
                throw new MsgException(string.Format(U5008.CANTBUYADPACKFORTHISUSER, AppSettings.RevShare.AdPack.MaxAdPacksForOtherUser,
                    AppSettings.RevShare.AdPack.AdPackNamePlural));
        }
    }
}