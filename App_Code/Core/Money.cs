using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Prem.PTC.Utils;
using Titan.Cryptocurrencies;

namespace Prem.PTC
{
    [Serializable]
    public class Money : IComparable<Money>
    {
        public decimal amount;

        #region Constructors

        public Money(decimal Amount)
        {
            amount = Math.Truncate(GetMultiplier() * Amount) / GetMultiplier();
        }

        public Money(int Amount)
        {
            amount = new decimal(Amount);
        }

        public Money(double Amount) : this(new decimal(Amount))
        {
        }

        public bool IsZero
        {
            get
            {
                decimal tempamount = Math.Floor(amount * GetMultiplier()) / GetMultiplier();
                if (tempamount == new decimal(0))
                    return true;
                return false;
            }
        }

        public virtual int GetMultiplier()
        {
            return (int)Math.Pow(10, CoreSettings.GetMaxDecimalPlaces());
        }

        protected Money GetMinPositiveValue()
        {
            return new Money(1 / (decimal)GetMultiplier());
        }

        public static Money MinPositiveValue
        {
            get
            {
                return Money.Zero.GetMinPositiveValue();
            }
        }

        #endregion Constructors

        #region Operators

        public static Money operator +(Money l, Money r)
        {
            return l.__DoAddition(r);
        }

        public static Money operator -(Money l, Money r)
        {
            return l.__DoSubtract(r);
        }

        public static Money operator *(Money l, Money r)
        {
            return l.__DoMultiply(r);
        }

        public static Money operator *(int l, Money r)
        {
            return r.__DoMultiply(l);
        }

        public static Money operator *(Money l, int r)
        {
            return l.__DoMultiply(r);
        }

        public static Money operator *(decimal l, Money r)
        {
            return r.__DoMultiply(l);
        }

        public static Money operator *(Money l, decimal r)
        {
            return l.__DoMultiply(r);
        }

        public static Money operator /(Money l, Money r)
        {
            return l.__DoDivision(r);
        }

        public static Money operator /(int l, Money r)
        {
            return r.__DoIntDivision(l);
        }

        public static Money operator /(Money l, int r)
        {
            return l.__DoDivision(r);
        }

        public static Money operator /(decimal l, Money r)
        {
            return r.__DoDecimalDivision(l);
        }

        public static Money operator /(Money l, decimal r)
        {
            return l.__DoDivision(r);
        }

        public static bool operator >=(Money l, Money r)
        {
            return l.__LeftEqualCompare(r);
        }

        public static bool operator <=(Money l, Money r)
        {
            return l.__RightEqualCompare(r);
        }

        public static bool operator >(Money l, Money r)
        {
            return l.__LeftCompare(r);
        }

        public static bool operator <(Money l, Money r)
        {
            return l.__RightCompare(r);
        }

        public static explicit operator Money(Decimal Amount)
        {
            return new Money(Amount);
        }

        public static bool operator ==(Money l, Money r)
        {
            if (Object.ReferenceEquals(l, null))
                return Object.ReferenceEquals(r, null);

            return l.__EqualsStrictCompare(r);
        }

        public override bool Equals(Object obj)
        {
            return this.__EqualsCompare(obj);
        }

        public static bool operator !=(Money l, Money r)
        {
            return !(l == r);
        }

        #region Virtual methods

        protected virtual Money __DoAddition(Money c2)
        {
            return new Money(this.amount + c2.amount);
        }

        protected virtual Money __DoSubtract(Money c2)
        {
            return new Money(this.amount - c2.amount);
        }

        protected virtual Money __DoMultiply(Money c2)
        {
            return new Money(this.amount * c2.amount);
        }

        protected virtual Money __DoMultiply(int c2)
        {
            return new Money(this.amount * c2);
        }

        protected virtual Money __DoMultiply(decimal c2)
        {
            return new Money(this.amount * c2);
        }

        protected virtual Money __DoDivision(Money c2)
        {
            return new Money(this.amount / c2.amount);
        }

        protected virtual Money __DoIntDivision(int c1)
        {
            return new Money(c1 / this.amount);
        }

        protected virtual Money __DoDivision(int c2)
        {
            return new Money(this.amount / c2);
        }

        protected virtual Money __DoDecimalDivision(decimal c1)
        {
            return new Money(c1 / this.amount);
        }

        protected virtual Money __DoDivision(decimal c2)
        {
            return new Money(this.amount / c2);
        }

        protected virtual bool __LeftEqualCompare(Money c2)
        {
            return (this.amount >= c2.amount);
        }

        protected virtual bool __RightEqualCompare(Money c2)
        {
            return (this.amount <= c2.amount);
        }

        protected virtual bool __LeftCompare(Money c2)
        {
            return (this.amount > c2.amount);
        }

        protected virtual bool __RightCompare(Money c2)
        {
            return (this.amount < c2.amount);
        }

        protected virtual bool __EqualsStrictCompare(Money c2)
        {
            if (Object.ReferenceEquals(this, c2)) return true;

            if ((object)this == null || (object)c2 == null) return false;

            return this.amount == c2.amount;
        }

        protected virtual bool __EqualsCompare(Object obj)
        {
            if (!(obj is Money)) return false;

            return this == (obj as Money);
        }

        #endregion

        #endregion Operators

        #region Properties

        public static Money Zero { get { return new Money(0); } }

        public static string RegularExpression
        {
            get
            { return "^[+-]?\\d{1,11}(\\.\\d{1,8})?$"; }
        }

        public static string NonNegativeRegularExpression
        {
            get { return "^+?\\d{1,11}(\\.\\d{1,8})?$"; }
        }




        #endregion Properties

        #region ToString() functions

        /// <summary>
        /// Trims trailing zeros with decimal point if not necessary
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CutEndingZeros(string input)
        {
            input = Regex.Replace(input, @"\.?0+$", String.Empty);

            return input;
        }

        /// <summary>
        /// Adds to string currency sign in proper place
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string AddCurrencySign(string input)
        {
            string currencySign = AppSettings.Site.MulticurrencySign;
            bool isCurrencySignBeforeNumber = (AppSettings.IsNull()) ? true : AppSettings.Site.IsCurrencySignBefore;
            int currencySignIndex = isCurrencySignBeforeNumber ? 0 : input.Length;
            string output = input.Insert(currencySignIndex, currencySign);

            return output;
        }

        /// <summary>
        /// Print Money class object, ex. $1,334.30042 $12.003
        /// </summary>
        public override string ToString()
        {
            string output = CurrencyExchangeHelper.TryCalculate(amount).ToStringGlobal();

            if (AppSettings.Site.CommasInNumbersEnabled)
            {
                String formatCommand = "{0:n" + CoreSettings.GetMaxDecimalPlaces(CurrencyType.Fiat) + "}";
                output = String.Format(new System.Globalization.CultureInfo("en-US"), formatCommand, Decimal.Parse(output));
            }

            //Show zeros?
            if (!AppSettings.Site.ShowRemainingZeros)
                output = CutEndingZeros(output);

            output = AddCurrencySign(output);

            return output;
        }

        /// <summary>
        /// Print Money class object in clear format, ex. 1334.300 12.0036
        /// </summary>
        public virtual string ToClearString()
        {
            string ts = amount.ToStringGlobal();
            if (!AppSettings.Site.ShowRemainingZeros)
                ts = CutEndingZeros(ts);
            return ts.Replace(",", "");
        }

        /// <summary>
        /// Print Money class object in clear two decimals format, ex. 1334.30, 12.10, 345.00
        /// </summary>
        public string ToShortClearString()
        {
            Decimal tempamount = Math.Floor(amount * 100) / 100;

            string ts = tempamount.ToStringGlobal(2);
            return ts.Replace(",", "");
        }

        /// <summary>
        /// Print Money class object in clear two decimals format, ex. $1334.30, $12.10, $345.00
        /// </summary>
        public virtual string ToShortString()
        {
            return AddCurrencySign(ToShortClearString());
        }

        public Money ToMulticurrency()
        {
            return CurrencyExchangeHelper.TryCalculate(this);
        }

        public Money FromMulticurrency()
        {
            return CurrencyExchangeHelper.TryFromCalculate(this);
        }

        #endregion ToString() functions

        #region Additional functions

        /// <summary>
        /// Makes Money * %, e.g. $3.045 * 123% or $0.056 * 5%
        /// percent = int (123, 5), HAS 100% PERFORMANCE PROTECTION
        /// </summary>
        /// <param name="Percent"></param>
        /// <returns></returns>
        public static Money MultiplyPercent(Money amount, int percent)
        {
            return MultiplyPercent(amount, Convert.ToDecimal(percent));
        }

        public static Money MultiplyPercent(Money amount, double percent)
        {
            return MultiplyPercent(amount, Convert.ToDecimal(percent));
        }

        public static Money MultiplyPercent(Money amount, decimal percent)
        {
            if (percent == 100)
                return amount;

            decimal percentInDecimal = percent / 100;
            return amount * percentInDecimal;
        }

        /// <summary>
        /// Returns negative amount of any Money instance
        /// </summary>
        /// <returns></returns>
        public Money Negatify()
        {
            if (this > Money.Zero)
                return new Money(-1) * this;

            return this;
        }

        /// <summary>
        /// Returns positive amount of any Money instance
        /// </summary>
        /// <returns></returns>
        public Money Positify()
        {
            if (this < Money.Zero)
                return new Money(-1) * this;

            return this;
        }

        protected static string TrimEndWhitespaceAndZeros(string s)
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
        /// Create new Money class object format. 
        /// Method is designed to convert human-readable money form to Money representation
        /// </summary>
        /// <param name="amount"></param>
        public static Money Parse(string amount)
        {
            if (amount == null) throw new ArgumentNullException();
            if (amount == "") return null;

            amount = amount.Replace(",", ".");
            Decimal ParsedAmount;

            try
            {
                ParsedAmount = Decimal.Parse(TrimEndWhitespaceAndZeros(amount), new System.Globalization.CultureInfo("en-US"));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can't parse " + TrimEndWhitespaceAndZeros(amount) + "; Are you sure it has correct Money value?");
            }

            return new Money(ParsedAmount);
        }

        public static bool TryParse(string amount, out Money result)
        {
            result = null;

            try { result = Parse(amount); }
            catch (Exception) { return false; }

            return true;
        }

        /// <summary>
        /// Get totals rounded to the ceiling [UP]
        /// </summary>
        /// <returns></returns>
        public int GetTotalsUp()
        {
            return (int)Math.Ceiling(amount);
        }

        /// <summary>
        /// Get totals rounded
        /// </summary>
        /// <returns></returns>
        public int GetTotals()
        {
            return (int)Math.Round(amount, MidpointRounding.ToEven);
        }

        /// <summary>
        /// Get real totals (rounded to the floor) [DOWN]
        /// </summary>
        /// <returns></returns>
        public int GetRealTotals()
        {
            return (int)amount;
        }

        /// <summary>
        /// Casts Money Object to Points (equal method GetRealTotals )
        /// </summary>
        /// <returns></returns>
        public int AsPoints()
        {
            return this.GetRealTotals();
        }


        /// <summary>
        /// Converts Money to Decimal. 
        /// </summary>
        public Decimal ToDecimal()
        {
            return amount;
        }

        /// <summary>
        /// Tries to subtract object - m1. Object and m1 must be positive otherwise "false"
        /// If object >= m1 then returns "true" and the method subtract
        /// Otherwise it returns "false"
        /// </summary>
        public bool TrySubtract(Money m1)
        {
            if (amount >= 0 && m1.amount >= 0 && amount >= m1.amount)
            {
                amount -= m1.amount;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Converts this Money to Points, using '10 points value' meter from Settings
        /// </summary>
        /// <returns></returns>
        public int ConvertToPoints()
        {
            return Titan.PointsConverter.ToPoints(this);
        }

        public int CompareTo(Money other)
        {
            if (other == null)
                return 1;

            return amount.CompareTo(other.amount);
        }

        /// <summary>
        /// Exchanges from currecny passed as parameter to Main currency
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public Money ExchangeFrom(string currencyCode)
        {
            Money output = Money.Zero;

            if (AppSettings.Payments.CurrencyMode == CurrencyMode.Fiat)
            {
                output = CurrencyExchangeHelper.FromCalculate(this, currencyCode);
            }
            else if (AppSettings.Payments.CurrencyMode == CurrencyMode.Cryptocurrency)
            {
                var cryptocurrencyExchange = CryptocurrencyFactory.Get(AppSettings.Site.CurrencyCode);
                output = new Money(cryptocurrencyExchange.ConvertFromMoney(this, currencyCode));
            }

            return output;
        }

        public static bool IsCryptoCurrency(string currencyCode)
        {
            CryptocurrencyType result;
            return Enum.TryParse(currencyCode, out result);
        }

        #endregion Additional functions
    }
}