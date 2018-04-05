using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Cryptocurrencies;
using Titan.CryptocurrencyPlatform;

public class CryptocurrencyPlatformManager
{
    public static void TryPlaceOrder(int OfferId, CryptocurrencyMoney CCAmount, String SellerDescription)
    {
        //Loading selected offer
        var SelectedOffer = new CryptocurrencyTradeOffer(OfferId);
        var OfferCreator = new Member(SelectedOffer.CreatorId);

        //Checking Creator's Balance
        if (SelectedOffer.OfferKind == CryptocurrencyOfferType.Buy)
        {
            if (SelectedOffer.CreatorId != Member.CurrentId)
                if (OfferCreator.GetCryptocurrencyBalance(CryptocurrencyType.BTC) < CCAmount)
                    throw new MsgException(U6010.ORDER_CREATORNOBALANCE);
        }
        else
        {
            if (SelectedOffer.CreatorId == Member.CurrentId)
                if (OfferCreator.GetCryptocurrencyBalance(CryptocurrencyType.BTC) < CCAmount)
                    throw new MsgException(U6010.ORDER_YOUNOBALANCE);
        }

        //If everything is good, creating transaction
        CryptocurrencyTradeTransaction NewTransaction = new CryptocurrencyTradeTransaction()
        {
            OfferId = OfferId,
            ClientId = Member.CurrentId,
            ExecutionTime = AppSettings.ServerTime,
            PaymentStatus = CryptocurrencyTransactionStatus.AwaitingPayment,
            CCAmount = CCAmount,
            SellerDescription = SellerDescription
        };

        //Freezing cryptocurrency for transaction time
        //Current user clicked sell
        if(SelectedOffer.OfferKind == CryptocurrencyOfferType.Buy)
            Member.Current.SubtractFromCryptocurrencyBalance(CryptocurrencyType.BTC, CCAmount.ToDecimal(), "Cryptocurrency trade", BalanceLogType.CryptocurrencyTrade);
        //Current user clicked buy
        else
            OfferCreator.SubtractFromCryptocurrencyBalance(CryptocurrencyType.BTC, CCAmount.ToDecimal(), "Cryptocurrency trade", BalanceLogType.CryptocurrencyTrade);

        //Descreasing existing offer CC amount left 
        SelectedOffer.AmountLeft = SelectedOffer.AmountLeft - CCAmount;
        if (SelectedOffer.AmountLeft <= CryptocurrencyMoney.Zero)
            SelectedOffer.Status = CryptocurrencyOfferStatus.Finished;
        SelectedOffer.Save();

        //Saving transaction, ESCROW starts here
        NewTransaction.Save(true);
    }

    public static int GetNumberOfTransactions(int userId)
    {
        return Convert.ToInt32(TableHelper.SelectScalar(String.Format(@"SELECT COUNT(*) FROM CryptoCurrencyTradeTransactions WHERE 
                                                                            OfferId IN (SELECT OfferId FROM CryptocurrencyFinishedTradeOffers WHERE (BuyerId={0} OR SellerId={0})) AND
                                                                            PaymentStatus IN ({1},{2},{3})", 
                                                                            userId,
                                                                            (int)CryptocurrencyTransactionStatus.Finished,
                                                                            (int)CryptocurrencyTransactionStatus.SolvedCryptocurrencyToOfferOwner,
                                                                            (int)CryptocurrencyTransactionStatus.SolvedCryptocurrencyToOfferClient)));
    }

    public static String GetHtmlRatingStringForUser(int userId)
    {
        return HtmlRatingGenerator.GenerateHtmlRating(RatingType.CryptocurrencyTrading, userId);
    }

    public static int GetNumberOfPositiveTransactions(int userId)
    {
        return GetNumberOfPositiveBuys(userId) + GetNumberOfPositiveSells(userId);
    }

    public static int GetNumberOfPositiveBuys(int userId)
    {
        return Convert.ToInt32(TableHelper.SelectScalar(GenerateQueryStringForNumberOfPositiveActions(CryptocurrencyOfferType.Buy, userId)));
    }

    public static int GetNumberOfPositiveSells(int userId)
    {
        return Convert.ToInt32(TableHelper.SelectScalar(GenerateQueryStringForNumberOfPositiveActions(CryptocurrencyOfferType.Sell, userId)));
               
    }

    private static String GenerateQueryStringForNumberOfPositiveActions(CryptocurrencyOfferType type, int userId)
    {
        String TypeOfUser = type == CryptocurrencyOfferType.Buy ? "BuyerId" : "SellerId";
        CryptocurrencyTransactionStatus TypeOfTransactionDispute = type == CryptocurrencyOfferType.Buy ? 
                                                                        CryptocurrencyTransactionStatus.SolvedCryptocurrencyToOfferOwner : 
                                                                        CryptocurrencyTransactionStatus.SolvedCryptocurrencyToOfferClient;

        String Query = String.Format(@"SELECT COUNT(*) FROM CryptoCurrencyTradeTransactions WHERE 
                                                                            OfferId IN (SELECT OfferId FROM CryptocurrencyFinishedTradeOffers WHERE {0}={1}) AND
                                                                            PaymentStatus IN ({2},{3})",
                                                                            TypeOfUser,
                                                                            userId,
                                                                            (int)CryptocurrencyTransactionStatus.Finished,
                                                                            (int)TypeOfTransactionDispute);

        return Query;
    }

    public static RatingMemberInfo LoadNewUserData(int userId)
    {
        int TotalActions = CryptocurrencyPlatformManager.GetNumberOfTransactions(userId);
        int TotalPositiveActions = CryptocurrencyPlatformManager.GetNumberOfPositiveTransactions(userId);

        return new RatingMemberInfo(TotalActions, TotalPositiveActions);
    }

    public static void CRON()
    {
        // IF CC ENABLED ?
        CryptocurrencyTradeTransaction.CheckIfEscrowPassedForAllActiveTransactions();
    }

}