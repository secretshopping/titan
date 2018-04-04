using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class CustomFeaturesManager
{
    public static void CRON()
    {
        //MUST BE RUN BEFORE MEMBERMANAGER.CRON() !!!
        if (TitanFeatures.IsEpadilla)
            EpadillaS4DSCustomizations.CRON();


    }
}