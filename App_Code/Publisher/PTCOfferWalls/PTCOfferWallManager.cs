using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Prem.PTC.Offers;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan.Publisher;

namespace Titan.Publish.PTCOfferWalls
{
    public class PTCOfferWallManager
    {
        public PublishersWebsite PublishersWebsite { get; private set; }
        string countryCode;
        int? age;
        string externalUsername;
        bool isMobile;
        public Gender Gender;

        public PTCOfferWallManager(string host, string countryCode, string externalUsername, 
            int publishersWebsiteId, bool isMobile, string age = null, string gender = null)
        {
            PublishersWebsite = PublishersWebsite.GetActiveWebsite(host, publishersWebsiteId);
            this.countryCode = countryCode;
            this.age = age.ToNullableInt();
            this.externalUsername = externalUsername;
            this.isMobile = isMobile;
            if (!Enum.TryParse(gender, true, out this.Gender))
                this.Gender = Gender.Null;
        }

        public List<PTCOfferWall> GetOfferWalls()
        {
            var ptcOfferWallViews = PTCOfferWallView.GetWatchedToday(externalUsername, PublishersWebsite.Id);

            var activeOffers = PTCOfferWall.GetActive(countryCode, this.age, Gender, isMobile);
            var offersToReturn = new List<PTCOfferWall>();

            foreach(var o in activeOffers)
            {
                if (ptcOfferWallViews.Count(v => v.PTCOfferWallId == o.Id) < o.SingleUserViewsPerDay)
                    offersToReturn.Add(o);
            }

            offersToReturn.Shuffle(new Random());

            return offersToReturn;
        }

        public static List<UserUrl> GetUserUrls(int ptcOfferWallId)
        {
            string query = string.Format(@"SELECT * FROM UserUrls uu 
                                    JOIN PTCOfferWallsUserUrls mappings 
                                    ON uu.Id = mappings.UserUrlId 
                                    WHERE mappings.PTCOfferWallId = {0}",
                                        ptcOfferWallId);

            return TableHelper.GetListFromRawQuery<UserUrl>(query);
        }      
    }
}
