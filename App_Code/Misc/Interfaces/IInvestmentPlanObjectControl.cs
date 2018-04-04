using Titan.InvestmentPlatform;

public interface IInvestmentPlanObjectControl
{
    InvestmentPlatformPlan PlatformPlan { get; set; }
    InvestmentUsersPlans UserPlan { get; set; }
    bool IncludedUsersEarning { get; set; }
    void DataBind();
}