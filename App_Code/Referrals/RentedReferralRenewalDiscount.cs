using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Prem.PTC.Memberships;

/// <summary>
/// Summary description for RentedReferralRenewalDiscount
/// </summary>
public class RentedReferralRenewalDiscount : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "RentedReferralRenewalDiscounts"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("Days")]
    public int Days { get { return _Days; } set { _Days = value; SetUpToDateAsFalse(); } }

    [Column("DiscountPercent")]
    public decimal DiscountPercent { get { return _DiscountPercent; } set { _DiscountPercent = value; SetUpToDateAsFalse(); } }

    int _Id, _Days;
    decimal _DiscountPercent;

    private RentedReferralRenewalDiscount(int days, decimal discountPercent)
    {
        Days = days;
        DiscountPercent = discountPercent;
    }
    public RentedReferralRenewalDiscount(int id) : base(id) { }

    public RentedReferralRenewalDiscount(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    public static void Create(int days, decimal discountPercent)
    {
        if (TableHelper.GetListFromRawQuery<RentedReferralRenewalDiscount>(string.Format("SELECT * FROM RentedReferralRenewalDiscounts WHERE Days = {0};", days)).Count > 0)
            throw new MsgException(string.Format("Discount with {0} days already exists", days));

        var newRange = new RentedReferralRenewalDiscount(days, discountPercent);
        newRange.Save();
    }

    public static List<RentedReferralRenewalDiscount> GetAll()
    {
        return TableHelper.GetListFromRawQuery<RentedReferralRenewalDiscount>("SELECT * FROM RentedReferralRenewalDiscounts ORDER BY Days");
    }

    public static Money GetRenewalPrice(IMembership membership, RentedReferralRenewalDiscount d)
    {
        var price = Money.MultiplyPercent((d.Days / (decimal)30) * membership.ReferralRentCost, (100 - membership.RenewalDiscount - d.DiscountPercent));
        return price;
    }
    public static RentedReferralRenewalDiscount GetDefaultObject()
    {
        return new RentedReferralRenewalDiscount(30, 0);
    }
}