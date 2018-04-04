using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using Prem.PTC.Offers;
using Prem.PTC;
using ExtensionMethods;
using Prem.PTC.Memberships;

namespace Titan
{
    public abstract class SyncedNetwork
    {
        protected AffiliateNetwork Network;
        protected bool ThrowExceptions = false;

        public SyncedNetwork(AffiliateNetwork Network, bool ThrowExceptions = false)
        {
            this.Network = Network;
            this.ThrowExceptions = ThrowExceptions;
        }


        public abstract void Synchronize();

        public abstract void Download();


        #region GlobalSyncFunctions

        //OFFER
        /// <summary>
        /// Affect CPAOffer ONLY. Doesn't affect CPAOfferOnHold AT ALL!!
        /// </summary>
        /// <param name="ThisOffer"></param>
        public static void AddOfferToSync(CPAOfferOnHold ThisOffer)
        {
            //Wasnt synced, now need to ADD to sync
            //Check if not already in CPAOffers but deactivated (Status = rejected, issync = false)
            var where = TableHelper.MakeDictionary("NetworkOfferIdInt", ThisOffer.NetworkOfferId);
            where.Add("NetworkName", ThisOffer.NetworkName);

            var PreviousOffers = TableHelper.SelectRows<CPAOffer>(where);
            if (PreviousOffers.Count > 0)
            {
                //It already exists, activate it
                CPAOffer OfferToActivate = PreviousOffers[0];
                OfferToActivate.Status = AdvertStatus.Active;
                OfferToActivate.IsSyncWithNetwork = true;
                OfferToActivate.Save();
            }
            else
            {
                //We need to add it
                CPAOffer NewOffer = new CPAOffer();
                NewOffer.IsFromAutomaticNetwork = ThisOffer.IsFromAutomaticNetwork;
                NewOffer.NetworkOfferId = ThisOffer.NetworkOfferId;
                NewOffer.Title = ThisOffer.Title;
                NewOffer.ImageURL = ThisOffer.ImageURL;
                NewOffer.TargetURL = ThisOffer.TargetURL;
                NewOffer.DateAdded = DateTime.Now;
                NewOffer.Status = AdvertStatus.Active;
                NewOffer.AdvertiserUsername = ThisOffer.AdvertiserUsername;
                NewOffer.LoginBoxRequired = ThisOffer.LoginBoxRequired;
                NewOffer.EmailBoxRequired = ThisOffer.EmailBoxRequired;
                NewOffer.LastCredited = OffersManager.DateTimeZero;
                NewOffer.Description = ThisOffer.Description;
                NewOffer.Category = ThisOffer.Category;
                NewOffer.BaseValue = ThisOffer.BaseValue;
                NewOffer.CreditsBought = ThisOffer.CreditsBought; //Infinity
                NewOffer.NetworkName = ThisOffer.NetworkName;
                NewOffer.NetworkRate = ThisOffer.NetworkRate;
                NewOffer.IsSyncWithNetwork = ThisOffer.IsSyncWithNetwork; // by default
                NewOffer.IsDaily = ThisOffer.IsDaily;
                NewOffer.MaxDailyCredits = ThisOffer.MaxDailyCredits;
                NewOffer.CopyGeolocation(ThisOffer);
                NewOffer.CreditOfferAs = ThisOffer.CreditOfferAs;
                NewOffer.RequiredMembership = Membership.Standard.Id;
                NewOffer.Save();
            }
        }

        /// <summary>
        /// Removes BOTH: CPAOffer and CPAOfferOnHOld
        /// Needs to save CPAOfferOnHold ONLY!!!!
        /// </summary>
        /// <param name="Offer"></param>
        public static void RemoveOfferFromSync(CPAOfferOnHold Offer)
        {
            Offer.IsSyncWithNetwork = false;

            //Was synced but IT IS NOT ANYMORE
            var where = TableHelper.MakeDictionary("NetworkOfferIdInt", Offer.NetworkOfferId);
            where.Add("NetworkName", Offer.NetworkName);

            var PreviousOffers = TableHelper.SelectRows<CPAOffer>(where);
            CPAOffer PreviousOffer = PreviousOffers[0];
            RemoveOfferFromSync(PreviousOffer);
            Offer.Save();
        }

        /// <summary>
        /// Doesnt handle CPAOfferOnHold
        /// No need to save
        /// </summary>
        /// <param name="Offer"></param>
        public static void RemoveOfferFromSync(CPAOffer Offer)
        {
            //Was synced but IT IS NOT ANYMORE
            Offer.IsSyncWithNetwork = false;
            Offer.Status = AdvertStatus.Rejected;
            Offer.Save();
        }

        #endregion GlobalSyncFunctions

        #region SyncHelpers

        /// <summary>
        /// Returns NULL if offer is not on the list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="OfferId"></param>
        /// <param name="OfferName"></param>
        /// <returns></returns>
        protected static CPAOffer HasThisOffer(List<CPAOffer> list, string id)
        {
            try
            {
                int ID = Convert.ToInt32(id);
                for (int i = 0; i < list.Count; ++i)
                {
                    if (Convert.ToInt32(list[i].NetworkOfferId) == ID)
                        return list[i];
                }
                return null;
            }
            catch (FormatException ex)
            {
                //Value is not integer
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].NetworkOfferId == id)
                        return list[i];
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Returns NULL if offer is not on the list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="OfferId"></param>
        /// <param name="OfferName"></param>
        /// <returns></returns>
        protected static CPAOfferOnHold HasThisOffer(List<CPAOfferOnHold> list, string id, string title)
        {
            try
            {
                int ID = Convert.ToInt32(id);
                for (int i = 0; i < list.Count; ++i)
                {
                    if (Convert.ToInt32(list[i].NetworkOfferId) == ID)
                        return list[i];
                }
                return null;
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is OverflowException)
                {
                    //Value is not integer
                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (list[i].NetworkOfferId == id)
                            return list[i];
                    }
                    return null;
                }
            }
            return null;
        }

        protected static void ConstructInsertQuery(StringBuilder sb, CPAOfferOnHold Offer)
        {
            sb.Append(" (")
              .Append(Convert.ToInt32(Offer.IsFromAutomaticNetwork))
              .Append(", ")
              .Append(Offer.NetworkOfferId)
              .Append(", '")
              .Append(Offer.Title.Replace("'", "''"))
              .Append("', '")
              .Append(Offer.TargetURL)
              .Append("', '")
              .Append(Offer.DateAdded.ToDBString())
              .Append("', ")
              .Append((int)Offer.Status)
              .Append(", '")
              .Append(Offer.ImageURL)
              .Append("', '")
              .Append(Offer.AdvertiserUsername)
              .Append("', ")
              .Append(Convert.ToInt32(Offer.LoginBoxRequired))
              .Append(", ")
              .Append(Convert.ToInt32(Offer.EmailBoxRequired))
              .Append(", '")
              .Append(Offer.LastCredited.ToDBString())
              .Append("', '")
              .Append(Offer.Description.Replace("'", "''"))
              .Append("', ")
              .Append((int)Offer.Category.Id)
              .Append(", ")
              .Append(Offer.BaseValue.ToClearString())
              .Append(", ")
              .Append(Offer.CreditsBought)
              .Append(", '")
              .Append(Offer.NetworkName)
              .Append("', '")
              .Append(Offer.NetworkRate)
              .Append("', ")
              .Append(Convert.ToInt32(Offer.IsSyncWithNetwork))
              .Append(", ")
              .Append(Convert.ToInt32(Offer.IsDaily))
              .Append(", ")
              .Append(Convert.ToInt32(Offer.MaxDailyCredits))
              .Append(", ")
              .Append(Convert.ToInt32(Offer.CreditOfferAs))
              .Append(", ")
              .Append(Offer.GetGeolocationSQL())
              .Append(")");

            sb.Append(",");
        }

        protected static StringBuilder ConstructInsertQueryStart()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"INSERT INTO CPAOffersOnHold (IsFromAutomaticNetwork, NetworkOfferIdInt, Title, TargetURL, DateAdded, 
                        Status, ImageURL, AdvertiserUsername, LoginBoxRequired, EmailBoxRequired, LastCredited, Description, 
                        CategoryId, BaseValue, CreditsBought, NetworkName, NetworkRate, IsSyncWithNetwork, IsDaily, MaxDailyCredits, CreditOfferAs, GeolocatedAgeMin, GeolocatedAgeMax, 
                        GeolocatedGender, GeolocatedCC, GeolocatedCities, GeolocationProfile) VALUES");
            return sb;
        }

        public static void HandleAddition(string name, string description, string rate, string banner, string url,
            string incent, string countriesCC, AffiliateNetwork Network, string id, List<CPAOfferOnHold> OnHoldList, ref bool AreSomeRecordsInQuery, ref StringBuilder Query, 
            string geolocatedCities = null, int ageMin = 0, int ageMax = 0, Gender gender = Gender.Null)
        {
            CPAOfferOnHold ThisOffer = SyncedNetwork.HasThisOffer(OnHoldList, id, name);

            if (ThisOffer != null)
            {
                //We have it, just change the status
                ThisOffer.Status = AdvertStatus.WaitingForAcceptance;
            }
            else
            {
                AreSomeRecordsInQuery = true;

                //Preparte the variables to fit database                

                Money UserRate = Money.Zero;
                try
                {
                    if (rate.Contains("."))
                        UserRate = Money.Parse(rate);
                    else
                        UserRate = Money.Parse(rate + ".0");
                    UserRate = Money.MultiplyPercent(UserRate, Network.DefaultPercentForMembers);
                }
                catch (Exception ex) { }

                if (name.Length > 50) name = name.Substring(0, 49);
                if (!string.IsNullOrEmpty(banner) && banner.Length > 150) banner = banner.Substring(0, 149);              

                    //We need to add it
                    CPAOfferOnHold NewOffer = new CPAOfferOnHold();
                    NewOffer.IsFromAutomaticNetwork = true;
                    NewOffer.NetworkOfferId = id;
                    NewOffer.Title = InputChecker.HtmlEncode(name);
                    NewOffer.ImageURL = banner;
                    NewOffer.TargetURL = url;
                    NewOffer.DateAdded = DateTime.Now;
                    NewOffer.Status = AdvertStatus.Active;
                    NewOffer.AdvertiserUsername = Network.Name;
                    NewOffer.LoginBoxRequired = false;
                    NewOffer.EmailBoxRequired = false;
                    NewOffer.LastCredited = OffersManager.DateTimeZero;
                    NewOffer.Description = InputChecker.HtmlEncode(description);
                    NewOffer.Category = new CPACategory(CPACategory.DEFAULT_CATEGORY_ID);
                    NewOffer.BaseValue = UserRate;
                    NewOffer.CreditsBought = 1000000; //Infinity
                    NewOffer.NetworkName = Network.Name;
                    NewOffer.NetworkRate = rate;
                    NewOffer.IsSyncWithNetwork = false; // by default
                    NewOffer.IsDaily = false;
                    NewOffer.MaxDailyCredits = 1;
                    NewOffer.AddGeolocation(new GeolocationUnit(countriesCC, geolocatedCities, ageMin, ageMax, gender));

                
                if (incent.Trim().ToUpper() == "NO")
                    NewOffer.CreditOfferAs = CreditOfferAs.NonCash;
                else
                    NewOffer.CreditOfferAs = CreditOfferAs.NetworkDefault;

                SyncedNetwork.ConstructInsertQuery(Query, NewOffer);
            }
        }

        #endregion SyncHelpers
        public static bool DoesNetworkExist(AffiliateNetworkSoftwareType Type)
        {
            string query = string.Format(@"SELECT * FROM NewAffiliateNetworks WHERE Status != {0}
                                           AND AffiliateNetworkSoftwareTypeInt = {1}", (int)NetworkStatus.Deleted, (int)Type);

            var network = TableHelper.GetListFromRawQuery<AffiliateNetwork>(query);
            if (network.Count > 0)
                return true;
            return false;
        }
    }

    #region SyncHelpers2

    public class CookieAwareWebClient : WebClient
    {
        private CookieContainer cookie = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = cookie;
            }
            return request;
        }
    }

    #endregion SyncHelpers2

}