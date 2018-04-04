using System;
using System.Linq;

namespace Titan.Publisher.Security
{
    public class ActionTrackerCreator<T> where T : ActionTracker
    {
        int publisherWebsiteId;
        string ip;
        Type type;
        int advertId;

        public ActionTrackerCreator(int publisherWebsiteId, string ip, int advertId)
        {
            this.publisherWebsiteId = publisherWebsiteId;
            this.ip = ip;
            this.type = typeof(T);
            this.advertId = advertId;
        }

        public T GetOrCreate()
        {

            var query = string.Format("SELECT TOP 1 * FROM {0} WHERE PublishersWebsiteId = {1} AND Ip = '{2}' AND AdvertId = {3};",
                type.Name, publisherWebsiteId, ip, advertId);

            var record = TableHelper.GetListFromRawQuery<T>(query).FirstOrDefault();

            if (record == null)
            {
                record = (T)Activator.CreateInstance(type, publisherWebsiteId, ip, advertId);
            }

            return record;
        }
    }
}