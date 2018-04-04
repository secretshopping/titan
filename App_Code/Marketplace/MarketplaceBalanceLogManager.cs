using ExtensionMethods;
using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Titan.Marketplace
{
    public static class MarketplaceBalanceLogManager
    {
        public static List<MarketplaceBalanceLog> GetList(int userId)
        {
            return TableHelper.GetListFromRawQuery<MarketplaceBalanceLog>
                (string.Format(@"SELECT * FROM MarketplaceBalanceLogs 
                                WHERE UserId = {0}  AND Amount > 0 
                                ORDER BY DateAdded DESC", userId));
        }

        public static void MoneySpent(int userId, Money amount)
        {
            var logs = GetList(userId);

            int index = 0;

            while (amount > Money.Zero && index < logs.Count)
            {
                if (logs[index].Amount < amount)
                {
                    amount -= logs[index].Amount;
                    logs[index].Amount = Money.Zero;
                }
                else
                {
                    logs[index].Amount -= amount;
                    amount = Money.Zero;
                }

                if (logs[index].Amount <= Money.Zero)
                    logs[index].Delete();
                else
                    logs[index].Save();

                index++;
            }
        }

        public static void CRON()
        {
            try
            {
                if (!AppSettings.TitanFeatures.AdvertMarketplaceEnabled)
                    return;

                AppSettings.Marketplace.Reload();

                if (AppSettings.Marketplace.MarketplaceFundsExpireAfterDays == -1)
                    return;

                var serverTime = AppSettings.ServerTime.ToDBString();

                var balanceUpdateCommand = string.Format(@"UPDATE Users SET MarketplaceBalance = MarketplaceBalance - 
            (SELECT SUM(Amount) FROM MarketplaceBalanceLogs mbl 
            WHERE mbl.UserId = Users.UserId 
            AND '{0}' > DateAdd(d, {1}, DateAdded))", serverTime, AppSettings.Marketplace.MarketplaceFundsExpireAfterDays);

                var balanceLogsCommand = string.Format(@"SELECT 
	            mbl.UserId AS userId, 
                - SUM(mbl.Amount) AS amount, 
                u.MarketplaceBalance AS state
            FROM MarketplaceBalanceLogs mbl 
            JOIN Users u
	        ON mbl.UserId = u.UserId 
            WHERE '{0}' > DateAdd(d, 10, DateAdded)
            GROUP BY mbl.UserId, u.MarketplaceBalance", serverTime);

                var clearMBLCommand = string.Format(@"DELETE FROM MarketplaceBalanceLogs WHERE '{0}' > DateAdd(d, {1}, DateAdded);",
                    serverTime, AppSettings.Marketplace.MarketplaceFundsExpireAfterDays);

                TableHelper.ExecuteRawCommandNonQuery(balanceLogsCommand);

                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    BalanceLogManager.AddRange(bridge.Instance, balanceLogsCommand, "Balance expiration", BalanceType.MarketplaceBalance, BalanceLogType.Other);
                }

                TableHelper.ExecuteRawCommandNonQuery(clearMBLCommand);
            }
            catch(Exception ex) { ErrorLogger.Log(ex); }
        }
    }

}

