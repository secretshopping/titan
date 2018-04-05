using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq;
using Prem.PTC.Payments;
using Prem.PTC.Members;

public class AntiCheatSystemSQLHelper
{
    private static string BanSQL(string detailedBanReason)
    {
        return String.Format(@"UPDATE Users SET AccountStatus = 'BannedOfCheating', AccountStatusInt = 6, DetailedBanReason = '{0}'", detailedBanReason);
    }
        
    public static string BanAllExceptTheOldestSQL()
    {
        

        return String.Format(@" WITH 
                                CTE_1 AS( --GETS ALL UNIQUE DUPLICATES
	                                SELECT PaymentAddress, ProcessorTypeInt, CustomPayoutProcessorId
	                                FROM UsersPaymentProcessorsAddresses
	                                WHERE PaymentAddress <> ''
	                                GROUP BY PaymentAddress, ProcessorTypeInt, CustomPayoutProcessorId
	                                HAVING COUNT(*) > 1
                                ),
                                CTE_2 AS( --GETS ALL DUPLICATES
	                                SELECT *
	                                FROM UsersPaymentProcessorsAddresses
	                                WHERE CONCAT(PaymentAddress,'#',ProcessorTypeInt,'#',CustomPayoutProcessorId) IN 
				                                (SELECT CONCAT(PaymentAddress,'#',ProcessorTypeInt,'#',CustomPayoutProcessorId) FROM CTE_1) 
                                )
                                UPDATE U 
                                SET AccountStatus = 'BannedOfCheating', AccountStatusInt = 6, DetailedBanReason = 'Duplicated payment address: ' + UPPA.PaymentAddress
                                FROM Users U
	                                INNER JOIN UsersPaymentProcessorsAddresses UPPA
	                                ON U.UserId = UPPA.UserId
                                WHERE U.UserId IN ( 
	                                SELECT UserId FROM  CTE_2 
	                                WHERE UserId NOT IN( --GETS ALL USERIDS FOR BAN
	                                SELECT inn.UserId
	                                FROM
	                                (SELECT t.UserId, t.LastChanged, t.PaymentAddress, 
		                                ROW_NUMBER() OVER(PARTITION BY t.PaymentAddress ORDER BY t.LastChanged) num
		                                FROM CTE_2 t) inn
	                                WHERE inn.num=1)
                                )
                                AND U.BypassSecurityCheck <> 5 
                                AND U.AccountStatusInt NOT IN (3,5,6)");
    }

    public static string BanAllSQL(string duplicateUsersColumn, string detailedBanReason, string additionalCondition)
    {
        return String.Format(@"
                WITH CTE AS(
                   SELECT UserId, 
                      COUNT(*) OVER(PARTITION BY {0}) as cnt
                   FROM Users
                   WHERE {1}
                )
                {2} WHERE UserId IN (SELECT UserId FROM CTE WHERE cnt > 1) AND BypassSecurityCheck <> 5 AND AccountStatusInt NOT IN (3,5,6)", 
                duplicateUsersColumn, additionalCondition, BanSQL(detailedBanReason));
    }

    public static string BanAllIPSQL(string duplicateUsersColumn, string detailedBanReason, string additionalCondition)
    {
        return String.Format(@"
               WITH CTE AS (
                SELECT UserId,
                    MIN(UserId) OVER (PARTITION BY {0}) as min_n,
                    MAX(UserId) OVER (PARTITION BY {0}) as max_n
                FROM IPHistoryLogs
                WHERE {1}
               )
               {2} WHERE UserId IN (SELECT UserId FROM CTE WHERE min_n <> max_n) AND BypassSecurityCheck <> 5 AND AccountStatusInt NOT IN (3,5,6)",
                duplicateUsersColumn, additionalCondition, BanSQL(detailedBanReason));
    }
}