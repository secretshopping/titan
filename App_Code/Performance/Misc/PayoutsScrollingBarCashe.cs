using Prem.PTC.Members;
using Prem.PTC.Payments;
using System;
using System.Text;
using System.Web.Caching;

public class PayoutsScrollingBarCashe : CacheBase
{
    protected override string Name { get { return "PayoutsScrollingBarCashe"; } }
    protected override TimeSpan KeepCacheFor { get { return TimeSpan.FromMinutes(3); } }
    protected override CacheItemPriority Priority { get { return CacheItemPriority.BelowNormal; } }
    protected override CacheItemRemovedCallback RemovedCallback { get { return null; } }
    protected override object GetDataFromSource()
    {
        //ASC because we add to sb(last one will be first one)
        var query = @"SELECT TOP 10 * FROM [PaymentProofs] ORDER BY [Date] DESC";
        var proofsList = TableHelper.GetListFromRawQuery<PaymentProof>(query);
        StringBuilder text = new StringBuilder();
        for (int i = 0; i < proofsList.Count; i++)
        {
            var user = new Member(proofsList[i].UserId);
            text.Append(user.Name);
            text.Append(string.Format(" (<img src=\"Images/Flags/{0}.png\" class=\"imagemiddle\" />{1})", user.CountryCode.ToLower(), user.Country));
            text.Append(string.Format("<span>{0}</span>", proofsList[i].Amount.ToString()));
        }

        return text;
    }
}