using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for CustomGroupManager
/// </summary>
public static class CustomGroupManager
{
    public static List<UserCustomGroup> GetOpenGroups()
    {
        DataTable openGroups;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            openGroups = bridge.Instance.ExecuteRawCommandToDataTable("SELECT * FROM UserCustomGroups ucg JOIN CustomGroups cg ON cg.Id = ucg.CustomGroupId WHERE ucg.AdPacksAdded < cg.AdPacksLimit ORDER BY cg.Number");
        }
        return TableHelper.GetListFromDataTable<UserCustomGroup>(openGroups, 10, true);
    }
    public static int GetUsersHighestExpiredGroupNumber(int userId)
    {
        string query = string.Format(@"SELECT MAX(cg.Number) FROM CustomGroups cg JOIN UserCustomGroups ucg ON cg.Id = ucg.CustomGroupId
WHERE ucg.Status = {0} AND ucg.CreatorUserId = {1}", (int)CustomGroupStatus.Expired, userId);

        int number;
        if (Int32.TryParse(Convert.ToString(TableHelper.SelectScalar(query)), out number))
            return number;
        else
            return 0;
    }

    public static List<UserCustomGroup> GetUsersOpenGroups(int userId)
    {
        DataTable openGroups;
        string query = string.Format(@"SELECT DISTINCT * FROM UserCustomGroups WHERE (Status = {0}) AND CreatorUserId = {1};", (int)CustomGroupStatus.InProgress, userId);
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            //            openGroups = bridge.Instance.ExecuteRawCommandToDataTable(string.Format(@"SELECT * FROM (SELECT DISTINCT ucg.* FROM UserCustomGroups ucg 
            //JOIN CustomGroups cg 
            //	ON cg.Id = ucg.CustomGroupId 
            //JOIN AdPacks ap 
            //	ON ap.UserCustomGroupId = ucg.id
            //WHERE  
            //	(ucg.AdPacksAdded < cg.AdPacksLimit OR ap.MoneyReturned < ap.MoneyToReturn) 
            //	AND ucg.CreatorUserId = {0}
            //) AS t ", userId));

            openGroups = bridge.Instance.ExecuteRawCommandToDataTable(query);

        }
        return TableHelper.GetListFromDataTable<UserCustomGroup>(openGroups, 10, true);
    }

    /// <summary>
    /// Returns All Custom Groups that have ever been created
    /// </summary>
    /// <returns></returns>
    public static List<CustomGroup> GetCreatedGroups()
    {
        DataTable createdGroups;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            createdGroups = bridge.Instance.ExecuteRawCommandToDataTable(@"SELECT cg.Id, cg.AdPacksLimit, cg.AcceleratedProfitPercentage,
                cg.NumberOfGroupsLimit, cg.CreatorsMinNumberOfAdPacks, cg.Color, cg.EnterLeaveAdPackMaxFillPercent
                FROM CustomGroups cg JOIN UserCustomGroups ucg ON cg.Id = ucg.CustomGroupId");

        }
        return TableHelper.GetListFromDataTable<CustomGroup>(createdGroups, 10, true);
    }

    public static string GetGroupsThatUserParticipatesInSQL(int userId)
    {
            return string.Format(@"SELECT DISTINCT ucg.CreatorUserId AS Creator, ucg.Name AS Name, cg.Color, ucg.AdPacksAdded, cg.AdPacksLimit,
    CAST(ucg.AdPacksAdded as decimal)/CAST(cg.AdPacksLimit as decimal) as Percentage, cg.AcceleratedProfitPercentage As Accelerated, ucg.Id AS UCGID, 
    (SELECT Username FROM Users WHERE UserId = ucg.CreatorUserId) AS CreatorName
    FROM UserCustomGroups ucg 
    JOIN CustomGroups cg ON ucg.CustomGroupId = cg.Id
    JOIN AdPacks ap ON ap.UserCustomGroupId = ucg.Id
    WHERE ap.UserId = {0}
    ORDER BY Percentage DESC", userId);
    }

    public static string GroupSqlCommand(CustomGroupStatus groupStatus)
    {
        string query = string.Format(@"SELECT DISTINCT ucg.CreatorUserId AS Creator, ucg.Name AS Name, cg.Color, ucg.AdPacksAdded, cg.AdPacksLimit,
CAST(ucg.AdPacksAdded as decimal) / CAST(cg.AdPacksLimit as decimal) as Percentage, cg.AcceleratedProfitPercentage AS Accelerated, ucg.Id AS UCGID, cg.AdPacksLimit - ucg.AdPacksAdded AS RequiredLeft,
  (SELECT Username FROM Users WHERE UserId = ucg.CreatorUserId) AS CreatorName
FROM UserCustomGroups ucg
JOIN CustomGroups cg ON ucg.CustomGroupId = cg.Id
JOIN AdPacks ap ON ap.UserCustomGroupId = ucg.Id 
WHERE ucg.Status = {0} ORDER BY Percentage DESC", (int)groupStatus);

        return query;
    }

    public static List<CustomGroup> GetAvailableGroups(int userId)
    {
        var allAdminGroupsList = GetAllAvailableGroups();
        var openGroups = GetUsersOpenGroups(userId);

        if (openGroups.Count == 0)
            return allAdminGroupsList;

        var availableGroups = new List<CustomGroup>();

        for (int i = 0; i < allAdminGroupsList.Count; i++)
        {
            int numberOfOpenPacks = 0;
            for (int j = 0; j < openGroups.Count; j++)
            {
                if (openGroups[j].CustomGroupId == allAdminGroupsList[i].Id)
                    numberOfOpenPacks++;
                if (numberOfOpenPacks < allAdminGroupsList[i].NumberOfGroupsLimit && j == openGroups.Count - 1)
                    availableGroups.Add(allAdminGroupsList[i]);
            }
        }

        return availableGroups;
    }

    private static List<CustomGroup> GetAllAvailableGroups()
    {
        DataTable allAdminGroups;
        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            allAdminGroups = bridge.Instance.ExecuteRawCommandToDataTable("SELECT * FROM CustomGroups");
        }
        var allAdminGroupsList = TableHelper.GetListFromDataTable<CustomGroup>(allAdminGroups, 100, true);
        return allAdminGroupsList;
    }


    public static int GetUsersHighestClosedGroup(Member user)
    {
        string query = string.Format(@"SELECT MAX(cg.Number) FROM CustomGroups cg 
JOIN UserCustomGroups ucg ON cg.Id = ucg.CustomGroupId
WHERE ucg.Status = {0} 
AND ucg.CreatorUserId = {1}", (int)CustomGroupStatus.Expired, user.Id);

        int number = 0;
        Int32.TryParse(Convert.ToString(TableHelper.SelectScalar(query)), out number);

        return number;
    }

    public static void AddRemoveUsersAdPacksToCustomGroup(int userId, int numberOfAdPacks, UserCustomGroup userCustomGroup, bool join)
    {
        CustomGroup customGroup = new CustomGroup(userCustomGroup.CustomGroupId);
        if (userId == userCustomGroup.CreatorUserId && !join)
        {
            var packsAvailableToWithdraw = AdPackManager.GetUsersActiveAdPacksForGroups(userId, customGroup, userCustomGroup.Id).Count - customGroup.CreatorsMinNumberOfAdPacks;
            if (numberOfAdPacks > packsAvailableToWithdraw)
                throw new MsgException(U4200.NOTENOUGHADPACKSAVAILABLEFORGROUPS.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural));
        }

        List<AdPack> allAdPacksList;
        if (join)
            allAdPacksList = AdPackManager.GetUsersActiveAdPacksForGroups(userId, customGroup, -1);
        else
            allAdPacksList = AdPackManager.GetUsersActiveAdPacksForGroups(userId, customGroup, userCustomGroup.Id);


        //HAS ENOUGH ADPACKS?
        if (allAdPacksList.Count < numberOfAdPacks)
            throw new MsgException(U4200.NOTENOUGHADPACKSAVAILABLEFORGROUPS.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural));

        IEnumerable<AdPack> adPacksList = (from a in allAdPacksList select a).Take(numberOfAdPacks);

        //NUMBER OF PACKS TO CLOSE
        if (join)
        {
            if (customGroup.AdPacksLimit - userCustomGroup.AdPacksAdded < numberOfAdPacks)
                throw new MsgException(U4200.TOOMANYPACKS.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) + " " + (customGroup.AdPacksLimit - userCustomGroup.AdPacksAdded));

            //Max number of creator/user adpack limit check
            var myCurrentAdPacksInThisGroupCount = AdPackManager.GetUsersActiveAdPacksForGroups(userId, customGroup, userCustomGroup.Id).Count;
            bool IsGroupCreator = userId == userCustomGroup.CreatorUserId;

            if (IsGroupCreator && myCurrentAdPacksInThisGroupCount + numberOfAdPacks > customGroup.CreatorsMaxNumberOfAdPacks)
                throw new MsgException(U4200.TOOMANYPACKS.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) + " " + (customGroup.CreatorsMaxNumberOfAdPacks - myCurrentAdPacksInThisGroupCount));

            if (!IsGroupCreator && myCurrentAdPacksInThisGroupCount + numberOfAdPacks > customGroup.UsersMaxNumberOfAdPacks)
                throw new MsgException(U4200.TOOMANYPACKS.Replace("%n%", AppSettings.RevShare.AdPack.AdPackNamePlural) + " " + (customGroup.UsersMaxNumberOfAdPacks - myCurrentAdPacksInThisGroupCount));
        }
        else
        {
            if (customGroup.AdPacksLimit == userCustomGroup.AdPacksAdded)
                throw new MsgException("Can't leave closed group");
        }

        if (join)
        {
            userCustomGroup.AdPacksAdded += numberOfAdPacks;
            foreach (AdPack adpack in adPacksList)
            {
                adpack.UserCustomGroupId = userCustomGroup.Id;
                adpack.Save();
            }
        }
        else
        {
            userCustomGroup.AdPacksAdded -= numberOfAdPacks;
            foreach (AdPack adpack in adPacksList)
            {
                adpack.UserCustomGroupId = -1;
                adpack.Save();
            }
        }

        userCustomGroup.Save();
    }


    public static void CreateUserCustomGroup(UserCustomGroup userGroup, CustomGroup customGroup, IEnumerable<AdPack> adPackList, Member user, string name, string description, string videoURL = "",
        string email = "", string skype = "", string phoneNumber = "", string facebookURL = "")
    {
        userGroup.CreatorUserId = user.Id;
        userGroup.Name = name;
        userGroup.Description = description;

        if (!string.IsNullOrWhiteSpace(videoURL))
            userGroup.PromoUrl = videoURL;

        if (!string.IsNullOrWhiteSpace(email))
            userGroup.Email = email;

        if (!string.IsNullOrWhiteSpace(skype))
            userGroup.Skype = skype;

        if (!string.IsNullOrWhiteSpace(phoneNumber))
            userGroup.PhoneNumber = phoneNumber;

        if (!string.IsNullOrWhiteSpace(facebookURL))
            userGroup.FacebookUrl = facebookURL;

        userGroup.CustomGroupId = customGroup.Id;
        userGroup.AdPacksAdded = adPackList.Count();
        userGroup.GotBonus = false;


        userGroup.Save();

        foreach (AdPack adpack in adPackList)
        {
            adpack.UserCustomGroupId = userGroup.Id;
            adpack.Save();
        }
    }

    public static void UpdateGroupNumbers()
    {
        var automaticGroups = GetAllAvailableGroups().OrderBy(x => x.AdPacksLimit);

        var number = 1;
        foreach (var group in automaticGroups)
        {
            group.Number = number;
            number++;
            group.Save();
        }
    }

    public static int GetNumberOfUsersCustomGroupInstances(int userId, int customGroupID)
    {
        int numberOfOpenInstances = (int)TableHelper.SelectScalar(string.Format(@"
SELECT COUNT(DISTINCT ucg.Id) FROM UserCustomGroups ucg JOIN CustomGroups cg ON ucg.CustomGroupId = cg.Id
JOIN AdPacks ap ON ap.UserCustomGroupId = ucg.id
WHERE ucg.CreatorUserId = {0}
AND ucg.CustomGroupId = {1}
AND ucg.Status = {2};", userId, customGroupID, (int)CustomGroupStatus.InProgress));

        return numberOfOpenInstances;
    }

    public static void IncreaseAdPacksReturnAmountInClosedGroup(CustomGroup customGroup, UserCustomGroup userCustomGroup)
    {
        var adPackTypes = AdPackTypeManager.GetAllActiveTypes();

        string roiEnlargedBy = "SELECT ROIEnlargedByPercentage FROM Memberships WHERE MembershipId = (SELECT UpgradeId FROM Users WHERE UserId = AdPacks.UserId)";

        foreach (var adPackType in adPackTypes)
        {
            int acceleratedPercent = adPackType.PackReturnValuePercentage + customGroup.AcceleratedProfitPercentage;

            string moneyToReturn = string.Format(@"CAST({0} * ({1} + ({2})) AS Money)/100", adPackType.Price.ToDecimal(), acceleratedPercent, roiEnlargedBy);
            string updateAdPacksQuery = string.Format(@"UPDATE AdPacks SET MoneyToReturn = {0} WHERE UserCustomGroupId = {1} AND AdPackTypeId = {2};",
                moneyToReturn, userCustomGroup.Id, adPackType.Id);

            TableHelper.ExecuteRawCommandNonQuery(updateAdPacksQuery);
        }

        int adPacksByOtherUsers = (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM AdPacks WHERE UserCustomGroupId = {0} AND UserId != {1}",
            userCustomGroup.Id, userCustomGroup.CreatorUserId));

        if (adPacksByOtherUsers > 0)
            userCustomGroup.GotBonus = false; //gets saved after this function has completed
        else
            userCustomGroup.GotBonus = true; //gets saved after this function has completed
    }

    public static List<KeyValuePair<string, int>> GetParticipantNamesAndAdPackCount(UserCustomGroup userCustomGroup)
    {
        List<KeyValuePair<string, int>> userNames = new List<KeyValuePair<string, int>>();
        var adPacks = TableHelper.GetListFromRawQuery<AdPack>(string.Format("SELECT ap.UserId, SUM(1) as TotalClicks FROM AdPacks ap WHERE ap.UserCustomGroupId = {0} GROUP BY ap.UserId", userCustomGroup.Id));
        foreach (var adPack in adPacks)
        {
            if (adPack.UserId != userCustomGroup.CreatorUserId)
            {
                string userName = new Member(adPack.UserId).Name;
                userNames.Add(new KeyValuePair<string, int>(userName, adPack.TotalClicks));
            }
        }
        return userNames;
    }


    public static int GetNumberOfUsersGroups(int userId, CustomGroupStatus status)
    {
        string query = string.Format(@"SELECT COUNT(*)  FROM UserCustomGroups ucg 
            WHERE ucg.CreatorUserId = {0}
            AND ucg.Status = {1};", userId, (int)status);
        return (int)TableHelper.SelectScalar(query);
    }

    public static int GetNumberOfAllClosedGroups(CustomGroupStatus status)
    {
        string query = string.Format(@"SELECT COUNT(*)  FROM UserCustomGroups ucg 
            WHERE ucg.Status = {0};", (int)status);
        return (int)TableHelper.SelectScalar(query);
    }

    public static int GetNumberOfUsersCreatedGroups(int userId)
    {
        return (int)TableHelper.SelectScalar((string.Format(@"SELECT COUNT(*) FROM UserCustomGroups WHERE CreatorUserId = {0}", userId)));
    }

    public static Money GetTotalCustomGroupsEarnings(int userCustomGroupId)
    {
        var adPackList = AdPackManager.GetAllAdPacksInCustomGroup(userCustomGroupId);
        Money earnings = Money.Zero;
        foreach (var adPack in adPackList)
        {
            earnings += adPack.MoneyReturned;
        }
        return earnings;
    }

    public static int GetNumberOfParticipantsInGroup(int userCustomGroupId)
    {
        return (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(DISTINCT UserId) FROM AdPacks WHERE UserCustomGroupId = {0}", userCustomGroupId));
    }

    /// <summary>
    /// Sets Status = Expired on closed Groups that returned all money
    /// </summary>
    public static void TrySetGroupsAsExpired()
    {
        var query = string.Format(@"UPDATE UserCustomGroups SET Status = {0} WHERE Id IN
                            (SELECT DISTINCT ucg.Id
                            FROM UserCustomGroups ucg
                            JOIN CustomGroups cg ON ucg.CustomGroupId = cg.Id
                            JOIN AdPacks ap ON ap.UserCustomGroupId = ucg.Id
                            WHERE ucg.AdPacksAdded = cg.AdPacksLimit
                            AND ucg.Id NOT IN(
	                            SELECT DISTINCT ucg.id
	                            FROM UserCustomGroups ucg 
	                            JOIN CustomGroups cg ON ucg.CustomGroupId = cg.Id
	                            JOIN AdPacks ap ON ap.UserCustomGroupId = ucg.Id
	                            WHERE ucg.AdPacksAdded = cg.AdPacksLimit
		                        AND ap.MoneyReturned < ap.MoneyToReturn
                                AND ap.UserId = ucg.CreatorUserId)
		                    )", (int)CustomGroupStatus.Expired);
        TableHelper.ExecuteRawCommandNonQuery(query);

    }
}
