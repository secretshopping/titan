
using Prem.PTC;
using System;

public class ForumHelper
{
    public enum BDProperty { Email, Name, BaseURL }

    public static void UpdateForumDB(BDProperty property, string value)
    {
        try
        {
            using (var bridge = ParserPool.Acquire(Database.Forum))
            {
                string command = "";

                switch (property)
                {
                    case BDProperty.Email:
                        command = @" UPDATE yaf_Registry SET VALUE = '" + value + "' WHERE RegistryID = 7 ";
                        break;
                    case BDProperty.Name:
                        command = @" UPDATE yaf_Board SET NAME = '" + value + "' WHERE BoardID = 1 ";
                        break;
                    case BDProperty.BaseURL:
                        command = @" UPDATE yaf_Registry SET VALUE = '" + value + "' WHERE RegistryID = 233 ";
                        break;
                }

                bridge.Instance.ExecuteRawCommandNonQuery(command);
            }
        }
        catch (Exception e) { }
    }

    public static void SetUserAsForumAdministrator(string username)
    {
        EditForumRankID(username, 1);
    }

    public static void SetUserAsForumModerator(string username)
    {
        EditForumRankID(username, 6);
    }

    public static void RemoveUserAsForumAdministrator(string username)
    {
        EditForumRankID(username, 4);
    }

    public static void RemoveUserAsForumModerator(string username)
    {
        EditForumRankID(username, 4);
    }

    private static void EditForumRankID(string username, int rankID)
    {
        using (var bridge = ParserPool.Acquire(Database.Forum))
        {
            bridge.Instance.ExecuteRawCommandNonQuery(String.Format("UPDATE yaf_User SET RankID = {0} WHERE Name = '{1}'", rankID, username));
        }
    }
}