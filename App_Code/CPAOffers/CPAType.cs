using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Titan;

namespace Prem.PTC.Offers
{
    public class CPAType
    {
        public static string GetText(CPACategory Category)
        {
            return Category.Name;
        }
    }
}