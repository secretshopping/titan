using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Prem.PTC.Payments
{
    public class PerfectMoneyAccount : PaymentAccount<PerfectMoneyAccountDetails>
    {
        private string CurrencyCode = AppSettings.Site.CurrencyCode;

        public PerfectMoneyAccount(PerfectMoneyAccountDetails details)
            : base(details) { }

        public override Money Balance
        {
            get
            {
                try
                {
                    PerfectMoney PM = new PerfectMoney();

                    Dictionary<string,string> result = PM.QueryBalance(accountDetails.Username, accountDetails.Password);

                    return Money.Parse(result[accountDetails.AccountNumber]);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    return new Money(0);
                }
            }
        }

        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {
            return CommitTransactionNormal(request);
        }

        private TransactionResponse CommitTransactionNormal(TransactionRequest request)
        {
            PerfectMoney PM = new PerfectMoney();
            var result = PM.Transfer(accountDetails.Username, accountDetails.Password, accountDetails.AccountNumber, request.PayeeId, Convert.ToDouble(request.Payment.ToShortClearString()), 0, 0);
            return new PerfectMoneyTransactionResponse(this, result);
        }


    }
}