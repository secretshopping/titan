using ExtensionMethods;
using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarchewkaOne.Titan.Balances
{
    public class WalletManager
    {
        public static void CRON()
        {
            if (AppSettings.Ethereum.ERC20TokenEnabled && AppSettings.Ethereum.ERC20TokensFreezeSystemEnabled)
            {
                String Query = String.Empty;
                int DaysOfFreeze = AppSettings.Ethereum.ERC20TokensFreezeTimeDays;
                DateTime FreezeEnd = AppSettings.ServerTime.AddDays(-DaysOfFreeze);

                //Lets update balances with unfreezed tokens
                Query = String.Format(@"
                    WITH CTE_TEST AS ( SELECT UserId, SUM(NumberOfTokens) AS Summum FROM UserFreezedTokens WHERE DateOfAction<='{0}' GROUP BY UserId )
                    UPDATE UCB SET UCB.Balance = 
	                    CASE 
		                    WHEN  UCB.CurrencyCode='ERCFreezed' THEN UCB.Balance-(SELECT Summum FROM CTE_TEST WHERE UserId = UCB.UserId )
		                    WHEN  UCB.CurrencyCode='ERC20Token' THEN UCB.Balance+(SELECT Summum FROM CTE_TEST WHERE UserId = UCB.UserId )
		                    ELSE  UCB.Balance
	                    END
                    FROM UserCryptocurrencyBalances UCB
                    WHERE UCB.UserId IN(SELECT UserId FROM CTE_TEST)",
                    FreezeEnd.ToDBString()
                    );
                TableHelper.ExecuteRawCommandNonQuery(Query);

                //Lets delete unnecessary data from UserFreezedTokens
                Query = String.Format("DELETE FROM UserFreezedTokens WHERE DateOfAction < '{0}'", FreezeEnd.ToDBString());
                TableHelper.ExecuteRawCommandNonQuery(Query);

            }
        }
    }
}


