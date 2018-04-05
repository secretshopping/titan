using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Resources;
using Prem.PTC.Advertising;
using Prem.PTC.Offers;
using Titan;
using System.Text;
using Prem.PTC.Members;
using Titan.Publisher;

namespace Titan.Publish.PTCOfferWalls
{
    public class PTCOfferWallSubmisson
    {


        #region Constructors
        int ptcOfferWallId;
        string externalUsername;
        string ptcOfferWallTitle;
        string subId2;
        string subId3;
        string ip;
        string countryCode;
        Gender gender;
        int age;
        PublishersWebsite publishersWebsite;

        public PTCOfferWallSubmisson(int ptcOfferWallId, string ptcOfferWallTitle, int publishersWebsiteId, string externalUsername, string subId2,
            string subId3, string ip, string countryCode, Gender gender = Prem.PTC.Members.Gender.Null, int? age = null)
        {
            this.publishersWebsite = new PublishersWebsite(publishersWebsiteId);
            this.ptcOfferWallId = ptcOfferWallId;
            this.externalUsername = externalUsername;
            this.ptcOfferWallTitle = ptcOfferWallTitle;

            this.subId2 = subId2;
            this.subId3 = subId3;
            this.ip = ip;
            this.countryCode = countryCode;
            this.gender = gender;
            this.age = age.HasValue ? age.Value : -1;
        }
        #endregion


        public void Credit()
        {
            var offerWall = new PTCOfferWall(ptcOfferWallId);
            offerWall.Watched();

            var crediter = new PtcOfferWallCrediter(offerWall.PricePaid / offerWall.CompletionTimesBought, publishersWebsite.UserId);
            var moneyLeftForPools = crediter.Credit(HandleSuccessfulCredit);
        }

        private void HandleSuccessfulCredit(Money payout)
        {
            try
            {
                PTCOfferWallView.Add(externalUsername, ptcOfferWallId, publishersWebsite.Id, payout);
                GlobalPostback.Create(GlobalPostbackType.PtcOfferWall, ptcOfferWallId, ptcOfferWallTitle, publishersWebsite.Id,
                    externalUsername, subId2, subId3, ip, countryCode, payout, gender, age).Send();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }

    }
}