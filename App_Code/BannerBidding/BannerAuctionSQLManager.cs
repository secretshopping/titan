using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Text;
using Prem.PTC.Advertising;
using ExtensionMethods;

namespace Titan.Advertising
{
    public class BannerAuctionSQLManager
    {
        public static void ExecuteCRONSQL()
        {
            using (var bridge = ParserPool.Acquire(Database.Client))
            {
                //Delete old records
                bridge.Instance.ExecuteRawCommandNonQuery(
                    String.Format(@"DELETE FROM BannerBids WHERE BannerAuctionId IN (SELECT Id FROM BannerAuctions WHERE DateStart < '{0}')",
                    DateTime.Now.Add(-Titan.Advertising.BannerAuction.DeleteOldAfter).ToDBString()));

                bridge.Instance.ExecuteRawCommandNonQuery(
                   String.Format(@"DELETE FROM BannerAuctions WHERE DateStart < '{0}'",
                   DateTime.Now.Add(-Titan.Advertising.BannerAuction.DeleteOldAfter).ToDBString()));

                //Add new records
                //Executing in 800 batches for good performance

                var latestAuctionSql = bridge.Instance.ExecuteRawCommandToDataTable(
                   String.Format(@"SELECT TOP 1 * FROM BannerAuctions ORDER BY DateStart DESC"));
                var latestAuctionList = TableHelper.GetListFromDataTable<BannerAuction>(latestAuctionSql, 1);

                var now = DateTime.Now;
                now = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
                DateTime StartingDate = latestAuctionList.Count > 0 ? latestAuctionList[0].DateStart.Add(AppSettings.BannerAdverts.AuctionTime) : now;
          
                bool Active = true;

                while (Active)
                {
                    bool AreRecordsInQuery = false;

                    //Batch start
                    var Query = ConstructInsertQueryStart();
                    int i = 0;
                    while (StartingDate < now.AddDays(BannerAuctionManager.AuctionsGeneratedUpfrontDays) && i < 400)
                    {
                        AreRecordsInQuery = true;

                        var dimenstions = TableHelper.SelectAllRows<BannerAdvertDimensions>();
                        foreach(var dimension in dimenstions)
                        {
                            BannerAuction auction1 = new BannerAuction();
                            auction1.DateStart = StartingDate;
                            auction1.BannerType = dimension;
                            ConstructInsertQuery(ref Query, auction1);
                            i++;
                        }

                        StartingDate = StartingDate.Add(AppSettings.BannerAdverts.AuctionTime);
                    }

                    if (AreRecordsInQuery)
                    {
                        if (Query[Query.Length - 1] == ',')
                            Query[Query.Length - 1] = ';';

                        bridge.Instance.ExecuteRawCommandNonQuery(Query.ToString());
                    }
                    else
                        Active = false;
                }
            }
        }

        protected static StringBuilder ConstructInsertQueryStart()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"INSERT INTO BannerAuctions (DateStart, BannerType) VALUES");
            return sb;
        }

        protected static void ConstructInsertQuery(ref StringBuilder sb, BannerAuction Auction)
        {
            sb.Append(" ('")
              .Append(Auction.DateStart.ToDBString())
              .Append("', ")
              .Append(Auction.BannerType.Id)
              .Append(")");

            sb.Append(",");
        }

        public static string GetCurrentSQL(BannerAdvertDimensions dimensions)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT TOP 1 * FROM BannerAuctions WHERE DateStart >= '");
            sb.Append((DateTime.Now.AddMinutes(-DateTime.Now.Minute - 1)).ToDBString());
            sb.Append("' AND DATEADD(hour, ");
            sb.Append(AppSettings.BannerAdverts.AuctionTime.Hours);
            sb.Append(", DateStart) < '");
            sb.Append((DateTime.Now.AddMinutes(-DateTime.Now.Minute).AddHours(AppSettings.BannerAdverts.AuctionTime.Hours)).ToDBString());
            sb.Append("' AND BannerType = ");
            sb.Append(dimensions.Id);
            return sb.ToString();
        }

        public static string GetFirstActiveAuctionSQL(BannerAdvertDimensions dimensions)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT TOP 1 * FROM BannerAuctions WHERE DateStart <= '");
            sb.Append(DateTime.Now.Add(AppSettings.BannerAdverts.AuctionTime).Add(AppSettings.BannerAdverts.AuctionTime).ToDBString());
            sb.Append("' AND DATEADD(hour, ");
            sb.Append(AppSettings.BannerAdverts.AuctionTime.Hours);
            sb.Append(", DateStart) > '");
            sb.Append(DateTime.Now.Add(AppSettings.BannerAdverts.AuctionTime).Add(AppSettings.BannerAdverts.AuctionTime).ToDBString());
            sb.Append("' AND BannerType = ");
            sb.Append(dimensions.Id);
            return sb.ToString();
        }
    }
}