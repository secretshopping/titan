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

namespace Prem.PTC.Payments
{
    public class AdvCashAccount : PaymentAccount<AdvCashAccountDetails>
    {
        private string CurrencyCode = AppSettings.Site.CurrencyCode;

        public AdvCashAccount(AdvCashAccountDetails details)
            : base(details)
        { }

        public override Money Balance
        {
            get
            {
                try
                {
                    MerchantWebServiceClient client = new MerchantWebServiceClient("MerchantWebServicePort");

                    var balances = client.getBalances(getAuthDTO());

                    foreach (var balance in balances)
                        if (balance.id == accountDetails.Username.Replace(" ", ""))
                            return new Money(balance.amount);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                }
                return Money.Zero;
            }
        }

        private authDTO getAuthDTO()
        {
            authDTO dto = new authDTO();
            dto.apiName = accountDetails.APIName;
            DateTime UtcNow = DateTime.UtcNow;
            dto.authenticationToken = HashingManager.SHA256(AdvCashAccount.APIPassword + ":" + UtcNow.ToString("yyyyMMdd") + ":" + UtcNow.ToString("HH"));
            dto.accountEmail = accountDetails.AccountEmail;

            return dto;
        }

        public override TransactionResponse CommitTransaction(TransactionRequest request)
        {

            try
            {
                MerchantWebServiceClient client = new MerchantWebServiceClient("MerchantWebServicePort");

                currency currencyEnum = currency.USD;
                Enum.TryParse(CurrencyCode, out currencyEnum);

                sendMoneyRequest req = new sendMoneyRequest();
                req.amount = request.Payment.ToDecimal();
                req.currency = currencyEnum;
                req.email = request.PayeeId.Replace(" ", "");
                req.note = request.Note;
                req.amountSpecified = true;
                req.currencySpecified = true;

                var response = client.sendMoney(getAuthDTO(), req);
                return new AdvCashTransactionResponse(this, response);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                return new AdvCashTransactionResponse(this, ex);
            }


        }

        public static string APIPassword
        {
            get
            {
                return HashingManager.GenerateMD5(AppSettings.Offerwalls.UniversalHandlerPassword + "AdvCash") + "1Ab";
            }
        }

    }
}