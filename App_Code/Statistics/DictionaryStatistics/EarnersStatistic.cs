
using Prem.PTC.Members;

public class TopEarnersStatistic : DictionaryStatisticBase<string, decimal>
{
    protected override string CacheName { get { return "EarnersTopStatistic"; } }

    public override string GetSqlQuery
    {
        //RETURNS BEST EARNERS
        get
        {
            return string.Format("SELECT {1} Username, TotalEarned FROM Users WHERE UserId<>1005 AND {0} ORDER BY TotalEarned DESC",
                MemberStatusHelper.PotentiallyActiveStatusesSQLHelper, "{0}");
        }
    }

    public override string KeyColumnName { get { return "Username"; } }
    public override string ValueColumnName { get { return "TotalEarned"; } }
}
