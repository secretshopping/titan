using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prem.PTC.Payments
{
    public class DepositHelper
    {
        public static void TransferToBalance(string username, Money money, string from, string transId, string targetBalance,
            bool isViaRepresentative = false, string cryptoCurrencyInfo = "")
        {
            Exceptions.HandleNonMsgEx(() =>
            {
                //Add income to stats
                if (!isViaRepresentative)
                    Statistics.Statistics.AddToCashflow(money);

                bool successful = (money >= AppSettings.Payments.MinimumTransferAmount);

                PaymentProcessor paymentProcessor = PaymentAccountDetails.GetFromStringType(from);
                Money moneyWithoutFee = money;

                if (!isViaRepresentative &&
                   (targetBalance == "Purchase Balance" || targetBalance == "Cash Balance" || targetBalance == "Marketplace Balance"))
                    moneyWithoutFee = PaymentAccountDetails.GetAmountWithoutFee(from, money);

                if (successful)
                {
                    Member user = new Member(username);
                    if (targetBalance == "Purchase Balance")
                        user.AddToPurchaseBalance(moneyWithoutFee, from + " transfer");
                    else if (targetBalance == "Cash Balance")
                        user.AddToCashBalance(moneyWithoutFee, from + " transfer");
                    else if (targetBalance == "Traffic Balance")
                        user.AddToTrafficBalance(money, from + " transfer");
                    else if (targetBalance == "Marketplace Balance")
                        user.AddToMarketplaceBalance(moneyWithoutFee, from + " transfer");
                    else if (targetBalance == "Points Balance")
                        user.AddToPointsBalance(moneyWithoutFee.ConvertToPoints(), from + " transfer");
                    user.SaveBalances();

                    if (targetBalance == "Purchase Balance" || targetBalance == "Cash Balance" || targetBalance == "Marketplace Balance")
                        //Update payment proportions
                        PaymentProportionsManager.MemberPaidIn(moneyWithoutFee, paymentProcessor, user);

                    //Add history
                    History.AddTransfer(username, moneyWithoutFee, from, targetBalance);

                    //TryAchievement
                    bool shouldBeSaved = user.TryToAddAchievements(
                            Achievements.Achievement.GetProperAchievements(
                            Achievements.AchievementType.AfterTransferringOnceAmount, moneyWithoutFee.GetTotals()));

                    if (shouldBeSaved)
                        user.Save();

                    //Check the contests
                    Contests.ContestManager.IMadeAnAction(Contests.ContestType.Transfer, user.Name, moneyWithoutFee, 0);
                    PurchasedItem.Create(user.Id, moneyWithoutFee, 1, "Transfer to " + targetBalance, PurchasedItemType.Transfer);

                    //Referral commission for sponsors when user does Cash Balance deposit
                    Titan.CashBalanceCrediter Crediter = (Titan.CashBalanceCrediter)Titan.CrediterFactory.Acquire(user, Titan.CreditType.CashBalanceDeposit);
                    Crediter.TryCreditReferer(moneyWithoutFee);

                }

                //AddLog
                if(!isViaRepresentative)
                    CompletedPaymentLog.Create(paymentProcessor, "Transfer to " + targetBalance, transId, false, username, moneyWithoutFee,
                        money - moneyWithoutFee, successful, cryptoCurrencyInfo);
            });
        }
    }
}