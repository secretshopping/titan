using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Titan.API
{
    public class ApiListItem
    {
        public string text { get; set; }
        public string value { get; set; }

        public ApiListItem(ListItem item)
        {
            this.text = item.Text;
            this.value = item.Value;
        }
    }
}