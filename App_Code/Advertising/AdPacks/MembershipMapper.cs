using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Resources;

namespace AdPackTypeMembershipMapper
{
    public class Mapper
    {
        public static string GetHtml()
        {
            var memberships = Membership.GetAll().Where(m => m.Status == MembershipStatus.Active).OrderBy(x => x.DisplayOrder);

            var typesWithMembership = memberships.Select(m => GetTypeSet(m)).ToList();
            string headers = CreateHeaders(memberships);
            List<string> rows = GetRows(typesWithMembership);
            return "<table id=\"AdPackTypeMembershipTable\" class='table table-striped'>" + headers + "<tbody>" + string.Join("", rows) + "</tbody></table>";
        }

        private static List<string> GetRows(List<List<AbstractType>> typesWithMembership)
        {
            var adpackTypes = AdPackTypeManager.GetAllActiveTypes();
            List<string> rows = new List<string>();
            foreach (var t in adpackTypes)
            {
                var merged = typesWithMembership.Select(x => x.Where(i => i.TypeName == t.Name).ToList());
                StringBuilder row = new StringBuilder();

                if (!(AppSettings.RevShare.AdPack.HideAdPackTypesWhenOneEnabled && adpackTypes.Count() <= 1))
                    row.AppendFormat("<tr><td><span style='color:{0}; font-weight:bold'>{1}</span></td>", t.Color, t.Name);

                foreach (var item in merged)
                {
  
                    foreach (var i in item)
                    {
                        row.AppendFormat("<td class=\"text-center\">{0}</td><td class=\"text-center\">{1}</td>", i.Roi, i.Repurchase);
                    }               
                }
                row.Append("</tr>");
                rows.Add(row.ToString());
            }
            return rows;
        }


        private static List<AbstractType> GetTypeSet(Membership membership)
        {
            List<AbstractType> sets = new List<AbstractType>();
            var adPackTypes = AdPackTypeManager.GetAllActiveTypes();

            foreach (var type in adPackTypes)
            {
                if (membership.DisplayOrder >= new Membership(type.RequiredMembership).DisplayOrder)
                {
                    var oldRoi = type.PackReturnValuePercentage;
                    var newRoi = membership.ROIEnlargedByPercentage;
                    var oldRepurchase = type.AdBalanceReturnPercentage;
                    var newRepurchase = membership.AdPackAdBalanceReturnPercentage;

                    sets.Add(new MappedType(type.Name, oldRoi, newRoi, oldRepurchase, newRepurchase, membership.Id));
                }
                else
                    sets.Add(new NullMappedType(type.Name, membership.Id));
            }

            return sets;
        }

        private static string CreateHeaders(IEnumerable<Membership> memberships)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder props = new StringBuilder();

            sb.Append("<thead><tr>");

            if(!(AppSettings.RevShare.AdPack.HideAdPackTypesWhenOneEnabled && AdPackManager.GetAdPackActiveTypesCount() <= 1))
                sb.AppendFormat("<th class=\"vcenter\">{0}</th>", string.Format(U6000.ADPACKTYPE, AppSettings.RevShare.AdPack.AdPackName));

            foreach (var m in memberships)
            {
                sb.AppendFormat("<th class=\"text-center\">{0} ({1})</th><th class=\"text-center\">{0} ({2})</th>", 
                    "<span style='color:" + m.Color + "'>" + m.Name + "</span>", "ROI", U6000.REPURCHASE);
            }
 
            props.Append("</tr></thead>");
            return sb.ToString() + props.ToString();
        }

        public static string GetHtmlFromCache()
        {
            var cache = new AdPackTypesMembershipTableCache();
            return cache.Get().ToString();
        }
    }

    public abstract class AbstractType
    {
        public string TypeName { get; protected set; }
        public string Roi { get; protected set; }
        public string Repurchase { get; protected set; }
        public int MembershipId { get; protected set; }
    }
    public class NullMappedType : AbstractType
    {
        public NullMappedType(string typeName, int membershipId)
        {
            TypeName = typeName;
            Roi = "-";
            Repurchase = "-";
            MembershipId = membershipId;
        }
    }
    public class MappedType : AbstractType
    {
        public MappedType(string typeName, int oldRoi, int newRoi, int oldRepurchase, int newRepurchase, int membershipId)
        {
            TypeName = typeName;
            //Roi = string.Format("{0}% + {1}%", oldRoi, newRoi);
            //Repurchase = string.Format("{0}% * {1}%", oldRepurchase, newRepurchase);
            Roi = string.Format("{0}%", oldRoi + newRoi);
            Repurchase = string.Format("{0}%", (oldRepurchase *newRepurchase/100));

            MembershipId = membershipId;
        }
    }
}