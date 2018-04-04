using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Memberships;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Titan.Pages
{
    public class UpgradePageHelper
    {
        public static string GetWarningMessage(Member User)
        {
            string result = String.Empty;

            if (!AppSettings.Points.LevelMembershipPolicyEnabled && User.Membership.Id != Membership.Standard.Id)
            {
                result = L1.WARNINGUPGRADE + ". ";

                if (AppSettings.Addons.IsProgressiveUpgradeEnabled)
                    result += U6006.PROGRESSIVEUPDATES;
            }

            return result;
        }

        public static ListItem[] GetMembershipPacks(Member User, ref Money FirstElementPrice)
        {
            List<ListItem> items = new List<ListItem>();
            foreach (MembershipPack pack in MembershipPack.AllPurchaseablePacks)
            {
                if (FirstElementPrice == Money.Zero)
                    FirstElementPrice = pack.GetPrice(User);

                items.Add(new ListItem(GetMembershipPackDescription(User,pack), pack.Id.ToString()));
            }
            return items.ToArray();
        }

        public static string GetMembershipPackDescription(Member User, MembershipPack membershipPack)
        {
            return membershipPack.Membership.Name + " (" + membershipPack.TimeDays.ToString() + " " + L1.DAYS + ") - " + membershipPack.GetPrice(User).ToString();
        }
    }
}