using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using System;
using Titan;

public static class ActivationFeeManager
{
    public static void TryMarkAccountActivationFeeAsPaid(Member user, Money amount, string from, string transId, string cryptoCurrencyInfo)
    {
        bool Successful = false;

        try
        {
            String Message = String.Format(" {0} money for account activation. ", amount);

            if (amount < AppSettings.Registration.AccountActivationFee)
            {
                Message += "Amount is lower than expected! Account not activated.";
                History.AddEntry(Member.CurrentName, HistoryType.Transfer, Message);

                PoolDistributionManager.AddProfit(ProfitSource.AccountActivationFee, amount);
                throw new Exception(String.Format("Account activation fee is lower than expected! ({0})", amount));
            }
                
            AccountActivationFeeCrediter Crediter = (AccountActivationFeeCrediter)CrediterFactory.Acquire(user, CreditType.AccountActivationFee);
            var moneyLeftForPools = Crediter.CreditReferer(amount);

            //Pools 
            PoolDistributionManager.AddProfit(ProfitSource.AccountActivationFee, moneyLeftForPools);

            user.IsAccountActivationFeePaid = true;
            user.Save();

            Message += "Account activated.";
            History.AddEntry(Member.CurrentName, HistoryType.Transfer, Message);

            Successful = true;
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }

        PaymentProcessor PP = PaymentAccountDetails.GetFromStringType(from);
        CompletedPaymentLog.Create(PP, "Activation Fee", transId, false, user.Name, amount, Money.Zero, Successful, cryptoCurrencyInfo);
    }
}