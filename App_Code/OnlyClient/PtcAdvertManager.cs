using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PtcAdvertManager
/// </summary>
public static class PtcAdvertManager
{
    public static Money GetDiscountedPTCPackPrice(PtcAdvertPack pack)
    {
        Money price = pack.Price;
        int pricePercentage = 100;

        //Cutom pack price dependable on number of  members
        if (TitanFeatures.isSatvetErturkmen && AppSettings.PtcAdverts.DynamicPTCPriceEnabled)
        {
            Money PricePer1000views = AppSettings.PtcAdverts.BasePricePer1000ViewsPtc +
                new Money((Decimal)AppSettings.TotalMembers / (Decimal)AppSettings.PtcAdverts.DecimalPricePer1000ViewsPtc);
            price = new Money((Convert.ToDecimal(pack.Ends.Value) / (Decimal)1000)) * PricePer1000views;
        }

        if (AppSettings.PtcAdverts.RegistrationDiscountEnabled && Member.IsLogged)
        {
            var discountStarts = AppSettings.PtcAdverts.RegistrationDiscountStartDate;
            var discountEnds = AppSettings.PtcAdverts.RegistrationDiscountStartDate.AddDays(AppSettings.PtcAdverts.RegistrationDiscountDays);
            var now = AppSettings.ServerTime;
            var userRegistered = Member.CurrentInCache.Registered;

            if (now >= discountStarts && now <= discountEnds && userRegistered >= discountStarts && userRegistered <= discountEnds)
                pricePercentage = 100 - (AppSettings.PtcAdverts.RegistrationDiscountValue < 100 ? AppSettings.PtcAdverts.RegistrationDiscountValue : 0);
        }

        return Money.MultiplyPercent(price, pricePercentage);
    }

    public static int PackComparision(PtcAdvertPack x, PtcAdvertPack y)
    {
        if ((int)x.Ends.EndMode < (int)y.Ends.EndMode)
            return 1;
        else if ((int)x.Ends.EndMode == (int)y.Ends.EndMode)
        {
            if (x.Price > y.Price)
                return 1;
            if (x.Price == y.Price)
                return 0;

            return -1;
        }
        return -1;
    }

}