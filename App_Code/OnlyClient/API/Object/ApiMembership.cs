using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.API
{
    public class ApiMembership
    {
        public string name { get; set; }
        public string color { get; set; }
        public List<KeyValuePair<string,string>> properties { get; set; }

        public ApiMembership(DataRow dataRow)
        {
            this.name = dataRow["Name"].ToString();
            this.color = dataRow["Color"].ToString();
            this.properties = new List<KeyValuePair<string, string>>();

            var hiddenProperties = MembershipProperty.GetPropsToHideForClient();
            var columnNames = dataRow.Table.Columns.Cast<DataColumn>()
                              .Select(x => new KeyValuePair<string,int>(x.ColumnName, x.Ordinal)).ToList();

            foreach (var columnName in columnNames)
                if (columnName.Key != "Name" && columnName.Key != "Color" && !hiddenProperties.Contains(columnName.Key))
                    properties.Add(
                        new KeyValuePair<string, string>(MembershipProperty.GetResourceLabel(columnName.Key), 
                        MembershipProperty.Format(columnName.Value, dataRow[columnName.Key].ToString(), false))
                     );
        }
    }
}