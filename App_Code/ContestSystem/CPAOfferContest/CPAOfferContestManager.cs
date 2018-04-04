using Prem.PTC;
using Prem.PTC.Contests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Titan;

/// <summary>
/// Summary description for CPAOfferContestManager
/// </summary>
public static class CPAOfferContestManager
{
    public static string GetCategoriesIncludedInContest(Dictionary<int, string> categories)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var c in categories)
        {
            sb.Append(c.Value);
            sb.Append(", ");
        }
        return sb.ToString();
    }

    public static Dictionary<int, string> GetCategoriesIncludedInContestDict(int contestId)
    {
        DataTable categoriesTable = new DataTable();
        Dictionary<int, string> categories = new Dictionary<int, string>();

        using (var bridge = ParserPool.Acquire(Database.Client))
        {
            categoriesTable = bridge.Instance.ExecuteRawCommandToDataTable(string.Format(@"SELECT CC.Id, CC.CategoryName FROM CPACategories CC 
                JOIN CPAOfferContests COC ON CC.Id = COC.CPACategoryId 
                WHERE ContestId = {0}", contestId));
        }

        foreach (DataRow row in categoriesTable.Rows)
        {
            categories.Add(Convert.ToInt32(row["Id"]), row["CategoryName"].ToString());
        }
        return categories;
    }

    public static bool IsIncludedInContest(int offerId)
    {
        var offerList = TableHelper.GetListFromRawQuery<CPAOfferContest>(string.Format(@"SELECT * FROM CPAOfferContests COC 
            JOIN CPAOffers CO ON COC.CPACategoryId = CO.CategoryId 
            JOIN Contests C ON C.Id = COC.ContestId
            WHERE CO.Id = {0}
            AND C.Status = {1}", offerId, (int)ContestStatus.Active));

        if (offerList.Count > 0)
            return true;
        return false;
    }
}