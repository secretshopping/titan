using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Prem.PTC.Members
{
    public enum Gender
    {
        Null = 0,
        Male = 1,
        Female = 2
    }


    public static class GenderHelper
    {
        /// <summary>
        /// Returns list control source with all genders' short descriptions as text 
        /// and int status ordinal as value
        /// </summary>
        public static ListItem[] GenderListItems
        {
            get
            {
                var query = from Gender gender in Enum.GetValues(typeof(Gender))
                            where gender != Gender.Null
                            orderby (int)gender
                            select new ListItem(gender.ShortDescription(), (int)gender + "");

                return query.ToArray();
            }
        }
    }


    public static class GenderExtensions
    {
        public static bool? MapToDatabase(this Gender gender)
        {
            switch (gender)
            {
                case Gender.Null: return null;
                case Gender.Male: return true;
                case Gender.Female: return false;

                default:
                    throw new ArgumentOutOfRangeException("MapToDatabase doesn't specify behaviour for "
                                                        + Enum.GetName(typeof(Gender), gender));
            }
        }

        public static Gender MapFromDatabase(this bool? isMale)
        {
            if (isMale == null) return Gender.Null;
            else if (isMale == true) return Gender.Male;
            else return Gender.Female;
        }

        /// <summary>
        /// Provides human readable, short description for each gender enum
        /// </summary>
        public static string ShortDescription(this Gender gender)
        {
            if (gender == Gender.Null) return "Unknown";

            return Enum.GetName(typeof(Gender), gender);
        }

        /// <summary>
        /// Returns 'M' for male, 'F' for female, 'U' for unknown
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string GetLetter(this Gender gender)
        {
            if (gender == Gender.Male) return "M";
            if (gender == Gender.Female) return "F";
            return "U";
        }
    }
}