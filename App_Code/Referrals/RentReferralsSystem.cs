using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Memberships;
using Prem.PTC.Members;
using System.Data;
using ExtensionMethods;
using MarchewkaOne.Titan.Balances;

namespace Prem.PTC.Referrals
{
    /// <summary>
    /// This class hadnles renting referrals: both normal system and BOT system
    /// Automatically decides about everything basing on admin panel settings
    /// </summary>
    public class RentReferralsSystem
    {
        private string Username;
        private int UserId;
        private IMembership Membership;
        private AppSettings.Referrals.RentingOption option;
        private static List<Member> activeUsers;
        private static int activeUsersIndex;
        private static Random random;

        private static List<Member> GetAvailableMembersForRent(string ExcludeThisUsername = null)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("Referer", "");
            dict.Add("AccountStatusInt", (int)MemberStatus.Active);
            dict.Add("IsRented", false);

            var RawResult = TableHelper.SelectRows<Member>(dict);
            var FinalResult = new List<Member>();

            // 0. ExcludeThisUsername
            // 1. MinLastClickingActivity
            // 2. BannedUsernameList

            var BannedUsernameList = GetListOfBannedRentedReferralUsernames();

            foreach (var elem in RawResult)
            {
                bool IsOk = true;

                // 0.
                if (ExcludeThisUsername != null)
                    if (elem.Name == ExcludeThisUsername)
                        IsOk = false;

                // 1.
                if (AppSettings.RentedReferrals.MinLastClickingActivity != TimeSpan.Zero)
                {
                    DateTime requiredDate = DateTime.Now.AddDays(-(AppSettings.RentedReferrals.MinLastClickingActivity.Days));
                    if (elem.LastActive == null || elem.LastActive < requiredDate)
                        IsOk = false;
                }

                // 2.
                if (IsOk)
                {
                    foreach (string bannedUsername in BannedUsernameList)
                        if (bannedUsername == elem.Name)
                            IsOk = false;
                }

                if (IsOk)
                    FinalResult.Add(elem);
            }

            return FinalResult;
        }

        public RentReferralsSystem(string username, IMembership membership)
        {
            Username = username;
            Membership = membership;
            option = AppSettings.Referrals.Renting;
            activeUsersIndex = 0;
            random = new Random();
            this.UserId = (new Member(username)).Id; //BAD
        }

        /// <summary>
        /// Rent the referral (Bot or Real based on the admin panel settings)
        /// </summary>
        /// <param name="count"></param>
        /// <exception cref="MsgException">When no refs available (and OnlyNormalRefs are sold)</exception>
        public void RentReferrals(int count)
        {
            int availableNormalReferrals = GetAvailableMembersForRent(Username).Count;

            if (option == AppSettings.Referrals.RentingOption.Normal)
            {
                // If no referrals available - exception
                if (count > availableNormalReferrals)
                    throw new MsgException(Resources.L1.ER_RENT_NOREFS);

                RentNormalReferrals(count);
            }
            else if (option == AppSettings.Referrals.RentingOption.DirectReferrals)
            {
                // If no referrals available - exception
                if (count > availableNormalReferrals)
                    throw new MsgException(Resources.L1.ER_RENT_NOREFS);

                RentDirectReferrals(count);
            }
            else if (option == AppSettings.Referrals.RentingOption.All)
            {
                int amountToBotRent = count - availableNormalReferrals;
                if (amountToBotRent < 0)
                    amountToBotRent = 0;

                if (availableNormalReferrals != 0)
                    RentNormalReferrals(count - amountToBotRent);

                if (amountToBotRent != 0)
                {
                    AcquireActiveUsersList(amountToBotRent);
                    RentBotReferrals(amountToBotRent);
                }
            }
            else if (option == AppSettings.Referrals.RentingOption.Bot)
            {
                AcquireActiveUsersList(count);
                RentBotReferrals(count);
            }

            //Check the contests
            Prem.PTC.Contests.ContestManager.IMadeAnAction(Prem.PTC.Contests.ContestType.Rented, Username, null, count);

            TryForceAutopay(Username);
        }

        public static void TryForceAutopay(string username)
        {
            bool forceAutopay = AppSettings.Referrals.RentedRefAutopayPolicy == AppSettings.Referrals.AutopayPolicy.AllReferrals &&
               (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM RentedReferrals WHERE OwnerUsername = '{0}' AND HasAutoPay = 1", username)) > 0;
            if (forceAutopay)
                TableHelper.ExecuteRawCommandNonQuery(string.Format("UPDATE RentedReferrals SET HasAutoPay = 1 WHERE OwnerUsername = '{0}'", username));
        }

        /// <summary>
        /// Recycles the referral given, basing on the membership price
        /// Throws MsgException when renting only normal and no refs are available
        /// </summary>
        /// <param name="id"></param>
        public static void RecycleReferral(int id, Parser parser)
        {
            if (AppSettings.Referrals.Renting == AppSettings.Referrals.RentingOption.Normal)
            {
                if (GetAvailableNormalReferralsCount() == 0)
                    throw new MsgException(Resources.L1.ER_RENT_NOREFS);

                var ReferralToRecycle = TableHelper.SelectRows<RentedReferral>(TableHelper.MakeDictionary("RefId", Convert.ToInt32(id)))[0];
                int HowManyDaysHeHasLeft = ReferralToRecycle.ExpireDate.Subtract(DateTime.Now).Days + 1;

                Member Owner = new Member(ReferralToRecycle.OwnerUsername);

                var rrm = new RentReferralsSystem(Owner.Name, Owner.Membership);
                rrm.RentNormalReferrals(1, HowManyDaysHeHasLeft);

                DeleteReferral(id, parser);
            }
            else
            {
                var ReferralToRecycle = TableHelper.SelectRows<RentedReferral>(TableHelper.MakeDictionary("RefId", Convert.ToInt32(id)))[0];
                int HowManyDaysHeHasLeft = ReferralToRecycle.ExpireDate.Subtract(DateTime.Now).Days + 1;
                Member Owner = new Member(ReferralToRecycle.OwnerUsername);
                var rrm = new RentReferralsSystem(Owner.Name, Owner.Membership);
                RentReferralsSystem.AcquireActiveUsersList(1);
                rrm.RentBotReferrals(1, HowManyDaysHeHasLeft);
                DeleteReferral(id, parser);
                //string Command = "UPDATE RentedReferrals SET BotClass = " + GetBotClass().ToString() + ", ReferralSince = @DATE, HasAutoPay = 'false', LastClick = null, TotalClicks = 0, LastPointableActivity = null, PointsEarnedToReferer = 0, ClicksStats = '0#0#0#0#0#0#0#0#0#0#0#0#0#0' WHERE RefId = " + id.ToString();
                //parser.ExecuteRawCommandNonQuery(TableHelper.GetSqlCommand(Command, DateTime.Now));
            }
        }

        /// <summary>
        /// Renews the referral given for fixed amount of days
        /// </summary>
        /// <param name="id"></param>
        public static void RenewReferral(int id, Parser parser)
        {
            DateTime oldExpireDate = Convert.ToDateTime(parser.Select("RentedReferrals", "ExpireDate", TableHelper.MakeDictionary("RefId", id)));
            DateTime newExpireDate = oldExpireDate.AddDays(AppSettings.RentedReferrals.CanBeRentedFor.Days);

            string Command = "UPDATE RentedReferrals SET ExpireDate = @DATE WHERE RefId = " + id.ToString();
            parser.ExecuteRawCommandNonQuery(TableHelper.GetSqlCommand(Command, newExpireDate));
        }

        public static void RenewReferral(int id, int numberOfDays)
        {
            var referral = new RentedReferral(id);

            referral.ExpireDate = referral.ExpireDate.AddDays(numberOfDays);
            referral.Save();
        }

        /// <summary>
        /// Sets the AutoPay feature ON the referrala
        /// </summary>
        /// <param name="id">Referral id</param>
        public static void SetAutopayOnReferral(int id, Parser parser)
        {
            string Command = "UPDATE RentedReferrals SET HasAutoPay = 'true' WHERE RefId = " + id.ToString();
            parser.ExecuteRawCommandNonQuery(Command);
        }

        /// <summary>
        /// Sets the AutoPay feature OFF the referral
        /// </summary>
        /// <param name="id">Referral id</param>
        public static void SetAutopayOffReferral(int id, Parser parser)
        {
            string Command = "UPDATE RentedReferrals SET HasAutoPay = 'false' WHERE RefId = " + id.ToString();
            parser.ExecuteRawCommandNonQuery(Command);
        }

        /// <summary>
        /// Deletes the referral 
        /// For REAL: cleans the Referer and IsRented and whole record in RentedReferrals 
        /// For BOTS: delete the whole record)
        /// </summary>
        /// <param name="id">Referral id</param>
        public static void DeleteReferral(int id, Parser parser)
        {
            //Check if normal or bot
            RentedReferral referral = (TableHelper.SelectRows<RentedReferral>(TableHelper.MakeDictionary("RefId", id)))[0];
            string command;
            if (referral.BotClass == -1)
            {
                command = "UPDATE Users SET Referer = '', IsRented = 'false', PointsEarnedToReferer = 0, LastPointableActivity = null WHERE Username = '" + referral.FiredByUsername + "'";
                parser.ExecuteRawCommandNonQuery(command);
            }
            //Anyway delete from rentedreferrals
            command = "DELETE FROM RentedReferrals WHERE RefId = " + id.ToString();
            parser.ExecuteRawCommandNonQuery(command);
        }

        /// <summary>
        /// Deletes X RentedReferrals randomly
        /// </summary>
        /// <param name="id">Referral id</param>
        public void DeleteRentedReferrals(int count)
        {
            var list = TableHelper.SelectRows<RentedReferral>(TableHelper.MakeDictionary("OwnerUsername", Username));
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                for (int i = 0; i < count && i < list.Count; ++i)
                {
                    DeleteReferral(list[i].Id, bridge.Instance);
                }

            }
        }

        /// <summary>
        /// Count for particular user
        /// </summary>
        /// <returns></returns>
        public int GetUserRentedReferralsCount()
        {
            return TableHelper.CountOf<RentedReferral>(TableHelper.MakeDictionary("OwnerUsername", Username));
        }

        public List<RentedReferral> GetUserRentedReferrals()
        {
            return TableHelper.SelectRows<RentedReferral>(TableHelper.MakeDictionary("OwnerUsername", Username));
        }
        public static List<string> GetListOfBannedRentedReferralUsernames()
        {
            //ONLY UPDATE THE CODE HERE - IT IS ALREADY SUPPORTED
            var list = new List<string>();
            list.Add("admin");
            list.Add("tester");
            return list;
        }

        public static int GetAvailableNormalReferralsCount()
        {
            return GetAvailableMembersForRent().Count;
        }

        public int GetAvailableNormalReferralsCountWithoutUser()
        {
            return GetAvailableMembersForRent(Username).Count;
        }

        public static int GetRentedNormalReferralsCount()
        {
            return TableHelper.CountOf<RentedReferral>(TableHelper.MakeDictionary("BotClass", -1));
        }

        public static int GetRentedBotReferralsCount()
        {
            int TotalCount = TableHelper.CountOf<RentedReferral>();
            return TotalCount - GetRentedNormalReferralsCount();
        }

        private void RentNormalReferrals(int count, int forHowManyDays = -1)
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                var list = GetAvailableMembersForRent(Username);
                var dict = new Dictionary<string, object>();

                for (int i = 0; i < count && i < list.Count; ++i)
                {
                    Member futureReferral = list[i];
                    futureReferral.Referer = Username;
                    futureReferral.ReferrerId = UserId;
                    futureReferral.IsRented = true;
                    futureReferral.PointsEarnedToReferer = 0;
                    futureReferral.LastPointableActivity = null;
                    futureReferral.IsSpotted = true;
                    futureReferral.Save();

                    //Now insert to RentedReferrals table

                    dict.Clear();
                    dict.Add("OwnerUsername", Username);
                    dict.Add("BotClass", -1);
                    dict.Add("FiredBy", futureReferral.Name);
                    dict.Add("ReferralSince", DateTime.Now);

                    //Assign expiration days
                    int ExpireInXDays = AppSettings.RentedReferrals.CanBeRentedFor.Days;
                    if (forHowManyDays != -1)
                        ExpireInXDays = forHowManyDays;

                    dict.Add("ExpireDate", DateTime.Now.AddDays(ExpireInXDays));

                    bridge.Instance.Insert(RentedReferral.TableName, dict);

                    ErrorLogger.Log(Username + " rented 1 normal referral (username: " + futureReferral.Name + ")", LogType.RefTrack);
                }
            }
        }

        private void RentDirectReferrals(int count, int forHowManyDays = -1)
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                var list = GetAvailableMembersForRent(Username);
                var dict = new Dictionary<string, object>();

                for (int i = 0; i < count && i < list.Count; ++i)
                {
                    Member futureReferral = list[i];
                    futureReferral.TryAddReferer(Username, true);
                }
            }
        }

        private void RentBotReferrals(int count, int howmanydays = -1)
        {
            DateTime rentTime = DateTime.Now.AddDays(AppSettings.RentedReferrals.CanBeRentedFor.Days);

            if (howmanydays != -1)
                rentTime = DateTime.Now.AddDays(howmanydays);

            var dict = new Dictionary<string, object>();
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                for (int i = 0; i < count; ++i)
                {
                    int botclass = GetBotClass();
                    string firedbyName = (activeUsers.Count > 1) ? (activeUsers[activeUsersIndex++ % (activeUsers.Count - 1)]).Name : Username;
                    //Insert to RentedReferrals table
                    dict.Clear();
                    dict.Add("OwnerUsername", Username);
                    dict.Add("FiredBy", firedbyName);
                    dict.Add("BotClass", botclass);
                    dict.Add("ReferralSince", DateTime.Now);
                    dict.Add("ExpireDate", rentTime);
                    bridge.Instance.Insert(RentedReferral.TableName, dict);

                    ErrorLogger.Log(Username + " rented 1 bot referral (class " + botclass + ")", LogType.RefTrack);
                }
            }
        }

        public static void AcquireActiveUsersList(int maxCount)
        {
            DataTable data;
            string Command = "SELECT TOP " + maxCount.ToString() + " * FROM Users WHERE LastActivityDate2 > @DATE AND TotalClicks > 3";

            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                data = bridge.Instance.ExecuteRawCommandToDataTable(TableHelper.GetSqlCommand(Command, DateTime.Now.AddDays(-1).AddHours(-12)));
            }

            activeUsers = TableHelper.GetListFromDataTable<Member>(data, 100, true);
        }

        private static int GetBotClass()
        {
            int BotQualityIndex = AppSettings.BotReferrals.BotQualityIndex;
            int NegativeBotsPercent = AppSettings.BotReferrals.InactiveBotPercentage;

            //Lets asssume: botqualityindex = 20, negativebotspercent = 30

            //We have NegativeBotsPercent to have our bot class=0 30%
            //And rest to have some class between 1 and 20: each has 100-30=70% 70/20= 3.5% 

            int poolNumber = random.Next(0, 100); // 76

            if (poolNumber <= NegativeBotsPercent) //0-30
                return 0;

            int rawDeal = 100 - NegativeBotsPercent; //70 (Rest)
            poolNumber = poolNumber - NegativeBotsPercent; // 76-30 = 46
            double chances = rawDeal / BotQualityIndex; // 70/20 = 3.5

            return (int)Math.Ceiling(poolNumber / chances); // 46/3.5 = 13.142.... => 14 (final class)
        }

        /// <summary>
        /// Handles direct referrals too, IT IS Fired after every successful watch ad action
        /// </summary>
        public void IClicked(PtcAdvert Ad)
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                //Get all rented referrals "fired by" (normal and bots) the user
                string Command = "SELECT * FROM RentedReferrals WHERE BotClass <> 0 AND FiredBy = '" + Username + "'";
                DataTable result = bridge.Instance.ExecuteRawCommandToDataTable(Command);
                var InterestingList = TableHelper.GetListFromDataTable<RentedReferral>(result, 100, true);

                foreach (RentedReferral referral in InterestingList)
                {
                    //Referral management
                    referral.LastClick = DateTime.Now;
                    referral.TotalClicks++;
                    //Stats
                    List<int> statlist = TableHelper.GetIntListFromString(referral.ClicksStats);
                    statlist[0]++;
                    referral.ClicksStats = TableHelper.GetStringFromIntList(statlist);
                    referral.Save();

                    //Add money to owneruser if can
                    Member OwnerUser = new Member(referral.OwnerUsername);
                    HandleOwnerUser(OwnerUser, Ad);
                }
            }            
        }

        public void IClicked(TrafficExchangeAdvert Ad)
        {
            //Handle direct ref if exists
            Member WhoFired = new Member(Username);
            if (WhoFired.HasReferer && WhoFired.IsRented == false)
            {
                Member OwnerUser = new Member(WhoFired.ReferrerId);
                HandleOwnerUser(OwnerUser, Ad, true);
            }
        }

        [Obsolete]
        private void HandleOwnerUser(Member OwnerUser, PtcAdvert Ad, bool FiredByDirectReferral = false, Member WhoFired = null)
        {
            bool CreditMoney = OwnerUser.HasClickedEnoughToProfitFromReferrals();

            //Check if earning only from Standard and Extended is enabled 
            //TO REMOVE
            if (CreditMoney && AppSettings.Misc.ExposureRefEarningsEnabled)
            {
                if (Ad.ExposureType == AdExposure.Standard || Ad.ExposureType == AdExposure.Extended)
                    CreditMoney = true;
                else
                    CreditMoney = false;
            }

            if (CreditMoney)
            {
                var Earnings = FiredByDirectReferral ? PtcAdvert.CalculateEarningsFromDirectReferral(OwnerUser, Ad) : PtcAdvert.CalculateEarningsFromRentedReferral(OwnerUser, Ad);
                
                if(FiredByDirectReferral)
                    OwnerUser.AddToMainBalance(Earnings, "PTC /ref/" + Username, BalanceLogType.Other);
                else
                    OwnerUser.AddToMainBalance(Earnings, "PTC /ref/", BalanceLogType.Other);

                bool isFullSaveRequired1 = false;
                bool isFullSaveRequired2 = false;

                //Achievements trial
                isFullSaveRequired1 = OwnerUser.TryToAddAchievements(
                    Prem.PTC.Achievements.Achievement.GetProperAchievements(
                    Prem.PTC.Achievements.AchievementType.AfterClicks, OwnerUser.TotalClicks + 1));

                string inter = (OwnerUser.TotalEarned + Earnings).ToClearString();
                Decimal tempMoney = Decimal.Parse(inter, new System.Globalization.CultureInfo("en-US"));


                isFullSaveRequired2 = OwnerUser.TryToAddAchievements(
                    Prem.PTC.Achievements.Achievement.GetProperAchievements(
                    Prem.PTC.Achievements.AchievementType.AfterEarning, Convert.ToInt32(tempMoney)));

                OwnerUser.IncreaseEarnings(Earnings);
                OwnerUser.IncreaseStatClicks(1);

                if (FiredByDirectReferral)
                {
                    var pointsEarnings = OwnerUser.Membership.DirectReferralAdvertClickEarningsPoints;
                    OwnerUser.AddToPointsBalance(pointsEarnings, "PTC /ref/" + Username);
                    OwnerUser.IncreaseDRClicks();
                    OwnerUser.IncreaseEarningsFromDirectReferral(Earnings);
                    if(WhoFired != null)
                    {
                        WhoFired.TotalPTCClicksToDReferer += 1;
                        WhoFired.TotalEarnedToDReferer += Earnings;
                        WhoFired.SaveStatisticsAndBalances();
                    }
                }
                else
                    OwnerUser.IncreaseRRClicks();

                if (isFullSaveRequired1 || isFullSaveRequired2)
                    OwnerUser.Save();
                else
                    OwnerUser.SaveStatisticsAndBalances();
            }
            else
            {
                OwnerUser.IncreaseStatClicks(1);

                if (FiredByDirectReferral)
                {
                    var Earnings = PtcAdvert.CalculateEarningsFromDirectReferral(OwnerUser, Ad);
                    OwnerUser.AddToMainBalance(Earnings, "PTC /ref/" + Username, BalanceLogType.Other);
                    OwnerUser.IncreaseEarnings(Earnings);

                    var pointsEarnings = OwnerUser.Membership.DirectReferralAdvertClickEarningsPoints;
                    OwnerUser.AddToPointsBalance(pointsEarnings, "PTC /ref/" + Username);
                    OwnerUser.IncreaseDRClicks();
                    OwnerUser.TotalEarned += Earnings;

                    if (WhoFired != null)
                    {
                        WhoFired.TotalPTCClicksToDReferer += 1;
                        WhoFired.TotalEarnedToDReferer += Earnings;
                        WhoFired.LastDRActivity = AppSettings.ServerTime;
                        WhoFired.SaveStatisticsAndBalances();
                    }
                }
                else
                    OwnerUser.IncreaseRRClicks();

                OwnerUser.SaveStatisticsAndBalances();
            }
        }

        private void HandleOwnerUser(Member OwnerUser, TrafficExchangeAdvert Ad, bool FiredByDirectReferral = false)
        {
            bool CreditMoney = OwnerUser.HasClickedEnoughToProfitFromReferrals();

            if (CreditMoney)
            {
                var Earnings = TrafficExchangeAdvert.CalculateEarningsFromDirectReferralTE(OwnerUser, Ad);

                OwnerUser.AddToMainBalance(Earnings, "TE /ref/" + Username, BalanceLogType.Other);
                bool isFullSaveRequired1 = false;
                bool isFullSaveRequired2 = false;

                string inter = (OwnerUser.TotalEarned + Earnings).ToClearString();
                Decimal tempMoney = Decimal.Parse(inter, new System.Globalization.CultureInfo("en-US"));

                isFullSaveRequired2 = OwnerUser.TryToAddAchievements(
                    Prem.PTC.Achievements.Achievement.GetProperAchievements(
                    Prem.PTC.Achievements.AchievementType.AfterEarning, Convert.ToInt32(tempMoney)));

                OwnerUser.IncreaseEarnings(Earnings);
                //OwnerUser.IncreaseStatClicks(1);

                //if (FiredByDirectReferral)
                //    OwnerUser.IncreaseDRClicks();
                //else
                //    OwnerUser.IncreaseRRClicks();

                if (isFullSaveRequired1 || isFullSaveRequired2)
                    OwnerUser.Save();
                else
                    OwnerUser.SaveBalances();
            }
            else
            {
                //OwnerUser.IncreaseStatClicks(1);

                //if (FiredByDirectReferral)
                //    OwnerUser.IncreaseDRClicks();
                //else
                //    OwnerUser.IncreaseRRClicks();

                //OwnerUser.SaveBalances();
            }
        }

        public static bool CanBeAutoPayedOn(DateTime expiredate)
        {
            int MinExpireDays = AppSettings.RentedReferrals.MinDaysToStartAutoPay;

            if (DateTime.Now.AddDays(MinExpireDays) <= expiredate)
                return true;

            return false;
        }

        /// <summary>
        /// Run this script everyday to manage referrals: delete expired, proceed autopay, recalculate statistics on RentedReferrals and renew BOT firedby
        /// NOTE: This script may take a while to complete
        /// NOTE: Run it before the Member Statistics recount !!!!!!!!!!!!!
        /// </summary>
        public static void CRON()
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                Parser parser = bridge.Instance;

                random = new Random();

                //Expirations
                DataTable ExpiredNormal = parser.ExecuteRawCommandToDataTable(
                    TableHelper.GetSqlCommand("SELECT * FROM RentedReferrals WHERE ExpireDate <= @DATE AND BotClass = -1", DateTime.Now));

                var ExpiredNormalList = TableHelper.GetListFromDataTable<RentedReferral>(ExpiredNormal, 100, true);
                foreach (RentedReferral referral in ExpiredNormalList)
                {
                    parser.ExecuteRawCommandNonQuery("UPDATE Users SET Referer = '', IsRented = 'false', LastPointableActivity = null, PointsEarnedToReferer = 0 WHERE Username = '" + referral.FiredByUsername + "'");
                }
                string Command = "DELETE FROM RentedReferrals WHERE ExpireDate <= @DATE";
                parser.ExecuteRawCommandNonQuery(TableHelper.GetSqlCommand(Command, DateTime.Now));

                var fullRentedList = TableHelper.SelectAllRows<RentedReferral>();
                AcquireActiveUsersList(fullRentedList.Count);
                activeUsersIndex = 0;
                bool areWeLoweringClasses = false;

                //With probablility 1/8 lower all bot classes
                if (random.Next(0, 8) == 2)
                {
                    areWeLoweringClasses = true;
                }


                foreach (RentedReferral referral in fullRentedList)
                {
                    //AutoPay managementd
                    if (referral.HasAutoPay)
                    {
                        //Clicked => Renew for 1 day, take money from owner
                        Member Owner = new Member(referral.OwnerUsername);
                        int numberOfRefs = fullRentedList.Count(x => x.OwnerUsername == Owner.Name);
                        Money autopayPrice = RentedReferralRangePrice.GetPriceForAutopay(Owner.Membership.DailyAutoPayCost, numberOfRefs);

                        if (referral.ClicksStatsList[0] > 0 && Owner.PurchaseBalance >= autopayPrice)
                        {
                            referral.ExpireDate = referral.ExpireDate.AddDays(1);
                            Owner.SubtractFromPurchaseBalance(autopayPrice, "AutoPay", BalanceLogType.Other);
                            Owner.Save();
                        }
                        else if(AppSettings.Referrals.RentedRefAutopayPolicy == AppSettings.Referrals.AutopayPolicy.UserChooses)
                            referral.HasAutoPay = false;

                        referral.Save();
                    }
                }

                //Recount clicks stats
                bridge.Instance.ExecuteRawCommandNonQuery(TableHelper.GetRawRecalculateCommand("RentedReferrals", "ClicksStats", false));

                //Append new active FiredBy
                Command = @";WITH CTE_CARS AS (SELECT FiredBy, ROW_NUMBER() OVER (ORDER BY RefId) AS RN FROM RentedReferrals WHERE BotClass > 0)
                                ,CTE_ENGINES AS (SELECT Username, ROW_NUMBER() OVER (ORDER BY NEWID()) AS RN FROM Users WHERE LastActivityDate2 > DATEADD(day, -2, GETDATE()) AND TotalClicks > 3)

                                UPDATE CTE_CARS SET CTE_CARS.FiredBy = CTE_ENGINES.Username
                                FROM CTE_CARS JOIN CTE_ENGINES ON (CTE_CARS.RN - CTE_ENGINES.RN) % (SELECT COUNT(1) FROM CTE_ENGINES) = 0";
                bridge.Instance.ExecuteRawCommandNonQuery(Command);

                //Lower classes
                if (areWeLoweringClasses)
                {
                    Command = "UPDATE RentedReferrals SET BotClass = BotClass - 1 WHERE BotClass > 0";
                    bridge.Instance.ExecuteRawCommandNonQuery(Command);
                }

            }
        }


        public static double GetAVG(int clicks, DateTime joined)
        {
            double TotalDays = DateTime.Now.Subtract(joined).TotalDays;

            double avg = (double)clicks / Math.Ceiling(TotalDays);
            return avg;
        }        
    }
}