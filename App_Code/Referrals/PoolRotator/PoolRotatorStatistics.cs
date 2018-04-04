using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PoolRotatorStatistics
{
    #region Global

    public static int TotalHitsDelivered
    {
        get
        {
            var result = TableHelper.SelectScalar("SELECT SUM(ClicksDelivered) FROM PoolRotatorLinkUsers");
            return (result is DBNull) ? 0 : (int)result;
        }
    }

    public static int TotalReferralsDelivered
    {
        get
        {
            var result = TableHelper.SelectScalar("SELECT SUM(ReferralsDelivered) FROM PoolRotatorLinkUsers");
            return (result is DBNull) ? 0 : (int)result;
        }
    }

    public static int MembersInThePool
    {
        get
        {
            return (int)TableHelper.SelectScalar("SELECT COUNT(*) FROM PoolRotatorLinkUsers WHERE IsActive = 1");
        }
    }

    #endregion

}