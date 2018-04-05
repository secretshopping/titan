using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Reflection;
using System.ComponentModel;

namespace Prem.PTC.Utils
{

    public static class ControlExtensions
    {
        public static Control FindHeaderControl(this Repeater repeater, string controlID)
        {
            return repeater.Controls[0].FindControl(controlID);
        }

        public static Control FindFooterControl(this Repeater repeater, string controlID)
        {
            return repeater.Controls[repeater.Controls.Count - 1].FindControl(controlID);
        }

    }

    public static class ListExtensions
    {
        public static List<T> Clone<T>(this List<T> listToClone)
        {
            return listToClone.ConvertAll<T>(item => item);
        }

        public static void Shuffle<T>(this IList<T> list, Random rnd)
        {
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, rnd.Next(i, list.Count));
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    /// <summary>
    /// Contains methods extending ListControls (RadioButtonList, ...)
    /// </summary>
    public static class ListControlExtensions
    {

        public static bool IsLoaded(this ListControl control)
        {
            return control.Items.Count > 0;
        }

        public static bool IsAnythingSelected(this ListControl control)
        {
            return control.SelectedIndex != -1;
        }

        public static void SetDictionaryDataSource(this ListControl list, Dictionary<string, string> dataSource)
        {
            list.DataValueField = "Key";
            list.DataTextField = "Value";
            list.DataSource = dataSource;
        }
    }

    public static class StringExtentions
    {
        /// <summary>
        /// Shortens string to limited length. Use with controls to display only
        /// text snippet
        /// </summary>
        /// <param name="chars">Maximal text length</param>
        /// <returns></returns>
        public static string Shorten(this string name, int chars)
        {
            if (name.ToCharArray().Count() > chars)
            {
                return name.Substring(0, Math.Max(chars - 3, 1)) + "...";
            }
            else return name;
        }

        public static string ReplaceEmptyString(this string potentiallyEmptyString, string replace = "N/A")
        {
            if (String.IsNullOrEmpty(potentiallyEmptyString)) return replace;
            else return potentiallyEmptyString;
        }

        public static string GenerateRandomString(int length, int alphaNumericalChars)
        {
            return System.Web.Security.Membership.GeneratePassword(length, alphaNumericalChars);
        }

        public static TimeSpan FromDays(this string timeSpanStringVersion)
        {
            return TimeSpan.FromDays(Convert.ToDouble(timeSpanStringVersion));
        }
    }


    public static class CommandEventArgsExtensions
    {
        public static int GetSelectedRowIndex(this CommandEventArgs e)
        {
            int rowIndex;
            bool parseResult = Int32.TryParse(e.CommandArgument.ToString(), out rowIndex);

            if (!parseResult || rowIndex < 0)
                throw new Exception("Check for CommandArgument=\'<%# Container.DataItemIndex %>\' in your button");

            return rowIndex;
        }
    }

    public static class RentingOptionsExtensions
    {
        public static string GetDescription(this AppSettings.Referrals.RentingOption option)
        {
            switch (option)
            {
                case AppSettings.Referrals.RentingOption.Null:
                    return "N/A";
                case AppSettings.Referrals.RentingOption.Normal:
                    return "Normal referrals";
                case AppSettings.Referrals.RentingOption.Bot:
                    return "Bots";
                case AppSettings.Referrals.RentingOption.All:
                    return "All";
                case AppSettings.Referrals.RentingOption.DirectReferrals:
                    return "Direct referrals";
                default:
                    throw new NotImplementedException(option.ToString());
            }
        }
    }

    public static class BannerExtensions
    {
        public static bool IsOKWithDimensons(this Banner banner)
        {
            foreach (var dimensions in BannerAdvertDimensions.GetActive())
                if (banner.Width == dimensions.Width && banner.Height == dimensions.Height)
                    return true;

            return false;
        }

        public static bool IsAdPackNormalBanner(this Banner banner)
        {
            return banner.Width == AppSettings.RevShare.AdPack.PackNormalBannerWidth &&
                   banner.Height == AppSettings.RevShare.AdPack.PackNormalBannerHeight;
        }

        public static bool IsAdPackConstantBanner(this Banner banner)
        {
            return banner.Width == AppSettings.RevShare.AdPack.PackConstantBannerWidth &&
                   banner.Height == AppSettings.RevShare.AdPack.PackConstantBannerHeight;
        }
    }

    public static class DecimalExtensions
    {
        public static decimal TruncateDecimals(this decimal value, int decimalPlaces)
        {
            decimal integralValue = Math.Truncate(value);

            decimal fraction = value - integralValue;

            decimal factor = (decimal)Math.Pow(10, decimalPlaces);

            decimal truncatedFraction = Math.Truncate(fraction * factor) / factor;

            decimal result = integralValue + truncatedFraction;

            return result;
        }

        /// <summary>
        /// Decimal ToString using settings for decimal places and constatnt globalization en-US
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToStringGlobal(this decimal value)
        {
            return ToStringGlobal(value, CoreSettings.GetMaxDecimalPlaces());
        }

        /// <summary>
        /// Decimal ToString using constatnt globalization en-US with diffrent decimal places
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxDecimalPlaces"></param>
        /// <returns></returns>
        public static string ToStringGlobal(this decimal value, int maxDecimalPlaces)
        {
            string output = value.ToString("F" + (maxDecimalPlaces > 0 ? maxDecimalPlaces.ToString() : String.Empty)
                , System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

            return output;
        }
    }

    public static class TimeSpanExtensions
    {
        private enum TimeSpanElement
        {
            Millisecond,
            Second,
            Minute,
            Hour,
            Day
        }

        public static string ToFriendlyDisplay(this TimeSpan timeSpan, int maxNrOfElements)
        {
            maxNrOfElements = Math.Max(Math.Min(maxNrOfElements, 5), 1);
            string stringContent = "<b>{0}</b> {1}";
            List<string> values = new List<string>();

            if (timeSpan.Days != 0)
                values.Add(string.Format(stringContent, timeSpan.Days, U6000.DAYS));
            if (timeSpan.Hours != 0)
                values.Add(string.Format(stringContent, timeSpan.Hours, U6000.HOURS));
            if (timeSpan.Minutes != 0)
                values.Add(string.Format(stringContent, timeSpan.Minutes, U6000.MINUTES));
            if (timeSpan.Seconds != 0)
                values.Add(string.Format(stringContent, timeSpan.Seconds, U6000.SECONDS));
            if (timeSpan.Milliseconds != 0)
                values.Add(string.Format(stringContent, timeSpan.Milliseconds, U6000.MILISECONDS));
            
            return string.Join(" ", values.Take(maxNrOfElements));
        }
    }

    public static class IntExtensions
    {
        public static int? ToNullableInt(this string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }
    }

    public static class WebClientExtensions
    {
        public static string DownloadStringWithHeaders(this WebClient client, string url)
        {
            WebRequestUtils.SetUpSecurityProtocols();
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
            return client.DownloadString(url);
        }
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)                    
                        return attr.Description;                    
                }
            }
            return null;
        }
    }
}