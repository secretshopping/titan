using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class StringSawSundayPool
{
    public static void CreditFromSundayPool()
    {
        try
        {
            var sundayPool = GlobalPool.Get(PoolsHelper.GetSundayPoolId());
            var poolPercent = TitanFeatures.StringSawSundayPool.Value;
            if (Money.MultiplyPercent(sundayPool.SumAmount, poolPercent) > Money.Zero)
            {
                var users = TableHelper.GetListFromRawQuery<Member>(string.Format(@"SELECT * FROM Users WHERE AccountStatusInt = {0} AND UpgradeId != {1} 
            AND UserId IN (SELECT DISTINCT UserId FROM AdPacks WHERE MoneyToReturn > MoneyReturned)", (int)MemberStatus.Active, Membership.Standard.Id));
                var toDistribute = Money.MultiplyPercent(sundayPool.SumAmount, poolPercent);
                var perUser = Money.Zero;

                if (users.Count > 0)
                    perUser = toDistribute / users.Count;

                if (perUser > Money.Zero)
                {
                    foreach (var user in users)
                    {
                        user.AddToMainBalance(perUser, "Sunday AdPack Distribution");
                        user.SaveBalances();
                    }
                    GlobalPoolManager.SubtractFromPool(sundayPool.GlobalPoolType, toDistribute);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }
}