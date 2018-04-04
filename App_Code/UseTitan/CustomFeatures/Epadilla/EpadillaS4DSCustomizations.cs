using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.IO;
using Titan.CustomFeatures;
using Prem.PTC.Utils.NVP;
using Prem.PTC;
using MarchewkaOne.Titan.Balances;
using Prem.PTC.Advertising;
using Prem.PTC.Offers;

public class EpadillaS4DSCustomizations
{
    public static Member GetMember(MemberAuthenticationData authData)
    {
        var requestParams = new NVPStringBuilder()
               .Append("operation", "login")
               .Append("username", authData.Username)
               .Append("password", authData.PrimaryPassword)
               .Append("domain", "WS_INTEGRATION_ACCESS")
               .Build();

        var jsonResponse = S4DSAPI.SendRequest("rest/AuthService", requestParams);
        S4DSAuthInfo AuthInfo = new S4DSAuthInfo(jsonResponse);

        if (!AuthInfo.IsOK)
            throw new MsgException("Unable to log in using those credentials.");

        if (!Member.Exists(authData.Username))
        {
            //Register
            TitanRegisterService.Register(authData.Username, authData.Username, 1234,
            new DateTime(1980, 1, 1), authData.PrimaryPassword, String.Empty, Gender.Male, null, String.Empty,
            String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, true, false, false, null, true);
        }

        Member Result = new Member(authData.Username);
        S4DSAuthenticationInformation.AddOrUpdate(Result.Id, AuthInfo);

        Result.S4DSPackages = GetMemberPackages(AuthInfo);
        Result.SaveCustomFeatures();

        //if (Result.S4DSPackages == 0)
        //    throw new MsgException("You cannot login because you have 0 packages.");

        return Result;
    }

    public static int GetMemberPackages(S4DSAuthInfo info)
    {
        return GetMemberPackages(info.personId, info.token);
    }

    public static int GetMemberPackages(string personId, string token)
    {
        try
        {
            var requestParams = new NVPStringBuilder()
                .Append("personId", personId)
                .Append("token", token)
                .Build();

            var jsonResponse = S4DSAPI.SendRequest("ConsultarAsesor", requestParams);

            return Convert.ToInt32(jsonResponse["points"].ToString());
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            return 0;
        }
    }

    public static void CRON()
    {
        if (TitanFeatures.IsEpadilla)
        {
            //1. Send info about balances to S4DS
            var UsersToSend = TableHelper.GetListFromRawQuery<Member>("SELECT * FROM Users WHERE Balance1 > 0");

            foreach (var userToSend in UsersToSend)
            {
                try
                {
                    CreditS4DSPacks(userToSend);
                }
                catch (Exception ex)
                {
                    ErrorLogger.Log(ex);
                }
            }

            try
            {
                //2. Reset all balances to 0
                using (var parser = ParserPool.Acquire(Database.Client).Instance)
                {
                    string dtCommand = String.Format(@"
            SELECT 
	            U.UserId AS userId, 
                -1 * U.Balance1 AS amount, 
                0.00000000 AS state
            FROM Users U 
            WHERE 
                U.Balance1 > 0");

                    BalanceLogManager.AddRange(parser, dtCommand, "S4DS daily deduction", BalanceType.MainBalance, BalanceLogType.Other);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }

            TableHelper.ExecuteRawCommandNonQuery("UPDATE Users SET Balance1 = 0 WHERE Balance1 > 0");
        }
    }

    public static void OrderEntryService(S4DSAuthenticationInformation info, int packages)
    {
        var requestParams = new NVPStringBuilder()
                .Append("personId", info.personId)
                .Append("token", info.token)
                .Append("productSKU", "99999997FG00000")
                .Append("quantity", packages)
                .Append("catalogId", 2)
            .Build();

        var jsonResponse = S4DSAPI.SendRequest("rest/OrderEntryServices", requestParams);
    }

    public static void CreditS4DSPacks(Member user)
    {
        var AdPacksList = TableHelper.GetListFromRawQuery<AdPack>(String.Format("SELECT * FROM AdPacks WHERE UserId={0} AND MoneyReturned < MoneyToReturn", user.Id));

        if (AdPacksList == null)
            return;

        int AmountOfActivePacks = AdPacksList.Count();

        Money CashPerPack = user.MainBalance / (Decimal)AmountOfActivePacks;

        if (CashPerPack > new Money(2))
            CashPerPack = new Money(2);

        foreach(AdPack availableAdPack in AdPacksList)
        {
            availableAdPack.MoneyReturned += CashPerPack;

            if (availableAdPack.MoneyReturned >= availableAdPack.MoneyToReturn)
                availableAdPack.PurchaseDate = DateTime.Now.Date;

            availableAdPack.Save();
        }
    }

    public static void AddS4DSPack(Member user, int amountOfPacks)
    {
        for (int i = 0; i < amountOfPacks; i++)
        {
            var newS4DSPack = new AdPack();

            newS4DSPack.MoneyToReturn = Money.Parse("120");
            newS4DSPack.UserId = user.Id;

            newS4DSPack.PurchaseDate = DateTime.Now;
            newS4DSPack.MoneyReturned = Money.Zero;
            newS4DSPack.UserCustomGroupId = -1;
            newS4DSPack.AdPackTypeId = -1;

            newS4DSPack.Save();
        }
    }
}




