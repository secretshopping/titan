using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayPal.Api;

namespace Prem.PTC.Payments
{
    public class PayPalTransactionResponse : TransactionResponse
    {
        internal PayPalTransactionResponse(PayPalAccount usedAccount, NameValuePairs response)
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
                string errorCode, message;

                if (!response.TryGetValue("error.errorId", out errorCode))
                {
                    response.TryGetValue("error(0).errorId", out errorCode);

                    if (!response.TryGetValue("error(0).errorId", out errorCode))
                        response.TryGetValue("L_ERRORCODE0", out errorCode);
                }

                if (!response.TryGetValue("error.message", out message))
                {
                    response.TryGetValue("error(0).message", out message);

                    if (!response.TryGetValue("error(0).message", out message))
                        response.TryGetValue("L_LONGMESSAGE0", out message);
                }

                Note = errorCode + ": " + message;
            }
            else
            {
                Note = Resources.U4000.MONEYSENT.Replace("%p%", "PayPal");
            }
        }

        internal PayPalTransactionResponse(PayPalAccount usedAccount, PayoutBatch response)
        {
            RawResponse = response.ToString();
            UsedAccount = usedAccount;
            Note = string.Empty;
            ReferenceNumber = response.batch_header.payout_batch_id;

            bool IsFail = response.batch_header.batch_status == "DENIED" ||
                          response.batch_header.batch_status == "CANCELED";

            IsSuccess = !IsFail;

            if (IsFail)
            {
                Note = response.batch_header.batch_status;
            }
            else
            {
                Note = Resources.U4000.MONEYSENT.Replace("%p%", "PayPal");
            }
        }
    }
}