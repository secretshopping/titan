using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class DbException : ApplicationException
{
    public DbException(string information)
        : base(information)
    {

    }
}

public class SQLConnectionOpenException : ApplicationException
{
    public SQLConnectionOpenException(string information)
        : base(information)
    {

    }
}

public class MbException : ApplicationException
{
    public MbException(string information)
        : base(information)
    {

    }
}

/// <summary> Will be thrown manually in the code.
/// Contains information that must be shown to the client
/// (e.g. "Choose another username" during registration) 
/// </summary>
public class MsgException : ApplicationException
{
    public MsgException(string information)
        : base(information)
    {

    }
}
/// <summary> Will be thrown manually in the code.
/// Contains success information that must be shown to the client
/// (e.g. "User succesfully tipped" after successful Shoutbox tip) 
/// </summary>
public class SuccessMsgException : ApplicationException
{
    public SuccessMsgException(string information)
        : base(information)
    {

    }
}

/// <summary>
/// Thrown is special situations, when there must be a message delivered to ASPX level,
/// but not shown to client (in opposite to MsgException)
/// </summary>
public class SpecialException : ApplicationException
{
    public SpecialException(string information)
        : base(information)
    {

    }
}

public static class Exceptions
{
    public static void HandleNonMsgEx(Action action)
    {
        try
        {
            action.Invoke();
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }
}