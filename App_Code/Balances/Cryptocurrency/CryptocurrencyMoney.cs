using System;
using System.Linq;
using System.Text;
using Prem.PTC.Utils;
using Titan.Cryptocurrencies;

namespace Prem.PTC
{
    [Serializable]
    public class CryptocurrencyMoney : Money
    {
        public CryptocurrencyType cryptocurrencyType;
        public Cryptocurrency cryptocurrency
        {
            get
            {
                if (_cryptocurrency == null)
                    _cryptocurrency = CryptocurrencyFactory.Get(cryptocurrencyType);
                return _cryptocurrency;
            }
        }
        private Cryptocurrency _cryptocurrency;

        public override int GetMultiplier()
        {
            return (int)Math.Pow(10, CryptocurrencyTypeHelper.GetDecimalPlaces(cryptocurrencyType));
        }

        public CryptocurrencyMoney(CryptocurrencyType cryptocurrencyType, decimal Amount) : base(Amount)
        {
            this.cryptocurrencyType = cryptocurrencyType;
        }

        public CryptocurrencyMoney(CryptocurrencyType cryptocurrencyType, int Amount) : base(Amount)
        {
            this.cryptocurrencyType = cryptocurrencyType;
        }

        public CryptocurrencyMoney(CryptocurrencyType cryptocurrencyType, double Amount) : base(Amount)
        {
            this.cryptocurrencyType = cryptocurrencyType;
        }

        /// <summary>
        /// Print CryptocurrencyMoney class object, ex. ฿4.30042801
        /// </summary>
        public override string ToString()
        {
            string output = amount.ToStringGlobal(CryptocurrencyTypeHelper.GetDecimalPlaces(cryptocurrencyType));

            if (AppSettings.Site.CommasInNumbersEnabled)
            {
                String formatCommand = "{0:n" + CryptocurrencyTypeHelper.GetDecimalPlaces(cryptocurrencyType) + "}";
                output = String.Format(new System.Globalization.CultureInfo("en-US"), formatCommand, Decimal.Parse(output));
            }

            output = CutEndingZeros(output);
            output = String.Format("{0}{1}{2}",
                cryptocurrency.CurrencyDisplaySignBefore,
                output,
                cryptocurrency.CurrencyDisplaySignAfter);

            return output;
        }

        /// <summary>
        /// Print CryptocurrencyMoney class object in clear format, ex. 1334.300 12.0036
        /// </summary>
        public override string ToClearString()
        {
            string ts = amount.ToStringGlobal(CryptocurrencyTypeHelper.GetDecimalPlaces(cryptocurrencyType));
            ts = CutEndingZeros(ts);
            return ts.Replace(",", "");
        }

        public override string ToShortString()
        {
            return this.ToString();
        }

        public new static CryptocurrencyMoney Parse(string amount, CryptocurrencyType type)
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
                throw new InvalidOperationException("Can't parse " + TrimEndWhitespaceAndZeros(amount) + "; Are you sure it has correct CryptocurrencyMoney value?");
            }

            return new CryptocurrencyMoney(type, ParsedAmount);
        }

        #region Operators

        protected override Money __DoAddition(Money c2)
        {
            return new CryptocurrencyMoney(this.cryptocurrencyType, this.amount + c2.amount);
        }

        protected override Money __DoSubtract(Money c2)
        {
            return new CryptocurrencyMoney(this.cryptocurrencyType, this.amount - c2.amount);
        }

        protected override Money __DoMultiply(Money c2)
        {
            return new CryptocurrencyMoney(this.cryptocurrencyType, this.amount * c2.amount);
        }

        protected override Money __DoMultiply(int c2)
        {
            return new CryptocurrencyMoney(this.cryptocurrencyType, this.amount * c2);
        }

        protected override Money __DoMultiply(decimal c2)
        {
            return new CryptocurrencyMoney(this.cryptocurrencyType, this.amount * c2);
        }

        protected override Money __DoDivision(Money c2)
        {
            return new CryptocurrencyMoney(this.cryptocurrencyType, this.amount / c2.amount);
        }

        protected override Money __DoIntDivision(int c1)
        {
            return new CryptocurrencyMoney(this.cryptocurrencyType, c1 / this.amount);
        }

        protected override Money __DoDivision(int c2)
        {
            return new CryptocurrencyMoney(this.cryptocurrencyType, this.amount / c2);
        }

        protected override Money __DoDecimalDivision(decimal c1)
        {
            return new CryptocurrencyMoney(this.cryptocurrencyType, c1 / this.amount);
        }

        protected override Money __DoDivision(decimal c2)
        {
            return new CryptocurrencyMoney(this.cryptocurrencyType, this.amount / c2);
        }

        protected override bool __EqualsCompare(Object obj)
        {
            if (!(obj is CryptocurrencyMoney)) return false;

            return this == (obj as CryptocurrencyMoney);
        }

        public static CryptocurrencyMoney operator +(CryptocurrencyMoney l, CryptocurrencyMoney r)
        {
            if (l.cryptocurrencyType != r.cryptocurrencyType)
                throw new InvalidOperationException("You can't operate on two different Cryptocurrency Types");

            return new CryptocurrencyMoney(l.cryptocurrencyType, l.amount + r.amount);
        }

        public static CryptocurrencyMoney operator -(CryptocurrencyMoney l, CryptocurrencyMoney r)
        {
            if (l.cryptocurrencyType != r.cryptocurrencyType)
                throw new InvalidOperationException("You can't operate on two different Cryptocurrency Types");

            return new CryptocurrencyMoney(l.cryptocurrencyType, l.amount - r.amount);
        }

        public static CryptocurrencyMoney operator *(CryptocurrencyMoney l, CryptocurrencyMoney r)
        {
            if (l.cryptocurrencyType != r.cryptocurrencyType)
                throw new InvalidOperationException("You can't operate on two different Cryptocurrency Types");

            return new CryptocurrencyMoney(l.cryptocurrencyType, l.amount * r.amount);
        }

        public static CryptocurrencyMoney operator *(int l, CryptocurrencyMoney r)
        {
            return new CryptocurrencyMoney(r.cryptocurrencyType, l * r.amount);
        }

        public static CryptocurrencyMoney operator *(CryptocurrencyMoney l, int r)
        {
            return new CryptocurrencyMoney(l.cryptocurrencyType, r * l.amount);
        }

        public static CryptocurrencyMoney operator *(decimal l, CryptocurrencyMoney r)
        {
            return new CryptocurrencyMoney(r.cryptocurrencyType, l * r.amount);
        }

        public static CryptocurrencyMoney operator *(CryptocurrencyMoney l, decimal r)
        {
            return new CryptocurrencyMoney(l.cryptocurrencyType, r * l.amount);
        }

        public static CryptocurrencyMoney operator /(CryptocurrencyMoney l, CryptocurrencyMoney r)
        {
            if (l.cryptocurrencyType != r.cryptocurrencyType)
                throw new InvalidOperationException("You can't operate on two different Cryptocurrency Types");

            return new CryptocurrencyMoney(l.cryptocurrencyType, l.amount / r.amount);
        }

        public static CryptocurrencyMoney operator /(int l, CryptocurrencyMoney r)
        {
            return new CryptocurrencyMoney(r.cryptocurrencyType, l / r.amount);
        }

        public static CryptocurrencyMoney operator /(CryptocurrencyMoney l, int r)
        {
            return new CryptocurrencyMoney(l.cryptocurrencyType, l.amount / r);
        }

        public static CryptocurrencyMoney operator /(decimal l, CryptocurrencyMoney r)
        {
            return new CryptocurrencyMoney(r.cryptocurrencyType, l / r.amount);
        }

        public static CryptocurrencyMoney operator /(CryptocurrencyMoney l, decimal r)
        {
            return new CryptocurrencyMoney(l.cryptocurrencyType, l.amount / r);
        }

        public static bool operator >=(CryptocurrencyMoney l, CryptocurrencyMoney r)
        {
            if (l.cryptocurrencyType != r.cryptocurrencyType)
                throw new InvalidOperationException("You can't operate on two different Cryptocurrency Types");

            return (l.amount >= r.amount);
        }

        public static bool operator <=(CryptocurrencyMoney l, CryptocurrencyMoney r)
        {
            if (l.cryptocurrencyType != r.cryptocurrencyType)
                throw new InvalidOperationException("You can't operate on two different Cryptocurrency Types");

            return (l.amount <= r.amount);
        }

        public static bool operator >(CryptocurrencyMoney l, CryptocurrencyMoney r)
        {
            if (l.cryptocurrencyType != r.cryptocurrencyType)
                throw new InvalidOperationException("You can't operate on two different Cryptocurrency Types");

            return (l.amount > r.amount);
        }

        public static bool operator <(CryptocurrencyMoney l, CryptocurrencyMoney r)
        {
            if (l.cryptocurrencyType != r.cryptocurrencyType)
                throw new InvalidOperationException("You can't operate on two different Cryptocurrency Types");

            return (l.amount < r.amount);
        }

        public static bool operator ==(CryptocurrencyMoney l, CryptocurrencyMoney r)
        {
            if (l.cryptocurrencyType != r.cryptocurrencyType)
                throw new InvalidOperationException("You can't operate on two different Cryptocurrency Types");

            if (Object.ReferenceEquals(l, r)) return true;

            if ((object)l == null || (object)r == null) return false;

            return l.amount == r.amount;
        }

        public static bool operator !=(CryptocurrencyMoney l, CryptocurrencyMoney r)
        {
            if (l.cryptocurrencyType != r.cryptocurrencyType)
                throw new InvalidOperationException("You can't operate on two different Cryptocurrency Types");

            return !(l == r);
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is CryptocurrencyMoney)) return false;

            return this == (obj as CryptocurrencyMoney);
        }


        #endregion Operators

    }
}