using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;

namespace Titan
{

    public class ShoutboxManager
    {
        private const int HOW_MANY_MESSAGES_KEPT = 50;

        private static string REMOVE_MESSAGES_SQL = "DELETE FROM ShoutboxMessages WHERE Id NOT IN (SELECT TOP " + HOW_MANY_MESSAGES_KEPT +
                                                            " Id FROM ShoutboxMessages WHERE IsDeleted = 0 ORDER BY SentDate DESC)";
        private static string REMOVE_OUTDATED_BANS = "DELETE FROM ShoutboxBannedUsernames WHERE BannedUntil < GETDATE()";
        
        public static void CRON()
        {
            if (AppSettings.Shoutbox.DisplayMode != ShoutboxDisplayMode.Disabled)
            {
                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    var parser = bridge.Instance;

                    parser.ExecuteRawCommandNonQuery(REMOVE_MESSAGES_SQL);
                    parser.ExecuteRawCommandNonQuery(REMOVE_OUTDATED_BANS);
                }
            }
        }

        public static List<IShoutboxContent> GetLatestRecords(int Count = HOW_MANY_MESSAGES_KEPT)
        {
            var results = new List<IShoutboxContent>();

            results.AddRange(ShoutboxMessage.GetLatestRecords(Count));

            if (AppSettings.Shoutbox.DisplayContent == ShoutboxDisplayContent.ChatAndEvents
                || AppSettings.Shoutbox.DisplayContent == ShoutboxDisplayContent.ChatAndEventsInSeparateTabs)
                results.AddRange(History.GetLatestRecords(Count));

            return results;
        }
    }
}