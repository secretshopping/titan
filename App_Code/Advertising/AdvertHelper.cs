using System;
using Prem.PTC.Utils;

namespace Prem.PTC.Advertising
{

    public static class AdvertHelper
    {
        public static int GetProgressInPercent(Advert advert)
        {
            return getProgressInPercent(advert.Ends, advert.Status, advert.Clicks, advert.StartDate, advert.ActiveTime);
        }

        /// <summary>
        /// Utility method for sql queries
        /// </summary>
        public static int GetProgressInPercent(int endMode, int endValue, int advertStatus, int clicks, DateTime? startDate, DateTime? statusLastChangedDate, int totalSecsActive)
        {
            End end = End.FromModeValue((End.Mode)endMode, endValue);
            AdvertStatus status = (AdvertStatus)advertStatus;

            return getProgressInPercent(end, status, clicks, startDate, statusLastChangedDate,totalSecsActive);
        }

        private static int getProgressInPercent(End end, AdvertStatus status, int clicks, DateTime? startDate, DateTime? statusLastChangedDate, int totalSecsActive)
        {
            var totalActive = TimeSpan.FromSeconds(totalSecsActive) +
                    (status == AdvertStatus.Active ? DateTime.Now - statusLastChangedDate.Value : TimeSpan.Zero);

            return getProgressInPercent(end, status, clicks, startDate, totalActive);
        }

        private static int getProgressInPercent(End end, AdvertStatus status, int clicks, DateTime? startDate, TimeSpan totalActive)
        {
            if (status == AdvertStatus.Null ||
                 status == AdvertStatus.WaitingForAcceptance) return 0;

            if (status == AdvertStatus.Finished ||
                status == AdvertStatus.Rejected || end.Value == 0) return 100;

            int returnValue = 0;

            switch (end.EndMode)
            {
                case End.Mode.Null:
                case End.Mode.Endless:
                    return 0;

                case End.Mode.Clicks:
                    returnValue = (int)(((double)clicks / end.Value) * 100);
                    break;

                case End.Mode.Days:
                    TimeSpan total = TimeSpan.FromDays(end.Value);
                    returnValue = (int)(((double)totalActive.Ticks / total.Ticks) * 100);
                    break;

                default:
                    throw new NotImplementedException("Not implemented: " + end);
            }

            if (returnValue > 100) return 100;
            if (returnValue < 0) return 0;

            return returnValue;

        }
    }
}