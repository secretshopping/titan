using Newtonsoft.Json.Linq;
using Prem.PTC.Utils.NVP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prem.PTC.Payments
{
    public class MPesaSapamaTransactionResponse : TransactionResponse
    {
        internal MPesaSapamaTransactionResponse(MPesaSapamaAccount usedAccount, Exception ex)
        {
            UsedAccount = usedAccount;
            RawResponse = ex.Message;
            Note = ex.Message;
            ReferenceNumber = ex.Message;
            IsSuccess = false;
        }

        internal MPesaSapamaTransactionResponse(MPesaSapamaAccount usedAccount, JObject response)
        {
            try
            {
                IsSuccess = false;
                UsedAccount = usedAccount;
                Note = response["message"].ToString();

                if (response["httpStatusCode"].ToString() == "200")
                {
                    //OK
                    IsSuccess = true;
                    ReferenceNumber = response["data"]["proxy_trans_id"].ToString();
                }
            }
            catch(Exception ex)
            {
                IsSuccess = true;
            }
        }
    }
}