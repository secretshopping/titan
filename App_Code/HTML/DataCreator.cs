using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Memberships;
using System.Data;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Achievements;
using Prem.PTC.Advertising;
using System.Text;

namespace Prem.PTC.HTML
{

    public class DataCreator
    {
        public static DataSet GetUpgradeDataSet()
        {
            //Get DataTable source
            DataTable dt, dt1;
            DataSet ds;
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                string query = string.Format(@"SELECT [MembershipId]
                   ,[Name]
                   ,[Status]
                   ,[DisplayOrder]
                   ,[AdvertClickEarnings]
                   ,[DirectReferralAdvertClickEarnings]
                   ,[RentedReferralAdvertClickEarnings]
                   ,[DirectReferralsLimit]
                   ,[RentedReferralsLimit]
                   ,[ReferralRentCost]
                   ,[RenewalDiscount]
                   ,[AdvertPointsEarnings]
                   ,[Color]
                   ,[RentedReferralRecycleCost]
                   ,[CanAutoPay]
                   ,[DailyAutoPayCost]
                   ,[MinRentingIntervalSecs]
                   ,[TrafficGridTrials]
                   ,[TrafficGridChances]
                   ,[TrafficGridShorterAd]
                   ,[OfferwallsProfitPercent]
                   ,[CashoutLimit]
                   ,[CashoutLimitIcreased]
                   ,[CPAProfitPercent]
                   ,[RefPercentEarningsOfferwalls1]
                   ,[MaxRefPackageCount]
                   ,[RefPercentEarningsCPA]
                   ,[HasInstantPayout]
                   ,[DirectReferralAdPackPurchaseEarnings]
                   ,[MaxDailyCashout]
                   ,[AdPackAdBalanceReturnPercentage]
                   ,[AdPackDailyRequiredClicks]
                   ,[ROIEnlargedByPercentage]
                   ,[SameUserCommissionToMainTransferFee]
                   ,[OtherUserMainToCommisionTransferFee]
                   ,[OtherUserMainToAdTransferFee]
                   ,[OtherUserMainToMainTransferFee]
                   ,[OtherUserPointsToPointsTransferFee]
                   ,[PTCCreditsPerView]
                   ,[PTCPurchaseCommissionPercent]
                   ,[PointsYourPTCAdBeingViewed]
                   ,[PointsPer1000viewsDeliveredToPoolRotator]
                   ,[AutosurfViewLimitMonth]
                   ,[MinAdsWatchedMonthlyToKeepYourLevel]
                   ,[MinPointsToHaveThisLevel]
                   ,[MaxActivePtcCampaignLimit]
                   ,[MaxGlobalCashout]
                   ,[MaxExtraAdPackSecondsForClicks]
                   ,[LevelChanceToWinAnyAward]
                   ,[LevelPointsPrizeChance]
                   ,[LevelPointsPrizeMin]
                   ,[LevelPointsPrizeMax]
                   ,[LevelAdCreditsChance]
                   ,[LevelAdCreditsMin]
                   ,[LevelAdCreditsMax]
                   ,[LevelDRLimitIncreasedChance]
                   ,[LevelDRLimitIncreasedMin]
                   ,[LevelDRLimitIncreasedMax]
                   ,[MaxUpgradedDirectRefs]
                   ,[DirectReferralBannerPurchaseEarnings]
                   ,[DirectReferralMembershipPurchaseEarnings]
                   ,[DirectReferralAdvertClickEarningsPoints]
                   ,[DirectReferralTrafficGridPurchaseEarnings]
                   ,[TrafficExchangeClickEarnings]
                   ,[DRTrafficExchangeClickEarnings]
                   ,[MaxDailyPtcClicks]
                   ,[MaxDailyPayouts]
                   ,[PublishersBannerClickProfitPercentage]
                   ,[PublishersCpaOfferProfitPercentage]
                   ,[PublishersInTextAdClickProfitPercentage]
                   ,[PublishersPtcOfferWallProfitPercentage]
                   ,[ExposureMiniClickEarnings]
                   ,[ExposureMiniDirectClickEarnings]
                   ,[ExposureMiniRentedClickEarnings]
                   ,[ExposureMicroClickEarnings]
                   ,[ExposureMicroDirectClickEarnings]
                   ,[ExposureMicroRentedClickEarnings]
                   ,[ExposureFixedClickEarnings]
                   ,[ExposureFixedDirectClickEarnings]
                   ,[ExposureFixedRentedClickEarnings]
                   ,[ExposureStandardClickEarnings]
                   ,[ExposureStandardDirectClickEarnings]
                   ,[ExposureStandardRentedClickEarnings]
                   ,[ExposureExtendedClickEarnings]
                   ,[ExposureExtendedDirectClickEarnings]
                   ,[ExposureExtendedRentedClickEarnings]
                   ,[MaxFacebookLikesPerDay]
                   ,[NewReferralReward]
                   ,[PointsPerNewReferral]
                   ,[MinReferralEarningsToCreditReward]
                   ,[MaxWithdrawalAllowedPerInvestmentPercent]                   
                   ,[MaxCommissionPayoutsPerDay]
                   ,[AdBalanceBonusForUpgrade]
                   ,[CashBalanceBonusForUpgrade]
                   ,[TrafficBalanceBonusForUpgrade]
                   ,[MiniVideoUploadPrice]
                   ,[MiniVideoWatchPrice]
                   ,[BlockPayoutDays]
                   FROM {0} WHERE {1} = {2} ORDER BY {3}", Membership.TableName, Membership.Columns.MembershipStatus, (int)MembershipStatus.Active, Membership.Columns.DisplayOrder);
                //dt = bridge.Instance.Select(Membership.TableName, TableHelper.MakeDictionary(Membership.Columns.MembershipStatus, (int)MembershipStatus.Active));
                              
                dt = bridge.Instance.ExecuteRawCommandToDataTable(query);

            }

            dt1 = FlipDataTable(dt);
            ds = new DataSet(Membership.TableName);
            ds.Tables.Add(dt1);

            return ds;
        }

        //public static DataSet GetAdPackTypeDataSet()
        //{
        //    //Get DataTable source
        //    DataTable dt, dt1;
        //    DataSet ds;
        //    using (var bridge = ParserPool.Acquire(Database.Client))
        //    {                
        //        string query = string.Format(@"SELECT Name, Color, Price, 
        //            MinNumberOfPreviousType, MaxInstancesOfAllAdpacks, MaxInstances, Clicks, DisplayTime, NormalBannerImpressions, ConstantBannerImpressions, 
        //            RequiredMembership, ValueOf1SecondInClicks, CustomGroupsEnabled, LoginAdsCredits, WithdrawLimitPercentage, TrafficExchangeSurfCredits
        //            FROM AdPackTypes WHERE Status = {0} ORDER BY Number; ", (int)AdPackTypeStatus.Active);

        //        dt = bridge.Instance.ExecuteRawCommandToDataTable(query);
        //    }

        //    dt1 = FlipDataTable(dt);
        //    ds = new DataSet("AdPAckTypes");
        //    ds.Tables.Add(dt1);

        //    return ds;
        //}

        public static DataSet GetTrafficGridDataSet()
        {
            //Get DataTable source
            DataTable dt, dt1;
            DataSet ds;

            var what = new System.Collections.Specialized.StringCollection();
            //what.Add("

            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                dt = bridge.Instance.Select(Membership.TableName, TableHelper.MakeDictionary(Membership.Columns.MembershipStatus, (int)MembershipStatus.Active));
            }

            dt1 = FlipDataTable(dt);
            ds = new DataSet(Membership.TableName);
            ds.Tables.Add(dt1);

            return ds;
        }

        public static DataTable FlipDataTable(DataTable dt)
        {
            DataTable table = new DataTable();
            //Get all the rows and change into columns
            for (int i = 0; i <= dt.Rows.Count; i++)
            {
                table.Columns.Add(Convert.ToString(i));
            }
            DataRow dr;
            //get all the columns and make it as rows
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                dr = table.NewRow();
                dr[0] = dt.Columns[j].ToString();
                for (int k = 1; k <= dt.Rows.Count; k++)
                    dr[k] = dt.Rows[k - 1][j];
                table.Rows.Add(dr);
            }

            return table;
        }

        public static string GenerateForumUserInfo(string username)
        {
            var sb = new System.Text.StringBuilder();

            if (username != "Guest") //We display nothing for guest users
            {
                Member User = new Member(username);

                string avatarUrl = (HttpContext.Current.Handler as System.Web.UI.Page).ResolveClientUrl(User.AvatarUrl);

                sb.Append("<div class=\"section\"><img src=\"")
                  .Append(avatarUrl)
                  .Append("\" width=\"70px\" height=\"70px\" style=\"border: 1px solid #666\"/></div><br/>");

                if (User.Achievements.Count > 1)
                {
                    //We have some achievements
                    sb.Append("<div class=\"section\">");

                    foreach (int achivId in User.Achievements)
                    {
                        //Empty list contains only -1
                        if (achivId != -1)
                        {
                            Achievement achiv = new Achievement(achivId);
                            sb.Append(HtmlCreator.GenerateUserAcheivementHTML(achiv, true));
                        }
                    }

                    sb.Append("</div><br/>");
                }

                sb.Append("<div class=\"section\">Membership: ")
                .Append(User.FormattedMembershipName)
                .Append("<br/><img src=\"Images/Flags/")
                .Append(User.CountryCode)
                .Append(".png\" class=\"imagemiddle\" /> ")
                .Append(User.Country)
                .Append("</div><br/>");

            }

            return sb.ToString();
        }
    }

}