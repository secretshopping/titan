using Newtonsoft.Json.Linq;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.ServiceModel;
using AfricasTalking;

namespace Prem.PTC.Payments
{
    public class MPesaAccount : PaymentAccount<MPesaAccountDetails>
    {
        private string CurrencyCode = AppSettings.Site.CurrencyCode;

        public MPesaAccount(MPesaAccountDetails details)
            : base(details)
        { }

        public override Money Balance
        {
            get
            {
                try
                {
                    AfricasTalkingGateway gateway = new AfricasTalkingGateway(accountDetails.Username, accountDetails.ApiKey);
                    dynamic response = gateway.getUserData();
                    return Money.Parse(response["balance"].ToString().Split(' ')[1]);
                }
                catch (Exception ex)
                {
                    return Money.Zero;
                }
            }
        }

        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {
            try
            {
                var recipients = new List<MobilePaymentB2CRecipient>();
                recipients.Add(new MobilePaymentB2CRecipient(request.PayeeId, AppSettings.Site.CurrencyCode, request.Payment.ToDecimal()));

                AfricasTalkingGateway gateway = new AfricasTalkingGateway(accountDetails.Username, accountDetails.ApiKey);
                var response = gateway.MobilePaymentB2CRequest(accountDetails.ProductName, recipients);

                return new MPesaTransactionResponse(this, response);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                return new MPesaTransactionResponse(this, ex);
            }
        }

    }
}