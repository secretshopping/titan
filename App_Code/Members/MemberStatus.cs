using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Prem.PTC.Members
{
    public enum MemberStatus
    {
        ///<summary>Means nothing, for compatibility issues</summary>
        Null = 0,

        ///<summary>The account is fully activated (normal state). Can login</summary>
        Active = 1,

        ///<summary>The account has been registered but email address is not confirmed. Can't login.</summary>
        Inactive = 2,

        ///<summary>The account has been cancelled by user request. Can't login.</summary>
        Cancelled = 3,

        ///<summary>The account has expired (user was inactive for a specified time). Can't login.</summary>
        Expired = 4,

        ///<summary>The account has been banned due to TOS violation. Can't login.</summary>
        BannedOfTos = 5,

        ///<summary>The account has been banned due to cheating. Can't login.</summary>
        BannedOfCheating = 6,

        ///<summary>The account is locked via the administrator. Can't login.</summary>
        Locked = 7,

        ///<summary>The account has been banned because it is blacklisted(username/IP/Country). Can't login.</summary>
        BannedBlacklisted = 8,

        ///<summary>The account has been locked and it is awaiting ProxStop SMS Verification</summary>
        AwaitingSMSPIN = 9,

        ///<summary>The member is on Vacation Mode</summary>
        VacationMode = 10
    }

    public static class MemberStatusHelper
    {
        public static MemberStatus[] PotentiallyActiveStatuses = { MemberStatus.Active, MemberStatus.AwaitingSMSPIN, MemberStatus.VacationMode };           

        public static string PotentiallyActiveStatusesSQLHelper
        {
            get
            {
                var statusList = string.Empty;
                foreach(var status in PotentiallyActiveStatuses)                
                    statusList += string.Format("{0},", (int)status);
                
                return string.Format("AccountStatusInt IN ({0})", statusList.TrimEnd(','));
            }
        }

        /// <summary>
        /// Returns list control source with all statuses' short descriptions as text 
        /// and int status ordinal as value
        /// </summary>
        public static ListItem[] ListItems
        {
            get
            {
                var query = from MemberStatus status in Enum.GetValues(typeof(MemberStatus))
                            where status != MemberStatus.Null
                            orderby (int)status
                            select new ListItem(status.ShortDescription(), (int)status + "");

                return query.ToArray();
            }
        }
    }

    public static class MemberStatusExtensions
    {
        /// <summary>
        /// Provides human readable, short description for each member status.
        /// </summary>
        public static string ShortDescription(this MemberStatus status)
        {
            if (status == MemberStatus.Null) return "Unknown";

            return Enum.GetName(typeof(MemberStatus), status);
        }
    }
}