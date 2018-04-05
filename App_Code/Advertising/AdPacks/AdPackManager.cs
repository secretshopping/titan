using ExtensionMethods;
using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Prem.PTC.Utils;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Titan;
using ExtensionMethods;
using Titan.Matrix;
using Titan.Leadership;
using Prem.PTC.HTML;
using Titan.Cryptocurrencies;

public static class AdPackManager
{
    public static List<AdPack> GetAllAdPacks(int adPacksAdvertId)
    {
        string command = String.Format("WHERE AdPacksAdvertId = {0} ORDER BY PurchaseDate", adPacksAdvertId);

        return TableHelper.GetListFromQuery<AdPack>(command);
    }

    /// <summary>
    /// Returns user's AdPacks that haven't returned all the money yet
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static List<AdPack> GetUsersActiveAdPacks(int userId)
    {
        string command = String.Format(@"SELECT * FROM AdPacks ap
WHERE ap.UserId = {0} AND ap.MoneyToReturn > ap.MoneyReturned", userId);

        return TableHelper.GetListFromRawQuery<AdPack>(command);
    }

    public static IEnumerable<AdPack> GetUsersActiveAdPacksWithoutGroup(int userId)
    {
        var adPacksList = GetUsersActiveAdPacks(userId);
        return from a in adPacksList.Where(x => x.UserCustomGroupId == -1) select a;
    }

    public static int GetNumberOfParticipantsInGroup(int userCustomGroupId)
    {
        int numberOfParticipants = Convert.ToInt32(TableHelper.SelectScalar(string.Format(@"SELECT COUNT(DISTINCT ap.UserId) FROM AdPacks ap
JOIN UserCustomGroups ucg on ucg.Id = ap.UserCustomGroupId
WHERE ucg.Id = {0}", userCustomGroupId)));
        return numberOfParticipants;
    }

    public static List<AdPack> GetUsersActiveAdPacksForGroups(int userId, CustomGroup customGroup, int userCustomGroupId = -1)
    {
        string command = String.Format(@"SELECT * FROM AdPacks ap
JOIN AdPackTypes apt ON apt.Id = ap.AdPackTypeId
WHERE ap.UserCustomGroupId = {0}
AND apt.CustomGroupsEnabled = 1
AND ap.AdPackTypeId != -1
AND ap.UserId = {1} AND (ap.MoneyReturned/ap.MoneyToReturn)*100 < {2}
ORDER BY (ap.MoneyReturned/ap.MoneyToReturn)", userCustomGroupId, userId, customGroup.EnterLeaveAdPackMaxFillPercent);
        return TableHelper.GetListFromRawQuery<AdPack>(command);
    }

    public static int GetNumberOfUsersAdPacksInCustomGroup(int userId, int userCustomGroupId)
    {
        int number = Convert.ToInt32(TableHelper.SelectScalar(string.Format(@"SELECT COUNT(*) from AdPacks WHERE UserCustomGroupId = {0} and UserId={1}", userCustomGroupId, userId)));
        return number;
    }


    public static int GetNumberOfUsersAdPacksInCustomGroupAvailableToWithdraw(int userId, int userCustomGroupId)
    {
        UserCustomGroup userCustomGroup = new UserCustomGroup(userCustomGroupId);
        CustomGroup customGroup = new CustomGroup(userCustomGroup.CustomGroupId);
        var active = GetUsersActiveAdPacksForGroups(userId, customGroup, userCustomGroupId).Count;

        var numberOfAvailableAdPacks = active - GetNumberOfUsersAdPacksInCustomGroup(userId, userCustomGroupId);
        return numberOfAvailableAdPacks;

    }
    public static void AddClickToAdPacksAd(int adPacksAdvertId)
    {
        TableHelper.ExecuteRawCommandNonQuery("EXEC AddAdPackAdvertClick " + adPacksAdvertId);
    }

    public static bool HasAvailableAdverts(int userId)
    {
        Dictionary<string, object> where = new Dictionary<string, object>();
        where.Add("CreatorUserId", userId);
        where.Add("Status", (int)AdvertStatus.Active);

        if (TableHelper.RowExists("AdPacksAdverts", where))
            return true;
        return false;
    }

    public static List<AdPacksAdvert> GetUsersAdverts(int userId)
    {
        DataTable usersAds;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            usersAds = bridge.Instance.ExecuteRawCommandToDataTable(string.Format("SELECT * FROM AdPacksAdverts WHERE CreatorUserId = {0} AND Status = {1}", userId, (int)AdvertStatus.Active));

        }
        var usersAdsList = TableHelper.GetListFromDataTable<AdPacksAdvert>(usersAds, 100, true);
        return usersAdsList;

    }

    public static List<AdPacksAdvert> GetAdPacksAdvertsForUsers(int dailyRequiredClicks)
    {
        var cache = new AdPackAdvertsForUsersCache();
        var adminIdCache = new AdminIdCache();

        DataTable usersAds = (DataTable)cache.Get();

        var usersAdsList = TableHelper.GetListFromDataTable<AdPacksAdvert>(usersAds, dailyRequiredClicks, true);
        var distinctAdPacks = new List<AdPacksAdvert>();
        var lowPriorityAdPacks = new List<AdPacksAdvert>();
        var highPriorityAdPacks = new List<AdPacksAdvert>();
        var normalPriorityAdPacks = new List<AdPacksAdvert>();

        AdPacksAdvert startPage = null;

        for (int i = 0; i < usersAdsList.Count; i++)
        {
            var item = usersAdsList[i];
            if (item.StartPageDate.HasValue && item.StartPageDate.Value.Date == DateTime.Now.Date)
                startPage = item;
            else if (item.Priority == 0)
                normalPriorityAdPacks.Add(item);
            else if (item.Priority > 0)
                highPriorityAdPacks.Add(item);
            else if (item.Priority < 0)
                lowPriorityAdPacks.Add(item);
        }

        highPriorityAdPacks.Sort((x, y) => x.Priority.CompareTo(y.Priority));
        lowPriorityAdPacks.Sort((x, y) => y.Priority.CompareTo(x.Priority));
        normalPriorityAdPacks.Shuffle();

        var adsCountToAdd = dailyRequiredClicks - highPriorityAdPacks.Count - lowPriorityAdPacks.Count;
        if (startPage != null)
            adsCountToAdd--;

        if (adsCountToAdd > 0 && normalPriorityAdPacks.Count > 0)
        {
            for (int i = 0; i < normalPriorityAdPacks.Count && i < adsCountToAdd; i++)
                distinctAdPacks.Add(normalPriorityAdPacks[i]);
        }

        if (distinctAdPacks.Count < adsCountToAdd)
        {
            var adminsAdsCount = adsCountToAdd - distinctAdPacks.Count;
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                var adminsAds = bridge.Instance.ExecuteRawCommandToDataTable(string.Format(@"SELECT DISTINCT TOP {0} apa.Id, apa.Status, apa.TargetUrl, apa.ConstantImagePath, apa.NormalImagePath, apa.CreatorUserId, apa.Title, apa.Description, apa.Priority
                                                                            FROM AdPacksAdverts apa 
                                                                            WHERE apa.CreatorUserId = {1} AND apa.Priority = 0 AND apa.Status = {2}",
                                                                            adminsAdsCount, (int)adminIdCache.Get(), (int)AdvertStatus.Active));

                var adminsAdsList = TableHelper.GetListFromDataTable<AdPacksAdvert>(adminsAds, adminsAdsCount, false);

                distinctAdPacks.AddRange(adminsAdsList);
            }
        }

        if (startPage != null)
            distinctAdPacks.Insert(0, startPage);

        for (int i = 0; i < highPriorityAdPacks.Count; i++)
            distinctAdPacks.Insert(startPage != null ? 1 : 0, highPriorityAdPacks[i]);

        distinctAdPacks.AddRange(lowPriorityAdPacks);
        return distinctAdPacks;

    }

    public static List<AdPacksAdvert> GetAdPacksWithDistinctAdverts(List<AdPacksAdvert> adPacksAdvertList)
    {
        var distinctAdPacks = new List<AdPacksAdvert>();

        foreach (var ad in adPacksAdvertList)
        {
            if (!distinctAdPacks.Any(el => el.Id == ad.Id))
                distinctAdPacks.Add(ad);
        }

        return distinctAdPacks;
    }

    public static List<AdPacksAdvert> GetUnwatchedAdsForUser(Member user)
    {
        var allAds = GetAdPacksAdvertsForUsers(user.Membership.AdPackDailyRequiredClicks);
        var adsForUser = new List<AdPacksAdvert>();

        foreach (var ad in allAds)
        {
            if (!user.RSAPTCAdsViewed.Contains(ad.Id))
                adsForUser.Add(ad);

        }

        adsForUser.Shuffle();

        return adsForUser;
    }

    public static Money GetAdPacksPrice(AdPackType adPackType, int numberOfPacks)
    {
        return adPackType.Price * numberOfPacks;
    }

    public static void BuyPacks(int numberOfPacks, int adPacksAdvertId, Member user, AdPackType adPackType, PurchaseBalances targetBalance, 
        int? userGroupIdNullable = null, int? groupIdNullable = null, bool forcePurchaseWithoutDeducingFunds = false, Member adPackOwner = null)
    {
        if (!TitanFeatures.IsClickmyad && !AppSettings.RevShare.AdPack.EnableAdvertChange && adPacksAdvertId == -1 && AppSettings.RevShare.AdPacksPolicy != AppSettings.AdPacksPolicy.HYIP)
            throw new MsgException(U6000.CANNOTBUYADPACKS);

        AdPacksForOtherUsers record = null;
        string note = string.Format("{0} purchase", AppSettings.RevShare.AdPack.AdPackName);
        //Buying AdPacks for oneself
        if (adPackOwner == null)
        {
            if (!user.HasThisMembershipOrHigher(adPackType.RequiredMembership))
                throw new MsgException(String.Format(U5006.YOUNEEDMEMBERSHIP, Membership.SelectName(adPackType.RequiredMembership)));

            int numberOfUsersAdPacks = GetNumberOfUsersAdPacks(user.Id, true, adPackType.Id);

            if (numberOfUsersAdPacks + numberOfPacks > adPackType.MaxInstances)
                throw new MsgException(U5004.TOOMANYADPACKOFTYPE.Replace("%a%", AppSettings.RevShare.AdPack.AdPackNamePlural)
                    .Replace("%b%", (adPackType.MaxInstances - numberOfUsersAdPacks).ToString()));

            int numberOfAllUsersAdpacks = GetNumberOfAllUsersAdPacks(user.Id, adPackType.Id);

            if (numberOfAllUsersAdpacks + numberOfPacks > adPackType.MaxInstancesOfAllAdpacks)
            {
                throw new MsgException(U5004.TOOMANYADPACKOFTYPE.Replace("%a%", AppSettings.RevShare.AdPack.AdPackNamePlural)
                       .Replace("%b%", (adPackType.MaxInstancesOfAllAdpacks - numberOfAllUsersAdpacks).ToString()));
            }

            var availableTypes = AdPackTypeManager.GetAllActiveTypesForUser(user);

            if (!availableTypes.Any(el => el.Id == adPackType.Id) || adPackType.Status != AdPackTypeStatus.Active)
                throw new MsgException("You cannot buy AdPacks of selected type.");

            adPackOwner = user;
        }
        else
        {
            AdPacksForOtherUsers.Validate(user.Id, adPackOwner.Id, numberOfPacks, out record);
            note += " for " + adPackOwner.Name;
        }

        //BUY ADPACKS
        var totalPrice = GetAdPacksPrice(adPackType, numberOfPacks);

        if (!forcePurchaseWithoutDeducingFunds)
            PurchaseOption.ChargeBalance(user, totalPrice, PurchaseOption.Features.AdPack.ToString(), targetBalance, note, BalanceLogType.AdPackPurchase);

        if (user != adPackOwner)
            AdPacksForOtherUsers.AddOrUpdate(record, user.Id, adPackOwner.Id, numberOfPacks);
        

        Money totalTrafficExchangeSurfCredits = Money.Zero;
        int totalLoginAdsCredits = 0;

        for (int i = 0; i < numberOfPacks; i++)
        {
            AdPack pack = new AdPack();
            pack.MoneyReturned = new Money(0);
            pack.AdPacksAdvertId = adPacksAdvertId;
            pack.TotalConstantBannerImpressions = 0;
            pack.TotalNormalBannerImpressions = 0;
            pack.ConstantBannerImpressionsBought = adPackType.ConstantBannerImpressions;
            pack.NormalBannerImpressionsBought = adPackType.NormalBannerImpressions;
            pack.ClicksBought = adPackType.Clicks;
            pack.PurchaseDate = DateTime.Now;
            pack.MoneyReturned = new Money(0);
            pack.MoneyToReturn = Money.MultiplyPercent(adPackType.Price, adPackType.PackReturnValuePercentage + adPackOwner.Membership.ROIEnlargedByPercentage);
            pack.UserCustomGroupId = -1;
            pack.UserId = adPackOwner.Id;
            pack.DistributionPriority = new Decimal(1);
            pack.AdPackTypeId = adPackType.Id;
            pack.DisplayTime = adPackType.DisplayTime;
            pack.BalanceBoughtType = targetBalance;
            pack.Save();

            totalTrafficExchangeSurfCredits += adPackType.TrafficExchangeSurfCredits;
            totalLoginAdsCredits += adPackType.LoginAdsCredits;
        }

        if (AppSettings.TitanFeatures.AdvertTrafficExchangeEnabled)
            adPackOwner.AddToTrafficBalance(totalTrafficExchangeSurfCredits, "Traffic Exchange Surf Credits", BalanceLogType.Other);

        adPackOwner.AddToLoginAdsCredits(totalLoginAdsCredits, note);
        adPackOwner.SaveBalances();

        Money moneyLeftForPools;
        if (TitanFeatures.isAri)
        {
            AriRevShareDistribution.AdPackAriCrediter crediter = new AriRevShareDistribution.AdPackAriCrediter(user);
            moneyLeftForPools = crediter.CreditReferer(totalPrice, targetBalance);
        }
        else
        {
            AdPackCrediter crediter = new AdPackCrediter(user);
            moneyLeftForPools = crediter.CreditReferer(totalPrice);
        }

        //Pools
        if (TitanFeatures.StringSawSundayPool.HasValue && AppSettings.ServerTime.DayOfWeek == DayOfWeek.Sunday)
            PoolDistributionManager.AddProfitToSundayPool(moneyLeftForPools);
        else
            PoolDistributionManager.AddProfit(ProfitSource.AdPacks, moneyLeftForPools);

        //Matrix
        MatrixBase.TryAddMemberAndCredit(user, totalPrice, AdvertType.AdPack);

        var purchasedItem = PurchasedItem.Create(user.Id, adPackType.Price, numberOfPacks,
            adPackType.Name + " " + AppSettings.RevShare.AdPack.AdPackName, PurchasedItemType.AdPack);

        if (TitanFeatures.isAri)
        {
            HtmlInvoiceGenerator generator = new HtmlInvoiceGenerator(purchasedItem);
            generator.SendPdfViaEmail();
        }

        //LeadershipSystem
        var list = new List<RestrictionKind>();
        list.Add(RestrictionKind.ActiveAdPacks);
        list.Add(RestrictionKind.ActiveAdPacksPrice);
        list.Add(RestrictionKind.DirectReferralsActiveAdPacks);
        list.Add(RestrictionKind.DirectReferralsActiveAdPacksPrice);
        LeadershipSystem.CheckSystem(list, user, 1);
    }

    public static List<AdPack> GetAllAdPacksInCustomGroup(int userCustomGroupId)
    {
        return TableHelper.GetListFromRawQuery<AdPack>(string.Format("SELECT * FROM AdPacks WHERE UserCustomGroupId = {0}", userCustomGroupId));
    }

    public static bool HasConstantBanner(int adPacksAdvertId)
    {
        AdPacksAdvert ad = new AdPacksAdvert(adPacksAdvertId);
        if (ad.ConstantImagePath == null)
            return false;
        return true;
    }

    public static bool HasNormalBanner(int adPacksAdvertId)
    {
        AdPacksAdvert ad = new AdPacksAdvert(adPacksAdvertId);
        if (ad.NormalImagePath == null)
            return false;
        return true;
    }

    public static int GetNumberOfStartPagesPurchasedForDay(DateTime date)
    {
        try
        {
            var ads = TableHelper.SelectScalar(string.Format(@"SELECT COUNT(*) FROM AdPacksAdverts
WHERE StartPageDate is not null
AND CAST(floor(cast(StartPageDate as float)) as datetime) = '{0}'", date.ToShortDateDBString()));

            return Convert.ToInt32(ads);
        }
        catch (Exception ex)
        {
            return 1000;
        }
    }

    public static int GetNumberOfUsersAdPacks(int userId, bool activePacks, int? adPackTypeId = null)
    {
        var sign = "<=";
        if (activePacks)
            sign = ">";

        StringBuilder query = new StringBuilder();
        query.AppendFormat("SELECT COUNT(Id) FROM AdPacks WHERE UserId = {0} AND MoneyToReturn {1} MoneyReturned AND AdPackTypeId != -1", userId, sign);

        if (adPackTypeId.HasValue)
            query.AppendFormat(" AND AdPackTypeId = {0}", adPackTypeId.Value);

        query.Append(";");

        return (int)TableHelper.SelectScalar(query.ToString());
    }

    public static int GetNumberOfAllUsersAdPacks(int userId, int? adPackTypeId = null)
    {
        if (adPackTypeId == null)
            return (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(Id) FROM AdPacks WHERE UserId = {0}", userId));
        else
            return (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(Id) FROM AdPacks WHERE UserId = {0} AND AdPackTypeId = {1}", userId, adPackTypeId));
    }

    public static Money GetUsersTotalMoneyReturnedFromAdPacks(int userId)
    {
        var result = TableHelper.SelectScalar(string.Format("SELECT SUM (MoneyReturned) FROM AdPacks WHERE UserId = {0};", userId));
        return (result is DBNull) ? Money.Zero : new Money(Convert.ToDecimal(result));
    }

    public static List<AdPack> GetAllUsersAdPacks(int userId)
    {
        return TableHelper.SelectRows<AdPack>(TableHelper.MakeDictionary("UserId", userId));
    }

    public static int GetAdDisplayTime(AdPacksAdvert ad)
    {
        var cache = new AdminIdCache();
        if (ad.CreatorUserId == (int)cache.Get())
            return AppSettings.RevShare.AdPack.AdminsAdvertDisplayTime;

        string query = string.Format(@"SELECT TOP 1 DisplayTime 
                                       FROM AdPacks                                      
                                       WHERE AdPacksAdvertId = {0}
                                       AND TotalClicks < ClicksBought
                                       ORDER BY PurchaseDate", ad.Id);

        return (int)TableHelper.SelectScalar(query);
    }

    public static int GetReqiuredClicksForExchange(int numberOfSeconds, AdPackType adPackType)
    {
        int oneSecondInClicks = adPackType.ValueOf1SecondInClicks;
        return numberOfSeconds * oneSecondInClicks;
    }

    public static void ExchangeClicksForSeconds(int numberOfSeconds, int adPackId, Member user)
    {
        AdPack adPack = new AdPack(adPackId);
        AdPackType adPackType = new AdPackType(adPack.AdPackTypeId);
        int clicksLeft = (adPack.ClicksBought - adPack.TotalClicks) - 1;

        int maxExtraSecondsLeft = user.Membership.MaxExtraAdPackSecondsForClicks - (adPack.DisplayTime - adPackType.DisplayTime);

        if (numberOfSeconds > maxExtraSecondsLeft)
            throw new MsgException(string.Format(U5006.CANNOTADDMORESECONDS, maxExtraSecondsLeft > 0 ? maxExtraSecondsLeft : 0));

        int requiredClicks = GetReqiuredClicksForExchange(numberOfSeconds, adPackType);

        if (clicksLeft < requiredClicks)
            throw new MsgException(string.Format(U5006.NOTENOUGHCLICKSFOREXCHANGE, requiredClicks + 1));

        adPack.DisplayTime = adPack.DisplayTime + numberOfSeconds;

        adPack.ClicksBought = adPack.ClicksBought - requiredClicks;
        adPack.Save();
    }

    public static void FinishAdPacksIfNotUpgradedFor(int days)
    {
        var lastUpgradeLimit = AppSettings.ServerTime.AddDays(-days).Date;
        var query = string.Format(@"UPDATE AdPacks SET MoneyReturned = MoneyToReturn
                                    WHERE UserId IN(SELECT UserId FROM Users WHERE HasEverUpgraded = 1 AND UpgradeExpires IS NOT NULL AND UpgradeExpires < '{0}')", lastUpgradeLimit.ToDBString());
        TableHelper.ExecuteRawCommandNonQuery(query);
    }

    public static void UnbindFromAdPacks(int adpacksAdvertId)
    {
        string query = string.Format("UPDATE AdPacks SET AdPacksAdvertId = -1 WHERE AdPacksAdvertId = {0}", adpacksAdvertId);
        TableHelper.ExecuteRawCommandNonQuery(query);
    }

    public static bool HasAdPackWithoutCampaigns(int userId)
    {
        string query = string.Format("SELECT COUNT(AdPacksAdvertId) FROM AdPacks WHERE AdPacksAdvertId = -1 AND UserId = {0};", userId);

        return AppSettings.RevShare.AdPacksPolicy == AppSettings.AdPacksPolicy.Standard && (int)TableHelper.SelectScalar(query) > 0;
    }

    public static TimeSpan GetAdSurfDeadline()
    {
        var deadline = AppSettings.ServerTime
                                            .Add(AppSettings.Site.TimeToNextCRONRun)
                                            .GetNextWeekday(AppSettings.RevShare.DayOfWeekDistribution);

        return deadline - AppSettings.ServerTime;
    }

    public static DataSet GetAdPackActiveTypesDataSet()
    {
        //Get DataTable source
        DataTable dt, dt1;
        DataSet ds;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            string query = string.Format(@"SELECT Name, Color, Price, 
                MinNumberOfPreviousType, MaxInstancesOfAllAdpacks, MaxInstances, Clicks, DisplayTime, NormalBannerImpressions, ConstantBannerImpressions, 
                RequiredMembership, ValueOf1SecondInClicks, CustomGroupsEnabled, LoginAdsCredits, WithdrawLimitPercentage, TrafficExchangeSurfCredits
                FROM AdPackTypes WHERE Status = {0} ORDER BY Number; ", (int)AdPackTypeStatus.Active);

            dt = bridge.Instance.ExecuteRawCommandToDataTable(query);
        }

        dt1 = DataCreator.FlipDataTable(dt);
        ds = new DataSet("AdPAckTypes");
        ds.Tables.Add(dt1);

        return ds;
    }

    /// <summary>
    /// Returns number of actualy activeated AdPack Types
    /// </summary>
    /// <returns></returns>
    public static int GetAdPackActiveTypesCount()
    {
        int result = -1;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            string query = string.Format(@"SELECT COUNT(1)
                    FROM AdPackTypes WHERE Status = {0}; ", (int)AdPackTypeStatus.Active);

            result = (int)bridge.Instance.ExecuteRawCommandScalar(query);
        }

        return result;
    }

    public static void TryBuyAdPackForTokens(int numberOfPacks, int advertId, Member user, AdPackType adPackType,
    int? userGroupIdNullable = null, int? groupIdNullable = null, bool forcePurchaseWithoutDeducingFunds = false, Member adPackOwner = null)
    {
        var TokenCryptocurrency = CryptocurrencyFactory.Get(CryptocurrencyType.ERC20Token);

        //Check all basic values
        Money TotalPriceForAdPacks = AdPackManager.GetAdPacksPrice(adPackType, numberOfPacks);
        Money TokenValue = TokenCryptocurrency.GetValue();
        Decimal TokenNumberNeeded = TotalPriceForAdPacks.ToDecimal() / TokenValue.ToDecimal();

        if (TokenValue <= Money.Zero)
            throw new MsgException("Amount of tokens can not be <= 0");

        if (TokenNumberNeeded > user.GetCryptocurrencyBalance(CryptocurrencyType.ERC20Token).ToDecimal())
            throw new MsgException(String.Format(U6012.NOFUNDSINWALLET, TokenCryptocurrency.Code));

        try
        {
            //Funds ok, lets proceed with tokens transfer
            user.SubtractFromCryptocurrencyBalance(CryptocurrencyType.ERC20Token, TokenNumberNeeded, "Purchase transfer", BalanceLogType.PurchaseTransfer);
            user.AddToPurchaseBalance(TotalPriceForAdPacks, "Purchase transfer", BalanceLogType.PurchaseTransfer);

            AdPackManager.BuyPacks(numberOfPacks, advertId, user, adPackType, PurchaseBalances.Purchase, adPackOwner: adPackOwner);
        }
        catch (MsgException ex)
        {
            user.AddToCryptocurrencyBalance(CryptocurrencyType.ERC20Token, TokenNumberNeeded, "Reversed Purchase transfer", BalanceLogType.ReversedPurchaseTransfer);
            user.SubtractFromPurchaseBalance(TotalPriceForAdPacks, "Reversed Purchase transfer", BalanceLogType.ReversedPurchaseTransfer);
            throw new MsgException("<br />" + ex.Message);
        }
    }

    public static void TryBuyAdPackFromAnotherBalance(int numberOfPacks, int advertId, Member user, AdPackType adPackType, BalanceType targetType,
    int? userGroupIdNullable = null, int? groupIdNullable = null, bool forcePurchaseWithoutDeducingFunds = false, Member adPackOwner = null)
    {
        Money TotalPriceForAdPacks = AdPackManager.GetAdPacksPrice(adPackType, numberOfPacks);

        if (TotalPriceForAdPacks <= Money.Zero)
            throw new MsgException("Value can not be <= 0");


        if (targetType == BalanceType.CommissionBalance)
        {
            if (TotalPriceForAdPacks > user.CommissionBalance)
                throw new MsgException(L1.NOTENOUGHFUNDS);

            try
            {
                //Funds ok, lets proceed with tokens transfer
                user.SubtractFromCommissionBalance(TotalPriceForAdPacks, "Purchase transfer", BalanceLogType.PurchaseTransfer);
                user.AddToPurchaseBalance(TotalPriceForAdPacks, "Purchase transfer", BalanceLogType.PurchaseTransfer);
                AdPackManager.BuyPacks(numberOfPacks, advertId, user, adPackType, PurchaseBalances.Purchase, adPackOwner: adPackOwner);
            }
            catch (MsgException ex)
            {
                user.SubtractFromPurchaseBalance(TotalPriceForAdPacks, "Reversed Purchase transfer", BalanceLogType.ReversedPurchaseTransfer);
                user.AddToPurchaseBalance(TotalPriceForAdPacks, "Purchase transfer", BalanceLogType.PurchaseTransfer);
                throw new MsgException("<br />" + ex.Message);
            }
        }
        else if (targetType == BalanceType.MainBalance)
        {
            if (TotalPriceForAdPacks > user.MainBalance)
                throw new MsgException(L1.NOTENOUGHFUNDS);

            try
            {
                //Funds ok, lets proceed with tokens transfer
                user.SubtractFromMainBalance(TotalPriceForAdPacks, "Purchase transfer", BalanceLogType.PurchaseTransfer);
                user.AddToPurchaseBalance(TotalPriceForAdPacks, "Purchase transfer", BalanceLogType.PurchaseTransfer);
                AdPackManager.BuyPacks(numberOfPacks, advertId, user, adPackType, PurchaseBalances.Purchase, adPackOwner: adPackOwner);
            }
            catch (MsgException ex)
            {
                user.SubtractFromPurchaseBalance(TotalPriceForAdPacks, "Reversed Purchase transfer", BalanceLogType.ReversedPurchaseTransfer);
                user.AddToMainBalance(TotalPriceForAdPacks, "Purchase transfer", BalanceLogType.PurchaseTransfer);
                throw new MsgException("<br />" + ex.Message);
            }
        }
        else if (targetType == BalanceType.BTC)
        {
            Money BTCValue = CryptocurrencyFactory.Get(CryptocurrencyType.BTC).GetValue();
            Decimal BTCAmountNeeded = TotalPriceForAdPacks.ToDecimal() / BTCValue.ToDecimal();

            if (BTCAmountNeeded > user.GetCryptocurrencyBalance(CryptocurrencyType.BTC).ToDecimal())
                throw new MsgException(String.Format(U6012.NOFUNDSINWALLET, "BTC"));

            try
            {
                //Funds ok, lets proceed with tokens transfer
                user.SubtractFromCryptocurrencyBalance(CryptocurrencyType.BTC, BTCAmountNeeded, "Purchase transfer", BalanceLogType.PurchaseTransfer);
                user.AddToPurchaseBalance(TotalPriceForAdPacks, "Purchase transfer", BalanceLogType.PurchaseTransfer);
                AdPackManager.BuyPacks(numberOfPacks, advertId, user, adPackType, PurchaseBalances.Purchase, adPackOwner: adPackOwner);
            }
            catch (MsgException ex)
            {
                user.SubtractFromPurchaseBalance(TotalPriceForAdPacks, "Reversed Purchase transfer", BalanceLogType.ReversedPurchaseTransfer);
                user.AddToCryptocurrencyBalance(CryptocurrencyType.BTC, BTCAmountNeeded, "Purchase transfer", BalanceLogType.PurchaseTransfer);
                throw new MsgException("<br />" + ex.Message);
            }
        }
        else
            throw new MsgException("Not implemented transaction");
    }


    public static void TryBuyAdPack(BalanceType fromBalance, int advertId, AdPackType adPackType, int numberOfPacks, Member adPackOwner = null)
    {
        if (Member.Current.IsAnyBalanceIsNegative())
            throw new MsgException(U6013.YOUCANTPROCEED);

        if (numberOfPacks <= 0)
            throw new MsgException(String.Format("Can't buy 0 {0}s", AppSettings.RevShare.AdPack.AdPackNamePlural));

        if (fromBalance == BalanceType.PurchaseBalance)
            AdPackManager.BuyPacks(numberOfPacks, advertId, Member.CurrentInCache, adPackType, PurchaseBalances.Purchase, adPackOwner: adPackOwner);
        else if (fromBalance == BalanceType.CashBalance)
            AdPackManager.BuyPacks(numberOfPacks, advertId, Member.CurrentInCache, adPackType, PurchaseBalances.Cash, adPackOwner: adPackOwner);
        else if (fromBalance == BalanceType.Token)
            AdPackManager.TryBuyAdPackForTokens(numberOfPacks, advertId, Member.CurrentInCache, adPackType, adPackOwner: adPackOwner);
        else 
            AdPackManager.TryBuyAdPackFromAnotherBalance(numberOfPacks, advertId, Member.CurrentInCache, adPackType, fromBalance, adPackOwner: adPackOwner);
    }
}