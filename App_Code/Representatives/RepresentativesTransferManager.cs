using System;
using Prem.PTC;
using Prem.PTC.Members;
using MemberExtentionMethods;
using SocialNetwork;
using Prem.PTC.Payments;
using Resources;

public class RepresentativesTransferManager
{
    int RepresentativeUserId;
    Member User;
    string targetBalanceName;
    PurchaseBalances targetBalance;

    public RepresentativesTransferManager(int userId, int representativeUserId)
    {
        if (userId == representativeUserId)
            throw new MsgException("You can't send money to yourself.");

        RepresentativeUserId = representativeUserId;
        User = new Member(userId);
        targetBalance = AppSettings.Payments.CashBalanceEnabled ? PurchaseBalances.Cash : PurchaseBalances.Purchase;
        targetBalanceName = targetBalance == PurchaseBalances.Cash ? "Cash Balance" : "Purchase Balance";
    }

    public static Money GetWithdrawalFees(Money amount)
    {
        return Money.MultiplyPercent(amount, AppSettings.Representatives.RepresentativesHelpWithdrawalFee);
    }

    public void InvokeDeposit(Money amount, string message)
    {
        //Anti-Injection Fix
        if (amount <= Money.Zero)
            throw new MsgException(L1.ERROR);

        //Check & subtract the represenative
        Member RepresentativeUser = new Member(RepresentativeUserId);

        if (targetBalance == PurchaseBalances.Cash)
        {
            if (RepresentativeUser.CashBalance < amount)
                throw new MsgException(U6010.REPRESENTATIVENOFUNDS);

            RepresentativeUser.SubtractFromCashBalance(amount, "Representative deposit", BalanceLogType.RepresentativeDeposit);
            RepresentativeUser.SaveBalances();
        }

        if (targetBalance == PurchaseBalances.Purchase)
        {
            if (RepresentativeUser.PurchaseBalance < amount)
                throw new MsgException(U6010.REPRESENTATIVENOFUNDS);

            RepresentativeUser.SubtractFromPurchaseBalance(amount, "Representative deposit", BalanceLogType.RepresentativeDeposit);
            RepresentativeUser.SaveBalances();
        }

        User.SendMessage(RepresentativeUserId, message, MessageType.RepresentativeDepositRequest, RepresentativeRequestStatus.Pending, amount);
    }

    public void InvokeWithdrawal(Money amount, string message)
    {
        User.SubtractFromMainBalance(amount, "Payout via representative");
        User.SaveBalances();

        Money fees = GetWithdrawalFees(amount);

        //Check & add to the represenative
        Member RepresentativeUser = new Member(RepresentativeUserId);
        RepresentativeUser.AddToMainBalance(amount, "Payout via representative");
        RepresentativeUser.SaveBalances();

        User.SendMessage(RepresentativeUserId, message, MessageType.RepresentativeWithdrawalRequest, RepresentativeRequestStatus.Pending,
            amount - fees);
    }

    public string TryConfirmDeposit(ConversationMessage transactionMessage)
    {
        //Make the deposit
        Money CommissionFee = GetWithdrawalFees(transactionMessage.RepresentativeTransferAmount);
        DepositHelper.TransferToBalance(User.Name, transactionMessage.RepresentativeTransferAmount-CommissionFee, "ViaRepresentative", String.Empty, targetBalanceName, true);

        //Deposit fee for Representative
        Member RepresentativeUser = new Member(RepresentativeUserId);
        RepresentativeUser.AddToMainBalance(CommissionFee, "Payout fee for representative deposit");
        RepresentativeUser.SaveBalances();

        //Send message to the user
        RepresentativeUser.SendMessage(User.Id, String.Format(U6010.DEPOSITSUCCESSFULLMESSAGE, transactionMessage.RepresentativeTransferAmount - CommissionFee, User.Name));

        return String.Format(U6010.DEPOSITSUCCESSFULLINFO, transactionMessage.RepresentativeTransferAmount - CommissionFee, User.Name);
    }

    public void TryCancelDeposit(string username, ConversationMessage transactionMessage)
    {
        Member RepresentativeUser = new Member(RepresentativeUserId);

        if (targetBalance == PurchaseBalances.Cash)
        {
            RepresentativeUser.AddToCashBalance(transactionMessage.RepresentativeTransferAmount, "Return for representative deposit cancellation");
            RepresentativeUser.SaveBalances();
        }

        if (targetBalance == PurchaseBalances.Purchase)
        {
            RepresentativeUser.AddToPurchaseBalance(transactionMessage.RepresentativeTransferAmount, "Return for representative deposit cancellation");
            RepresentativeUser.SaveBalances();
        }
    }

    public string ConfirmWithdrawal(ConversationMessage transactionMessage)
    {
        Money amount = transactionMessage.RepresentativeTransferAmount;

        //Lets validate
        //PayoutManager.ValidatePayout(User, amount);
        PayoutManager.CheckMaxPayout(PaymentProcessor.ViaRepresentative, User, amount);

        //Update payout proportions
        PaymentProportionsManager.MemberPaidOut(amount , PaymentProcessor.ViaRepresentative, User);

        History.AddCashout(User.Name, amount);

        User.MoneyCashout += amount;
        User.IsPhoneVerifiedBeforeCashout = false;
        User.CashoutsProceed++;
        User.Save();

        //Add paymentproof
        PaymentProof.Add(User.Id, amount, PaymentType.Manual, PaymentProcessor.ViaRepresentative);

        //Send message to the user
        Member RepresentativeUser = new Member(RepresentativeUserId);
        RepresentativeUser.SendMessage(User.Id, U6010.WITHDRAWALCONFIRMED);

        return U6010.WITHDRAWALCONFIRMED;
    }

    public void RejectWithdrawal(ConversationMessage transactionMessage)
    {
        Money fullAmount = transactionMessage.RepresentativeTransferAmount / ((100-AppSettings.Representatives.RepresentativesHelpWithdrawalFee)/100);

        User.AddToMainBalance(fullAmount, "Representative payout rejected");
        User.SaveBalances();

        //Check & subtract from the represenative
        Member RepresentativeUser = new Member(RepresentativeUserId);
        RepresentativeUser.SubtractFromMainBalance(fullAmount, "Representative payout rejected");
        RepresentativeUser.SaveBalances();
    }
    

}
