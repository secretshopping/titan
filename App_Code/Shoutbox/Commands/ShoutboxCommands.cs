using Prem.PTC;
using Prem.PTC.Members;
using Resources;
using System;
using System.Web;

/// <summary>
/// Summary description for ShoutboxMessageRestrictions
/// </summary>
public static class ShoutboxCommands
{
    public static readonly string cookieName = "commandConfirmation";
    internal static void TryExecuteCommand(string message)
    {
        if (!ContainsCommand(message))
            return;

        string command = GetCommand(message);

        if (HttpContext.Current.Request.Cookies[cookieName] == null && command != CommandTypes.enableCommands.ToString())
        {
            throw new MsgException(U4200.COMMANDWARNING);
        }

        if (command == CommandTypes.tip.ToString() && AppSettings.Shoutbox.TipCommandEnabled)
        {
            TipCommand.TryTipUser(message, Member.Current);
        }
        else if(command == CommandTypes.enableCommands.ToString())
        {
            EnableCommand.TryEnableCommands();
        }
        else
            throw new MsgException(U4200.INVALIDSHOUTBOXCOMMAND);
    }

    private static string GetCommand(string message)
    {
        foreach (string command in Enum.GetNames(typeof(CommandTypes)))
        {
            if (message.StartsWith("/" + command))
            {
                return command;
            }
        }
        throw new MsgException(U4200.INVALIDSHOUTBOXCOMMAND);
    }

    private static bool ContainsCommand(string message)
    {
        if (message.StartsWith("/"))
            return true;
        return false;
    }

    public enum CommandTypes
    {
        tip = 1,
        enableCommands = 2
    }
}