using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class VersionManager
{
    //The only place in Admin Panel to update the version
    //In script you should also modify ~/titan_version.txt file upon update
    public static int CurrentVersion = 6013; 

    public static int GetNextVersion(int version)
    {
        if (version == 5009)
            return 6000;

        return version + 1;
    }
}