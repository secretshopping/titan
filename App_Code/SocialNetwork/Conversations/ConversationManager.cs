using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtensionMethods;

namespace SocialNetwork
{
    public static class ConversationManager
    {
        public static int GetEscrowTimeLeft(DateTime timeOfCreation)
        {
            TimeSpan TimeLeft = AppSettings.ServerTime - timeOfCreation;
            int MinutesLeft = AppSettings.Representatives.RepresentativesEscrowTime - Convert.ToInt32(TimeLeft.TotalMinutes);
            return (MinutesLeft > 0) ? MinutesLeft : 0;
        }

        public static String GetHtmlEscrowTimeLeft(DateTime timeOfCreation)
        {
            return HtmlCreator.GetColoredTime(GetEscrowTimeLeft(timeOfCreation));
        }

        public static void CRON()
        {
            if(AppSettings.Representatives.RepresentativesHelpDepositEnabled || AppSettings.Representatives.RepresentativesHelpWithdrawalEnabled)
            {
                DateTime AllowedDateOfCreation = AppSettings.ServerTime.AddMinutes(-AppSettings.Representatives.RepresentativesEscrowTime);

                //Make all representative deposits/withdrawals where escrow timed out as disputes
                TableHelper.ExecuteRawCommandNonQuery(String.Format(@"UPDATE ConversationMessages SET RepresentativeRequestStatus={0} WHERE
                                                                    MessageType IN ({1},{2}) AND
                                                                    RepresentativeRequestStatus = {3} AND
                                                                    DateTime < '{4}'",
                                                                        (int)RepresentativeRequestStatus.InDispute,
                                                                        (int)MessageType.RepresentativeDepositRequest,
                                                                        (int)MessageType.RepresentativeWithdrawalRequest,
                                                                        (int)RepresentativeRequestStatus.Pending,
                                                                        AllowedDateOfCreation.ToDBString()));
            }
        }
    }
}