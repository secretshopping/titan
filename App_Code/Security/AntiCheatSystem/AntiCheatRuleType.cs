using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public enum AntiCheatRuleType
{
    MultipleFacebook = 0, //DONE
    DuplicatedPaymentAddresses = 5, //DONE

    SameRegisteredIP = 10, //DONE
    SameRegisteredIPSameDay = 3, //DONE


    SameIPAddress = 8,
    SameIPAddressSameDay = 9 

}