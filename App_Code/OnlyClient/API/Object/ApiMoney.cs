using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiMoney
    {
        public decimal amount { get; set; }
        public string formattedAmount { get; set; }

        public ApiMoney(Money amount)
        {
            this.amount = amount.ToDecimal();
            formattedAmount = amount.ToString();
        }
    }
}