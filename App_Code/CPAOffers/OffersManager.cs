using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Advertising;
using Titan;
using MarchewkaOne.Titan.CPAOffers;
using ExtensionMethods;

namespace Prem.PTC.Offers
{
    public class OffersManager
    {
        #region Basics

        public static DateTime DateTimeZero
        {
            get
            {
                return DateTime.Now.Zero();
            }
        }

        private bool _isDaily = false;

        public bool isDailyOfers
        {
            get
            {
                return _isDaily;
            }
            set
            {
                if (_isDaily != value)
                {
                    _isDaily = value;
                    ZeroSubmissionValues();
                }
            }
        }

        private DeviceType _SelectedDeviceType = DeviceType.Desktop;

        public DeviceType SelectedDeviceType
        {
            get
            {
                return _SelectedDeviceType;
            }
            set
            {
                if (_SelectedDeviceType != value)
                {
                    _SelectedDeviceType = value;
                    ZeroSubmissionValues();
                }
            }
        }

        protected void ZeroSubmissionValues()
        {
            sub_completed = null;
            sub_ignored = null;
            sub_reported = null;
            sub_denied = null;
            sub_under = null;
            sub_pending = null;
        }

        private Member user;

        public OffersManager(Member User)
        {
            _isDaily = false;
            user = User;
        }

        private List<CPAOffer> allactiveoffers;

        private List<OfferRegisterEntry> allmemberoffers;
        private List<OfferRegisterEntry> sub_completed;
        private List<OfferRegisterEntry> sub_ignored;
        private List<OfferRegisterEntry> sub_reported;
        private List<OfferRegisterEntry> sub_denied;
        private List<OfferRegisterEntry> sub_under;
        private List<OfferRegisterEntry> sub_pending;

        public List<CPAOffer> AllActiveOffers
        {
            get
            {
                if (allactiveoffers == null)
                {
                    var ActiveNetworks = AffiliateNetwork.AllActiveNetworkNames;
                    var AllActiveOffersRegardlessNetworkState = TableHelper.SelectRows<CPAOffer>(TableHelper.MakeDictionary("Status", (int)AdvertStatus.Active));

                    allactiveoffers = new List<CPAOffer>();

                    //We don't want to display offers with Network Status != Active
                    foreach (CPAOffer offer in AllActiveOffersRegardlessNetworkState)
                    {
                        if (offer.GlobalDailySubmitsRestricted && OfferRegisterEntry.GetAllTodaysSubmissionsCountForOffer(offer.Id) >= offer.MaxGlobalDailySubmits)
                            continue;

                        if (String.IsNullOrEmpty(offer.NetworkName))
                            allactiveoffers.Add(offer);

                        if (!String.IsNullOrEmpty(offer.NetworkName) && ActiveNetworks.Contains(offer.NetworkName))
                            allactiveoffers.Add(offer);
                    }
                }

                return allactiveoffers;
            }
        }

        public List<OfferRegisterEntry> AllMemberSubmissions
        {
            get
            {
                if (allmemberoffers == null)
                {
                    allmemberoffers = TableHelper.SelectRows<OfferRegisterEntry>(TableHelper.MakeDictionary("Username", user.Name));
                }

                return allmemberoffers;
            }
        }

        public List<OfferRegisterEntry> SubmissionsCompleted
        {
            get
            {
                if (sub_completed == null)
                    UpdateSubmissions();
                return sub_completed;
            }
        }

        public List<OfferRegisterEntry> SubmissionsIgnored
        {
            get
            {
                if (sub_ignored == null)
                    UpdateSubmissions();
                return sub_ignored;
            }
        }

        public List<OfferRegisterEntry> SubmissionsReported
        {
            get
            {
                if (sub_reported == null)
                    UpdateSubmissions();
                return sub_reported;
            }
        }

        public List<OfferRegisterEntry> SubmissionsDenied
        {
            get
            {
                if (sub_denied == null)
                    UpdateSubmissions();
                return sub_denied;
            }
        }

        public List<OfferRegisterEntry> SubmissionsUnderReview
        {
            get
            {
                if (sub_under == null)
                    UpdateSubmissions();
                return sub_under;
            }
        }

        public List<OfferRegisterEntry> SubmissionsPending
        {
            get
            {
                if (sub_pending == null)
                    UpdateSubmissions();
                return sub_pending;
            }
        }

        private void UpdateSubmissions()
        {
            sub_completed = new List<OfferRegisterEntry>();
            sub_ignored = new List<OfferRegisterEntry>();
            sub_reported = new List<OfferRegisterEntry>();
            sub_denied = new List<OfferRegisterEntry>();
            sub_under = new List<OfferRegisterEntry>();
            sub_pending = new List<OfferRegisterEntry>();

            foreach (var elem in AllMemberSubmissions)
            {
                if (!AppSettings.CPAGPT.DailyNotDailyButtonsEnabled ||
                    (AppSettings.CPAGPT.DailyNotDailyButtonsEnabled && (elem.IsDaily && isDailyOfers || !elem.IsDaily && !isDailyOfers)))
                {
                    if (!AppSettings.CPAGPT.DeviceTypeDistinctionEnabled ||
                        (AppSettings.CPAGPT.DeviceTypeDistinctionEnabled && elem.DeviceType == SelectedDeviceType))
                    {
                        if (elem.Status == OfferStatus.Completed)
                            sub_completed.Add(elem);
                        else if (elem.Status == OfferStatus.Ignored)
                            sub_ignored.Add(elem);
                        else if (elem.Status == OfferStatus.Reported)
                            sub_reported.Add(elem);
                        else if (elem.Status == OfferStatus.Denied)
                            sub_denied.Add(elem);
                        else if (elem.Status == OfferStatus.UnderReview)
                            sub_under.Add(elem);
                        else if (elem.Status == OfferStatus.Pending)
                            sub_pending.Add(elem);
                    }
                }
            }
        }

        #endregion Basics

        public List<CPAOffer> AllActiveOffersForMember
        {
            get
            {
                List<CPAOffer> result = new List<CPAOffer>();

                foreach (var elem in AllActiveOffers)
                {
                    //Check geolocation
                    if (elem.IsGeolocationMeet(user) && !elem.IsBlockedForMember(user.Id) && user.HasThisMembershipOrHigher(elem.RequiredMembership))
                    {
                        //Check submission-realated stuff
                        try
                        {
                            OfferRegisterEntry.CheckDuplicateAndStatus(user.Name, elem);

                            if (!TitanFeatures.IsBobbyDonev)
                                result.Add(elem);

                            if (TitanFeatures.IsBobbyDonev && elem.OfferLevel != "0")
                                result.Add(elem);
                        }
                        catch (Exception) { }
                    }
                }

                return result;
            }
        }

        public List<CPAOffer> ActiveNotDailyOffersForMember
        {
            get
            {
                return AllActiveOffersForMember.FindAll(x => x.IsDaily == false);
            }
        }

        public List<CPAOffer> ActiveDailyOffersForMember
        {
            get
            {
                return AllActiveOffersForMember.FindAll(x => x.IsDaily == true);
            }
        }
    }
}