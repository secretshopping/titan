using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Members;
using Prem.PTC;
using Resources;

namespace MarchewkaOne.Titan.Balances
{
    public class TransferManager
    {
        private Member from, to;

        public TransferManager(Member from, Member to)
        {
            this.from = from;
            this.to = to;
        }

        public void TryTransfer(Money value)
        {
            //Anti-Injection Fix
            if (value <= new Money(0))
                throw new MsgException(L1.ERROR);

            if (value > from.MainBalance)
                throw new MsgException(L1.NOTENOUGHFUNDS);

            CheckPersonalSettings();

            if (from.Name == to.Name)
                throw new MsgException("You can't send money to yourself :-)");

            from.SubtractFromMainBalance(value, "Member transfer to " + to.Name);
            Money amountWithFee = Money.Zero;

            if (AppSettings.Payments.TransferMode == TransferFundsMode.AllowPointsAndMainBalance ||
                AppSettings.Payments.TransferMode == TransferFundsMode.AllowMainBalanceOnly)
            {
                amountWithFee = GetAmountWithFee(value, TransferFeeType.OtherUserMainToMain, to);
                to.AddToMainBalance(amountWithFee, "Member transfer from " + from.Name);
            }

            else if (AppSettings.Payments.TransferMode == TransferFundsMode.AllowMainToPurchaseBalance)
            {
                amountWithFee = GetAmountWithFee(value, TransferFeeType.OtherUserMainToPurchase, to);

                to.AddToPurchaseBalance(amountWithFee, "Member transfer from " + from.Name);
            }

            from.SaveBalances();
            to.SaveBalances();

            History.AddTransfer(from.Name, value, from.Name, to.Name);
            History.AddTransfer(to.Name, amountWithFee, from.Name, to.Name);

            //Pools
            Money moneyLeft = value - amountWithFee;
            PoolDistributionManager.AddProfit(ProfitSource.TransferFees, moneyLeft);
        }

        public void TryTransfer(Int32 value)
        {
            //Anti-Injection Fix
            if (value <= 0)
                throw new MsgException(L1.ERROR);

            if (value > from.PointsBalance)
                throw new MsgException(L1.NOTENOUGHFUNDS);

            CheckPersonalSettings();

            if (from.Name == to.Name)
                throw new MsgException("You can't send money to yourself :-)");

            from.SubtractFromPointsBalance(value, "Member transfer to " + to.Name);

            int amountWithFee = GetAmountWithFee(value, TransferFeeType.OtherUserPointsToPoints, to);

            to.AddToPointsBalance(amountWithFee, "Member transfer from " + from.Name);

            from.SaveBalances();
            to.SaveBalances();

            History.AddTransfer(from.Name, new Money(value), from.Name, to.Name, true);
            History.AddTransfer(to.Name, new Money(amountWithFee), from.Name, to.Name, true);

            //Pools
            //GlobalPoolManager.AddToPool(GlobalPoolType.SystemPool, new Money(value - GetAmountWithFee(value)));
        }

        private void CheckPersonalSettings()
        {
            if (from.TransferPermission == TransferFundsPermission.DenyAll ||
                from.TransferPermission == TransferFundsPermission.AllowReceivingOnly)
                throw new MsgException(U3500.CANTSEND);

            if (to.TransferPermission == TransferFundsPermission.DenyAll ||
                to.TransferPermission == TransferFundsPermission.AllowSendingOnly)
                throw new MsgException(U3500.CANRECEIVE);

        }

        public static Money GetAmountWithFee(Money amount, TransferFeeType feeType, Member user)
        {
            int FeePercent = GetTransferFeePercent(feeType, user);

            Money Fee = Money.MultiplyPercent(amount, FeePercent);

            if (FeePercent > 0 && Fee == Money.Zero)
                Fee = Money.MinPositiveValue;

            return amount - Fee;
        }

        public Int32 GetAmountWithFee(Int32 amount, TransferFeeType feeType, Member user)
        {
            int FeePercent = GetTransferFeePercent(feeType, user);

            Money Fee = Money.MultiplyPercent(new Money(amount), FeePercent);

            if (FeePercent > 0 && Fee.AsPoints() == 0)
                Fee = new Money(1);

            return amount - Fee.AsPoints();
        }

        private static int GetTransferFeePercent(TransferFeeType feeType, Member user)
        {
            switch (feeType)
            {
                case TransferFeeType.SameUserCommissionToMain:
                    return user.Membership.SameUserCommissionToMainTransferFee;
                case TransferFeeType.OtherUserMainToPurchase:
                    return user.Membership.OtherUserMainToAdTransferFee;
                case TransferFeeType.OtherUserMainToMain:
                    return user.Membership.OtherUserMainToMainTransferFee;
                case TransferFeeType.OtherUserPointsToPoints:
                    return user.Membership.OtherUserPointsToPointsTransferFee;
                default: return 0;

            }
        }
    }

}