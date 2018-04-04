using MarchewkaOne.Titan.Balances;
using Newtonsoft.Json;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.Publisher.InTextAds
{
    public class InTextAdvertTag : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "InTextAdvertsTags"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("InTextAdvertId")]
        public int InTextAdvertId { get { return _InTextAdvertId; } protected set { _InTextAdvertId = value; SetUpToDateAsFalse(); } }

        [Column("Tag")]
        public string Tag { get { return _Tag; } protected set { _Tag = value.ToLower(); SetUpToDateAsFalse(); } }

        #endregion Columns

        int _Id, _InTextAdvertId;
        string _Tag;

        public InTextAdvertTag(int id) : base(id) { }

        public InTextAdvertTag(DataRow row, bool isUpToDate = true)
                    : base(row, isUpToDate) { }

        private InTextAdvertTag(int inTextAdvertId, string tag)
        {
            InTextAdvertId = inTextAdvertId;
            Tag = tag;
        }

        public static void CreateMapping(int inTextAdvertId, string tag)
        {
            if (!RecordExists(inTextAdvertId, tag))
            {
                var item = new InTextAdvertTag(inTextAdvertId, tag);
                item.Save();
            }
        }

        private static bool RecordExists(int inTextAdvertId, string tag)
        {
            var whereDict = new Dictionary<string, object>();
            whereDict["InTextAdvertId"] = inTextAdvertId;
            whereDict["Tag"] = tag;
            return TableHelper.RowExists(InTextAdvertTag.TableName, whereDict);
        }



        public static List<InTextAdvertTag> GetActive()
        {
            string query = string.Format(@"SELECT tags.Id, tags.Tag, tags.InTextAdvertId FROM InTextAdverts ads 
                                    JOIN InTextAdvertsTags tags 
                                    ON tags.InTextAdvertId = ads.Id 
                                    WHERE ads.Status = {0};",
                                        (int)AdvertStatus.Active);

            return TableHelper.GetListFromRawQuery<InTextAdvertTag>(query);
        }

        [Obsolete]
        public static string GetActiveTagsAsJson()
        {
            var activeTags = GetActive();

            var json = JsonConvert.SerializeObject(activeTags.Select(t => t.Tag));
            return json;
        }
    }
}