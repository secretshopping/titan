using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AdPackTypeManager
/// </summary>
public static class AdPackTypeManager
{
    public static void UpdateTypeNumbers()
    {
        var types = GetAllTypes().OrderBy(x => x.Number);

        var number = 0;
        foreach (var type in types)
        {
            type.Number = number;
            number++;
            type.Save();
        }
    }

    public static List<AdPackType> GetAllTypes()
    {
        return TableHelper.SelectAllRows<AdPackType>();
    }

    public static List<AdPackType> GetAllActiveTypes()
    {
        return TableHelper.GetListFromRawQuery<AdPackType>(string.Format("SELECT * FROM AdPackTypes WHERE Status = {0} ORDER BY PRICE", (int)AdPackTypeStatus.Active));
    }

    public static List<AdPackType> GetAllActiveTypesForUser(Member user)
    {
        var allActiveTypes = GetAllActiveTypes().OrderBy(x => x.Number).ToList();

        List<AdPackType> activeTypesForUser = new List<AdPackType>();

        for (int i = 0; i < allActiveTypes.Count; i++)
        {
            if ((i == 0 || GetNumberOfUsersAdPacks(allActiveTypes[i - 1].Id, user.Id) >= allActiveTypes[i].MinNumberOfPreviousType) && user.HasThisMembershipOrHigher(allActiveTypes[i].RequiredMembership))
                activeTypesForUser.Add(allActiveTypes[i]);
        }
        return activeTypesForUser;
    }

    public static int GetNumberOfUsersAdPacks(int adpackTypeId, int userId)
    {
        return (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(Id) FROM AdPacks WHERE AdPackTypeId = {0} AND UserId = {1}", adpackTypeId, userId));
    }

    public static int GetNumberOfAdPacks(int adpackTypeId)
    {
        return (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(Id) FROM AdPacks WHERE AdPackTypeId = {0}", adpackTypeId));
    }

    public static Money GetWithdrawalLimit(int userId)
    {
        var allTypes = GetAllTypes();

        Money limit = Money.Zero;

        foreach (var type in allTypes)
        {
            int numberOfPacks = GetNumberOfUsersAdPacks(type.Id, userId);
            limit += numberOfPacks * Money.MultiplyPercent(type.Price, type.WithdrawLimitPercentage);
        }

        return limit;
    }
}