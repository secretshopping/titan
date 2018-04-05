using System;

namespace Prem.PTC.Payments
{
    public class RevolutTransactionResponse : TransactionResponse
    {
        internal RevolutTransactionResponse(RevolutAccount usedAccount, Exception ex)
        {
            UsedAccount = usedAccount;
            RawResponse = ex.Message;
            Note = ex.Message;
            ReferenceNumber = ex.Message;
            IsSuccess = false;
        }

        internal RevolutTransactionResponse(RevolutAccount usedAccount, dynamic response)
        {
            try
            {
                IsSuccess = false;
                UsedAccount = usedAccount;

                if (response.state == "completed")
                {
                    //OK
                    IsSuccess = true;
                    ReferenceNumber = response.id;
                    Note = response.created_at;
                }
                else
                {
                    Note = response.reason;
                }
            }
            catch (Exception ex)
            {
                IsSuccess = true;
            }
        }
    }
}