using Prem.PTC.Members;
using System;

public class TotalMembersCache : GlobalStatisticsCacheBase
{
    protected override string Name { get { return "TotalMembers"; } }
    protected override object GetDataFromSource()
    {
        var query = string.Format("SELECT COUNT(UserId) FROM Users WHERE AccountStatusInt != {0}", (int)MemberStatus.Cancelled);
        return (Int32)TableHelper.SelectScalar(query);
    }
}