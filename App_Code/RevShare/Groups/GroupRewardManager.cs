using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan;
using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Advertising;

public class GroupRewardManager
{
    public static void Apply(CustomGroup type, UserCustomGroup userCustomGroup)
    {
        //Temporary (for better performance)
        List<AdPackType> typesList = TableHelper.SelectAllRows<AdPackType>();
        AdPack creatorAdPack = null;
        var types = new Dictionary<int, AdPackType>();
        foreach (var typeList in typesList)
            types.Add(typeList.Id, typeList);

        Random random = new Random();
        List<int> adPackIds = new List<int>();

        //Value of purchases
        Money memberPurchasesAmount = Money.Zero;
        Money creatorPurchasesAmount = Money.Zero;
        var adpacks = AdPackManager.GetAllAdPacksInCustomGroup(userCustomGroup.Id);

        foreach (var adpack in adpacks)
            if (adpack.UserId != userCustomGroup.CreatorUserId)
            {
                memberPurchasesAmount += types[adpack.AdPackTypeId].Price;
                adPackIds.Add(adpack.Id);
            }
            else
                creatorPurchasesAmount += types[adpack.AdPackTypeId].Price;

        //Reward for creator
        var creatorReward = Money.MultiplyPercent(memberPurchasesAmount, type.CreatorRewardBonusPercent);
        Member GroupCreator = new Member(userCustomGroup.CreatorUserId);
        GroupCreator.AddToPurchaseBalance(creatorReward, "Custom Group creator bonus");
        GroupCreator.SaveBalances();

        //Prize1   
        var prize1Reward = Money.MultiplyPercent(memberPurchasesAmount, type.FirstRewardPercent);
        int wonPrize1Ticket = random.Next(0, adPackIds.Count);
        Member Prize1Winner = new Member((new AdPack(adPackIds[wonPrize1Ticket])).UserId);
        Prize1Winner.AddToPurchaseBalance(prize1Reward, "Custom Group 1st prize");
        Prize1Winner.SaveBalances();

        //Prize2
        var prize2Reward = Money.MultiplyPercent(creatorPurchasesAmount, type.SecondRewardPercent);
        int wonPrize2Ticket = random.Next(0, adPackIds.Count);
        Member Prize2Winner = new Member((new AdPack(adPackIds[wonPrize2Ticket])).UserId);
        Prize2Winner.AddToPurchaseBalance(prize2Reward, "Custom Group 2nd prize");
        Prize2Winner.SaveBalances();

        userCustomGroup.BonusExtraInformation =
            String.Format("Creator username: <b>{0}</b>, reward: <b>{1}</b>, 1st prize winner: <b>{2}</b> ({3}), 2nd prize winner: <b>{4}</b> ({5}).",
            GroupCreator.Name, creatorReward.ToString(), Prize1Winner.Name, prize1Reward.ToString(), Prize2Winner.Name, prize2Reward.ToString());

    }
}