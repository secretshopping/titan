using Prem.PTC;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class RegistrationGiftCrediter
{
    public static void CreditGiftToUser(Member newUser)
    {
        const int AmountOfUsersSystemStart = 5;
        const int AmountOfUsersToCredit = 5;
        Money RegistrationGift = Money.Parse("3");

        if (TitanFeatures.IsNightwolf)
        {
            int AmountOfUsersInSystem = (int)TableHelper.SelectScalar("SELECT COUNT(*) FROM Users");
            if (AmountOfUsersInSystem <= AmountOfUsersSystemStart + AmountOfUsersToCredit)
            {
                Member TargetUser = new Member(newUser.Id);
                TargetUser.AddToCashBalance(RegistrationGift, "Account Registration Bonus", BalanceLogType.RegistrationBonus);
                TargetUser.Save();
            }
                
        }
    }
}