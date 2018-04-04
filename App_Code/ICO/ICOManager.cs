using Prem.PTC.Members;
using System;
using Prem.PTC;
using Resources;
using Titan.Cryptocurrencies;
using Prem.PTC.Utils;
using ExtensionMethods;

namespace Titan.ICO
{
    public class ICOManager
    {
        public static void TryPurchaseTokens(Member user, ICOStage stage, int numberOfTokens, BalanceType targetBalance)
        {
            var TokenCryprocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);

            if (numberOfTokens <= 0)
                throw new MsgException(U5006.AMOUNTEQUALZERO);

            if (numberOfTokens > stage.GetAvailableTokens())
                throw new MsgException(U6012.NOTOKENSLEFT);

            int userPurchasesInLast15min = ICOPurchase.GetUserPurchasesInLast15Min(stage.Id, user.Id);

            if (userPurchasesInLast15min + numberOfTokens > AppSettings.ICO.ICOPurchaseLimitPerUserPer15mins)
                throw new MsgException(String.Format(U6012.COINSEXCEED15MIN, "<b>" + userPurchasesInLast15min + "</b>", TokenCryprocurrency.Code));

            //All OK, let's charge the balance
            //If freeze system is enabled, purchased tokens are freezed
            CryptocurrencyType TypeOfPurchasedCoins = AppSettings.Ethereum.ERC20TokensFreezeSystemEnabled ? CryptocurrencyType.ERCFreezed : CryptocurrencyType.ERC20Token;
            Money totalAmount = numberOfTokens * stage.TokenPrice;

            if (targetBalance == BalanceType.PurchaseBalance)
            {
                if (totalAmount > user.PurchaseBalance)
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                user.SubtractFromPurchaseBalance(totalAmount, TokenCryprocurrency.Code + " purchase", BalanceLogType.CoinPurchase);
                user.SaveBalances();
            }
            else if (targetBalance == BalanceType.BTC)
            {
                decimal amountInBTC = (totalAmount.ToDecimal() / 
                    CryptocurrencyFactory.Get(CryptocurrencyType.BTC).GetValue().ToDecimal()).TruncateDecimals(8);

                if (amountInBTC > user.GetCryptocurrencyBalance(CryptocurrencyType.BTC).ToDecimal())
                    throw new MsgException(L1.NOTENOUGHFUNDS);

                user.SubtractFromCryptocurrencyBalance(CryptocurrencyType.BTC, amountInBTC,
                    TokenCryprocurrency.Code + " purchase", BalanceLogType.CoinPurchase);
            }
            else
                throw new ArgumentException("Invalid argument: " + targetBalance.ToString(), "targetBalance");

            //Add history entry
            History.AddPurchase(user.Name, totalAmount, TokenCryprocurrency.Code);

            //Credit coins to Referrer
            decimal tokensCreditedToReferrer = 0;
            if (user.HasReferer)
            {
                var Referer = new Member(user.ReferrerId);
                tokensCreditedToReferrer = (Referer.Membership.ICOPurchasePercent / 100) * numberOfTokens;

                if (tokensCreditedToReferrer > 0)
                {
                    Referer.AddToCryptocurrencyBalance(TypeOfPurchasedCoins, tokensCreditedToReferrer, TokenCryprocurrency.Code + " purchase /ref/"
                        + user.Name, BalanceLogType.CoinPurchase);

                    if (TypeOfPurchasedCoins == CryptocurrencyType.ERCFreezed)
                        UserFreezedToken.Add(Referer.Id, tokensCreditedToReferrer);

                    user.IncreaseERC20TokensEarningsForDRef(tokensCreditedToReferrer);
                    user.SaveStatistics();
                }
            }

            //Add purchase entry
            ICOPurchase.Add(user.Id, numberOfTokens, tokensCreditedToReferrer + (decimal)numberOfTokens, stage.Id);

            //Add coins to balance
            user.AddToCryptocurrencyBalance(TypeOfPurchasedCoins, numberOfTokens, TokenCryprocurrency.Code + " purchase", BalanceLogType.CoinPurchase);
            if (TypeOfPurchasedCoins == CryptocurrencyType.ERCFreezed)
                UserFreezedToken.Add(user.Id, Decimal.Parse(numberOfTokens.ToString()));
        }
    }
}