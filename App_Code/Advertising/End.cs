using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC.Advertising;
using System.Web.UI.WebControls;

namespace Prem.PTC.Utils
{
    /// <summary>
    /// Summary description for AdvertEnd
    /// </summary>
    public abstract class End
    {
        public enum Mode { Null = 0, Days = 2, Clicks = 3, Endless = 4 }

        public abstract Mode EndMode { get; }
        public virtual double Value { get; protected set; }

        public static End Null { get { return new NullEnd(); } }
        public static End Endless { get { return new EndlessEnd(); } }
        public static End FromDays(int daysNumber) { return new DaysEnd(daysNumber); }
        public static End FromDays(string daysNumber)
        {
            return FromDays(Convert.ToInt32(daysNumber));
        }
        public static End FromDays(TimeSpan days) { return new DaysEnd(days); }
        public static End FromDate(DateTime date) { return new DaysEnd(date - AppSettings.ServerTime); }
        public static End FromDate(DateTime? date) { return (date == null) ? Null : FromDate(date); }
        public static End FromClicks(int clicks) { return new ClicksEnd(clicks); }
        public static End FromClicks(string clicks) { return FromClicks(Convert.ToInt32(clicks)); }
        public static End FromModeValue(Mode endMode, double value)
        {
            switch (endMode)
            {
                case Mode.Null: return new NullEnd();
                case Mode.Days: return new DaysEnd(value);
                case Mode.Clicks: return new ClicksEnd((int)value);
                case Mode.Endless: return new EndlessEnd();
                default:
                    throw new NotImplementedException("End mode not implemented: " + endMode);
            }
        }

        protected End() { }

        protected End(double totalValue)
        {
            Value = totalValue;
        }

        public virtual End AddValue(double daysClicks)
        {
            if (daysClicks == 0) return this;

            return FromModeValue(EndMode, Value + daysClicks);
        }

        public abstract string ToTypeString();

        public override string ToString()
        {
            string type = ToTypeString();

            if (!String.IsNullOrEmpty(type))
                return Value + " " + ToTypeString();

            return Value.ToString();
        }

        public static ListItem[] ListControlSource
        {
            get
            {
                var query = from Mode value in Enum.GetValues(typeof(Mode))
                            where value != Mode.Null
                            select new ListItem(value.ToString(), Convert.ToString((int)value));

                return query.ToArray();
            }
        }
    }

    internal sealed class NullEnd : End
    {
        public override End.Mode EndMode { get { return Mode.Null; } }

        internal NullEnd() { }

        public override string ToString() { return "N/A"; }
        public override string ToTypeString() { return "N/A"; }
        public override End AddValue(double daysClicks) { return this; }
        public override double Value { get { return 0; } }
    }

    public sealed class DaysEnd : End
    {
        public override End.Mode EndMode { get { return Mode.Days; } }

        internal DaysEnd(TimeSpan afterTotalDays) : this(Math.Max((int)afterTotalDays.TotalDays, 0)) { }
        internal DaysEnd(double afterDays) : base(afterDays) { }

        public override string ToTypeString() { return Resources.L1.DAYS; }
    }

    public sealed class ClicksEnd : End
    {
        public override End.Mode EndMode { get { return Mode.Clicks; } }

        internal ClicksEnd(double afterClicks) : base(afterClicks) { }

        public override double Value
        {
            get { return (int)base.Value; }
            protected set { base.Value = (int)value; }
        }

        public override string ToTypeString()
        {
            //if (Prem.PTC.AppSettings.BannerAdverts.ImpressionsEnabled)
            //    return Resources.L1.CLICKSSMALL + "/" + Resources.L1.IMPRESSIONSSMALL;
            //
            //return Resources.L1.CLICKSSMALL;
            return String.Empty;
        }
    }

    public sealed class EndlessEnd : End
    {
        public override End.Mode EndMode { get { return Mode.Endless; } }

        internal EndlessEnd() { }

        public override string ToString() { return "+\u221E"; }
        public override string ToTypeString() { return "endless"; }

        public override End AddValue(double daysClicks) { return this; }

        public override double Value { get { return Double.PositiveInfinity; } }
    }
}