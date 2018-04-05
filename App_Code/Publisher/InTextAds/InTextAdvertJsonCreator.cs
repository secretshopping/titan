using Newtonsoft.Json;
using Prem.PTC;
using Prem.PTC.Advertising;
using System.Text;
using System.Linq;
using System.Data;
using System;

namespace Titan.Publisher.InTextAds
{
    public class InTextAdvertJsonCreator
    {
        MappedInTextAdvertCollection mapped;
        public InTextAdvertJsonCreator()
        {
            this.mapped = new MappedInTextAdvertCollection();
        }

        public string GetAdsAsJson()
        {
            var distinctAds = GetDistinctAds();
            string json = JsonConvert.SerializeObject(distinctAds);
            return json;
        }

        #region Private Methods

        void Map()
        {
            var dataTable = new DataTable();
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                dataTable = bridge.Instance.ExecuteRawCommandToDataTable(GetSelectQueryQuery());
            }

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                int id = dataTable.Rows[i].Field<int>("Id");
                string title = dataTable.Rows[i].Field<string>("Title");
                string description = dataTable.Rows[i].Field<string>("Description");
                string tag = dataTable.Rows[i].Field<string>("Tag").ToLower();
                var result = new MappedInTextAdvert(id, title, description, tag);
                mapped.Add(result);
            }
        }

        /// <summary>
        /// Returns MappedInTextAdvertCollection with distinct Tags 
        /// </summary>
        MappedInTextAdvertCollection GetDistinctAds()
        {
            Map();
            var groupedCollection = mapped.GroupBy(m => m.Tag);
            var distinctCollection = new MappedInTextAdvertCollection();
            foreach (var item in groupedCollection)
            {
                var index = new Random().Next(0, item.Count());

                distinctCollection.Add(item.ToArray()[index]);
            }
            return distinctCollection;
        }

        string GetSelectQueryQuery()
        {
            string query = string.Format(@"SELECT ads.Id, ads.Title, ads.Description, tags.Tag
                                    FROM InTextAdverts ads 
                                    JOIN InTextAdvertsTags tags 
                                    ON tags.InTextAdvertId = ads.Id 
                                    WHERE ads.Status = {0};",
                                    (int)AdvertStatus.Active);

            return query;
        }
        #endregion
    }
}