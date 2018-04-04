using System;

namespace Titan.Registration
{
    public static class RegistrationEnums
    {
        public static String GetTextForAccountActivationFeeVia(AccountActivationFeeVia type)
        {
            if (type == AccountActivationFeeVia.MainSite)
                return "Main site";

            if (type == AccountActivationFeeVia.UserPanel)
                return "User panel with Pop-up";

            return String.Empty;
        }
    }

    public enum RegistrationFieldType
    {
        Null = 0,

        TextBox = 1,
        CheckBox = 2
    }

    public enum AccountActivationFeeVia
    {
        MainSite  = 0,
        UserPanel = 1
    }


}
