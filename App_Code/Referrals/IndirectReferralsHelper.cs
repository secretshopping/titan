using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan;
using Prem.PTC;
using Prem.PTC.Members;
using System.Text;
using ExtensionMethods;

public class IndirectReferralsHelper
{
    private Member LoggedUser;
    private List<KeyValuePair<Member,Member>> fullList;
    public Dictionary<int, int> HowManyOnEachTier = null;
    public Dictionary<int, int> HowManyEverUpgradedOnEachTier = null;
    private Money TotalDeposits = Money.Zero;
    public Dictionary<int, StringBuilder> UsernamesOnEachTier = null;

    public IndirectReferralsHelper(Member User, bool forLeadership = false)
    {
        LoggedUser = User;

        HowManyOnEachTier = new Dictionary<int, int>();
        UsernamesOnEachTier = new Dictionary<int, StringBuilder>();
        HowManyEverUpgradedOnEachTier = new Dictionary<int, int>();
        fullList = new List<KeyValuePair<Member, Member>>();

        for (int i = 1; i <= 10; ++i)
        {
            HowManyOnEachTier.Add(i, 0);
            UsernamesOnEachTier.Add(i, new StringBuilder());
            HowManyEverUpgradedOnEachTier.Add(i, 0);
        }
        _GetIndirectReferralsCountForMember(LoggedUser, 1, AppSettings.Referrals.ReferralEarningsUpToTier, forLeadership);
    }

    public List<KeyValuePair<Member, Member>> GetAllIndirectReferralsForGraph()
    {
        return fullList;
    }

    public int GetIndirectReferralsCountForMember()
    {
        int counter = 0;
        for (int i = 2; i <= AppSettings.Referrals.ReferralEarningsUpToTier; ++i)
            counter += HowManyOnEachTier[i];
        return counter;
    }

    private void _GetIndirectReferralsCountForMember(Member User, int startDepth, int MaxDepth, bool forLeadership = false)
    {
        if (startDepth > MaxDepth)
        {
            return;
        }
        List<Member> list = null;
        if (!forLeadership)
            list = User.GetDirectReferralsList();
        else
            list = User.DirectReferralsWithHighestMembership;

        for (int i = 0; i < list.Count; i++)
        {
            fullList.Add(new KeyValuePair<Member, Member>(User, list[i]));
            HowManyOnEachTier[startDepth]++;
            UsernamesOnEachTier[startDepth].Append(list[i].Name + ", ");

            if(AppSettings.Payments.RefTiersMaxWeeklyPayoutEnabled && list[i].HasEverUpgraded)
            {
                HowManyEverUpgradedOnEachTier[startDepth]++;
            }

            _GetIndirectReferralsCountForMember(list[i], startDepth + 1, MaxDepth, forLeadership);
        }

        return;
    }

    public Money GetTotalIndirectReferralsDeposit(Member User, int startDepth, int MaxDepth, DateTime startDate, DateTime endDate)
    {

        if (startDepth > MaxDepth)
        {
            return TotalDeposits;
        }

        List<Member> list = User.DirectReferralsWithHighestMembership;
        string query = string.Format("SELECT SUM(Amount) FROM CompletedTransactions WHERE WhenMade >= {0} AND WhenMade < {1} AND UserId = {2};", startDate.ToShortDateDBString(), endDate.ToShortDateDBString(), @"'{0}'");
        foreach (Member elem in list)
        {
            try
            {
                TotalDeposits += new Money(Convert.ToDecimal(TableHelper.SelectScalar(string.Format(query, elem.Id))));
            }
            catch (InvalidCastException) { }
            GetTotalIndirectReferralsDeposit(elem, startDepth + 1, MaxDepth, startDate, endDate);
        }

        return TotalDeposits;
    }
}