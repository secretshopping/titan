using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiTransferData
    {
        public List<ApiTransferPaymentProcessor> processors { get; set; }

        public ApiTransferData(List<ApiPaymentProcessor> processors)
        {
            this.processors = processors.Select(elem => new ApiTransferPaymentProcessor(elem.text, elem.value)).ToList();
        }
    }
}