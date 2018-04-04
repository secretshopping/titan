using System;
using System.Xml;
using Prem.PTC.Advertising;
using System.Data;
using Titan;

namespace Prem.PTC.Offers
{
    public class PointClickTrackSyncedNetwork : SyncedNetwork
    {
        public PointClickTrackSyncedNetwork(AffiliateNetwork Network, bool ThrowExceptions = false) : base(Network, ThrowExceptions) { }

        public static void SynchronizeAll(bool ThrowExceptions = false)
        {
            var where = TableHelper.MakeDictionary("AffiliateNetworkSoftwareTypeInt", (int)AffiliateNetworkSoftwareType.PointClickTrack);
            where.Add("Status", (int)NetworkStatus.Active);
            var PerformaList = TableHelper.SelectRows<AffiliateNetwork>(where);

            if (PerformaList.Count == 0 && ThrowExceptions)
                throw new MsgException("There are no Active networks to synchronize");

            foreach (var elem in PerformaList)
            {
                var SN = new PointClickTrackSyncedNetwork(elem);
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

                XmlDocument doc = GetElement(Network.PublisherPassword, Network.PublisherUsername, ThrowExceptions);
                XmlElement root = doc.DocumentElement;

                var InterestingList = TableHelper.GetListFromDataTable<CPAOffer>(dt, 100, true);
                var idNodes = root.SelectNodes("/campaigns/campaign/id");

                foreach (XmlNode node in idNodes)
                {
                    string id = node.InnerText;
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

                XmlDocument doc = GetElement(Network.PublisherPassword, Network.PublisherUsername, ThrowExceptions);
                XmlElement root = doc.DocumentElement;
                XmlNodeList elemList = root.ChildNodes;
                int TotalCount = elemList.Count;

                XmlNodeList ids = root.SelectNodes("/offers/offer/offer_id");
                XmlNodeList names = root.SelectNodes("/offers/offer/offer_name");
                XmlNodeList descriptions = root.SelectNodes("/offers/offer/requirements");
                XmlNodeList commissions = root.SelectNodes("/offers/offer/rate");
                XmlNodeList countries = root.SelectNodes("/offers/offer/allowed_countries");
                XmlNodeList incentives = root.SelectNodes("/offers/offer/cash_incentive_allowed");
                XmlNodeList banners = root.SelectNodes("/offers/offer/creatives/image_url");
                XmlNodeList urls = root.SelectNodes("/offers/offer/creatives/publisher_url");


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
                            var offer = elemList[MainIterator];

                            string id = ids[MainIterator].InnerText;
                            string name = names[MainIterator].InnerText;
                            string description = descriptions[MainIterator].InnerText;
                            string incent = incentives[MainIterator].InnerText;
                            string url = urls[MainIterator].InnerText.Replace("sid1/", "sid1/%USERNAME%");
                            string banner = banners[MainIterator].InnerText;
                            string rate = commissions[MainIterator].InnerText;
                            var countriesList = countries[MainIterator].SelectNodes("country");
                            string countriesString = "";
                            if (countriesList.Count == 1 && countriesList[0].InnerText.ToUpper() == "ALL")
                                countriesString = "";
                            else
                            {
                                foreach (XmlElement country in countriesList)
                                {
                                    countriesString += country.InnerText.Trim() + ", ";
                                }
                                if (countriesString.EndsWith(", "))
                                    countriesString = countriesString.Remove(countriesString.Length - 2);
                            }                            

                            if (!string.IsNullOrWhiteSpace(url))
                                SyncedNetwork.HandleAddition(name, description, rate, banner, url, incent, countriesString, base.Network, id, OnHoldList, ref AreSomeRecordsInQuery, ref Query);

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

        private static XmlDocument GetElement(string apiKey, string userName, bool ThrowExceptions = false)
        {
            try
            {
                using (MyWebClient c = new MyWebClient())
                {
                    var data = c.DownloadString(string.Format("http://pointclicktrack.com/api/offers/format/xml/username/{0}/key/{1}/", userName, apiKey));
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(data);

                    return doc;
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
        #endregion SyncHelpers
    }
}