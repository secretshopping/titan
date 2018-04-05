using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using System.Data;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using Titan.Shares;
using Prem.PTC.Advertising;

public partial class Leaderboard : System.Web.UI.Page
{
    public static int RecordsShown { get { return 10; } }
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessManager.RedirectIfDisabled(AppSettings.TitanFeatures.StatisticsLeaderboardEnabled);
    }

    protected void ReferralGridView_DataSource_Init(object sender, EventArgs e)
    {
        ReferralGridView_DataSource.EnableCaching = !AppSettings.Site.DeveloperModeEnabled;
        ReferralGridView_DataSource.CacheDuration = 600;
        ReferralGridView_DataSource.CacheExpirationPolicy = DataSourceCacheExpiry.Absolute;

        ReferralGridView_DataSource.SelectCommand = string.Format(
            @"SELECT TOP {0}
                Username, 
                (SELECT COUNT(*) FROM Users WHERE ReferrerId = u.UserId) RefCount 
            FROM Users u 
            WHERE u.UserId != {1} AND u.UserId != 1005
            GROUP BY u.Username, u.UserId
            HAVING (SELECT COUNT(*) FROM Users WHERE ReferrerId = u.UserId)  > 0 
            ORDER BY RefCount DESC", RecordsShown, AppSettings.RevShare.AdminUserId);

    }

    protected void ReferralGridView_DataBound(object sender, EventArgs e)
    {
        ReferralGridView.Columns[0].HeaderText = L1.USERNAME;
        ReferralGridView.Columns[1].HeaderText = L1.REFERRALS;
    }

    protected void CommissionsGridView_DataSource_Init(object sender, EventArgs e)
    {
        CommissionsGridView_DataSource.SelectCommand = string.Format(
           @"SELECT TOP {0} Username, TotalDirectReferralsEarned 
            FROM Users u 
            WHERE u.UserId != {1} AND u.UserId != 1005 AND TotalDirectReferralsEarned > 0
            ORDER BY TotalDirectReferralsEarned DESC", RecordsShown, AppSettings.RevShare.AdminUserId);
    }

    protected void CommissionsGridView_DataBound(object sender, EventArgs e)
    {
        CommissionsGridView.Columns[0].HeaderText = L1.USERNAME;
        CommissionsGridView.Columns[1].HeaderText = L1.AMOUNT;
    }

    protected void TotalEarnedGridView_DataSource_Init(object sender, EventArgs e)
    {
        TotalEarnedGridView_DataSource.SelectCommand = string.Format(
           @"SELECT TOP {0} Username, TotalEarned 
            FROM Users 
            WHERE TotalEarned > 0 AND UserId != {1} AND UserId != 1005
            ORDER BY TotalEarned DESC", RecordsShown, AppSettings.RevShare.AdminUserId);
    }

    protected void TotalEarnedGridView_DataBound(object sender, EventArgs e)
    {
        TotalEarnedGridView.Columns[0].HeaderText = L1.USERNAME;
        TotalEarnedGridView.Columns[1].HeaderText = L1.AMOUNT;
    }

    protected void CountryGridView_DataSource_Init(object sender, EventArgs e)
    {
        CountryGridView_DataSource.EnableCaching = !AppSettings.Site.DeveloperModeEnabled;
        CountryGridView_DataSource.CacheDuration = 600;
        CountryGridView_DataSource.CacheExpirationPolicy = DataSourceCacheExpiry.Absolute;

        CountryGridView_DataSource.SelectCommand = string.Format(
            @"SELECT TOP {0} Country, COUNT(Country) CountryCount 
            FROM Users 
            GROUP BY Country 
            ORDER BY CountryCount DESC", RecordsShown);
    }

    protected void CountryGridView_DataBound(object sender, EventArgs e)
    {
        CountryGridView.Columns[0].HeaderText = L1.COUNTRY;
        CountryGridView.Columns[1].HeaderText = U6000.USERS;
    }

}
