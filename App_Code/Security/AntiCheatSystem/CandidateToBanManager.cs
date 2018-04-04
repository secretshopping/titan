using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Members;

public class CandidateToBanManager
{
    public static void Ban(int recordId)
    {
        CandidateToBan target = new CandidateToBan(recordId);

        Member Recur = new Member(target.UserId);
        Recur.BanCheater(target.Note);
        Recur.SaveStatus();

        target.Delete();
    }

    public static void NotBan(int recordId)
    {
        CandidateToBan target = new CandidateToBan(recordId);
        target.IsWhitelisted = true;
        target.Save();
    }

    public static void TryAdd(int userId, string note)
    {
        if (!CandidateToBanManager.IsWhiteListedOrDuplicated(userId, note))
        {
            CandidateToBan Newly = new CandidateToBan();
            Newly.UserId = userId;
            Newly.DateOccured = DateTime.Now;
            Newly.Note = note;
            Newly.Type = AntiCheatRuleType.SameIPAddress; //Always the same
            Newly.IsWhitelisted = false;
            Newly.Save();
        }
    }

    public static bool IsWhiteListedOrDuplicated(int userId, string note)
    {
        int count = Convert.ToInt32(TableHelper.SelectScalar("SELECT COUNT(*) FROM CandidatesToBan WHERE UserId = " + userId + " AND Note = '" + note + "'"));
        return count > 0;
    }
}