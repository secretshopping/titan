using System;
using System.Linq;
using System.Text;
using Prem.PTC.Advertising;
using System.Data;
using Titan;
using Newtonsoft.Json.Linq;

namespace Prem.PTC.Offers
{

    public class HasOffersSyncedNetwork : SyncedNetwork
    {
        private static int apiCallsCount { get; set; }
        private static System.Diagnostics.Stopwatch timer { get; set; }

        private static void ApiCalled()
        {
            if (timer == null)
                timer = new System.Diagnostics.Stopwatch();
            if (!timer.IsRunning)
                timer.Start();
            apiCallsCount++;
            //ErrorLogger.Log("Timer: " + timer.Elapsed.TotalSeconds + "s," + "Api calls: " + apiCallsCount, LogType.Other);
            var elapsedSeconds = timer.Elapsed.TotalSeconds;
            if (apiCallsCount == 50 && elapsedSeconds <= 10)
            {
                System.Threading.Thread.Sleep((11 - (int)Math.Ceiling(elapsedSeconds)) * 1000);
                apiCallsCount = 0;
            }
            if (timer.Elapsed.TotalSeconds >= 10)
                timer.Reset();
        }

        public HasOffersSyncedNetwork(AffiliateNetwork Network, bool ThrowExceptions = false) : base(Network, ThrowExceptions)
        {
        }

        public static void SynchronizeAll(bool ThrowExceptions = false)
        {
            var where = TableHelper.MakeDictionary("AffiliateNetworkSoftwareTypeInt", (int)AffiliateNetworkSoftwareType.HasOffers);
            where.Add("Status", (int)NetworkStatus.Active);
            var hasOffersList = TableHelper.SelectRows<AffiliateNetwork>(where);

            if (hasOffersList.Count == 0 && ThrowExceptions)
                throw new MsgException("There are no Active networks to synchronize");

            foreach (var elem in hasOffersList)
            {
                var SN = new HasOffersSyncedNetwork(elem);
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

                var element = GetElement(Network.PublisherPassword, Network.PublisherUsername, ThrowExceptions);
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
                    throw new MsgException("There was a problem with synchronizing " + Network.Name + ": " + ex.Message + ". Check your username/password and if the website is available");
            }
        }

        public override void Download()
        {
            AffiliateNetwork Network = base.Network;

            try
            {
                DataTable dt = new DataTable();
                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    dt = bridge.Instance.ExecuteRawCommandToDataTable("SELECT * FROM CPAOffersOnHold WHERE NetworkName = '" + Network.Name + "' ORDER BY NetworkOfferIdInt ASC");
                }

                JArray element = GetElement(Network.PublisherPassword, Network.PublisherUsername, ThrowExceptions);
                var OnHoldList = TableHelper.GetListFromDataTable<CPAOfferOnHold>(dt, 100, true);

                //Lets process it in 1-1000 batches
                int TotalCount = element.Count;
                int BatchesCount = Convert.ToInt32(Math.Ceiling((double)TotalCount / (double)1000.0));
                int MainIterator = 0;

                for (int i = 0; i < BatchesCount; ++i)
                {
                    var Query = ConstructInsertQueryStart();
                    bool AreSomeRecordsInQuery = false;

                    for (int j = 0; j < 1000 && MainIterator < TotalCount; j++)
                    {
                        var offer = element[MainIterator];

                        string id = offer["id"].ToString();
                        string name = offer["name"].ToString();
                        string description = offer["description"].ToString();
                        string rate = offer["default_payout"].ToString();

                        CPAOfferOnHold ThisOffer = SyncedNetwork.HasThisOffer(OnHoldList, id, name);

                        if (ThisOffer != null)
                        {
                            //We have it, just change the status
                            ThisOffer.Status = Advertising.AdvertStatus.WaitingForAcceptance;
                        }
                        else
                        {
                            AreSomeRecordsInQuery = true;

                            //Preparte the variables to fit database
                            Money UserRate;
                            if (rate.Contains("."))
                                UserRate = Money.Parse(rate);
                            else
                                UserRate = Money.Parse(rate + ".0");
                            UserRate = Money.MultiplyPercent(UserRate, Network.DefaultPercentForMembers);

                            if (name.Length > 50) name = name.Substring(0, 49);

                            string url = GetTrackingUrl(Network.PublisherPassword, Network.PublisherUsername, id);
                            //if (string.IsNullOrEmpty(url))
                            //{
                            //    System.Diagnostics.Debug.WriteLine(id);
                            //    MainIterator++;
                            //    continue;
                            //}
                            string countries = GetCountries(id, Network.PublisherPassword, Network.PublisherUsername);

                            //We need to add it
                            CPAOfferOnHold NewOffer = new CPAOfferOnHold();
                            NewOffer.IsFromAutomaticNetwork = true;
                            NewOffer.NetworkOfferId = id;
                            NewOffer.Title = name;
                            NewOffer.TargetURL = url;
                            NewOffer.DateAdded = DateTime.Now;
                            NewOffer.Status = Advertising.AdvertStatus.Active;
                            NewOffer.AdvertiserUsername = Network.Name;
                            NewOffer.LoginBoxRequired = false;
                            NewOffer.EmailBoxRequired = false;
                            NewOffer.LastCredited = OffersManager.DateTimeZero;
                            NewOffer.Description = description;
                            NewOffer.Category = new Titan.CPACategory(Titan.CPACategory.DEFAULT_CATEGORY_ID);
                            NewOffer.BaseValue = UserRate;
                            NewOffer.CreditsBought = 1000000; //Infinity
                            NewOffer.NetworkName = Network.Name;
                            NewOffer.NetworkRate = rate;
                            NewOffer.IsSyncWithNetwork = false; // by default
                            NewOffer.IsDaily = false;
                            NewOffer.MaxDailyCredits = 1;
                            NewOffer.AddGeolocation(new GeolocationUnit(countries));
                            NewOffer.CreditOfferAs = CreditOfferAs.NetworkDefault;

                            SyncedNetwork.ConstructInsertQuery(Query, NewOffer);
                        }

                        //Increase the iterator
                        MainIterator++;

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

        private string GetCountries(string offerId, string apiKey, string networkId)
        {
            try
            {
                var queryString = new StringBuilder();
                queryString.Append("https://api.hasoffers.com/Apiv3/json?NetworkId=");
                queryString.Append(networkId);
                queryString.Append("&api_key=");
                queryString.Append(apiKey);
                queryString.Append("&Target=Affiliate_Offer&Method=getTargetCountries&ids[]=");
                queryString.Append(offerId);

                string json = null;
                using (MyWebClient c = new MyWebClient())
                {
                    ApiCalled();
                    json = c.DownloadString(queryString.ToString());
                }

                JObject o = JObject.Parse(json);
                var response = o["response"];
                var data = ((JArray)response["data"])[0];
                var countryArray = data["countries"].Children();
                var countries = countryArray.Select(c => c.ElementAt(0)["code"].ToString());

                return string.Join(",", countries);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private string GetTrackingUrl(string apiKey, string networkId, string offerId)
        {
            try
            {
                var queryString = new StringBuilder();
                queryString.Append("https://api.hasoffers.com/Apiv3/json?NetworkId=");
                queryString.Append(networkId);
                queryString.Append("&api_key=");
                queryString.Append(apiKey);
                queryString.Append("&Target=Affiliate_Offer&Method=generateTrackingLink&offer_id=");
                queryString.Append(offerId);
                queryString.Append("&params[aff_sub]=[USERNAME]");

                using (MyWebClient c = new MyWebClient())
                {
                    ApiCalled();
                    var json = c.DownloadString(queryString.ToString());

                    JObject o = JObject.Parse(json);
                    var response = o["response"];
                    var data = response["data"];
                    var url = data["click_url"];

                    return url.ToString();
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        #region SyncHelpers

        private static JArray GetElement(string apiKey, string networkId, bool ThrowExceptions = false)
        {
            try
            {
                var queryString = new StringBuilder();
                queryString.Append("https://");
                queryString.Append(networkId);
                queryString.Append(".api.hasoffers.com/Apiv3/json?");
                queryString.Append("api_key=");
                queryString.Append(apiKey);
                //queryString.Append(@"&Target=Affiliate_Offer&Method=findAll&filters[currency][NULL]=&filters[payout_type][NOT_EQUAL_TO]=cpa_percentage&filters[status]=active");
                queryString.Append(@"&Target=Affiliate_Offer&Method=findAll&filters[status]=active");

                using (MyWebClient c = new MyWebClient())
                {
                    ApiCalled();
                    var json = c.DownloadString(queryString.ToString());

                    JObject o = JObject.Parse(json);
                    var response = o["response"];
                    var data = ((response["data"]).Children()).Children();
                    var offers = data["Offer"];

                    return new JArray(offers);
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