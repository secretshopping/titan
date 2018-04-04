using System;
using System.Collections.Generic;
using System.Linq;
using Prem.PTC;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using System.Xml;
using System.Text;
using Prem.PTC.Utils;
using Prem.PTC.Members;
using Prem.PTC.Referrals;
using Prem.PTC.Advertising;
using Prem.PTC.Offers;
using Prem.PTC.Payments;
using System.Data;
using Titan;

namespace Prem.PTC.Offers
{

    public class PerformaManager : SyncedNetwork
    {
        public PerformaManager(AffiliateNetwork Network, bool ThrowExceptions = false) : base(Network, ThrowExceptions)
        {        
        }

        public static void SynchronizeAll(bool ThrowExceptions = false)
        {
            var where = TableHelper.MakeDictionary("AffiliateNetworkSoftwareTypeInt", (int)AffiliateNetworkSoftwareType.Performa);
            where.Add("Status", (int)NetworkStatus.Active);
            var PerformaList = TableHelper.SelectRows<AffiliateNetwork>(where);

            if (PerformaList.Count == 0 && ThrowExceptions)
                throw new MsgException("There are no Active networks to synchronize");

            foreach (var elem in PerformaList)
            {
                var SN = new PerformaManager(elem);
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

                XmlElement element = GetElement(Network.Name, Network.PerformaUsername, Network.PerformaPassword, Network.PerformaWebsiteId, ThrowExceptions);
                var InterestingList = TableHelper.GetListFromDataTable<CPAOffer>(dt, 100, true);

                foreach (XmlNode offer in element.ChildNodes)
                {
                    string id = offer["id"].InnerText;
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

                XmlElement element = GetElement(Network.Name, Network.PerformaUsername, Network.PerformaPassword, Network.PerformaWebsiteId, ThrowExceptions);
                var OnHoldList = TableHelper.GetListFromDataTable<CPAOfferOnHold>(dt, 100, true);

                //Lets process it in 1-1000 batches
                int TotalCount = element.ChildNodes.Count;
                int BatchesCount = Convert.ToInt32(Math.Ceiling((double)TotalCount / (double)1000.0));
                int MainIterator = 0;

                for (int i = 0; i < BatchesCount; ++i)
                {
                    var Query = ConstructInsertQueryStart();
                    bool AreSomeRecordsInQuery = false;

                    for (int j = 0; j < 1000 && MainIterator < TotalCount; j++)
                    {
                        XmlNode offer = element.ChildNodes[MainIterator];

                        string id = offer["id"].InnerText;
                        string name = offer["name"].InnerText;
                        string type = offer["type"].InnerText;
                        string url = offer["url"].InnerText;
                        string description = offer["description"].InnerText;
                        string requirements = offer["requirements"].InnerText;
                        string rate = offer["rate"].InnerText; // e.g. 0.24
                        string categoy = offer["categoy"].InnerText;
                        string countries = offer["countries"].InnerText;
                        string incent = offer["incent"].InnerText; //'Yes' 'No'
                        string banner = offer["banner"].InnerText;

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

                            StringBuilder sb = new StringBuilder();
                            if (!string.IsNullOrEmpty(description))
                            {
                                sb.Append(description);
                                sb.Append("; ");
                            }
                            sb.Append(requirements);

                            Money UserRate;
                            if (rate.Contains("."))
                                UserRate = Money.Parse(rate);
                            else
                                UserRate = Money.Parse(rate + ".0");
                            UserRate = Money.MultiplyPercent(UserRate, Network.DefaultPercentForMembers);

                            if (name.Length > 50) name = name.Substring(0, 49);
                            if (!string.IsNullOrEmpty(banner) && banner.Length > 150) banner = banner.Substring(0, 149);

                            string ParsedDescription = sb.ToString();
                            if (ParsedDescription.Length >= 4000)
                                ParsedDescription = ParsedDescription.Substring(0, 3998);

                            //We need to add it
                            CPAOfferOnHold NewOffer = new CPAOfferOnHold();
                            NewOffer.IsFromAutomaticNetwork = true;
                            NewOffer.NetworkOfferId = id;
                            NewOffer.Title = name;
                            NewOffer.ImageURL = banner;
                            NewOffer.TargetURL = url;
                            NewOffer.DateAdded = DateTime.Now;
                            NewOffer.Status = Advertising.AdvertStatus.Active;
                            NewOffer.AdvertiserUsername = Network.Name;
                            NewOffer.LoginBoxRequired = false;
                            NewOffer.EmailBoxRequired = false;
                            NewOffer.LastCredited = OffersManager.DateTimeZero;
                            NewOffer.Description = ParsedDescription;
                            NewOffer.Category = new Titan.CPACategory(Titan.CPACategory.DEFAULT_CATEGORY_ID);
                            NewOffer.BaseValue = UserRate;
                            NewOffer.CreditsBought = 1000000; //Infinity
                            NewOffer.NetworkName = Network.Name;
                            NewOffer.NetworkRate = rate;
                            NewOffer.IsSyncWithNetwork = false; // by default
                            NewOffer.IsDaily = false;
                            NewOffer.MaxDailyCredits = 1;
                            //NewOffer.GeolocatedCountries = countries;

                            if (incent.Trim() == "No")
                                NewOffer.CreditOfferAs = CreditOfferAs.NonCash;
                            else
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

        #region SyncHelpers

        private static XmlElement GetElement(string networkUrl, string username, string password, string websiteId, bool ThrowExceptions = false)
        {
            string xml = GetExportResponse(networkUrl, username, password, websiteId);
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(xml);
                return doc.DocumentElement;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                ErrorLogger.Log(xml);
                if (ThrowExceptions)
                    throw new MsgException(ex.Message);
            }

            return null;
        }

        private static string GetExportResponse(string networkUrl, string username, string password, string websiteId)
        {
            using (var client = new CookieAwareWebClient())
            {
                client.BaseAddress = @"http://" + networkUrl + "/publishers/";
                var loginData = new NameValueCollection();
                loginData.Add("username", username);
                loginData.Add("password", password);
                client.UploadValues("login.php?next", "POST", loginData);

                //Now you are logged in and can request pages    
                var formData = new NameValueCollection();
                formData.Add("export", "xml");
                formData.Add("next", "1");
                formData.Add("yti1", "");
                formData.Add("yti2", "");
                formData.Add("yti3", "");
                formData.Add("yti4", "");
                formData.Add("yti5", "");
                var result = client.UploadValues("campaigns/export.php?website=" + websiteId, "POST", formData);

                string xml = Encoding.UTF8.GetString(result);
                xml = xml.TrimStart();
                return xml;
            }
        }

        public static bool IsWebsiteIDOK(string networkUrl, string username, string password, string websiteId)
        {
            string resp = GetExportResponse(networkUrl, username, password, websiteId);
            if (resp.StartsWith("The website provided is not valid"))
                return false;
            return true;
        }

        #endregion SyncHelpers

    }



}