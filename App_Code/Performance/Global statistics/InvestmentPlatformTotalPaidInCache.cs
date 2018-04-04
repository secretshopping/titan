using Prem.PTC;

public class InvestmentPlatformTotalPaidInCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "InvestmentPlatformTotalPaidIn"; } }

    protected override object GetDataFromSource()
    {
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            string command = "SELECT SUM(ipp.Price) FROM InvestmentPlatformPlans ipp JOIN InvestmentUsersPlans iup ON ipp.Id = iup.PlanId";
            string tempresult = bridge.Instance.ExecuteRawCommandScalar(command).ToString();

            return tempresult != string.Empty ? Money.Parse(tempresult) : Money.Zero;
        }
    }
}