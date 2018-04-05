using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiWithdrawalData
    {
        public UnpaidPayoutRequests unpaidPayoutRequests { get; set; }
        public List<ApiPaymentProcessor> processors { get; set; }
        public bool isPinEnabled { get; set; }
        public ApiMoney mainBalance { get; set; }

        public ApiWithdrawalData(UnpaidPayoutRequests unpaidPayoutRequests, List<ApiPaymentProcessor> processors, bool isPinEnabled, 
            Money mainBalance)
        {
            this.unpaidPayoutRequests = unpaidPayoutRequests;
            this.processors = processors;
            this.isPinEnabled = isPinEnabled;
            this.mainBalance = new ApiMoney(mainBalance);
        }
    }
}