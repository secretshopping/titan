using Prem.PTC;
using Prem.PTC.Contests;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Titan;
using ExtensionMethods;
using Prem.PTC.Members;

public static class ForumContestHelper
{
    public static List<ContestParticipant> GetForumParticipants(Contest contest)
    {

        try
        {
            var users = TableHelper.GetListFromQuery<Member>(@"WHERE Users.AccountStatusInt = 9 
                                                                    OR Users.AccountStatusInt = 1 
                                                                    OR Users.AccountStatusInt = 10");
            var participants = new List<string>();

            foreach (Member user in users)
            {
                if (contest.CanMemberParticipate(user, true))
                    participants.Add(user.Name);
            }

            DataTable result;
            using (var bridge = ParserPool.Acquire(Database.Forum))
            {
                result = bridge.Instance.ExecuteRawCommandToDataTable(@"select COUNT(m.MessageID), us.Name from yaf_Message m 
                                                                            INNER JOIN yaf_Topic t ON t.TopicID = m.TopicID 
                                                                            INNER JOIN yaf_User us ON us.UserID = m.UserID
                                                                            INNER JOIN yaf_Forum f ON f.ForumID = t.ForumID 
                                                                            WHERE m.IsDeleted = 0 
                                                                            AND m.IsApproved = 1 
                                                                            AND m.Posted > '" + contest.DateStart.ToDBString() +
                                                                            "' AND m.Posted < '" + contest.DateEnd.ToDBString() +
                                                                            "' AND f.ForumID in (select ForumId from ForumContests where ContestId = " + contest.Id + @") 
                                                                            GROUP BY us.Name;");
            }

            List<ContestParticipant> list = new List<ContestParticipant>(result.Rows.Count);

            foreach (DataRow row in result.Rows)
            {
                ContestParticipant participant = new ContestParticipant();
                participant.Username = (string)row[1];

                if (participants.Contains(participant.Username))
                {
                    participant.Points = new Money(Convert.ToDecimal(row[0]));
                    participant.LatestAction = DateTime.Now.Zero();
                    participant.ContestId = contest.Id;

                    list.Add(participant);
                }
            }



            return list;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public static Dictionary<int, string> GetForumsIncludedInContest(int contestId)
    {

        DataTable forumNamesTable = new DataTable();
        Dictionary<int, string> forumNames = new Dictionary<int, string>();

        using (var bridge = ParserPool.Acquire(Database.Forum))
        {
            forumNamesTable = bridge.Instance.ExecuteRawCommandToDataTable(String.Format("SELECT f.Name, f.ForumID from yaf_Forum f " +
            "INNER JOIN ForumContests fc ON f.ForumID = fc.ForumId WHERE fc.ContestId ={0}", contestId));
        }

        foreach (DataRow row in forumNamesTable.Rows)
        {
            forumNames.Add(Convert.ToInt32(row["ForumID"]), row["Name"].ToString());
        }
        return forumNames;
    }

    public static string GetForumsIncludedInContest(Dictionary<int, string> forums)
    {
        string forumNames = string.Empty;
        foreach (var forum in forums)
        {
            forumNames += forum.Value + ", ";

        }
        return forumNames;
    }


}