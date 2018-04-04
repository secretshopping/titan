using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;

public class PayoutLimit
{
    public static Money GetGlobalLimitValue(Member User)
    {
        Money LimitBase = User.Membership.CashoutLimit;
        Money LimitAdded = User.CashoutsProceed * User.Membership.CashoutLimitIcreased;
        
        Money FinalLimit = LimitBase + LimitAdded;
        FinalLimit = Money.Parse(FinalLimit.ToShortClearString());

        if (FinalLimit > AppSettings.Payments.MemberMaxCashoutLimit)
            return AppSettings.Payments.MemberMaxCashoutLimit;

        return FinalLimit;
    }

    public static Money GetProperLimitValue(Member User, CustomPayoutProcessor PP)
    {
        if (PP.OverrideGlobalLimit)
            return PP.CashoutLimit;

        return GetGlobalLimitValue(User);
    }

    public static Money GetProperLimitValue(Member User, PaymentAccountDetails PP)
    {
        if (PP.OverrideGlobalLimit)
            return PP.CashoutLimit;

        return GetGlobalLimitValue(User);
    }
}