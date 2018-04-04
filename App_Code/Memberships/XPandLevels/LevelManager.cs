using Prem.PTC;
using Prem.PTC.Members;

public class LevelManager
{
    public static void PointsChanged(Member user, bool setMinPoints = true, bool triggerActions = true)
    {
        if (AppSettings.Points.LevelMembershipPolicyEnabled)
        {
            var properMembership = LevelMemberships.Get(user.PointsBalance);

            if (user.MembershipId != properMembership.Id)
            {
                string oldMembershipName = user.Membership.Name;

                var notification = new LevelNotification
                {
                    UserId = user.Id
                };

                if (user.Membership.MinPointsToHaveThisLevel <= user.PointsBalance)
                {
                    user.Upgrade(properMembership);
                    History.AddLevelUp(user.Name, user.Membership.Name);
                    if (triggerActions)
                    {
                        //Add random levelup prize
                        LevelupManager manager = new LevelupManager(user);
                        manager.AddPrize(ref notification);                       
                    }

                    notification.IsUpgrade = true;
                }
                else
                {
                    user.Downgrade(properMembership, setMinPoints);

                    var membershipName = user.Membership.Name;
                    History.AddLevelExpiration(user.Name, membershipName);
                    notification.IsUpgrade = false;
                }

                notification.MembershipName = properMembership.Name;
                notification.IsDisplayed = false;
                notification.Save();
            }
        }
    }

    public static int NextLevelValue(int points)
    {
        var next = LevelMemberships.GetNext(points);

        if (next == null)
            return LevelMemberships.Get(points).MinPointsToHaveThisLevel;

        return next.MinPointsToHaveThisLevel;
    }
}
