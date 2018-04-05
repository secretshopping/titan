using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;

namespace Prem.PTC
{
    [Serializable]
    public class Points
    {
        private static int Max_Decimal_Places = 2;

        //Money container
        private Decimal amount;

        #region Constructors

        public Points(Decimal Amount)
        {
            amount = Amount;
        }

        private Points(Money Amount)
        {
            amount = Amount.ToDecimal();
        }

        public Points(int Amount)
        {
            amount = new Decimal(Amount);
        }

        public Points(double Amount)
        {
            amount = new Decimal(Amount);
        }

        #endregion Constructors


        #region Operators

        public static Points operator +(Points l, Points r)
        {
            return new Points(l.amount + r.amount);
        }

        public static Points operator -(Points l, Points r)
        {
            return new Points(l.amount - r.amount);
        }

        public static Points operator *(Points l, Points r)
        {
            return new Points(l.amount * r.amount);
        }

        public static Points operator *(int l, Points r)
        {
            return new Points(l * r.amount);
        }

        public static Points operator *(Points l, int r)
        {
            return new Points(r * l.amount);
        }

        public static Points operator *(Decimal l, Points r)
        {
            return new Points(l * r.amount);
        }

        public static Points operator *(Points l, Decimal r)
        {
            return new Points(r * l.amount);
        }

        public static bool operator >=(Points l, Points r)
        {
            return (l.amount >= r.amount);
        }

        public static bool operator <=(Points l, Points r)
        {
            return (l.amount <= r.amount);
        }

        public static bool operator >(Points l, Points r)
        {
            return (l.amount > r.amount);
        }

        public static bool operator <(Points l, Points r)
        {
            return (l.amount < r.amount);
        }

        public static explicit operator Points(Decimal Amount)
        {
            return new Points(Amount);
        }

        public static implicit operator Points(int Amount)
        {
            return new Points(Amount);
        }

        public static implicit operator Points(Money Amount)
        {
            return new Points(Amount);
        }

        public static bool operator ==(Points l, Points r)
        {
            if (System.Object.ReferenceEquals(l, r)) return true;

            if ((object)l == null || (object)r == null) return false;

            return l.amount == r.amount;
        }

        public static bool operator !=(Points l, Points r)
        {
            return !(l == r);
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is Points)) return false;

            return this == (obj as Points);
        }

        #endregion Operators


        private static string TrimEndWhitespaceAndZeros(string s)
        {
            if (s.Contains('.'))
            {
                string res = s.TrimEnd(whitespaceAndZero);

                if (res.EndsWith("."))
                    res = res.Substring(0, res.Length - 1);

                return res;
            }
            return s;
        }

        private static char[] whitespaceAndZero = new[] {
            ' ',
            '\t',
            '\r',
            '\n',
            '\u000b', // vertical tab
            '\u000c', // form feed
            '0'
        };

        /// <summary>
        /// Create new Money class object format {X}.XXX like Ex $1.240 = Money.Parse("1.240")
        /// $1.001 = Money.Parse("1.001"), $13 = Money.Parse("13") or, for example Money.Parse("13.00")
        /// Money.Parse("") returns null, but Money.Parse(null) throws an exception.
        /// Method is designed to convert human-readable money form to Money representation
        /// </summary>
        /// <param name="amount"></param>
        public static Points Parse(string amount)
        {
            if (amount == null) throw new System.ArgumentNullException();
            if (amount == "") return null;

            amount = amount.Replace(",", ".");
            Decimal ParsedAmount;

            try
            {
                ParsedAmount = Decimal.Parse(TrimEndWhitespaceAndZeros(amount), new System.Globalization.CultureInfo("en-US"));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can't parse " + TrimEndWhitespaceAndZeros(amount) + "; Are you sure it has correct Points value?");
            }

            return new Points(ParsedAmount);
        }

        public static bool TryParse(string amount, out Points result)
        {
            result = null;

            try { result = Parse(amount); }
            catch (Exception) { return false; }

            return true;
        }

        public static int GetPointsPer1d()
        {
            return GetPointsPer1d(AppSettings.Memberships.TenPointsValue);
        }

        public static decimal GetCurrencyPer1Point()
        {
            return 1m / (decimal)GetPointsPer1d();
        }

        public static int GetPointsPer1d(Money points)
        {
            int top = 10;
            Decimal bottom = points.ToDecimal();

            Decimal result = (Decimal)top / bottom;
            return Convert.ToInt32(Decimal.Floor(result));
        }

        public static string GetPointsConversionInfo()
        {
            return String.Format("<b>{0}</b> = <b>{1}</b> {2}",
                new Money(1).ToString(),
                GetPointsPer1d().ToString(),
                AppSettings.PointsName);
        }

        public Decimal ToDecimal()
        {
            return amount;
        }

        #region ToString() functions

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(amount.ToString("F" + Max_Decimal_Places, System.Globalization.CultureInfo.CreateSpecificCulture("en-US")));
            return sb.ToString();
        }

        #endregion ToString() functions


    }
}