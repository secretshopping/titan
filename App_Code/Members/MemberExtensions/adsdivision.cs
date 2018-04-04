using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        public bool IsBronze
        {
            get
            {
                if (this.TotalDirectReferralsEarned >= new Money(500))
                    return true;
                return false;
            }
        }

        public bool IsSilver
        {
            get
            {
                if (this.TotalDirectReferralsEarned >= new Money(1500))
                    return true;
                return false;
            }
        }

        public bool IsGold
        {
            get
            {
                if (this.TotalDirectReferralsEarned >= new Money(5000))
                    return true;
                return false;
            }
        }

        public Money GetDailyLimit()
        {
            if (IsGold)
                return new Money(2500);

            if (IsGold)
                return new Money(1000);

            if (IsGold)
                return new Money(250);

            return new Money(150);
        }

        public int GetLeaderBonusPercent()
        {
            int closedGroup = CustomGroupManager.GetUsersHighestExpiredGroupNumber(this.Id);

            if (closedGroup == 1)
                return 6;

            if (closedGroup == 2)
                return 7;

            if (closedGroup == 3)
                return 8;

            if (closedGroup == 4)
                return 9;

            if (closedGroup == 5)
                return 10;

            return 5; //default
        }

        public string GetLeaderColor()
        {
            int closedGroup = CustomGroupManager.GetUsersHighestExpiredGroupNumber(this.Id);

            if (closedGroup == 1)
                return "#70981a";

            if (closedGroup == 2)
                return "#1fa5d7";

            if (closedGroup == 3)
                return "#cfaf1a";

            if (closedGroup == 4)
                return "#c63522";

            if (closedGroup == 5)
                return "#b123c1";

            return "white"; //default
        }

        public static void CheckFinishedLeader()
        {
            var leaderGroups = TableHelper.GetListFromRawQuery<UserCustomGroup>("SELECT * FROM UserCustomGroups WHERE Status = 3 AND GotBonus = 0");
            foreach (var leaderGroup in leaderGroups)
            {
                Money returned = GetTotalCustomGroupsEarningsNoOwner(leaderGroup.Id, leaderGroup.CreatorUserId);
                Member Creator = new Member(leaderGroup.CreatorUserId);

                //leaderGroup.GotBonus = true;
                leaderGroup.Save();

                Creator.AddToMainBalance(Money.MultiplyPercent(returned, Creator.GetLeaderBonusPercent()), "Leader bonus");
                Creator.SaveBalances();
            }
        }

        public static Money GetTotalCustomGroupsEarningsNoOwner(int userCustomGroupId, int ownerId)
        {
            var adPackList = AdPackManager.GetAllAdPacksInCustomGroup(userCustomGroupId);
            Money earnings = Money.Zero;
            foreach (var adPack in adPackList)
            {
                if (adPack.UserId != ownerId)
                    earnings += adPack.MoneyReturned;
            }
            return earnings;
        }
    }
}