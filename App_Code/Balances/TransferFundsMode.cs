using Prem.PTC.Utils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

public enum TransferFundsMode
{
    Null = 0,
    [Description("Allow Points And Main Balance")]
    AllowPointsAndMainBalance = 1,
    [Description("Allow Points Only")]
    AllowPointsOnly = 2,
    [Description("Allow Main Balance Only")]
    AllowMainBalanceOnly = 3,
    [Description("Deny All")]
    DenyAll = 4,
    [Description("Allow Main To Purchase Balance")]
    AllowMainToPurchaseBalance = 5
}

public enum TransferFeeType
{
    SameUserCommissionToMain = 1,
    OtherUserMainToPurchase = 2,
    OtherUserMainToMain = 3,
    OtherUserPointsToPoints = 4
}

public enum TransferFundsPermission
{
    Null = 0,
    [Description("Allow All")]
    AllowAll = 1,
    [Description("Allow Sending Only")]
    AllowSendingOnly = 2,
    [Description("Allow Receiving Only")]
    AllowReceivingOnly = 3,
    [Description("Deny All")]
    DenyAll = 4
}

public enum MaximumPayoutPolicy
{
    Constant = 1,
    Percentage = 2
}

public enum CurrencyMode
{
    Fiat = 0,
    Cryptocurrency = 1
}

public enum TokenCryptocurrencyValue
{
    Static = 0,
    DynamicFromInternalExchange = 1
}

public class TransferFundsPermissionHelper
{
    public static ListItem[] ListItems
    {
        get
        {

            var query = from TransferFundsPermission status in Enum.GetValues(typeof(TransferFundsPermission))
                        where status != TransferFundsPermission.Null
                        orderby (int)status
                        select new ListItem(status.GetDescription(), (int)status + "");

            return query.ToArray();
        }
    }
}

public class TransferFundsModeHelper
{
    public static ListItem[] ListItems
    {
        get
        {

            var query = from TransferFundsMode status in Enum.GetValues(typeof(TransferFundsMode))
                        where status != TransferFundsMode.Null
                        orderby (int)status
                        select new ListItem(status.GetDescription(), (int)status + "");

            return query.ToArray();
        }
    }
}