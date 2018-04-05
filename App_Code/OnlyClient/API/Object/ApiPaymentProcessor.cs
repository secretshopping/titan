using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiPaymentProcessor
    {
        public string text { get; set; }
        public string value { get; set; }

        public ApiPaymentProcessor(string text, string value)
        {
            this.text = text;
            this.value = value;
        }
    }

    public class ApiTransferPaymentProcessor
    {
        public string text { get; set; }
        public string value { get; set; }
        public List<ApiPaymentProcessor> transferableTo { get; set; }

        public ApiTransferPaymentProcessor(string text, string value)
        {
            this.text = text;
            this.value = value;
            this.transferableTo = TransferHelper.GetAvailableListItems(value).
                Select(elem => new ApiPaymentProcessor(elem.Text, elem.Value)).ToList();
        }
    }
}