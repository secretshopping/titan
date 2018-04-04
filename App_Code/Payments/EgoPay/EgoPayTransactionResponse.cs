using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prem.PTC.Payments
{
    public class EgoPayTransactionResponse : TransactionResponse
    {
        internal EgoPayTransactionResponse(EgoPayAccount usedAccount, NameValuePairs response)
        {
            RawResponse = response.ToString();
            UsedAccount = usedAccount;
            Note = string.Empty;

            string refNo;
            response.TryGetValue("responseEnvelope.correlationId", out refNo);
            ReferenceNumber = refNo;

            string success = "";
            bool isAck = response.TryGetValue("paymentExecStatus", out success);
            IsSuccess = isAck && String.Equals(success, "COMPLETED", StringComparison.OrdinalIgnoreCase);

            if (!IsSuccess)
            {
                string severity, errorCode, message;

                if (!response.TryGetValue("error.severity", out severity))
                    response.TryGetValue("error(0).severity", out severity);
                if (!response.TryGetValue("error.errorId", out errorCode))
                    response.TryGetValue("error(0).errorId", out errorCode);
                if (!response.TryGetValue("error.message", out message))
                    response.TryGetValue("error(0).message", out message);

                Note = severity + " " + errorCode + ": " + message;
            }
        }
    }
}