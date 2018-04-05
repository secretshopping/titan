using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Prem.PTC.Advertising
{

    /// <summary>
    /// Available Advert Statues
    /// </summary>
    public enum AdvertStatus
    {
        Null = 0,
        WaitingForAcceptance = 1,
        Active = 2,
        Paused = 3,
        Finished = 4,
        Rejected = 5,
        Stopped = 6,
        Deleted = 7
    };

    public enum AdPackTypeStatus
    {
        Active = 1,
        Paused = 2
    }

    public enum SurfAdsPackStatus
    {
        Active = 1,
        Paused = 2
    }

    public enum MarketplaceIPNStatus
    {
        Pending = 1,
        Confirmed = 2
    }

    public enum CreditLineRequestStatus
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3
    }

    public enum TestimonialStatus
    {
        Active = 1,
        Paused = 2,
        Rejected = 3
    }

    public static class AdvertStatusExtensions
    {
        public static ISet<AdvertStatus> GetNextStatuses(this AdvertStatus currentStatus)
        {

            HashSet<AdvertStatus> modes = new HashSet<AdvertStatus>();

            switch (currentStatus)
            {
                case AdvertStatus.Null:
                    modes.Add(AdvertStatus.WaitingForAcceptance);
                    break;

                case AdvertStatus.WaitingForAcceptance:
                    modes.Add(AdvertStatus.Active);
                    modes.Add(AdvertStatus.Paused);
                    modes.Add(AdvertStatus.Rejected);
                    break;

                case AdvertStatus.Active:
                    modes.Add(AdvertStatus.Paused);
                    modes.Add(AdvertStatus.Finished);
                    break;

                case AdvertStatus.Paused:
                    modes.Add(AdvertStatus.Active);
                    modes.Add(AdvertStatus.Finished);
                    break;

                case AdvertStatus.Finished:
                    modes.Add(AdvertStatus.Active);
                    modes.Add(AdvertStatus.Paused);
                    break;
                case AdvertStatus.Rejected:
                    break;

                case AdvertStatus.Stopped:
                    break;

                default: throw new NotImplementedException();
            }

            return modes;
        }

        public static bool IsBefore(this AdvertStatus thisStatus, AdvertStatus otherStatus)
        {
            return true;
        }

        public static bool IsAfter(this AdvertStatus thisStatus, AdvertStatus otherStatus)
        {
            return otherStatus.IsBefore(thisStatus);
        }


        public static ListItem[] GetListControlSource(AdvertStatus status)
        {
            var nextStatuses = status.GetNextStatuses();

            var query = from nextStatus in nextStatuses
                        orderby (int)nextStatus ascending
                        select GetSingleItemListControlSource(nextStatus);

            return query.ToArray();
        }

        public static ListItem GetSingleItemListControlSource(AdvertStatus status)
        {
            return new ListItem(status.ToString(), Convert.ToString((int)status));
        }

        public static bool CanBeRemoved(this AdvertStatus status)
        {
            if (status == AdvertStatus.Finished || status == AdvertStatus.Rejected)
                return true;
            return false;
        }

        public static AdvertStatus GetStartingStatus()
        {
            if (AppSettings.Site.AutoApprovalCampaignsEnabled && AppSettings.Site.AutoActiveCampaignsEnabled)
                return AdvertStatus.Active;

            if (AppSettings.Site.AutoApprovalCampaignsEnabled)
                return AdvertStatus.Paused;

            return AdvertStatus.WaitingForAcceptance;
        }

        public static AdvertStatus GetStatusAfterApproval()
        {
            if (AppSettings.Site.AutoActiveCampaignsEnabled)
                return AdvertStatus.Active;

            return AdvertStatus.Paused;
        }
    }
}