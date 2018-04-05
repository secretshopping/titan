using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public static class TransferOptionConst
{
    public const string PointsTransfer = "Points";
    public const string JackpotTransferPrefix = "Jackpot-";

    public static string JackpotTransfer(string jackpotName)
    {
        return JackpotTransferPrefix + jackpotName;
    }
}