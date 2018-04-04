using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Titan.API
{
    public class ApiJackpot
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public ApiMoney ticketPrice { get; set; }
        public string prize { get; set; }
        public int numberOfWinningTickets { get; set; }
        public int participats { get; set; }
        public int allTickets { get; set; }
        public int yourTickets { get; set; }

        public ApiJackpot(Jackpot jackpot, int userId)
        {
            id = jackpot.Id;
            title = jackpot.Name;
            startDate = jackpot.StartDate;
            endDate = jackpot.EndDate;
            ticketPrice = new ApiMoney(jackpot.TicketPrice);
            prize = GetPrizeText(jackpot);

            participats = jackpot.NumberOfParticipants.HasValue ? jackpot.NumberOfParticipants.Value : 0;
            allTickets = jackpot.NumberOfTickets.HasValue ? jackpot.NumberOfTickets.Value : 0;
            yourTickets = JackpotManager.GetNumberOfUsersTickets(jackpot.Id, userId);
            numberOfWinningTickets = 2; //Temp
        }

        private string GetPrizeText(Jackpot jackpot)
        {
            StringBuilder sb = new StringBuilder();

            if (jackpot.AdBalancePrizeEnabled)
            {
                sb.Append("Purchase Balance: ");
                sb.Append(jackpot.AdBalancePrize.ToString());
                sb.Append(",");
            }

            if (jackpot.MainBalancePrizeEnabled)
            {
                sb.Append("Main Balance: ");
                sb.Append(jackpot.MainBalancePrize.ToString());
                sb.Append(",");
            }

            if (jackpot.LoginAdsCreditsPrizeEnabled)
            {
                sb.Append("Login Ads: ");
                sb.Append(jackpot.LoginAdsCreditsPrize.ToString());
                sb.Append(",");
            }

            if (jackpot.UpgradePrizeEnabled)
            {
                sb.Append("Upgrade: ");
                var membership = new Membership(jackpot.UpgradeIdPrize);
                sb.Append(membership.Name + " (" + jackpot.UpgradeDaysPrize + " days");
                sb.Append(",");
            }

            return sb.ToString().TrimEnd(',');
        }
    }
}