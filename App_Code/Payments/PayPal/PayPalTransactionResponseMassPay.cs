using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prem.PTC.Payments
{
    public class PayPalTransactionResponseForMassPay : TransactionResponse
    {
        internal PayPalTransactionResponseForMassPay(PayPalAccount usedAccount, NameValuePairs response)
        {
            RawResponse = response.ToString();
            UsedAccount = usedAccount;
            Note = string.Empty;

            string refNo;
            response.TryGetValue("CORRELATIONID", out refNo);
            ReferenceNumber = refNo;

            string success = "";
            bool isAck = response.TryGetValue("ACK", out success);
            IsSuccess = isAck && String.Equals(success, "Success", StringComparison.OrdinalIgnoreCase);

            if (!IsSuccess)
            {
                string severity, errorCode, message;

                if ((!response.TryGetValue("error.severity", out severity)) || !response.TryGetValue("error(0).severity", out severity))
                    response.TryGetValue("L_SHORTMESSAGE0", out severity);
                if ((!response.TryGetValue("error.errorId", out errorCode)) || !response.TryGetValue("error(0).errorId", out errorCode))
                    response.TryGetValue("L_ERRORCODE0", out errorCode);
                if ((!response.TryGetValue("error.message", out message)) || !response.TryGetValue("error(0).message", out message))
                    response.TryGetValue("L_LONGMESSAGE0", out message);

                Note = severity + " " + errorCode + ": " + message;
            }
            else
            {
                Note = Resources.U4000.MONEYSENT.Replace("%p%", "PayPal");
            }
        }
    }
}