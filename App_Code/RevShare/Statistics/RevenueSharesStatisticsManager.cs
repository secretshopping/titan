using Prem.PTC;
using Prem.PTC.Advertising;

public class RevenueSharesStatisticsManager
{
    public int ActiveAdPacks, EligibleAdPacks, TotalSoldAdPacks, FinishedAdPacks;
    public Money TotalOwed;
    public RevenueSharesStatisticsManager()
    {
        //Total
        TotalSoldAdPacks = TableHelper.CountOf<AdPack>();

        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            //Active
            ActiveAdPacks = (int)bridge.Instance.ExecuteRawCommandScalar(@"
                SELECT COUNT(*) 
                FROM AdPacks 
                WHERE
                    MoneyReturned < MoneyToReturn");

            //Finished
            FinishedAdPacks = TotalSoldAdPacks - ActiveAdPacks;

            //Eligible
            DistributionSQLHelper helper = new DistributionSQLHelper(bridge.Instance);
            helper.SetStartingDistributionPriority();

            EligibleAdPacks = (int)bridge.Instance.ExecuteRawCommandScalar(@"
                SELECT COUNT(*) 
                FROM AdPacks 
                WHERE
                    DistributionPriority > 0");

            Money.TryParse(TableHelper.SelectScalar("SELECT SUM(MoneyToReturn - MoneyReturned) FROM AdPacks").ToString(), out TotalOwed);

            TotalOwed = TotalOwed == null ? Money.Zero : TotalOwed;
        }
    }
}