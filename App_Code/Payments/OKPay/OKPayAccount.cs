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
    public class OKPayAccount : PaymentAccount<OKPayAccountDetails>
    {
        public OKPayAccount(OKPayAccountDetails details)
            : base(details) { }

        public override Money Balance
        {
            get
            {
                try
                {
                    //Creating proxy client
                    using (var proxy = new I_OkPayAPIClient())
                    {
                        //Get Balance
                        var balance = proxy.Wallet_Get_Currency_Balance(Details.Username, SecurityToken, AppSettings.Site.CurrencyCode);
                        return new Money(balance.Amount);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                    return Money.Zero;
                }
            }
        }

        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {
            try
            {
                //Creating proxy client
                using (var proxy = new I_OkPayAPIClient())
                {
                    string ID = HashingManager.GenerateMD5(DateTime.Now + request.MemberName + AppSettings.Site.Url);

                    //Send Money
                    var OI = proxy.Send_Money(Details.Username, SecurityToken, request.PayeeId, AppSettings.Site.CurrencyCode, Decimal.Parse(request.Payment.ToShortClearString()), 
                        request.Note, true, ID);

                    return new OKPayTransactionResponse(this, OI);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                return new OKPayTransactionResponse(this, ex);
            }
        }

        
        private string SecurityToken
        {
            get
            {
                return HashingManager.SHA256(accountDetails.APISecret + DateTime.UtcNow.ToString(":yyyyMMdd:HH"));
            }
        }

    }
}