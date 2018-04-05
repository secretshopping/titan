using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RentedReferralRangePrice
/// </summary>
public class RentedReferralRangePrice : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "RentedReferralRangePrices"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("MinReferrals")]
    public int MinReferrals { get { return _MinReferrals; } set { _MinReferrals = value; SetUpToDateAsFalse(); } }

    [Column("MaxReferrals")]
    public int MaxReferrals { get { return _MaxReferrals; } set { _MaxReferrals = value; SetUpToDateAsFalse(); } }


    [Column("PricePerReferralEnlargedBy")]
    public Money PricePerReferralEnlargedBy { get { return _PricePerReferralEnlargedBy; } set { _PricePerReferralEnlargedBy = value; SetUpToDateAsFalse(); } }


    [Column("AutopayPriceEnlargedBy")]
    public Money AutopayPriceEnlargedBy { get { return _AutopayPriceEnlargedBy; } set { _AutopayPriceEnlargedBy = value; SetUpToDateAsFalse(); } }


    int _Id, _MinReferrals, _MaxReferrals;
    Money _PricePerReferralEnlargedBy, _AutopayPriceEnlargedBy;

    private RentedReferralRangePrice(int minReferrals, int maxReferrals, Money pricePerReferralEnlargedBy, Money autopayPriceEnlargedBy)
    {
        MinReferrals = minReferrals;
        MaxReferrals = maxReferrals;
        PricePerReferralEnlargedBy = pricePerReferralEnlargedBy;
        AutopayPriceEnlargedBy = autopayPriceEnlargedBy;
    }
    public RentedReferralRangePrice(int id) : base(id) { }

    public RentedReferralRangePrice(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    public static void Create(int minReferrals, int maxReferrals, Money pricePerReferralEnlargedBy, Money autopayPriceEnlargedBy)
    {
        if (minReferrals >= maxReferrals)
            throw new MsgException("Max Referrals must be greater than MinReferrals");

        var allRanges = GetAll();
        if (allRanges.Any(x => x.MinReferrals == minReferrals && x.MaxReferrals == maxReferrals))
            throw new MsgException("Range already exists");


        var newRange = new RentedReferralRangePrice(minReferrals, maxReferrals, pricePerReferralEnlargedBy, autopayPriceEnlargedBy);
        newRange.Save();
    }

    private static List<RentedReferralRangePrice> GetAll()
    {
        return TableHelper.SelectAllRows<RentedReferralRangePrice>();
    }

    private static RentedReferralRangePrice GetRange(int numberOfReferrals)
    {
        string query = string.Format("SELECT TOP 1 * FROM RentedReferralRangePrices WHERE MinReferrals <= {0} AND MaxReferrals >= {0}", numberOfReferrals);
        var range = TableHelper.GetListFromRawQuery<RentedReferralRangePrice>(query).FirstOrDefault();
        return range;
    }

    public static Money GetPriceForSingleRef(Money basePrice, int numberOfReferrals)
    {
        var range = GetRange(numberOfReferrals);
        if (range == null)
            return basePrice;

        return basePrice + range.PricePerReferralEnlargedBy;
    }

    public static Money GetPriceForAutopay(Money basePrice, int numberOfReferrals)
    {
        if (AppSettings.Referrals.RentedRefAutopayPolicy == AppSettings.Referrals.AutopayPolicy.UserChooses)
            return basePrice;

        var range = GetRange(numberOfReferrals);

        if (range == null)
            return basePrice;

        return basePrice + range.AutopayPriceEnlargedBy;
    }

}