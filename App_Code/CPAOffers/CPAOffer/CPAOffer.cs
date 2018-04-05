using System;
using System.Data;
using Prem.PTC.Members;
using System.Collections.Generic;
using System.Web;
using Prem.PTC.Advertising;
using Titan;
using System.Web.UI;
using MarchewkaOne.Titan.CPAOffers;

namespace Prem.PTC.Offers
{
    [Serializable]
    public class CPAOffer : CPAOfferBase
    {
        #region Constructors

        public static new string TableName { get { return "CPAOffers"; } }
        protected override string dbTable { get { return TableName; } }

        private const string UsernameToReplace1 = "[USERNAME]";
        private const string UsernameToReplace2 = "%USERNAME%";

        private const string EmailToReplace1 = "[EMAIL]";
        private const string EmailToReplace2 = "%EMAIL%";

        private const string AgeToReplace1 = "[AGE]";
        private const string AgeToReplace2 = "%AGE%";

        private const string GenderToReplace1 = "[GENDER]";
        private const string GenderToReplace2 = "%GENDER%";

        public CPAOffer() : base() { }
        public CPAOffer(int id): base(id) { }
        public CPAOffer(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Core

        #region Banner

        private void initBanner(string imagePath)
        {
            try { _bannerImage = Banner.FromFile(imagePath); }
            catch (System.IO.FileNotFoundException ex)
            {
                ErrorLogger.Log(ex);
                _bannerImage = Banner.Empty;
            }
        }

        private void bannerImage_PreSave(bool IsFromAdminPanel, string baseurl = "")
        {
            if (_bannerImage != null)
            {
                if (!_bannerImage.IsSaved)
                {
                    if (IsFromAdminPanel)
                        _bannerImage.SaveOnClient(AppSettings.FolderPaths.BannerAdvertImages);
                    else
                        _bannerImage.Save(AppSettings.FolderPaths.BannerAdvertImages);
                }

                ImageURL = _bannerImage.Path;
            }

        }

        #endregion Banner

        #region Help

        public static int CheckCollectedLevelsAmount(string username)
        {
            string query = String.Format(@"SELECT COUNT(DISTINCT C.OfferLevel) 
                                            FROM OfferRegisterEntries O 
                                            JOIN  CPAOffers C ON C.Id = O.OfferId 
                                            WHERE O.Username = '{0}' 
                                            AND C.OfferLevel NOT LIKE '0'
                                            AND O.OfferStatus = 2", username);

            return (int)TableHelper.SelectScalar(query);
        }

        public string GetTargetURL(bool ShouldAddNetworkInformation = true)
        {
            if (ShouldAddNetworkInformation)
            {
                try
                {
                    var user = Member.Current;
                    var username = user.Name;

                    if (IsFromAutomaticNetwork)
                    {
                        AffiliateNetwork Network = TableHelper.SelectRows<AffiliateNetwork>(TableHelper.MakeDictionary("DisplayName", this.AdvertiserUsername))[0];

                        if (Network.AffiliateNetworkSoftwareType == AffiliateNetworkSoftwareType.Performa)
                            return TargetURL + username + "/" + NetworkName + "/";
                        else if (Network.AffiliateNetworkSoftwareType == AffiliateNetworkSoftwareType.CPALead)
                            return TargetURL + username;
                        else if (Network.AffiliateNetworkSoftwareType == AffiliateNetworkSoftwareType.AdGateMedia)
                            return TargetURL + username;
                    }

                    return TargetURL.Replace(UsernameToReplace1, username).Replace(UsernameToReplace2, username)
                        .Replace(EmailToReplace1, user.Email).Replace(EmailToReplace2, user.Email)
                        .Replace(AgeToReplace1, user.Age.ToString()).Replace(AgeToReplace2, user.Age.ToString())
                        .Replace(GenderToReplace1, user.Gender.GetLetter()).Replace(GenderToReplace2, user.Gender.GetLetter());
                }
                catch (Exception ex)
                { }
            }
            return TargetURL;
        }

        /// <summary>
        /// Checks offers status. It also saves the changes after it's done
        /// </summary>
        public void PerformStatusControlCheck()
        {
            if (!this.IsFromAutomaticNetwork)
            {
                int currentVal = 0;
                int completedVal = 0;
                int MaxVal = CreditsBought;

                foreach (var elem in Entries)
                {
                    if (elem.Status == OfferStatus.Completed || elem.Status == OfferStatus.Pending || elem.Status == OfferStatus.UnderReview)
                        currentVal++;
                    if (elem.Status == OfferStatus.Completed)
                        completedVal++;
                }

                bool savestatus = false;

                if (currentVal < MaxVal && Status == AdvertStatus.Stopped)
                {
                    Status = AdvertStatus.Active;
                    savestatus = true;
                }
                else if (currentVal >= MaxVal && Status == AdvertStatus.Active)
                {
                    Status = AdvertStatus.Stopped;
                    savestatus = true;
                }

                if (completedVal >= MaxVal && Status != AdvertStatus.Finished)
                {
                    Status = AdvertStatus.Finished;
                    savestatus = true;
                }

                if (savestatus)
                    this.Save();
            }
        }

        public void Save(bool forceSave = false, bool IsFromAdminPanel = false)
        {
            bannerImage_PreSave(IsFromAdminPanel);
            base.Save(forceSave);
        }

        public static void Delete(int id)
        {
            TableHelper.DeleteRows<CPAOffer>("id", id);
        }

        public List<OfferRegisterEntry> Entries
        {
            get
            {
                if (_entries == null)
                    _entries = TableHelper.SelectRows<OfferRegisterEntry>(TableHelper.MakeDictionary("OfferId", Id));

                return _entries;
            }
        }

        /// <summary>
        /// Result: 0 1 2 3 4 5
        /// </summary>
        public int Rating
        {
            get
            {
                if (_rating == -1)
                {
                    _completed = 0;
                    int CompletedUnderDenied = 0;

                    foreach (var entry in Entries)
                    {
                        if (entry.Status == OfferStatus.Completed)
                            _completed++;
                        else if (entry.Status == OfferStatus.Denied ||
                                 entry.Status == OfferStatus.Pending ||
                                 entry.Status == OfferStatus.UnderReview)
                            CompletedUnderDenied++;
                    }

                    if (CompletedUnderDenied + _completed == 0)
                        _rating = 0;
                    else
                    {
                        double result = ((double)5 * _completed) / ((double)CompletedUnderDenied + (double)_completed);
                        _rating = Convert.ToInt32(result);
                    }

                }
                return _rating;
            }
        }

        public new DateTime LastCredited
        {
            get
            {
                _DateLastCredited = OffersManager.DateTimeZero;
                foreach (var entry in Entries)
                {
                    if (entry.Status == OfferStatus.Completed && entry.DateCompleted > _DateLastCredited)
                    {
                        _DateLastCredited = entry.DateCompleted;
                    }                   
                }
                return _DateLastCredited;
            }
            set { }
        }

        public int CompletedTimes
        {
            get
            {
                if (_completed == -1)
                {
                    _completed = 0;
                    foreach (var entry in Entries)
                    {
                        if (entry.Status == OfferStatus.Completed)
                            _completed++;
                    }
                }
                return _completed;
            }
        }

        public string AVGCreditingTime
        {
            get
            {
                if (avgcredit == "-1")
                {
                    TimeSpan average = TimeSpan.MinValue;
                    int howmanycounts = 0;

                    foreach (var entry in Entries)
                    {
                        if (entry.DateCompleted != OffersManager.DateTimeZero)
                        {
                            //Offer has been completed
                            TimeSpan time = entry.DateCompleted.Subtract(entry.DateAdded);
                            if (average == TimeSpan.MinValue)
                                average = time;
                            else
                                average += time;

                            howmanycounts++;
                        }
                    }

                    if (howmanycounts == 0)
                        avgcredit = "N/A";
                    else
                    {
                        double result = average.TotalDays / (double)howmanycounts;
                        avgcredit = result.ToString("F1") + " day(s)";
                    }
                }
                return avgcredit;
            }
        }

        public string GetLastCredited()
        {
            string lastcredited = (LastCredited == OffersManager.DateTimeZero) ? "N/A" : LastCredited.ToShortDateString();
            return lastcredited;
        }

        #endregion Help

        #region Panels

        public UserControl ToPanel(int UserCPAOffersPercent)
        {
            UserControl panel = CreateCPAOfferBox(GetTargetURL(), ImageURL, Title, Description, DateAdded,
                CPAType.GetText(Category), GetAmount(UserCPAOffersPercent), Rating, CompletedTimes, Id, AVGCreditingTime, GetLastCredited(), LoginBoxRequired, EmailBoxRequired, GetFinalCreditAs());

            return panel;
        }

        public UserControl ToCompletedPanel(DateTime Completed, int UserCPAOffersPercent, int OfferRegId)
        {
            UserControl panel = CreateCPAOfferBox(GetTargetURL(), ImageURL, Title, Description, DateAdded,
                CPAType.GetText(Category), GetAmount(UserCPAOffersPercent), Rating, CompletedTimes, OfferRegId, AVGCreditingTime, GetLastCredited(), LoginBoxRequired, EmailBoxRequired, GetFinalCreditAs(),
                Resources.L1.COMPLETED + ": " + Completed.ToString());

            return panel;
        }

        public UserControl ToDeniedPanel(int UserCPAOffersPercent, int OfferRegId)
        {

            UserControl panel = CreateCPAOfferBox(GetTargetURL(), ImageURL, Title, Description, DateAdded,
                CPAType.GetText(Category), GetAmount(UserCPAOffersPercent), Rating, CompletedTimes, OfferRegId, AVGCreditingTime, GetLastCredited(), LoginBoxRequired, EmailBoxRequired, GetFinalCreditAs(),
                "");

            return panel;
        }

        public UserControl ToIgnoredPanel(int UserCPAOffersPercent, int OfferRegId, string sender = "")
        {

            UserControl panel = CreateCPAOfferBox(GetTargetURL(), ImageURL, Title, Description, DateAdded,
                CPAType.GetText(Category), GetAmount(UserCPAOffersPercent), Rating, CompletedTimes, OfferRegId, AVGCreditingTime, GetLastCredited(), LoginBoxRequired, EmailBoxRequired, GetFinalCreditAs(),
               "", true, sender);

            return panel;
        }

        public UserControl ToPendingPanel(int UserCPAOffersPercent, string loginID, string emailID, int OfferRegId, string sender = "")
        {

            string text = "";

            if (!string.IsNullOrEmpty(loginID))
                text += "<b>Login ID</b>: " + loginID + "<br/>";

            if (!string.IsNullOrEmpty(emailID))
                text += "<b>Email ID</b>: " + emailID + "<br/>";

            UserControl panel = CreateCPAOfferBox(GetTargetURL(), ImageURL, Title, Description, DateAdded,
                CPAType.GetText(Category), GetAmount(UserCPAOffersPercent), Rating, CompletedTimes, OfferRegId, AVGCreditingTime, GetLastCredited(), LoginBoxRequired, EmailBoxRequired, GetFinalCreditAs(),
                text, true, sender);
            return panel;
        }

        public UserControl ToReportedPanel(int UserCPAOffersPercent, string reportedText, int OfferRegId, string sender = "")
        {
            UserControl panel = CreateCPAOfferBox(GetTargetURL(), ImageURL, Title, Description, DateAdded,
                CPAType.GetText(Category), GetAmount(UserCPAOffersPercent), Rating, CompletedTimes, OfferRegId, AVGCreditingTime, GetLastCredited(), LoginBoxRequired, EmailBoxRequired, GetFinalCreditAs(),
                 reportedText, true, sender);
            return panel;
        }

        public UserControl ToUnderReviewPanel(int UserCPAOffersPercent, int OfferRegId)
        {

            UserControl panel = CreateCPAOfferBox(GetTargetURL(), ImageURL, Title, Description, DateAdded,
                CPAType.GetText(Category), GetAmount(UserCPAOffersPercent), Rating, CompletedTimes, OfferRegId, AVGCreditingTime, GetLastCredited(), LoginBoxRequired, EmailBoxRequired, GetFinalCreditAs(),
                 "");
            return panel;
        }

        #endregion Panels

        #region Actions
        public void TranferOfferFromActiveToHold(int offerId)
        {
            String queryInsert = String.Empty;
            String queryDelete = String.Empty;
            String OffersTable = CPAOffer.TableName;           
            String OffersHoldTable = CPAOfferOnHold.TableName; 

            CPAOfferOnHold newOfferToHold = new CPAOfferOnHold
            {
                DateAdded = this.DateAdded,
                BaseValue = this.BaseValue,
                IntCategory = this.IntCategory,
                Title = this.Title,
                Description = this.Description,
                LastCredited = this.LastCredited,
                IsDaily = this.IsDaily,
                Status = this.Status,
                ImageURL = this.ImageURL,
                LoginBoxRequired = this.LoginBoxRequired,
                EmailBoxRequired = this.EmailBoxRequired,
                AdvertiserUsername = this.AdvertiserUsername,
                CreditsBought = this.CreditsBought,
                TargetURL = this.TargetURL,
                IsFromAutomaticNetwork = this.IsFromAutomaticNetwork,
                NetworkName = this.NetworkName,
                NetworkOfferId = this.NetworkOfferId,
                NetworkRate = this.NetworkRate,
                IsSyncWithNetwork = this.IsSyncWithNetwork,
                IsIgnored = this.IsIgnored,
                CreditOfferAs = this.CreditOfferAs,
                MaxDailyCredits = this.MaxDailyCredits,
                GeolocatedCC = this.GeolocatedCC,
                GeolocatedCities = this.GeolocatedCities,
                GeolocatedAgeMin = this.GeolocatedAgeMin,
                GeolocatedAgeMax = this.GeolocatedAgeMax,
                GeolocationProfile = this.GeolocationProfile,
                GeolocatedGender = this.GeolocatedGender, 
                RequiredMembership = this.RequiredMembership,
                OfferLevel = this.OfferLevel
            };

            newOfferToHold.Save();
            this.Delete();
        }

        public static bool ExistsAffiliateNetworkOffer(string networkOfferId, string networkName)
        {
            int Count = (int)TableHelper.SelectScalar(String.Format(@"SELECT COUNT(*) FROM {0} WHERE NetworkOfferIdInt='{1}' AND NetworkName='{2}'", TableName, networkOfferId, networkName));
            return Count >= 1;
        }
        #endregion

        public string GetDescription()
        {
            return Mailer.ReplaceNewLines(this.Description);
        }

        public CreditAs GetFinalCreditAs()
        {
            if (CreditOfferAs == CreditOfferAs.NonCash)
                return CreditAs.Points;
            else if (CreditOfferAs == CreditOfferAs.NetworkDefault)
            {
                try
                {
                    var target = TableHelper.SelectRows<AffiliateNetwork>(TableHelper.MakeDictionary("DisplayName", NetworkName))[0];
                    return target.CreditAs;
                }
                catch (Exception ex) {  }
            }

            return CreditAs.MainBalance;
        }

        public bool IsBlockedForMember(int userId)
        {
            var blockedList = TableHelper.SelectRows<CPACategoriesBlocked>(TableHelper.MakeDictionary("UserId", userId));

            bool IsBlocked = false;

            foreach (var blockedElem in blockedList)
                if (blockedElem.CPACategoryId == this.IntCategory)
                    IsBlocked = true;

            return IsBlocked;
        }

        #region HTML

        public UserControl CreateCPAOfferBox(string TargetURL, string OfferImageURL, string Title, string Description, DateTime DateAdded,
        string Category, Money Amount, int Rating, int CompletedTimes, int OfferId, string AVGCreditingTime,
        string lastcredited, bool RequireLoginBox, bool RequireEmailBox, CreditAs creditAs, string RightText = null, bool ShowReturnButton = false,
        string sender = "")
        {
            UserControl objControl = (UserControl)(HttpContext.Current.Handler as Page).LoadControl("~/Controls/Advertisements/CPAOffer.ascx");
            var parsedControl = objControl as ICPAOfferObjectControl;

            parsedControl.Object = this;
            parsedControl.Text = RightText;
            parsedControl.ShowReturnButton = ShowReturnButton;
            parsedControl.Sender = sender;

            parsedControl.DataBind();

            return (UserControl)parsedControl;
        }

        #endregion HTML

    }
}