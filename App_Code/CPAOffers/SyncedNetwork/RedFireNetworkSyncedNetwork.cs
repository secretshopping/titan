using System;
using Prem.PTC.Advertising;
using System.Data;
using Titan;
using Newtonsoft.Json.Linq;

namespace Prem.PTC.Offers
{
    public class RedFireNetworkSyncedNetwork : SyncedNetwork
    {
        public RedFireNetworkSyncedNetwork(AffiliateNetwork Network, bool ThrowExceptions = false) : base(Network, ThrowExceptions) { }

        public static void SynchronizeAll(bool ThrowExceptions = false)
        {
            var where = TableHelper.MakeDictionary("AffiliateNetworkSoftwareTypeInt", (int)AffiliateNetworkSoftwareType.RedFireNetwork);
            where.Add("Status", (int)NetworkStatus.Active);
            var PerformaList = TableHelper.SelectRows<AffiliateNetwork>(where);

            if (PerformaList.Count == 0 && ThrowExceptions)
                throw new MsgException("There are no Active networks to synchronize");

            foreach (var elem in PerformaList)
            {
                var SN = new RedFireNetworkSyncedNetwork(elem);
                SN.Synchronize();
            }
        }

        public override void Synchronize()
        {
            AffiliateNetwork Network = base.Network;

            try
            {
                DataTable dt = new DataTable();
                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    dt = bridge.Instance.ExecuteRawCommandToDataTable("SELECT * FROM CPAOffers WHERE NetworkName = '" + Network.Name + "' ORDER BY NetworkOfferIdInt ASC");
                }

                JArray element = GetElement(Network.PublisherPassword, ThrowExceptions);

                var InterestingList = TableHelper.GetListFromDataTable<CPAOffer>(dt, 100, true);

                foreach (var offer in element)
                {
                    string id = offer["id"].ToString();
                    //Check if already on the list
                    CPAOffer ThisOffer = SyncedNetwork.HasThisOffer(InterestingList, id);
                    if (ThisOffer != null)
                    {
                        //We have it
                        ThisOffer.Status = Advertising.AdvertStatus.WaitingForAcceptance;
                    }
                }

                //Now, remove the offers that shouldn't be on the list
                foreach (var elem in InterestingList)
                    if (elem.Status != AdvertStatus.WaitingForAcceptance)
                    {
                        //"Delete" it
                        elem.Status = AdvertStatus.Rejected;
                        elem.IsSyncWithNetwork = false;
                        elem.Save();
                    }

                Network.LastSyncDate = DateTime.Now;
                Network.Save();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                if (base.ThrowExceptions)
                    throw new MsgException("There was a problem with synchronizing " + Network.Name + ": " + ex.Message + ". Check your Affiliate Id/API Key and if the website is available");
            }
        }

        public override void Download()
        {
            #region Universal

            AffiliateNetwork Network = base.Network;

            try
            {
                DataTable dt = new DataTable();
                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    dt = bridge.Instance.ExecuteRawCommandToDataTable("SELECT * FROM CPAOffersOnHold WHERE NetworkName = '" + Network.Name + "' ORDER BY NetworkOfferIdInt ASC");
                }
                var OnHoldList = TableHelper.GetListFromDataTable<CPAOfferOnHold>(dt, 100, true);

                #endregion Universal

                JArray element = GetElement(Network.PublisherPassword, ThrowExceptions);
                int TotalCount = element.Count;

                //Lets process it in 1-1000 batches
                int BatchesCount = Convert.ToInt32(Math.Ceiling((double)TotalCount / (double)400.0));
                int MainIterator = 0;

                for (int i = 0; i < BatchesCount; ++i)
                {
                    var Query = ConstructInsertQueryStart();
                    bool AreSomeRecordsInQuery = false;

                    for (int j = 0; j < 400 && MainIterator < TotalCount; j++)
                    {
                        try
                        {
                            var offer = element[MainIterator];

                            string id = offer["offer_id"].ToString();
                            string name = offer["offer_name"].ToString();
                            string description = offer["offer_requirements"].ToString();
                            string rate = offer["offer_commission"].ToString();
                            string incent = offer["offer_allows_incent"].ToString();

                            string url = String.Empty;
                            string banner = String.Empty;

                            string countries = GetGeolocation(Network.PublisherPassword, id).Replace("UK", "GB");
                            var creatives = GetCreatives(Network.PublisherPassword, id);

                            if (creatives["offer_creative_banners"].HasValues)
                            {
                                url = creatives["offer_creative_banners"][0]["banner_tracking_link"].ToString().Replace("&subid=&", "&subid=%USERNAME%&"); ;
                                banner = creatives["offer_creative_banners"][0]["banner_creative_url"].ToString();
                            }
                            else if (creatives["offer_text_creatives"].HasValues)
                            {
                                url = creatives["offer_text_creatives"][0]["creative_link"].ToString().Replace("&subid=&", "&subid=%USERNAME%&"); ;
                            }

                            if (!string.IsNullOrWhiteSpace(url) && !url.ToUpper().Contains("NOT APPROVED"))
                                SyncedNetwork.HandleAddition(name, description, rate, banner, url, incent, countries, base.Network, id, OnHoldList, ref AreSomeRecordsInQuery, ref Query);

                            //Increase the iterator
                            MainIterator++;
                        }
                        catch (Exception ex) { ErrorLogger.Log(ex); }
                    }
                    //Now execute the batch;
                    if (AreSomeRecordsInQuery)
                    {
                        using (var bridge = ParserPool.Acquire(Database.Client))
                        {
                            if (Query[Query.Length - 1] == ',')
                                Query[Query.Length - 1] = ';';
                            bridge.Instance.ExecuteRawCommandNonQuery(Query.ToString());
                        }
                    }
                }

                //Now, remove the offers that shouldn't be on the list
                foreach (var elem in OnHoldList)
                    if (elem.Status != AdvertStatus.WaitingForAcceptance)
                        elem.Delete();

                Network.LastSyncDate = DateTime.Now;
                Network.Save();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                if (base.ThrowExceptions)
                    throw new MsgException("There was a problem with offers export/download " + Network.Name + ": "
                        + ex.Message + ". Check your username/password and if the website is available");
            }
        }

        #region SyncHelpers

        private static JArray GetElement(string apiKey, bool ThrowExceptions = false)
        {
            try
            {
                using (MyWebClient c = new MyWebClient())
                { 
                    var data = c.DownloadString(string.Format("http://redfirenetwork.afftrack.com/apiv2/?key={0}&action=offers&format=json&limit=1652", apiKey));

                    JObject o = JObject.Parse(data);
                    return (JArray)o["offers"];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                if (ThrowExceptions)
                    throw new MsgException(ex.Message);
            }
            return null;
        }

        private static string GetGeolocation(string apiKey, string offerId)
        {
            string result = "";

            try
            {
                using (MyWebClient c = new MyWebClient())
                {
                    var data = c.DownloadString(string.Format("http://redfirenetwork.afftrack.com/apiv2/?key={0}&action=offers_countries&format=json&id={1}", apiKey, offerId));

                    JObject o = JObject.Parse(data);
                    result = o["Offer " + offerId].ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }

            return result;
        }

        private static JObject GetCreatives(string apiKey, string offerId)
        {
            try
            {
                using (MyWebClient c = new MyWebClient())
                {
                    var data = c.DownloadString(string.Format("http://redfirenetwork.afftrack.com/apiv2/?key={0}&action=offer_creatives&format=json&pid={1}", apiKey, offerId));

                    JObject o = JObject.Parse(data);
                    return (JObject)((JArray)o["Offer Creatives"])[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }

            return null;
        }
        #endregion SyncHelpers
    }
}