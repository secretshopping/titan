using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Titan;
using Prem.PTC.Members;

public class ProfilingManager
{
    /// <summary>
    /// Checks whether the member already completed the Profiling survey or not. If no profiling is available, returns 'true' too
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static bool IsProfiled(Member user)
    {
        if (!AppSettings.Authentication.CustomFieldsAsSurvey) //No profile survey
            return true;

        var where = TableHelper.MakeDictionary("IsHidden", false); where.Add("IsRequired", true);
        var requiredFields = TableHelper.SelectRows<CustomRegistrationField>(where);
        
        foreach (var requiredField in requiredFields)
        {
            if (!user.Custom.ContainsKey(requiredField.StringID))
                return false;
        }
        return true;
    }
}