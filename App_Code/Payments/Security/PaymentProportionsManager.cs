using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PaymentProportionsManager
{
    private Member user;

    public PaymentProportionsManager(Member _user)
    {
        this.user = _user;
    }

    /// <summary>
    /// Get payout maxiumum amount based on the proportions
    /// </summary>
    /// <param name="processor"></param>
    /// <returns></returns>
    public Money GetMaximum(PaymentProcessor processor)
    {
        var allProportions = GetAllProportions();

        //Check if we have some records
        if (allProportions.Count == 0)
            return new Money(2000000000); //no limit

        var proportion = GetProportion(allProportions, processor);
        if (proportion == null)
            return Money.Zero;

        //We have records
        decimal totalIn = proportion.TotalIn.ToDecimal();
        decimal totalInSum = allProportions.Sum(elem => elem.TotalIn.ToDecimal());

        if (totalInSum == 0)
            return new Money(2000000000);

        int percent = Convert.ToInt32((totalIn / totalInSum) * 100);
        Money maximum = Money.MultiplyPercent(user.TotalEarned - proportion.TotalOut, percent);

        return maximum;
    }

    /// <summary>
    /// Run after the member paid in the money
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="amount"></param>
    private void PaidIn(Money amount, PaymentProcessor processor)
    {
        var allProportions = GetAllProportions();
        PaymentProportions proportion = GetProportion(allProportions, processor);
        if (proportion == null)
        {
            proportion = new PaymentProportions();
            proportion.TotalIn = amount;
            proportion.TotalOut = Money.Zero;
            proportion.UserId = user.Id;
            proportion.Processor = processor;
        }
        else
            proportion.TotalIn += amount;

        proportion.Save();
    }

    /// <summary>
    /// Run after the member paid out the money
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="amount"></param>
    private void PaidOut(Money amount, PaymentProcessor processor)
    {
        var allProportions = GetAllProportions();
        PaymentProportions proportion = GetProportion(allProportions, processor);
        if (proportion == null)
        {
            proportion = new PaymentProportions();
            proportion.TotalIn = Money.Zero;
            proportion.TotalOut = amount;
            proportion.UserId = user.Id;
            proportion.Processor = processor;
        }
        else
            proportion.TotalOut += amount;

        proportion.Save();
    }

    private void UndoPaidOut(Money amount, PaymentProcessor processor)
    {
        var proportions = GetProportions(processor);
        if (proportions.Count == 1)
        {
            if (proportions[0].TotalOut - amount > Money.Zero)
                proportions[0].TotalOut -= amount;
            else
                proportions[0].TotalOut = Money.Zero;

            proportions[0].Save();
        }
    }

    /// <summary>
    /// Returns proportions for particular processor
    /// </summary>
    /// <param name="processor"></param>
    /// <returns>Proportions for particular user</returns>
    private List<PaymentProportions> GetProportions(PaymentProcessor processor)
    {
        var where = TableHelper.MakeDictionary("Processor", (int)processor);
        where.Add("UserId", user.Id);
        return TableHelper.SelectRows<PaymentProportions>(where);
    }

    private PaymentProportions GetProportion(List<PaymentProportions> allProportions, PaymentProcessor processor)
    {
        var proportion = from a in allProportions where a.Processor == processor select a;
        return proportion.FirstOrDefault();
    }

    public List<PaymentProportions> GetAllProportions()
    {
        var query = string.Format("SELECT * FROM PaymentProportions WHERE UserId = {0} AND TotalIn != 0 AND TotalOut != 0", user.Id);
        return TableHelper.GetListFromRawQuery<PaymentProportions>(query);
    }

    public Money TotalPaidIn
    {
        get
        {
            Money result = Money.Zero;
            var list = GetAllProportions();

            foreach (var elem in list)
                result += elem.TotalIn;

            return result;
        }
    }

    public Money TotalPaidOut
    {
        get
        {
            Money result = Money.Zero;
            var list = GetAllProportions();

            foreach (var elem in list)
                result += elem.TotalOut;

            return result;
        }
    }

    public int GetPercentage(PaymentProcessor processor)
    {
        var allProportions = GetAllProportions();

        var proportion = GetProportion(allProportions, processor);
        if (proportion == null)
            return 0;

        //We have records
        decimal totalIn = proportion.TotalIn.ToDecimal();
        decimal totalInSum = allProportions.Sum(elem => elem.TotalIn.ToDecimal());
        if (totalInSum == 0)
            return 100;

        int percent = Convert.ToInt32((totalIn / totalInSum) * 100);
        return percent;
    }

    public static void MemberPaidOut(Money amount, PaymentProcessor processor, Member user, bool IsCustomPayoutProcessor = false)
    {
        if (amount == Money.Zero) //we don't want add 0 to tables
            return;

        if (!IsCustomPayoutProcessor)
        {
            PaymentProportionsManager ppm = new PaymentProportionsManager(user);
            ppm.PaidOut(amount, processor);
        }
    }

    public static void MemberPaidIn(Money amount, PaymentProcessor processor, Member user)
    {
        if (amount == Money.Zero) //we don't want add 0 to tables
            return;

        PaymentProportionsManager ppm = new PaymentProportionsManager(user);
        ppm.PaidIn(amount, processor);
    }

    public static void UndoMemberPaidOut(Money amount, PaymentProcessor processor, Member user)
    {
        if (amount == Money.Zero) //we don't want add 0 to tables
            return;

        PaymentProportionsManager ppm = new PaymentProportionsManager(user);
        ppm.UndoPaidOut(amount, processor);
    }
}