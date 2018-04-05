using System;
using System.Net;
using Prem.PTC.Advertising;
using System.Data;
using Titan;
using Newtonsoft.Json.Linq;

namespace Prem.PTC.Offers
{
    public class AdGateSyncedNetwork : SyncedNetwork
    {
        public AdGateSyncedNetwork(AffiliateNetwork Network, bool ThrowExceptions = false) : base(Network, ThrowExceptions) { }

        public static void SynchronizeAll(bool ThrowExceptions = false)
        {
            var where = TableHelper.MakeDictionary("AffiliateNetworkSoftwareTypeInt", (int)AffiliateNetworkSoftwareType.AdGateMedia);
            where.Add("Status", (int)NetworkStatus.Active);
            var PerformaList = TableHelper.SelectRows<AffiliateNetwork>(where);

            if (PerformaList.Count == 0 && ThrowExceptions)
                throw new MsgException("There are no Active networks to synchronize");

            foreach (var elem in PerformaList)
            {
                var SN = new AdGateSyncedNetwork(elem);
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

                JArray element = GetElement(Network.PublisherUsername, Network.PublisherPassword, ThrowExceptions);

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

                JArray element = GetElement(Network.PublisherUsername, Network.PublisherPassword, ThrowExceptions);
                int TotalCount = element.Count;

                #region Universal

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

                #endregion Universal

                            var offer = element[MainIterator];

                            string id = offer["id"].ToString();
                            string name = offer["name"].ToString();
                            string url = offer["tracking_url"].ToString();
                            string description = offer["requirements"].ToString();
                            string rate = offer["payout"].ToString(); // e.g. 0.24

                            string countries = "";
                            try
                            {

                                string[] ctrs = offer["country"].ToString().Split(',');

                                if (ctrs[0] == "-" || ctrs.Length > 20)
                                    countries = "";
                                else
                                {
                                    foreach (var elem in ctrs)
                                    {
                                        countries += elem + ",";
                                    }
                                }

                            }
                            catch (Exception ex) { }
                            
                            string incent = "Yes";// offer["incent"].ToString(); //'Yes' 'No'
                            string banner = ""; // offer["banner"].ToString();

                            //try
                            //{
                            //    banner = offer["creatives"][0]["url"].ToString();
                            //}
                            //catch (Exception ex) { }

                
                            #region Universal


                            SyncedNetwork.HandleAddition(name, description, rate, banner, url, incent, countries, base.Network, id, OnHoldList, ref AreSomeRecordsInQuery, ref Query);

                            //Increase the iterator
                            MainIterator++;

                        }
                        catch (Exception ex) {}
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
                            #endregion Universal

        }

        #region SyncHelpers

        private static JArray GetElement(string affiliateId, string apiKey, bool ThrowExceptions = false)
        {
            try
            {
                using (WebClient c = new MyWebClient())
                {
                    var data = c.DownloadString("https://api.adgatemedia.com/v1/offers?aff=" + affiliateId + "&api_key=" + apiKey);

                    JArray o = JArray.Parse(data);

                    return o;
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