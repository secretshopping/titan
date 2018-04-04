﻿using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Balances;
using Titan.Cryptocurrencies;
using Titan.ICO;

namespace Titan.InternalExchange
{
    public static class InternalExchangeManager
    {
        #region Queries

        private static String QueryAskProcedureFormat
        {
            get
            {
                return "EXEC up_InternalExchangeAsk @AskUserId = {0}, @AskAmount = {1}, @AskValue = {2}";
            }
        }

        private static String QueryBidProcedureFormat
        {
            get
            {
                return "EXEC up_InternalExchangeBid @BidUserId = {0}, @BidAmount = {1}, @BidValue = {2}";
            }
        }

        private static String QueryAskWithdrawProcedureFormat
        {
            get
            {
                return "EXEC up_InternalExchangeWithdrawAsk @OfferId = {0}, @UserId = {1}, @ForceAdmin = {2}";
            }
        }

        private static String QueryBidWithdrawProcedureFormat
        {
            get
            {
                return "EXEC up_InternalExchangeWithdrawBid @OfferId = {0}, @UserId = {1}, @ForceAdmin = {2}";
            }
        }

        private static String QueryGetUserInternalExchangeAskAmountFunctionFormat
        {
            get
            {
                return "SELECT dbo.uf_GetUserInternalExchangeAskAmount({0})";
            }
        }

        private static InternalExchangeOfferResponse ExecuteAskOfferQuery(int userId, decimal numberOfStock, decimal pricePerStock)
        {
            string query = QueryAskProcedureFormat;
            query = string.Format(query, userId, numberOfStock, pricePerStock);

            InternalExchangeOfferResponse response = TableHelper.GetListFromRawQuery<InternalExchangeOfferResponse>(query).FirstOrDefault();
            return response;
        }

        private static InternalExchangeOfferResponse ExecuteBidOfferQuery(int userId, decimal numberOfStock, decimal pricePerStock)
        {
            string query = QueryBidProcedureFormat;
            query = string.Format(query, userId, numberOfStock, pricePerStock);

            InternalExchangeOfferResponse response = TableHelper.GetListFromRawQuery<InternalExchangeOfferResponse>(query).FirstOrDefault();
            return response;
        }

        /// <summary>
        /// Withdrawing offer form exchange and reurns assets.
        /// userId is only to prvent anyone else then creator of an offer to withdraw it.
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="userId"></param>
        private static void ExecuteAskWithdrawOfferQuery(int offerId, int userId, bool forceAdmin)
        {
            string query = QueryAskWithdrawProcedureFormat;
            query = string.Format(query, offerId, userId, forceAdmin);

            TableHelper.ExecuteRawCommandNonQuery(query);
        }

        /// <summary>
        /// Withdrawing offer form exchange and reurns assets.
        /// userId is only to prvent anyone else then creator of an offer to withdraw it.
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="userId"></param>
        private static void ExecuteBidWithdrawOfferQuery(int offerId, int userId, bool forceAdmin)
        {
            string query = QueryBidWithdrawProcedureFormat;
            query = string.Format(query, offerId, userId, forceAdmin);

            TableHelper.ExecuteRawCommandNonQuery(query);
        }

        private static decimal SelectGetUserInternalExchangeAskAmount(int userId)
        {
            string query = QueryGetUserInternalExchangeAskAmountFunctionFormat;
            query = string.Format(query, userId);

            return (decimal)TableHelper.SelectScalar(query);
        }

        #endregion

        public static decimal GetAvailableFunds(Member currentUser)
        {
            return currentUser.GetDecimalBalance(AppSettings.InternalExchange.InternalExchangePurchaseVia);
        }

        public static decimal GetAvailableStock(Member currentUser)
        {
            return currentUser.GetDecimalBalance(AppSettings.InternalExchange.InternalExchangeStockType);
        }

        public static String GetBalanceSign(bool isStock)
        {
            BalanceType targetBalance = isStock == true ? AppSettings.InternalExchange.InternalExchangeStockType : AppSettings.InternalExchange.InternalExchangePurchaseVia;

            if (BalanceTypeHelper.IsCryptoBalance(targetBalance))
            {
                Cryptocurrency crypto = CryptocurrencyFactory.Get(CryptocurrencyTypeHelper.ConvertToCryptocurrencyType(targetBalance));
                if (String.IsNullOrEmpty(crypto.CurrencyDisplaySignBefore))
                    return crypto.CurrencyDisplaySignAfter.Trim();
                return crypto.CurrencyDisplaySignBefore.Trim();
            }
            return AppSettings.Site.CurrencySign.Trim();
        }

        public static String GetBalanceCode(bool isStock)
        {
            BalanceType targetBalance = isStock == true ? AppSettings.InternalExchange.InternalExchangeStockType : AppSettings.InternalExchange.InternalExchangePurchaseVia;

            if (BalanceTypeHelper.IsCryptoBalance(targetBalance))
                return CryptocurrencyFactory.Get(CryptocurrencyTypeHelper.ConvertToCryptocurrencyType(targetBalance)).Code;

            return AppSettings.Site.CurrencyCode;
        }

        public static String RecognizeCurrencyAndReturnString(bool asStock, decimal value)
        {
            BalanceType targetType = asStock ? AppSettings.InternalExchange.InternalExchangeStockType : AppSettings.InternalExchange.InternalExchangePurchaseVia;

            if (BalanceTypeHelper.IsCryptoBalance(targetType))
                return new CryptocurrencyMoney(CryptocurrencyTypeHelper.ConvertToCryptocurrencyType(targetType), value).ToString();

            return new Money(value).ToString();
        }

        public static String RecognizeCurrencyAndReturnString(bool asStock, String value)
        {
            return RecognizeCurrencyAndReturnString(asStock, Decimal.Parse(value));
        }

        public static Money GetOfferFee(decimal numberOfStock, decimal pricePerStock, bool isAsk)
        {
            decimal commission = isAsk
                ? AppSettings.InternalExchange.InternalExchangeAskCommissionPercent
                : AppSettings.InternalExchange.InternalExchangeBidCommissionPercent;

            return new Money(numberOfStock * pricePerStock * (commission / 100m));
        }

        

        private static InternalExchangeOfferResponse PlaceAskOffer(int userId, decimal numberOfStock, decimal pricePerStock)
        {
            decimal commission = AppSettings.InternalExchange.InternalExchangeAskCommissionPercent;
            Money transactionFee = GetOfferFee(numberOfStock, pricePerStock, true);

            Member user = new Member(userId);

            if (GetAvailableStock(user) >= numberOfStock
                && GetAvailableFunds(user) >= transactionFee.ToDecimal())
            {
                InternalExchangeOfferResponse response = ExecuteAskOfferQuery(userId, numberOfStock, pricePerStock);

                //if(GetBalanceCode(false) == AppSettings.Site.CurrencyCode)
                //    PoolDistributionManager.AddProfit(ProfitSource.InternalExchangeFee, transactionFee);

                return response;
            }
            else
            {
                throw new MsgException(L1.NOTENOUGHFUNDS);
            }
        }

        private static InternalExchangeOfferResponse PlaceBidOffer(int userId, decimal numberOfStock, decimal pricePerStock)
        {
            decimal commission = AppSettings.InternalExchange.InternalExchangeBidCommissionPercent;
            Money transactionValue = new Money(numberOfStock * pricePerStock);
            Money transactionFee = GetOfferFee(numberOfStock, pricePerStock, false);

            Member user = new Member(userId);

            if (GetAvailableFunds(user) >= (transactionValue + transactionFee).ToDecimal())
            {
                InternalExchangeOfferResponse response = ExecuteBidOfferQuery(userId, numberOfStock, pricePerStock);

                //if (GetBalanceCode(false) == AppSettings.Site.CurrencyCode)
                //    PoolDistributionManager.AddProfit(ProfitSource.InternalExchangeFee, transactionFee);

                return response;
            }
            else
            {
                throw new MsgException(L1.NOTENOUGHFUNDS);
            }
        }

        public static InternalExchangeOfferResponse TryPlaceOrder(int currentId, decimal numberOfStock, decimal valueOfStock, bool isAsk)
        {
            if(isAsk)
                return PlaceAskOffer(currentId, numberOfStock, valueOfStock);
            else
                return PlaceBidOffer(currentId, numberOfStock, valueOfStock);
        }
        
        public static void WithdrawOffer(int offerId, bool isAsk)
        {
            WithdrawOffer(offerId, Member.CurrentId, false, isAsk);
        }

        public static void WithdrawOffer(int offerId, int userId, bool forceAdmin, bool isAsk)
        {
            if (isAsk)
            {
                ExecuteAskWithdrawOfferQuery(offerId, userId, forceAdmin);
            }
            else
            {
                ExecuteBidWithdrawOfferQuery(offerId, userId, forceAdmin);
            }
        }

        public static decimal GetUserInternalExchangeAskAmount(int userId)
        {
            return SelectGetUserInternalExchangeAskAmount(userId);
        }

        public static String GetTestChartData()
        {
//return @"Date,Open,High,Low,Close,Volume
//6-Jun-14T05:00,50,100,45,100,666;";
return @"Date,Open,High,Low,Close,Volume
9-Jun-14T05:00,62.40,63.34,61.79,62.88,37617413
6-Jun-14T05:00,63.37,63.48,62.15,62.50,42442096
5-Jun-14T05:00,63.66,64.36,62.82,63.19,47352368
4-Jun-14T05:00,62.45,63.59,62.07,63.34,36513991
3-Jun-14T05:00,62.62,63.42,62.32,62.87,32216707
2-Jun-14T05:00,63.23,63.59,62.05,63.08,35995537
30-May-14T05:00,63.95,64.17,62.56,63.30,45283577
29-May-14T05:00,63.84,64.30,63.51,63.83,42699670
28-May-14T05:00,63.39,64.14,62.62,63.51,47795088
27-May-14T05:00,61.62,63.51,61.57,63.48,55681663
23-May-14T05:00,60.41,61.45,60.15,61.35,38293993
22-May-14T05:00,60.94,61.48,60.40,60.52,54200116
21-May-14T05:00,58.56,60.50,58.25,60.49,58991505
20-May-14T05:00,59.50,60.19,58.18,58.56,53931469
19-May-14T05:00,57.89,59.56,57.57,59.21,43033925
16-May-14T05:00,58.31,58.45,57.31,58.02,47933075
15-May-14T05:00,59.26,59.38,57.52,57.92,56813940
14-May-14T05:00,59.53,60.45,58.95,59.23,47428583
13-May-14T05:00,59.66,60.89,59.51,59.83,48525476
12-May-14T05:00,57.98,59.90,57.98,59.83,48575487
9-May-14T05:00,56.85,57.65,56.38,57.24,52583858
8-May-14T05:00,57.23,58.82,56.50,56.76,61251053
7-May-14T05:00,58.77,59.30,56.26,57.39,78587247
6-May-14T05:00,60.98,61.15,58.49,58.53,55900809
5-May-14T05:00,59.67,61.35,59.18,61.22,46057411
2-May-14T05:00,61.30,61.89,60.18,60.46,54189197
1-May-14T05:00,60.43,62.28,60.21,61.15,82428606
30-Apr-14T05:00,57.58,59.85,57.16,59.78,76093004
29-Apr-14T05:00,56.09,58.28,55.84,58.15,75557202
28-Apr-14T05:00,58.05,58.31,54.66,56.14,107757756
25-Apr-14T05:00,59.97,60.01,57.57,57.71,92501529
24-Apr-14T05:00,63.60,63.65,59.77,60.87,138769345
23-Apr-14T05:00,63.45,63.48,61.26,61.36,96564750
22-Apr-14T05:00,62.65,63.44,62.22,63.03,60631312
21-Apr-14T05:00,59.46,61.24,59.15,61.24,60363619
17-Apr-14T05:00,59.30,60.58,58.72,58.94,88040346
16-Apr-14T05:00,59.79,60.19,57.74,59.72,78773521
15-Apr-14T05:00,59.29,59.68,55.88,59.09,108622706
14-Apr-14T05:00,60.09,60.45,57.78,58.89,72324603
11-Apr-14T05:00,57.60,60.31,57.31,58.53,91451960
10-Apr-14T05:00,63.08,63.18,58.68,59.16,114987616
9-Apr-14T05:00,59.63,62.46,59.19,62.41,100215307
8-Apr-14T05:00,57.68,58.71,57.17,58.19,78835935
7-Apr-14T05:00,55.90,58.00,55.44,56.95,108487569
4-Apr-14T05:00,59.94,60.20,56.32,56.75,125465774
3-Apr-14T05:00,62.55,63.17,59.13,59.49,83859330
2-Apr-14T05:00,63.21,63.91,62.21,62.72,66276613
1-Apr-14T05:00,60.46,62.66,60.24,62.62,59291210
31-Mar-14T05:00,60.78,61.52,59.87,60.24,53011205
28-Mar-14T05:00,61.34,61.95,59.34,60.01,67051528
27-Mar-14T05:00,60.51,61.90,57.98,60.97,112649694
26-Mar-14T05:00,64.74,64.95,60.37,60.38,97689774
25-Mar-14T05:00,64.89,66.19,63.78,64.89,68785500
24-Mar-14T05:00,67.19,67.36,63.36,64.10,85695872
21-Mar-14T05:00,67.53,67.92,66.18,67.24,60041228
20-Mar-14T05:00,68.01,68.23,66.82,66.97,44438500
19-Mar-14T05:00,69.17,69.29,67.46,68.24,43980558
18-Mar-14T05:00,68.76,69.60,68.30,69.19,40827226
17-Mar-14T05:00,68.18,68.95,66.62,68.74,52196699
14-Mar-14T05:00,68.49,69.43,67.46,67.72,48226824
13-Mar-14T05:00,71.29,71.35,68.15,68.83,57091157
12-Mar-14T05:00,69.86,71.35,69.00,70.88,46400431
11-Mar-14T05:00,72.50,72.59,69.96,70.10,59615238
10-Mar-14T05:00,70.77,72.15,70.51,72.03,59949746
7-Mar-14T05:00,71.08,71.18,69.47,69.80,38985763
6-Mar-14T05:00,71.88,71.89,70.25,70.84,46126260
5-Mar-14T05:00,69.69,71.97,69.62,71.57,74649486
4-Mar-14T05:00,68.66,68.90,67.62,68.80,42164222
3-Mar-14T05:00,66.96,68.05,66.51,67.41,56900444
28-Feb-14T05:00,69.47,69.88,67.38,68.46,66900863
27-Feb-14T05:00,69.34,70.01,68.87,68.94,41695855
26-Feb-14T05:00,70.19,71.22,68.85,69.26,55400399
25-Feb-14T05:00,70.95,71.00,69.45,69.85,52189031
24-Feb-14T05:00,68.74,71.44,68.54,70.78,76951946
21-Feb-14T05:00,69.69,69.96,68.45,68.59,70991892
20-Feb-14T05:00,67.73,70.11,65.73,69.63,131043748
19-Feb-14T05:00,67.05,69.08,67.00,68.06,64258631
18-Feb-14T05:00,66.94,67.54,66.07,67.30,43862297
14-Feb-14T05:00,67.50,67.58,66.72,67.09,36786427
13-Feb-14T05:00,64.18,67.33,64.05,67.33,62013396
12-Feb-14T05:00,64.92,65.06,64.05,64.45,47409857
11-Feb-14T05:00,63.75,65.00,63.35,64.85,45746832
10-Feb-14T05:00,64.30,64.49,63.47,63.55,43736562
7-Feb-14T05:00,62.27,64.57,62.22,64.32,60835746
6-Feb-14T05:00,61.46,62.78,61.46,62.16,42153754
5-Feb-14T05:00,62.74,63.16,61.27,62.19,53032420
4-Feb-14T05:00,62.05,63.14,61.82,62.75,46064897
3-Feb-14T05:00,63.03,63.77,60.70,61.48,75105994
31-Jan-14T05:00,60.47,63.37,60.17,62.57,87930298
30-Jan-14T05:00,62.12,62.50,60.46,61.08,150438699
29-Jan-14T05:00,54.61,54.95,53.19,53.53,98089932
28-Jan-14T05:00,54.02,55.28,54.00,55.14,48364998
27-Jan-14T05:00,54.73,54.94,51.85,53.55,74142331
24-Jan-14T05:00,56.15,56.42,54.40,54.45,55545338
23-Jan-14T05:00,56.37,56.68,55.69,56.63,47996403
22-Jan-14T05:00,58.85,59.31,57.10,57.51,61495880
21-Jan-14T05:00,56.60,58.58,56.50,58.51,48734147
17-Jan-14T05:00,57.30,57.82,56.07,56.30,40883205
16-Jan-14T05:00,57.26,58.02,56.83,57.19,34599775
15-Jan-14T05:00,57.98,58.57,57.27,57.60,33730619
14-Jan-14T05:00,56.46,57.78,56.10,57.74,37590987
13-Jan-14T05:00,57.91,58.25,55.38,55.91,63106519
10-Jan-14T05:00,57.13,58.30,57.06,57.94,42529258
9-Jan-14T05:00,58.65,58.96,56.65,57.22,92349222
8-Jan-14T05:00,57.60,58.41,57.23,58.23,56800776
7-Jan-14T05:00,57.70,58.55,57.22,57.92,77329009
6-Jan-14T05:00,54.42,57.26,54.05,57.20,68974359
3-Jan-14T05:00,55.02,55.65,54.53,54.56,38287706
2-Jan-14T05:00,54.83,55.22,54.19,54.71,43257622
31-Dec-13T05:00,54.12,54.86,53.91,54.65,43152127
30-Dec-13T05:00,54.93,55.18,53.43,53.71,68307317
27-Dec-13T05:00,57.48,57.68,55.25,55.44,60465751
26-Dec-13T05:00,58.32,58.38,57.37,57.73,55101367
24-Dec-13T05:00,58.27,58.58,56.91,57.96,46617754
23-Dec-13T05:00,55.50,58.32,55.45,57.77,98296983
20-Dec-13T05:00,54.91,55.15,54.23,55.12,239823912
19-Dec-13T05:00,54.34,55.19,53.95,55.05,89825393
18-Dec-13T05:00,55.57,55.89,53.75,55.57,76003479
17-Dec-13T05:00,54.75,55.18,54.24,54.86,78751463
16-Dec-13T05:00,53.27,54.50,52.91,53.81,85118518
13-Dec-13T05:00,51.61,53.50,51.34,53.32,82640992
12-Dec-13T05:00,51.03,52.07,50.66,51.83,92723034
11-Dec-13T05:00,50.56,50.77,49.01,49.38,65776366
10-Dec-13T05:00,48.62,50.77,48.54,50.24,68478561
9-Dec-13T05:00,48.06,48.97,47.74,48.84,36055891
6-Dec-13T05:00,48.98,49.39,47.71,47.94,42937659
5-Dec-13T05:00,48.15,48.70,47.87,48.34,43855036
4-Dec-13T05:00,46.46,48.77,46.26,48.62,60890176
3-Dec-13T05:00,46.75,47.20,46.29,46.73,32085905
2-Dec-13T05:00,46.90,47.54,46.26,47.06,50773647
29-Nov-13T05:00,46.75,47.21,46.50,47.01,22953916
27-Nov-13T05:00,45.97,46.67,45.53,46.49,44993195
26-Nov-13T05:00,44.66,46.17,43.55,45.89,82016490
25-Nov-13T05:00,46.36,46.65,44.04,44.82,82565324
22-Nov-13T05:00,47.04,47.27,45.96,46.23,40545375
21-Nov-13T05:00,46.99,47.46,46.68,46.70,34886170
20-Nov-13T05:00,46.61,47.55,46.31,46.43,53932698
19-Nov-13T05:00,46.26,47.00,45.72,46.36,75602413
18-Nov-13T05:00,48.47,48.84,45.80,45.83,85909884
15-Nov-13T05:00,49.11,49.48,48.71,49.01,42452937
14-Nov-13T05:00,48.70,49.57,48.03,48.99,75117049
13-Nov-13T05:00,46.23,48.74,46.06,48.71,79245346
12-Nov-13T05:00,46.00,47.37,45.83,46.60,68195832
11-Nov-13T05:00,47.04,47.53,45.73,46.20,80909626
8-Nov-13T05:00,47.81,48.65,47.25,47.53,70731178
7-Nov-13T05:00,49.24,49.87,47.30,47.56,97127618
6-Nov-13T05:00,50.26,50.45,48.71,49.12,67889337
5-Nov-13T05:00,47.79,50.18,47.51,50.10,76835006
4-Nov-13T05:00,49.36,49.75,48.02,48.22,80371218
1-Nov-13T05:00,50.85,52.09,49.72,49.75,95032876
31-Oct-13T05:00,47.16,52.00,46.50,50.20,248809006
30-Oct-13T05:00,50.00,50.21,48.75,49.01,127072652
29-Oct-13T05:00,50.73,50.79,49.25,49.40,102143469
28-Oct-13T05:00,51.54,51.70,49.61,50.23,73472347
25-Oct-13T05:00,53.18,53.24,51.88,51.95,45085348
24-Oct-13T05:00,52.38,52.84,51.59,52.44,46775185
23-Oct-13T05:00,51.75,52.25,51.13,51.90,57207154
22-Oct-13T05:00,54.33,54.76,52.20,52.68,83203892
21-Oct-13T05:00,54.68,54.81,53.51,53.85,58235283
18-Oct-13T05:00,54.18,54.82,53.60,54.22,88260093
17-Oct-13T05:00,51.12,52.22,50.95,52.21,71521899
16-Oct-13T05:00,50.04,51.24,49.90,51.14,64678247
15-Oct-13T05:00,49.99,51.00,49.18,49.50,81166571
14-Oct-13T05:00,48.31,49.63,47.91,49.51,68780552
11-Oct-13T05:00,49.18,49.87,48.79,49.11,58428451
10-Oct-13T05:00,47.86,49.68,47.83,49.05,99773784
9-Oct-13T05:00,47.38,47.84,45.26,46.77,147296862
8-Oct-13T05:00,50.60,50.60,47.08,47.14,136081330
7-Oct-13T05:00,50.73,51.29,50.40,50.52,57203957
4-Oct-13T05:00,49.77,51.16,49.57,51.04,74446947
3-Oct-13T05:00,50.47,50.72,49.06,49.18,82045323
2-Oct-13T05:00,50.13,51.10,49.95,50.28,62834429
1-Oct-13T05:00,49.97,51.03,49.45,50.42,98113699
30-Sep-13T05:00,50.14,51.60,49.80,50.23,100095417
27-Sep-13T05:00,50.29,51.28,49.86,51.24,81410460
26-Sep-13T05:00,50.01,50.60,49.50,50.39,98220046
25-Sep-13T05:00,49.23,49.54,48.46,49.46,87879619
24-Sep-13T05:00,48.50,49.66,48.16,48.45,136716101
23-Sep-13T05:00,47.28,47.55,46.29,47.19,75319202
20-Sep-13T05:00,46.32,47.60,45.74,47.49,115508400
19-Sep-13T05:00,45.51,46.05,45.23,45.98,63972369
18-Sep-13T05:00,44.84,45.47,44.40,45.23,79316945
17-Sep-13T05:00,42.50,45.44,42.43,45.07,91934557
16-Sep-13T05:00,44.85,44.94,42.43,42.51,70807761
13-Sep-13T05:00,45.04,45.08,43.93,44.31,52765299
12-Sep-13T05:00,45.53,45.62,44.65,44.75,68072239
11-Sep-13T05:00,43.39,45.09,43.11,45.04,71676653
10-Sep-13T05:00,44.24,44.26,43.23,43.60,54540282
9-Sep-13T05:00,44.36,44.79,43.70,44.04,75794696
6-Sep-13T05:00,43.09,44.61,42.40,43.95,117535626
5-Sep-13T05:00,41.79,42.76,41.77,42.66,50035380
4-Sep-13T05:00,42.01,42.17,41.44,41.78,42581854
3-Sep-13T05:00,41.84,42.16,41.51,41.87,48774896
30-Aug-13T05:00,42.02,42.26,41.06,41.29,67735053
29-Aug-13T05:00,40.89,41.78,40.80,41.28,58303395
28-Aug-13T05:00,39.96,40.85,39.88,40.55,57918194
27-Aug-13T05:00,40.68,41.20,39.42,39.64,72695050
26-Aug-13T05:00,40.90,41.94,40.62,41.34,94162358
23-Aug-13T05:00,39.00,40.63,38.93,40.55,86442283
22-Aug-13T05:00,38.37,38.75,38.34,38.55,21931163
21-Aug-13T05:00,38.38,38.85,38.14,38.32,46116868
20-Aug-13T05:00,38.35,38.58,37.69,38.41,57995140
19-Aug-13T05:00,37.43,38.28,37.14,37.81,57609591
16-Aug-13T05:00,36.97,37.49,36.90,37.08,45840714
15-Aug-13T05:00,36.36,37.07,36.02,36.56,56521095
14-Aug-13T05:00,36.83,37.55,36.62,36.65,48423890
13-Aug-13T05:00,38.24,38.32,36.77,37.02,65379198
12-Aug-13T05:00,38.20,38.50,38.10,38.22,31160951
9-Aug-13T05:00,38.59,38.74,38.01,38.50,43620024
8-Aug-13T05:00,39.13,39.19,38.43,38.54,41300906
7-Aug-13T05:00,38.61,38.94,37.70,38.87,68854764
6-Aug-13T05:00,39.11,39.25,37.94,38.55,63950791
5-Aug-13T05:00,38.43,39.32,38.25,39.19,79994774
2-Aug-13T05:00,37.66,38.49,37.50,38.05,73058424
1-Aug-13T05:00,37.30,38.29,36.92,37.49,106066472
31-Jul-13T05:00,37.96,38.31,36.33,36.80,154828679
30-Jul-13T05:00,35.65,37.96,35.32,37.63,173582710
29-Jul-13T05:00,34.07,35.63,34.01,35.43,124884870
26-Jul-13T05:00,33.77,34.73,33.56,34.01,136028897
25-Jul-13T05:00,33.54,34.88,32.75,34.36,365935212
24-Jul-13T05:00,26.32,26.53,26.05,26.51,82635587
23-Jul-13T05:00,26.10,26.30,25.97,26.13,28221534
22-Jul-13T05:00,25.99,26.13,25.72,26.04,27526213
19-Jul-13T05:00,25.82,26.11,25.60,25.88,46544938
18-Jul-13T05:00,26.75,26.77,26.12,26.18,24806825
17-Jul-13T05:00,26.37,26.78,26.30,26.65,21518463
16-Jul-13T05:00,26.39,26.75,26.01,26.32,30817554
15-Jul-13T05:00,25.93,26.43,25.65,26.28,24233957
12-Jul-13T05:00,25.74,25.93,25.55,25.91,16537840
11-Jul-13T05:00,25.96,26.00,25.45,25.81,26777354
10-Jul-13T05:00,25.58,25.83,25.47,25.80,26721794
9-Jul-13T05:00,25.07,25.49,25.03,25.48,30387889
8-Jul-13T05:00,24.47,25.04,24.42,24.71,27073983
5-Jul-13T05:00,24.65,24.66,24.20,24.37,20229451
3-Jul-13T05:00,24.22,24.71,24.15,24.52,10404332
2-Jul-13T05:00,24.70,24.77,24.30,24.41,18394008
1-Jul-13T05:00,24.97,25.06,24.62,24.81,20582195
28-Jun-13T05:00,24.68,24.98,24.42,24.88,96778879
27-Jun-13T05:00,24.24,24.84,24.21,24.66,34694013
26-Jun-13T05:00,24.51,24.65,23.99,24.16,29890205
25-Jun-13T05:00,24.14,24.43,24.04,24.25,24719988
24-Jun-13T05:00,23.95,24.11,23.38,23.94,40625948
21-Jun-13T05:00,24.59,24.70,24.05,24.53,45826173
20-Jun-13T05:00,24.28,24.74,23.65,23.90,42765586
19-Jun-13T05:00,24.20,25.19,24.10,24.31,31790525
18-Jun-13T05:00,24.09,24.69,24.08,24.21,36709004
17-Jun-13T05:00,23.91,24.25,23.75,24.02,33664419
14-Jun-13T05:00,23.56,23.89,23.26,23.63,30561387
13-Jun-13T05:00,23.72,23.83,23.26,23.73,31189247
12-Jun-13T05:00,24.16,24.26,23.58,23.77,26445790
11-Jun-13T05:00,24.03,24.35,24.00,24.03,29676383;";
            
        }
    }
}