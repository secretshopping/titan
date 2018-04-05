using System;
using Prem.PTC.Payments;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        public string GetPaymentAddress(PaymentProcessor processor)
        {
            var result = UsersPaymentProcessorsAddress.GetAddress(this.Id, new PaymentProcessorInfo(processor));

            if (result == null)
                return String.Empty;

            return result.PaymentAddress;
        }

        public string GetPaymentAddress(int customPayoutProcessorId)
        {
            var result = UsersPaymentProcessorsAddress.GetAddress(this.Id, new PaymentProcessorInfo(customPayoutProcessorId));

            if (result == null)
                return String.Empty;

            return result.PaymentAddress;
        }

        public void SetPaymentAddress(PaymentProcessor processor, string address)
        {
            UsersPaymentProcessorsAddress.TrySetAddress(this.Id, new PaymentProcessorInfo(processor), address);
        }

        public void SetPaymentAddress(int customPayoutProcessorId, string address)
        {
            UsersPaymentProcessorsAddress.TrySetAddress(this.Id, new PaymentProcessorInfo(customPayoutProcessorId), address);
        }

        public UnpaidPayoutRequests GetUnpaidPayoutRequests()
        {
            var UnpaidRequests = TableHelper.GetListFromRawQuery<PayoutRequest>(
                String.Format("SELECT * FROM PayoutRequests WHERE Username = '{0}' AND IsPaid = 0", this.Name));

            if (UnpaidRequests.Count == 0)
                return new UnpaidPayoutRequests(false, String.Empty);

            //We have some unpaid requests
            Money sum = Money.Zero;
            DateTime last = AppSettings.ServerTime;

            foreach (var UnpaidRequest in UnpaidRequests)
            {
                sum += UnpaidRequest.Amount;
                if (UnpaidRequest.RequestDate < last)
                    last = UnpaidRequest.RequestDate;
            }

            return new UnpaidPayoutRequests(true, Resources.L1.PRNOTPAID.Replace("%var1%", "<b>" +
                UnpaidRequests.Count.ToString() + "</b>").Replace("%var2%", "<b>" + sum.ToShortClearString() + "</b>").Replace("%var3%", last.ToShortDateString()));

        }

        public DateTime? GetLastWithdrawalDate()
        {
            var ppQuery = string.Format("SELECT TOP 1 {0} FROM {1} WHERE {2} != 'REJECTED' AND {3} = '{4}' ORDER BY {0} DESC",
                PayoutRequest.Columns.RequestDate, PayoutRequest.TableName, PayoutRequest.Columns.PaymentProcessor, PayoutRequest.Columns.Username, Name);

            var cryptoQuery = string.Format("SELECT TOP 1 RequestDate FROM CryptocurrencyWithdrawRequests WHERE Status = {0} AND UserId = {1} ORDER BY RequestDate DESC",
                (int)WithdrawRequestStatus.Rejected, Id);

            var ppLastDate = (DateTime?)TableHelper.SelectScalar(ppQuery);
            var cryptoLastDate = (DateTime?)TableHelper.SelectScalar(cryptoQuery);

            if (ppLastDate == null)
                return cryptoLastDate;

            if (cryptoLastDate == null)
                return ppLastDate;

            if (ppLastDate > cryptoLastDate)
                return ppLastDate;
            else
                return cryptoLastDate;
        }
    }
}