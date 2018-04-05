using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public enum Pools
{
    AdministratorProfit = 0,
    AdPackRevenueReturn = 1,
    CreditLine = 2,
    PvpJackpotGamePool = 3
}

public class PoolsHelper
{
    public static int GetBuiltInProfitPoolId(Pools pool)
    {
        string poolName = string.Empty;
        switch (pool)
        {
            case Pools.AdministratorProfit:
                poolName = "Administrator Profit";
                break;
            case Pools.AdPackRevenueReturn:
                poolName = "AdPack Profit Distribution";
                break;
            case Pools.CreditLine:
                poolName = "Credit Line";
                break;
            case Pools.PvpJackpotGamePool:
                poolName = "PvpJackpot Game Pool";
                break;
            default: throw new NotImplementedException(string.Format("Wrong enum value: {0}", pool.ToString()));
        }

        int? poolId = TableHelper.SelectScalar(string.Format("SELECT TOP 1 Id FROM Pools WHERE Name = '{0}'", poolName)) as int?;
        try
        {
            if (poolId == null)
                throw new NullReferenceException(string.Format("Error: no '{0}' Pool in Pools table", poolName));
        }
        catch (NullReferenceException ex)
        {
            ErrorLogger.Log(ex);
        }
        return poolId.Value;
    }

    public static string GetPoolName(int poolId)
    {
        var poolName = TableHelper.GetListFromRawQuery<Pool>(string.Format("SELECT * FROM Pools WHERE Id = {0}", poolId))[0].Name;
        return poolName;
    }

    public static int GetSundayPoolId()
    {
        return (int)TableHelper.SelectScalar("SELECT TOP 1 Id FROM Pools WHERE Name = 'StringSawSundayPool'");
    }

    public static bool ProfitPoolDistributionExists(int poolId)
    {
        var rules = (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM ProfitPoolDistribution WHERE Pool = {0} AND ProfitPoolPercent > 0", poolId));
        return rules > 0;
    }

    public static bool JackpotPvpGamePoolExists()
    {
        if (Convert.ToInt32(TableHelper.SelectScalar("SELECT COUNT(*) FROM Pools WHERE Name='PvpJackpot Game Pool'")) > 0)
            return true;

        return false;
    }

    public static void CreateJackpotPvpPoolIfNotExists()
    {
        if (!JackpotPvpGamePoolExists())
        {
            var PvpJackpotPool = new Pool();
            PvpJackpotPool.Name = "PvpJackpot Game Pool";
            PvpJackpotPool.Status = UniversalStatus.Active;
            PvpJackpotPool.Save();

            ProfitPoolDistribution.Add(ProfitSource.JackpotPvp, PvpJackpotPool, 100);
        }
    }
}
[Serializable]
public class Pool : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "Pools"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("Name")]
    public string Name { get { return _Name; } set { _Name = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

    public UniversalStatus Status
    {
        get { return (UniversalStatus)StatusInt; }
        set { StatusInt = (int)value; }
    }

    int _Id, _Status;
    string _Name;

    public Pool()
        : base() { }

    public Pool(int id) : base(id) { }

    public Pool(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    #endregion Columns

    public static List<Pool> GetAllPools()
    {
            return TableHelper.SelectAllRows<Pool>();
    }

    public static List<Pool> GetActivePools()
    {
        return TableHelper.GetListFromRawQuery<Pool>(string.Format("SELECT * FROM Pools WHERE Status = {0};", (int)UniversalStatus.Active));
    }


}
