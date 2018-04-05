using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiBalance
    {
        public int id { get; set; }
        public string name { get; set; }
        public ApiMoney value { get; set; }

        public ApiBalance(string name, BalanceType type, ApiMoney amount)
        {
            this.id = (int)type;
            this.name = name;
            value = amount;
        }
    }
}