using System;
using System.Net;
using Prem.PTC.Advertising;
using System.Data;
using Titan;
using Newtonsoft.Json.Linq;

namespace Prem.PTC.Offers
{

    public class CPALeadSyncedNetwork : SyncedNetwork
    {
        public CPALeadSyncedNetwork(AffiliateNetwork Network, bool ThrowExceptions = false)
            : base(Network, ThrowExceptions)
        {
        }

        public static void SynchronizeAll(bool ThrowExceptions = false)
        {
            var where = TableHelper.MakeDictionary("AffiliateNetworkSoftwareTypeInt", (int)AffiliateNetworkSoftwareType.CPALead);
            where.Add("Status", (int)NetworkStatus.Active);
            var PerformaList = TableHelper.SelectRows<AffiliateNetwork>(where);

            if (PerformaList.Count == 0 && ThrowExceptions)
                throw new MsgException("There are no Active networks to synchronize");

            foreach (var elem in PerformaList)
            {
                var SN = new CPALeadSyncedNetwork(elem);
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

                JObject element = GetElement(Network.PerformaUsername, ThrowExceptions);

                var InterestingList = TableHelper.GetListFromDataTable<CPAOffer>(dt, 100, true);

                foreach (var offer in element["offers"])
                {
                    string id = offer["campid"].ToString();
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
                    throw new MsgException("There was a problem with synchronizing " + Network.Name + ": " + ex.Message + ". Check your username/password and if the website is available");
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

                JObject element = GetElement(Network.PerformaUsername, ThrowExceptions);
                int TotalCount = Convert.ToInt32(element["number_offers"]);

                #region Universal

                //Lets process it in 1-1000 batches
                int BatchesCount = Convert.ToInt32(Math.Ceiling((double)TotalCount / (double)1000.0));
                int MainIterator = 0;

                for (int i = 0; i < BatchesCount; ++i)
                {
                    var Query = ConstructInsertQueryStart();
                    bool AreSomeRecordsInQuery = false;

                    for (int j = 0; j < 1000 && MainIterator < TotalCount; j++)
                    {

                        try
                        {

                #endregion Universal

                            var offer = element["offers"][MainIterator];

                            string id = offer["campid"].ToString();
                            string name = offer["title"].ToString();
                            string url = offer["link"].ToString();
                            string description = offer["description"].ToString();
                            string rate = offer["amount"].ToString(); // e.g. 0.24

                            string countries = "";
                            try
                            {
                                countries = offer["country"].ToString(); // 'PL' 'US'
                            }
                            catch (Exception ex) { }
                            
                            string incent = "Yes";// offer["incent"].ToString(); //'Yes' 'No'
                            string banner = ""; // offer["banner"].ToString();

                            try
                            {
                                banner = offer["creatives"][0]["url"].ToString();
                            }
                            catch (Exception ex) { }

                            #region Universal

                            SyncedNetwork.HandleAddition(name, description, rate, banner, url, incent, countries, base.Network, id, OnHoldList, ref AreSomeRecordsInQuery, ref Query);

                            //Increase the iterator
                            MainIterator++;

                        }
                        catch (Exception ex) { }
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

        private static JObject GetElement(string CPAId, bool ThrowExceptions = false)
        {
            try
            {
                using (WebClient c = new MyWebClient())
                {
                    var data = c.DownloadString("http://www.cpalead.com/dashboard/reports/campaign_json.php?id=" + CPAId);

                    JObject o = JObject.Parse(data);

                    if (o["status"].ToString() != "success")
                        throw new MsgException(o["status"].ToString());

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