using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Memberships;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Resources;
using Prem.PTC.Statistics;
using System.Text;
using Prem.PTC.Offers;
using Titan.Cryptocurrencies;

public class PayoutManager
{
    private Money AmountToPayout;
    private Member User;
    private String TargetPaymentProcessor;

    private bool IsCustomPayoutProcessor;
    private int CustomPayoutProcessorId;
    private string TargetAccount;

    private bool IsAutomaticButAvoveTheLimit = false;
    private Money TheLimit = new Money(0);

    TransactionRequest request = null;
    TransactionResponse response = null;

    public PayoutManager(Member user, Money amount, String pp, bool isCustomPP, int customPPId, string targetCustomAccount)
    {
        this.AmountToPayout = amount;
        this.User = user;
        this.TargetPaymentProcessor = pp;
        this.IsCustomPayoutProcessor = isCustomPP;
        this.CustomPayoutProcessorId = customPPId;
        this.TargetAccount = targetCustomAccount;
    }

    public string TryMakePayout()
    {
        Money withdrawalFee = Money.Zero;

        if (!IsCustomPayoutProcessor)
        {
            var processor = PaymentAccountDetails.GetFirstGateway(TargetPaymentProcessor);
            withdrawalFee = Money.MultiplyPercent(AmountToPayout, processor.WithdrawalFeePercent);

            var address = UsersPaymentProcessorsAddress.GetAddress(User.Id, new PaymentProcessorInfo((PaymentProcessor)Enum.Parse(typeof(PaymentProcessor), processor.AccountType)));
            if (address != null)
            {
                if (TargetAccount != address.PaymentAddress)
                    throw new MsgException("Don't try to use account ID which is different from your current one. To change account ID go to user settings panel.");

                if (address.LastChanged.AddDays(processor.DaysToBlockWithdrawalsAfterAccountChangename) > AppSettings.ServerTime)
                    throw new MsgException(string.Format(U6007.CANTWITHDRAWDAYSLIMIT, (address.LastChanged.AddDays(processor.DaysToBlockWithdrawalsAfterAccountChangename).DayOfYear - AppSettings.ServerTime.DayOfYear)));
            }
            else
                User.SetPaymentAddress(processor.Id, TargetAccount);
        }
        else
        {
            var blockingDays = new CustomPayoutProcessor(CustomPayoutProcessorId).DaysToBlockWithdrawalsAfterAccounChange;
            var address = UsersPaymentProcessorsAddress.GetAddress(User.Id, new PaymentProcessorInfo(CustomPayoutProcessorId));
            if (address != null)
            {
                if (TargetAccount != address.PaymentAddress)
                    throw new MsgException("Don't try to use account ID which is different from your current one. To change account ID go to user settings panel.");

                if (address.LastChanged.AddDays(blockingDays) > AppSettings.ServerTime)
                    throw new MsgException(string.Format(U6007.CANTWITHDRAWDAYSLIMIT, (address.LastChanged.AddDays(blockingDays).DayOfYear - AppSettings.ServerTime.DayOfYear)));
            }
            else
                User.SetPaymentAddress(CustomPayoutProcessorId, TargetAccount);
        }

        ValidatePayout(User, AmountToPayout);

        //Check the gateway limit (global limit, pp custom limit)
        CheckBaseLimits();

        //CLP ---> extension for private client CLP
        User = Titan.CLP.CLPManager.CheckCashoutBonus(User, AmountToPayout);

        if (IsManualPayout()) //Decide if we go with automatic or manual
        {
            return TryMakeManualPayout(withdrawalFee);
        }
        else
        {
            return TryMakeInstantPayout(withdrawalFee);
        }

        return "";
    }

    /// <summary>
    /// This method is also called when withdrawing BTC
    /// </summary>
    /// <param name="user"></param>
    /// <param name="amountToPayout"></param>
    public static void ValidatePayout(Member user, Money amountToPayout)
    {
        //Check the balance
        if (amountToPayout > user.MainBalance)
            throw new MsgException(L1.NOTENOUGHFUNDS);

        ValidatePayoutNotConnectedToAmount(user);

        //Check Max Daily Payout limit for user
        if (user.PaidOutToday + amountToPayout > user.Membership.MaxDailyCashout)
            throw new MsgException(U5002.YOUCANNOTCASHOUTLIMIT + " " + user.PaidOutToday.ToString());

        //Check Max Global Payout limit for user
        if (user.MoneyCashout + amountToPayout > user.Membership.MaxGlobalCashout)
            throw new MsgException(U5006.YOUCANNOTCASHOUTGLOBALLIMIT + " " + user.Membership.MaxGlobalCashout.ToString());

        //Check weekly limit
        if (AppSettings.Payments.RefTiersMaxWeeklyPayoutEnabled)
        {
            var maxBasedOnRefTiers = RefTiersWeeklyPayoutLimitHelper.GetUserLimit(user);
            if (maxBasedOnRefTiers != null && user.PaidOutThisWeek + amountToPayout > maxBasedOnRefTiers)
                throw new MsgException(string.Format(U5008.WEEKLYLIMITERROR, maxBasedOnRefTiers, user.PaidOutThisWeek.ToString()));
        }

    }

    public static void ValidatePayoutNotConnectedToAmount(Member user)
    {
        //Check the status
        if (user.Status == MemberStatus.VacationMode)
            throw new MsgException(U4000.YOUAREINVACATIONMODE);

        //Account verification
        if (AppSettings.Authentication.IsDocumentVerificationEnabled && user.VerificationStatus != VerificationStatus.Verified)
            throw new MsgException(U5006.ACCOUNTNOTVERIFIED);

        if (user.NumberOfPayoutsToday + 1 > user.Membership.MaxDailyPayouts)
            throw new MsgException(string.Format(U6000.TOOMANYWITHDRAWSTODAY, user.Membership.MaxDailyPayouts));

        //Payout Days
        if (!AppSettings.Representatives.RepresentativesIgnoreWitdrawalRules || !user.IsRepresentative())
            PayoutManager.CheckPayoutsDays();

        //Check negative balance
        if (user.IsAnyBalanceIsNegative())
            throw new MsgException(U6012.NEGATIVEBALANCEWITHDRAWAL);

        //Validate Credit Loans
        if (CreditLineManager.UserHasUnpaidLoans(user.Id))
            throw new MsgException(U6008.REPAYCREDITLINETOWITHDRAW);

        //Special Check amount of collected offers from different levels  
        if (TitanFeatures.IsBobbyDonev && CPAOffer.CheckCollectedLevelsAmount(user.Name) < 20)
            throw new MsgException(String.Format("You didn't collect enaugh CPA/GPT offers from different levels yet. You have already collected and confirmed {0} levels.", CPAOffer.CheckCollectedLevelsAmount(user.Name)));

        //Special Check amount of collected offers from different levels
        if (TitanFeatures.IsBobbyDonev && user.CashoutsProceed > 0)
            throw new MsgException("You already did one payout. You can't do more.");

        //Check days restriction between each withdrawals
        var lastWidthdrawalDate = user.GetLastWithdrawalDate();
        if (lastWidthdrawalDate != null && lastWidthdrawalDate > AppSettings.ServerTime.AddDays(-user.Membership.BlockPayoutDays))
            throw new MsgException(string.Format(U6013.TIMEBETWEENPAYOUTERROR, user.Membership.BlockPayoutDays));
    }

    /// <summary>
    /// FROM AP
    /// </summary>
    public static void MarkAsPaid(PayoutRequest request)
    {
        request.IsPaid = true;
        request.Save();

        //Add payment proof
        Member User = new Member(request.Username);
        PaymentProof.Add(User, request);

        //Add history
        History.AddCashout(request.Username, request.Amount);
    }

    /// <summary>
    /// FROM AP
    /// </summary>
    /// <param name="request"></param>
    public static void RejectRequest(PayoutRequest request)
    {
        Member User = new Member(request.Username);

        try
        {
            PaymentProcessor processor = PaymentAccountDetails.GetFromStringType(request.PaymentProcessor);
            PaymentProportionsManager.UndoMemberPaidOut(request.Amount, processor, User);
        }
        catch
        {
            //processor = custom processor
        }

        request.IsPaid = true;
        request.PaymentProcessor = "REJECTED";
        request.Save();

        User.MoneyCashout -= request.Amount;
        User.AddToMainBalance(request.Amount, "Payout rejected");
        User.Save();

        History.AddCashoutRejection(User.Name, request.Amount.ToString());
    }

    /// <summary>
    /// FROM AP
    /// </summary>
    /// <param name="request"></param>
    /// <param name="gateway"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    public static bool MakePayout(PayoutRequest request, PaymentAccountDetails gateway, ref Transaction transaction)
    {
        transaction.Commit();
        if (transaction.Response.IsSuccess)
        {
            request.IsPaid = true;
            request.IsRequest = false;
            request.Save();

            Member User = new Member(request.Username);

            //Add payment proof
            PaymentProof.Add(User, request);

            //Add statistics
            var stats = new Statistics(StatisticsType.Cashflow);
            stats.AddToData2(request.Amount);
            stats.Save();

            //Add history
            History.AddCashout(request.Username, request.Amount);


            if (gateway is PayPalAccountDetails)


                // Log because payment may have response status SuccessWithWarning
                // More on this: https://developer.paypal.com/webapps/developer/docs/classic/api/NVPAPIOverview/
                logPayout("Payout successful", request, transaction.Response, request.PaymentProcessor);

            return true;
        }
        else
        {
            logPayout("Payout unsuccessful", request, transaction.Response, request.PaymentProcessor);
            return false;
        }
    }

    public bool TryMakeInvestmentLevelsPayout()
    {
        // payoutRequest --> change property to false (it isn't request)
        var req = new PayoutRequest
        {
            Amount = AmountToPayout,
            IsPaid = true,
            RequestDate = DateTime.Now,
            Username = User.Name,
            IsRequest = false,
            BalanceType = BalanceType.InvestmentLevels
        };

        //User payment address
        string paymentAddress = PaymentAccountDetails.GetPaymentProcessorUserAccount(User, TargetPaymentProcessor);

        if (String.IsNullOrEmpty(paymentAddress))
            throw new MsgException(U5004.YOUMUST);

        request = new TransactionRequest(User.Name, paymentAddress, AmountToPayout);
        Transaction transaction = TransactionFactory.CreateTransaction(request, TargetPaymentProcessor);
        response = transaction.Commit();

        req.PaymentAddress = paymentAddress;
        req.PaymentProcessor = TargetPaymentProcessor;

        if (!response.IsSuccess)
        {
            if (request != null && response != null)
                logPayout("Payout unsuccessful", request, response, req.PaymentProcessor);
            return false;
        }
        req.Save();
        
        History.AddInvestmentLevelCashout(User.Name, AmountToPayout);        
        
        //Add to daily cashout
        AppSettings.Payments.GlobalCashoutsToday += AmountToPayout;
        AppSettings.Payments.Save();

        //Add outcome to stats (Data2);
        var stats = new Statistics(StatisticsType.Cashflow);
        stats.AddToData2(AmountToPayout);
        stats.Save();

        //Add paymentproof
        PaymentProof.Add(User.Id, AmountToPayout, PaymentType.Instant, PaymentAccountDetails.GetFromStringType(TargetPaymentProcessor));

        // Log because payment may have response status SuccessWithWarning
        // More on this: https://developer.paypal.com/webapps/developer/docs/classic/api/NVPAPIOverview/
        logPayout("Payout successful", request, response, req.PaymentProcessor);

        ErrorLogger.Log(string.Format("{0}: {1}", U3501.AUTOMATICCASHOUTSUCC, response.Note));
        return true;
    }

    private string TryMakeInstantPayout(Money fee)
    {
        // payoutRequest --> change property to false (it isn't request)
        PayoutRequest req = new PayoutRequest();

        req.Amount = AmountToPayout - fee;
        req.IsPaid = true;
        req.RequestDate = DateTime.Now;
        req.Username = User.Name;
        req.IsRequest = false;
        req.BalanceType = BalanceType.MainBalance;

        //Check if daily limit is not reached
        if (AppSettings.Payments.GlobalCashoutsToday + AmountToPayout - fee > AppSettings.Payments.GlobalCashoutLimitPerDay)
            throw new MsgException(L1.TOOMANYCASHOUTS + " " + AppSettings.Payments.GlobalCashoutLimitPerDay.ToString());

        //User payment address
        string paymentAddress = PaymentAccountDetails.GetPaymentProcessorUserAccount(User, TargetPaymentProcessor);

        if (String.IsNullOrEmpty(paymentAddress))
            throw new MsgException(U5004.YOUMUST);

        request = new TransactionRequest(User.Name, paymentAddress, AmountToPayout - fee);
        Transaction transaction = TransactionFactory.CreateTransaction(request, TargetPaymentProcessor);
        response = transaction.Commit();

        req.PaymentAddress = paymentAddress;
        req.PaymentProcessor = TargetPaymentProcessor;

        if (!response.IsSuccess)
        {
            if (request != null && response != null)
                PayoutManager.logPayout("Payout unsuccessful", request, response, req.PaymentProcessor);
            throw new MsgException(response.Note);
        }

        req.Save();

        History.AddCashout(User.Name, AmountToPayout);

        User.SubtractFromMainBalance(AmountToPayout, "Payout");
        User.MoneyCashout += AmountToPayout - fee;
        User.IsPhoneVerifiedBeforeCashout = false;
        User.CashoutsProceed++;
        User.Save();

        //Update payout proportions
        PaymentProportionsManager.MemberPaidOut(AmountToPayout - fee, PaymentAccountDetails.GetFromStringType(TargetPaymentProcessor), User, IsCustomPayoutProcessor);

        //Add to daily cashout
        AppSettings.Payments.GlobalCashoutsToday += AmountToPayout - fee;
        AppSettings.Payments.Save();

        //Add outcome to stats (Data2);
        var stats = new Statistics(StatisticsType.Cashflow);
        stats.AddToData2(AmountToPayout);
        stats.Save();

        //Add paymentproof
        PaymentProof.Add(User.Id, AmountToPayout, PaymentType.Instant, PaymentAccountDetails.GetFromStringType(TargetPaymentProcessor));

        // Log because payment may have response status SuccessWithWarning
        // More on this: https://developer.paypal.com/webapps/developer/docs/classic/api/NVPAPIOverview/
        logPayout("Payout successful", request, response, req.PaymentProcessor);

        return U3501.AUTOMATICCASHOUTSUCC + ": " + response.Note;
    }

    private string TryMakeManualPayout(Money fee)
    {
        //Manual, add to payoutrequests
        PayoutRequest req = new PayoutRequest();

        //Calculate fees for CustomPP
        Money Fees = fee;
        if (IsCustomPayoutProcessor)
        {
            CustomPayoutProcessor CPP = new CustomPayoutProcessor(CustomPayoutProcessorId);
            Fees = Fees + CPP.MoneyFee;
            Fees = Fees + (AmountToPayout * (CPP.PercentageFee * new Money(0.01)));

            if (string.IsNullOrWhiteSpace(TargetAccount))
                throw new MsgException(U4200.ACCOUNTFIELDNOTBLANK);
        }

        req.Amount = AmountToPayout - Fees;
        req.IsPaid = false;
        req.RequestDate = DateTime.Now;
        req.Username = User.Name;
        req.IsRequest = true;
        req.BalanceType = BalanceType.MainBalance;

        if (IsCustomPayoutProcessor)
        {
            //CustomPP
            CustomPayoutProcessor CPP = new CustomPayoutProcessor(CustomPayoutProcessorId);
            req.PaymentAddress = TargetAccount;
            req.PaymentProcessor = CPP.Name;

            //MPesa check
            if (CPP.Name.ToLower() == "mpesa" && (TargetAccount.Length != 10 || !TargetAccount.StartsWith("07")))
                throw new MsgException("Check your phone number format. The number should be 07******** (10 length)");
        }
        else
        {
            string paymentAddress = PaymentAccountDetails.GetPaymentProcessorUserAccount(User, TargetPaymentProcessor);

            if (String.IsNullOrEmpty(paymentAddress))
                throw new MsgException(U5004.YOUMUST);

            if (TargetPaymentProcessor == "Payeer" && paymentAddress.Contains("@"))
                throw new MsgException(U6006.VALIDPAYEER);

            req.PaymentAddress = paymentAddress;
            req.PaymentProcessor = TargetPaymentProcessor;
        }

        req.Save();

        User.SubtractFromMainBalance(AmountToPayout, "Payout");
        User.MoneyCashout += (AmountToPayout - Fees);
        User.IsPhoneVerifiedBeforeCashout = false;
        User.CashoutsProceed++;
        User.Save();

        //Update payout proportions
        if (!IsCustomPayoutProcessor)
            PaymentProportionsManager.MemberPaidOut(AmountToPayout - Fees, PaymentAccountDetails.GetFromStringType(TargetPaymentProcessor), User, IsCustomPayoutProcessor);

        if (IsAutomaticButAvoveTheLimit)
            return U3501.CASHOUTSUCC + ": " + U3500.CASHOUT_APPROVE.Replace("%n%", TheLimit.ToString());
        else
            return U3501.CASHOUTSUCC + ": " + U3500.CASHOUT_MESSAGE.Replace("%n1%", (AmountToPayout - Fees).ToString()).Replace("%n2%", Fees.ToString());
    }

    public bool IsManualPayout()
    {
        return IsManual(this.User, this.IsCustomPayoutProcessor, this.IsAutomaticButAvoveTheLimit);
    }

    public static bool IsManualCryptocurrencyPayout(Member user)
    {
        return IsManual(user, false, false, true);
    }

    private static bool IsManual(Member user, bool isCustomPayoutProcessor = true, bool isAutomaticButAvoveTheLimit = true, bool isCryptocurrency = false)
    {
        if (!AppSettings.Payments.IsInstantPayout)
            return true;

        if (!isCryptocurrency)
        {
            if (isCustomPayoutProcessor)
                return true;

            if (isAutomaticButAvoveTheLimit)
                return true;
        }

        //Now memberships
        if (!user.Membership.HasInstantPayout)
        {
            //Now check Bonus requirements
            if (user.CashoutsProceed < AppSettings.Payments.InstantPayoutMinCashoutsNumber)
                return true;

            if (user.TotalCPACompleted < AppSettings.Payments.InstantPayoutMinOffersCompleted)
                return true;

            if (DateTime.Now.Subtract(user.Registered).Days < AppSettings.Payments.InstantPayoutMinRegisteredDays)
                return true;
        }

        if (user.IsExcludedFromInstantPayment)
            return true;

        return false;
    }

    public bool HasManualPayout()
    {
        CheckBaseLimits();

        if (IsManualPayout())
            return true;
        return false;
    }

    private void CheckBaseLimits()
    {
        if (IsCustomPayoutProcessor)
            CheckMaxPayout(PaymentProcessor.CustomPayoutProcessor, User, AmountToPayout, CustomPayoutProcessorId);
        else
            CheckMaxPayout(PaymentAccountDetails.GetFromStringType(TargetPaymentProcessor), User, AmountToPayout);

        PaymentAccountDetails TheGateway = null;

        if (!IsCustomPayoutProcessor)
        {
            TheGateway = PaymentAccountDetails.GetFirstGateway(TargetPaymentProcessor);
        }

        if (TheGateway == null && !IsCustomPayoutProcessor)
            throw new MsgException("Sorry, no payment gateway available at the moment. Please try again soon.");

        Money minPayout = Money.Zero;

        if (IsCustomPayoutProcessor)
            minPayout = PayoutLimit.GetProperLimitValue(User, new CustomPayoutProcessor(CustomPayoutProcessorId));
        else
            minPayout = PayoutLimit.GetProperLimitValue(User, TheGateway);

        if (AmountToPayout < minPayout)
            throw new MsgException(U2502.AMOUNTHIGHER + ": " + minPayout.ToString());

        //Cashout limits for automatic
        if (TheGateway != null && AppSettings.Payments.IsInstantPayout)
        {
            TheLimit = TheGateway.ManualCashoutAfterExceeding;

            if (AmountToPayout > TheGateway.ManualCashoutAfterExceeding)
                IsAutomaticButAvoveTheLimit = true;
        }
    }

    public static void logPayout(string message, TransactionRequest request, TransactionResponse response,
        string paymentProcessor)
    {
        PaymentProcessor processor = PaymentAccountDetails.GetFromStringType(paymentProcessor);

        ErrorLogger.Log(String.Format(@"{0}:
                        PayoutRequest ID: {1}
                        Raw response: {2}", message, request.PayeeId, response.RawResponse), ErrorLoggerHelper.GetTypeFromProcessor(processor), true);
    }

    public static void CheckMaxPayout(PaymentProcessor processor, Member user, Money amountToPayout, int CustomPayoutProcessorId = -1)
    {
        var errorNote = "";
        Money maxPayout = GetMaxPossiblePayout(processor, user, out errorNote);

        if (processor == PaymentProcessor.CustomPayoutProcessor && CustomPayoutProcessorId != -1)
        {
            var CustomProcessor = new CustomPayoutProcessor(CustomPayoutProcessorId);
            CustomProcessor.CheckMaxValueOfPendingRequestsPerDay(amountToPayout);
        }

        if (amountToPayout > maxPayout)
            throw new MsgException(errorNote);
    }

    public static Money GetMaxPossiblePayout(PaymentProcessor processor, Member user, out string errorNote)
    {
        PaymentProportionsManager manager = new PaymentProportionsManager(user);

        decimal maxDailyPayout = user.Membership.MaxDailyCashout.ToDecimal() - user.PaidOutToday.ToDecimal();
        if (maxDailyPayout < 0)
            maxDailyPayout = 0;

        decimal maxGlobalPayout = user.Membership.MaxGlobalCashout.ToDecimal() - user.MoneyCashout.ToDecimal();

        decimal maxFromProcessor = 0m;
        decimal maxSinglePayout = 0m;
        decimal maxBasedOnAdPacks = 0m;

        if (AppSettings.Payments.MaximumPayoutPolicy == MaximumPayoutPolicy.Constant)
            maxSinglePayout = AppSettings.Payments.MaximumPayoutConstant.ToDecimal();
        else if (AppSettings.Payments.MaximumPayoutPolicy == MaximumPayoutPolicy.Percentage)
            maxSinglePayout = Money.MultiplyPercent(user.MainBalance, AppSettings.Payments.MaximumPayoutPercentage).ToDecimal();

        Dictionary<string, decimal> dic = new Dictionary<string, decimal>();
        dic.Add(L1.MEMBERSHIP, maxDailyPayout);
        dic.Add(U6011.PAYMENTS, maxSinglePayout);
        dic.Add(U5002.YOUCANNOTCASHOUTLIMIT, maxGlobalPayout);

        if (AppSettings.Payments.ProportionalPayoutLimitsEnabled && processor != PaymentProcessor.CustomPayoutProcessor)
        {
            maxFromProcessor = manager.GetMaximum(processor).ToDecimal();
            dic.Add(string.Format(U6011.PROPORTIONALLIMITSPROCESSOR), maxFromProcessor);
        }

        if (AppSettings.Payments.AdPackTypeWithdrawLimitEnabled)
        {
            maxBasedOnAdPacks = (AdPackTypeManager.GetWithdrawalLimit(user.Id) - user.MoneyCashout).ToDecimal();

            if (maxBasedOnAdPacks < 0m)
                maxBasedOnAdPacks = 0m;

            dic.Add(AppSettings.RevShare.AdPack.AdPackNamePlural, maxBasedOnAdPacks);
        }

        //Maximum withdrawal of deposited amount %
        if (user.Membership.MaxWithdrawalAllowedPerInvestmentPercent < 1000000000)
        {
            PaymentProportionsManager ppm = new PaymentProportionsManager(user);
            Money invested = ppm.TotalPaidIn;
            Money withdrawn = ppm.TotalPaidOut;
            Money canwithdraw = Money.MultiplyPercent(invested, user.Membership.MaxWithdrawalAllowedPerInvestmentPercent);

            dic.Add(U6011.MEMBERSHIPMAXWITDRAWAL, (canwithdraw - withdrawn).ToDecimal());
        }

        var min = dic.OrderBy(x => x.Value).First();
        var money = new Money(min.Value);
        errorNote = string.Format(U6011.PAYOUTREQUESTTOOHIGH, money.ToString(), min.Key);

        if (money > user.MainBalance)
        {
            errorNote = string.Format(U6012.PAYOUTREQUESTBALANCEERROR, user.MainBalance.ToString());
            money = user.MainBalance;
        }

        return money > Money.Zero ? money : Money.Zero;
    }

    public static void CheckPayoutsDays()
    {
        if (!(AppSettings.Payments.GetAllowedPaymentDay(AppSettings.ServerTime.DayOfWeek)))
            throw new MsgException(string.Format(U6004.NOWITHDRAWTODAY, GetPayoutDays()));
    }

    public static string GetPayoutDays()
    {
        StringBuilder daysOfWeek = new StringBuilder();
        if (!(AppSettings.Payments.GetAllowedPaymentDay(AppSettings.ServerTime.DayOfWeek)))
        {
            if (AppSettings.Payments.GetAllowedPaymentDay(DayOfWeek.Monday))
                daysOfWeek.Append(" " + U6004.MONDAY);
            if (AppSettings.Payments.GetAllowedPaymentDay(DayOfWeek.Tuesday))
                daysOfWeek.Append(" " + U6004.TUESDAY);
            if (AppSettings.Payments.GetAllowedPaymentDay(DayOfWeek.Wednesday))
                daysOfWeek.Append(" " + U6004.WEDNESDAY);
            if (AppSettings.Payments.GetAllowedPaymentDay(DayOfWeek.Thursday))
                daysOfWeek.Append(" " + U6004.THURSDAY);
            if (AppSettings.Payments.GetAllowedPaymentDay(DayOfWeek.Friday))
                daysOfWeek.Append(" " + U6004.FRIDAY);
            if (AppSettings.Payments.GetAllowedPaymentDay(DayOfWeek.Saturday))
                daysOfWeek.Append(" " + U6004.SATURDAY);
            if (AppSettings.Payments.GetAllowedPaymentDay(DayOfWeek.Sunday))
                daysOfWeek.Append(" " + U6004.SUNDAY);
        }
        return daysOfWeek.ToString();
    }
}