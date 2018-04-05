using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.News
{
    public class NewsSQLHelper
    {
        public static void CreditWriters()
        {
            //Credit
            var amount = "(SELECT [amount] FROM CTE WHERE CTE.userId = Users.UserId)";
            var updateCommand = String.Format(@"
                                  UPDATE Users SET 
                                    Users.Balance1 = Users.Balance1 + {0},
                                    Users.TotalEarned = Users.TotalEarned + {0},            
                                    Users.StatsEarned = cast(({0}+CONVERT(decimal(19,8),(CAST(SUBSTRING(Users.StatsEarned,0, CHARINDEX('#', Users.StatsEarned)) AS varchar(30))))) as varchar(1000)) + SUBSTRING(Users.StatsEarned,CHARINDEX('#', Users.StatsEarned), 1000)
                                  WHERE Users.UserId IN (SELECT DISTINCT UserId FROM CTE);", amount);
            TableHelper.ExecuteRawCommandNonQuery(GetWritersCTE() + updateCommand);

            //Add Balance Logs
            var balanceLogsCommand = GetWritersCTE() + "SELECT * FROM CTE;";
            BalanceLogManager.AddRange(balanceLogsCommand, "Article writing", BalanceType.MainBalance, BalanceLogType.ArticleWriting);

            //Add Money credited to articles table
            var creditArticlesQuery = @"
                        WITH CTE AS (
	                        SELECT av.ArticleId AS Article, FLOOR((COUNT(av.Id))/1000) AS Quantity, m.ArticleCreatorCPM AS PricePerQuantity FROM ArticleViews av 
		                                            INNER JOIN Articles a ON av.ArticleId = a.Id 
							                        INNER JOIN Users u ON u.UserId = a.CreatorUserId
							                        INNER JOIN Memberships m ON m.MembershipId = u.UpgradeId
		                                            WHERE av.CreatorCredited = 0 
		                                            GROUP BY av.ArticleId, m.ArticleCreatorCPM HAVING FLOOR((COUNT(av.Id))/1000) > 0
                         )

                         UPDATE Articles SET CreatorMoneyEarned = CreatorMoneyEarned + (SELECT Quantity * PricePerQuantity FROM CTE WHERE Articles.Id = CTE.Article) 
                         WHERE Id IN (SELECT DISTINCT Article FROM CTE);";
            TableHelper.ExecuteRawCommandNonQuery(creditArticlesQuery);


            //Mark views as credited
            var query = @"WITH CTE AS
                        (	SELECT av.ArticleId AS Article, FLOOR((COUNT(av.Id))/1000) AS Quantity FROM ArticleViews av 
		                    INNER JOIN Articles a ON av.ArticleId = a.Id 
		                    WHERE av.CreatorCredited = 0 
		                    GROUP BY av.ArticleId HAVING FLOOR((COUNT(av.Id))/1000) > 0
                        )

                        UPDATE ArticleViews SET ArticleViews.CreatorCredited = 1
                        WHERE ArticleViews.ArticleId IN (SELECT DISTINCT Article FROM CTE) AND ArticleViews.Id IN (
	                        SELECT TOP (SELECT Quantity*1000 FROM CTE WHERE ArticleViews.ArticleId = CTE.Article) av.Id FROM ArticleViews av 
	                        WHERE av.ArticleId  IN (SELECT Article FROM CTE WHERE Article = ArticleViews.ArticleId) ORDER BY av.ViewDate ASC
                        )
                        ";
            TableHelper.ExecuteRawCommandNonQuery(query);
        }

        public static void CreditInfluencers()
        {
            //Credit
            var amount = "(SELECT [amount] FROM CTE WHERE CTE.userId = Users.UserId)";
            var updateCommand = String.Format(@"
                                  UPDATE Users SET 
                                    Users.Balance1 = Users.Balance1 + {0},
                                    Users.TotalEarned = Users.TotalEarned + {0},            
                                    Users.StatsEarned = cast(({0}+CONVERT(decimal(19,8),(CAST(SUBSTRING(Users.StatsEarned,0, CHARINDEX('#', Users.StatsEarned)) AS varchar(30))))) as varchar(1000)) + SUBSTRING(Users.StatsEarned,CHARINDEX('#', Users.StatsEarned), 1000),
                                    Users.StatsArticlesTotalSharesMoney = cast(({0}+CONVERT(decimal(19,8),(CAST(SUBSTRING(Users.StatsArticlesTotalSharesMoney,0, CHARINDEX('#', Users.StatsArticlesTotalSharesMoney)) AS varchar(30))))) as varchar(1000)) + SUBSTRING(Users.StatsArticlesTotalSharesMoney,CHARINDEX('#', Users.StatsArticlesTotalSharesMoney), 1000)
                                  WHERE Users.UserId IN (SELECT DISTINCT UserId FROM CTE);", amount);
            TableHelper.ExecuteRawCommandNonQuery(GetInfluencersCTE() + updateCommand);

            //Add Balance Logs
            var balanceLogsCommand = GetInfluencersCTE() + "SELECT * FROM CTE;";
            BalanceLogManager.AddRange(balanceLogsCommand, "Article sharing", BalanceType.MainBalance, BalanceLogType.ArticleSharing);


            //Mark views as credited
            var query = @"WITH CTE AS
                        (	SELECT av.ArticleId AS Article, av.InfluencerUserId AS Influencer, FLOOR((COUNT(av.Id))/1000) AS Quantity FROM ArticleViews av 
		                    WHERE av.InfluencerUserId > 0 AND av.InfluencerCredited = 0 
		                    GROUP BY av.ArticleId, av.InfluencerUserId HAVING FLOOR((COUNT(av.Id))/1000) > 0
                        )
                        UPDATE ArticleViews SET ArticleViews.InfluencerCredited = 1
                        WHERE 
							ArticleViews.ArticleId IN (SELECT DISTINCT Article FROM CTE) AND 
							ArticleViews.InfluencerUserId IN (SELECT DISTINCT Influencer FROM CTE) AND 
							ArticleViews.Id IN (
	                        SELECT TOP 
							(SELECT Quantity*1000 FROM CTE WHERE ArticleViews.ArticleId = CTE.Article AND ArticleViews.InfluencerUserId = CTE.Influencer) 
							av.Id FROM ArticleViews av 
	                        WHERE av.ArticleId  IN (SELECT Article FROM CTE WHERE Article = ArticleViews.ArticleId) AND av.InfluencerUserId IN (SELECT Influencer FROM CTE WHERE Influencer = ArticleViews.InfluencerUserId) 
							ORDER BY av.ViewDate ASC
                        )
                        ";
            TableHelper.ExecuteRawCommandNonQuery(query);
        }

        private static string GetWritersCTE()
        {
            //Returns CTE table with: userId, amount (amount to be credited to user), state = current Main Balance state
            //We credit users per 1000 article reads
            return String.Format(@"
                    WITH CTE AS
                    (
	                    SELECT X.UserId AS [userId], (SUM(Quantity) * M.ArticleCreatorCPM) AS [amount], U.Balance1 AS [state] FROM
	                    (
		                    SELECT a.CreatorUserId AS UserId, av.ArticleId AS Article, FLOOR((COUNT(av.Id))/1000) AS Quantity FROM ArticleViews av 
		                    INNER JOIN Articles a ON av.ArticleId = a.Id 
		                    WHERE av.CreatorCredited = 0 
		                    GROUP BY av.ArticleId, a.CreatorUserId HAVING FLOOR((COUNT(av.Id))/1000) > 0
	                    ) AS X 
	                    JOIN Users U
	                    ON U.UserId = X.UserId
	                    JOIN Memberships M
	                    ON U.UpgradeId = M.MembershipId
	                    GROUP BY X.UserId, Quantity, U.Balance1, M.ArticleCreatorCPM
                    ) 
                    ");
        }

        private static string GetInfluencersCTE()
        {
            //Returns CTE table with: userId, amount (amount to be credited to user), state = current Main Balance state
            //We credit users per 1000 article reads
            return String.Format(@"
                    WITH CTE AS
                    (
	                    SELECT X.UserId AS [userId], (SUM(Quantity) * M.ArticleInfluencerCPM) AS [amount], U.Balance1 AS [state] FROM
	                    (
		                    SELECT av.InfluencerUserId AS UserId, av.ArticleId AS Article, FLOOR((COUNT(av.Id))/1000) AS Quantity FROM ArticleViews av 
		                    WHERE av.InfluencerUserId > 0 AND av.InfluencerCredited = 0 
		                    GROUP BY av.ArticleId, av.InfluencerUserId HAVING FLOOR((COUNT(av.Id))/1000) > 0
	                    ) AS X 
	                    JOIN Users U
	                    ON U.UserId = X.UserId
	                    JOIN Memberships M
	                    ON U.UpgradeId = M.MembershipId
	                    GROUP BY X.UserId, Quantity, U.Balance1, M.ArticleInfluencerCPM
                    ) 
                    ");
        }
    }


}