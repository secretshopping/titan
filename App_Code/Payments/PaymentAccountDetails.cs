using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Collections;
using Prem.PTC.Members;
using System.Web.UI.WebControls;

namespace Prem.PTC.Payments
{
    /* Create base table SQL
     * 
    CREATE TABLE [dbo].[]
    (
	    [Id] INT IDENTITY(1,1) NOT NULL,
	    [IsActive] BIT NOT NULL,
	    [CashflowDirections] INT NOT NULL,
	    [CashoutPriority] INT NOT NULL,
	    [IsInstantCashout] BIT NOT NULL,
	    [Username] VARCHAR(200) NULL,
	    [ManualCashoutAfterExceeding] DECIMAl(19, 8) NOT NULL DEFAULT 0,
	    [OverrideGlobalLimit] BIT NOT NULL,
	    [CashoutLimit] DECIMAL(19, 8) NOT NULL DEFAULT 0,
	    [StaticFee] DECIMAL(19, 8) NOT NULL DEFAULT 0,
	    [PercentFee] DECIMAL(5, 2) NOT NULL DEFAULT 0,
	    [WithdrawalFeePercent] DECIMAL(5, 2) NOT NULL DEFAULT 0,
        [DaysToBlockWithdrawalsAfterAccountChange] INT NOT NULL DEFAULT 0
    );
    */

    public abstract class PaymentAccountDetails : BaseTableObject
    {
        #region Columns

        public static class Columns
        {
            public const string IsActive = "IsActive";
            public const string CashflowDirections = "CashflowDirections";
            public const string CashoutPriority = "CashoutPriority";
            public const string IsInstantCashout = "IsInstantCashout";
            public const string Username = "Username";
            public const string ManualCashoutAfterExceeding = "ManualCashoutAfterExceeding";
            public const string OverrideGlobalLimit = "OverrideGlobalLimit";
            public const string CashoutLimit = "CashoutLimit";
            public const string StaticFee = "StaticFee";
            public const string PercentFee = "PercentFee";
            public const string WithdrawalFeePercent = "WithdrawalFeePercent";
            public const string DaysToBlockWithdrawalsAfterAccountChange = "DaysToBlockWithdrawalsAfterAccountChange";
        }

        [Column(Columns.IsActive)]
        public bool IsActive { get { return _isActive; } set { _isActive = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CashflowDirections)]
        protected int CashflowDirections { get { return _cashflowDirections; } set { _cashflowDirections = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CashoutPriority)]
        public int CashoutPriority { get { return _cashoutPriority; } set { _cashoutPriority = value; SetUpToDateAsFalse(); } }

        [Column(Columns.IsInstantCashout)]
        public bool IsInstantCashout { get { return _isInstantCashout; } set { _isInstantCashout = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Username)]
        public string Username { get { return _username; } set { _username = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ManualCashoutAfterExceeding)]
        public Money ManualCashoutAfterExceeding { get { return mexceed; } set { mexceed = value; SetUpToDateAsFalse(); } }

        [Column(Columns.OverrideGlobalLimit)]
        public bool OverrideGlobalLimit { get { return _OverrideGlobalLimit; } set { _OverrideGlobalLimit = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CashoutLimit)]
        public Money CashoutLimit { get { return _CashoutLimit; } set { _CashoutLimit = value; SetUpToDateAsFalse(); } }

        [Column(Columns.StaticFee)]
        public Money StaticFee { get { return _StaticFee; } set { _StaticFee = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PercentFee)]
        public decimal PercentFee { get { return _PercentFee; } set { _PercentFee = value; SetUpToDateAsFalse(); } }

        [Column(Columns.WithdrawalFeePercent)]
        public decimal WithdrawalFeePercent { get { return _WithdrawalFeePercent; } set { _WithdrawalFeePercent = value; SetUpToDateAsFalse(); } }

        [Column(Columns.DaysToBlockWithdrawalsAfterAccountChange)]
        public int DaysToBlockWithdrawalsAfterAccountChangename { get { return _daysToBlockWithdrawalsAfterAccountChange; } set { _daysToBlockWithdrawalsAfterAccountChange = value; SetUpToDateAsFalse(); } }

        private int _cashflowDirections, _cashoutPriority, _daysToBlockWithdrawalsAfterAccountChange;
        private bool _isActive, _isInstantCashout, _OverrideGlobalLimit;
        private string _username;
        private Money mexceed, _CashoutLimit, _StaticFee;
        private decimal _PercentFee, _WithdrawalFeePercent;

        public GatewayCashflowDirection Cashflow
        {
            get { return (GatewayCashflowDirection)CashflowDirections; }
            set { CashflowDirections = (int)value; }
        }

        #endregion Columns

        /// <summary>
        /// Returns Payza, PayPal, Liberty Reserve etc. depending on account type
        /// </summary>
        public abstract string AccountType { get; }

        /// <summary>
        /// Creates account associated with this details
        /// </summary>
        public abstract PaymentAccount Account { get; }
        public abstract ButtonGenerationStrategy GetStrategy();

        #region Constructors

        public PaymentAccountDetails() : base() { }
        public PaymentAccountDetails(int id) : base(id) { }
        public PaymentAccountDetails(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public PaymentProcessor GetProcessorType()
        {
            return GetFromStringType(AccountType);
        }        

        #region Modify when adding new

        public static IEnumerable<Type> PaymentAccountDetailsSubclasses
        {
            get
            {
                // NOTE: Working (hardcoded) version:
                List<Type> paymentAccountDetails = new List<Type>();
                paymentAccountDetails.Add(typeof(PayPalAccountDetails));
                paymentAccountDetails.Add(typeof(PayzaAccountDetails));
                paymentAccountDetails.Add(typeof(PerfectMoneyAccountDetails));
                paymentAccountDetails.Add(typeof(OKPayAccountDetails));
                paymentAccountDetails.Add(typeof(EgoPayAccountDetails));
                paymentAccountDetails.Add(typeof(SolidTrustPayAccountDetails));
                paymentAccountDetails.Add(typeof(PayeerAccountDetails));
                paymentAccountDetails.Add(typeof(NetellerAccountDetails));
                paymentAccountDetails.Add(typeof(AdvCashAccountDetails));
                paymentAccountDetails.Add(typeof(PaparaAccountDetails));
                paymentAccountDetails.Add(typeof(MPesaAccountDetails));
                paymentAccountDetails.Add(typeof(LocalBitcoinsAccountDetails));
                paymentAccountDetails.Add(typeof(MPesaSapamaAccountDetails));
                paymentAccountDetails.Add(typeof(RevolutAccountDetails));
                return paymentAccountDetails;
            }
        }

        //Backward compatibility
        public static Type GetGatewayType(string gatewayType)
        {
            if ("Payza".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(PayzaAccountDetails);
            else if ("PayPal".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(PayPalAccountDetails);
            else if ("PerfectMoney".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(PerfectMoneyAccountDetails);
            else if ("OKPay".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(OKPayAccountDetails);
            else if ("EgoPay".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(EgoPayAccountDetails);
            else if ("Neteller".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(NetellerAccountDetails);
            else if ("SolidTrustPay".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(SolidTrustPayAccountDetails);
            else if ("Payeer".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(PayeerAccountDetails);
            else if ("AdvCash".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(AdvCashAccountDetails);
            else if ("Papara".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(PaparaAccountDetails);
            else if ("MPesa".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(MPesaAccountDetails);
            else if ("MPesaAgent".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(MPesaSapamaAccountDetails);
            else if ("LocalBitcoins".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(LocalBitcoinsAccountDetails);
            else if ("Revolut".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return typeof(RevolutAccountDetails);
            else
                throw new ArgumentOutOfRangeException("\"" + gatewayType + "\" not supported.");
        }

        //Backward compatibility
        public static PaymentAccountDetails GetGateway(int gatewayId, string gatewayType)
        {
            if ("Payza".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new PayzaAccountDetails(gatewayId);
            else if ("PayPal".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new PayPalAccountDetails(gatewayId);
            else if ("PerfectMoney".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new PerfectMoneyAccountDetails(gatewayId);
            else if ("OKPay".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new OKPayAccountDetails(gatewayId);
            else if ("EgoPay".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new EgoPayAccountDetails(gatewayId);
            else if ("Neteller".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new NetellerAccountDetails(gatewayId);
            else if ("SolidTrustPay".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new SolidTrustPayAccountDetails(gatewayId);
            else if ("Payeer".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new PayeerAccountDetails(gatewayId);
            else if ("AdvCash".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new AdvCashAccountDetails(gatewayId);
            else if ("Papara".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new PaparaAccountDetails(gatewayId);
            else if ("MPesa".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new MPesaAccountDetails(gatewayId);
            else if ("MPesaAgent".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new MPesaSapamaAccountDetails(gatewayId);
            else if ("LocalBitcoins".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new LocalBitcoinsAccountDetails(gatewayId);
            else if ("Revolut".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return new RevolutAccountDetails(gatewayId);
            else
                throw new ArgumentOutOfRangeException("\"" + gatewayType + "\" not supported.");
        }

        //Backward compatibility
        public static PaymentProcessor GetFromStringType(string gatewayType)
        {
            if ("Payza".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.Payza;
            else if ("PayPal".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.PayPal;
            else if ("PerfectMoney".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.PerfectMoney;
            else if ("OKPay".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.OKPay;
            else if ("EgoPay".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.EgoPay;
            else if ("Neteller".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.Neteller;
            else if ("SolidTrustPay".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.SolidTrustPay;
            else if ("Payeer".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.Payeer;
            else if ("AdvCash".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.AdvCash;
            else if ("Papara".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.Papara;
            else if ("MPesa".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.MPesa;
            else if ("MPesaAgent".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.MPesaAgent;
            else if ("LocalBitcoins".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.LocalBitcoins;
            else if ("ViaRepresentative".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.ViaRepresentative;
            else if ("Revolut".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.Revolut;
            else if ("CoinPayments".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.CoinPayments;
            else if ("Coinbase".Equals(gatewayType, StringComparison.OrdinalIgnoreCase))
                return PaymentProcessor.Coinbase;
            else
                throw new ArgumentOutOfRangeException("\"" + gatewayType + "\" not supported.");
        }

        #endregion Modify when adding new

        public static string GetPaymentProcessorUserAccount(Member User, string TargetPaymentProcessor)
        {
            PaymentProcessor processor = GetFromStringType(TargetPaymentProcessor);
            return User.GetPaymentAddress(processor);
        }


        #region Misc

        /// <summary>
        /// Get all gateways regardless of their type
        /// </summary>
        /// <remarks>Method could be very slow, so it's good habit to cache method's output</remarks>
        /// <exception cref="DbException" />
        public static List<PaymentAccountDetails> AllGateways
        {
            get
            {
                var subclasses = PaymentAccountDetailsSubclasses;
                MethodInfo selectAllRows = SelectAllRowsMethod;

                List<PaymentAccountDetails> accounts = new List<PaymentAccountDetails>();

                foreach (Type subclass in subclasses)
                {
                    var genericSelectAllRows = selectAllRows.MakeGenericMethod(subclass);
                    var result = genericSelectAllRows.Invoke(null, null) as IEnumerable<PaymentAccountDetails>;
                    if (result != null) accounts.AddRange(result);
                }

                return accounts;
            }
        }

        public static List<PaymentAccountDetails> AllUniqueGateways
        {
            get
            {
                return AllGateways
                    .GroupBy(gateway => gateway.AccountType)
                    .Select(gateway => gateway.First())
                    .ToList();
            }
        }

        public static List<PaymentAccountDetails> AvailableUniquePayoutGateways
        {
            get
            {
                return AllGateways
                    .Where(gateway => gateway.IsActive && (gateway.CashflowDirections == 1 || gateway.CashflowDirections == 3))
                    .GroupBy(gateway => gateway.AccountType)
                    .Select(gateway => gateway.First())
                    .ToList();
            }
        }

        private static MethodInfo SelectAllRowsMethod
        {
            get
            {
                return typeof(TableHelper).GetMethod("SelectAllRows");
            }
        }

        public static Type[] PaymentAccountDetailsClasses
        {
            get
            {
                return PaymentAccountDetailsSubclasses.ToArray();
            }
        }

        public static PaymentAccountDetails GetFirstGateway(string TargetPaymentProcessor, bool Income = false)
        {
            Type t = GetGatewayType(TargetPaymentProcessor);
            var instance = Activator.CreateInstance(t);

            var gatewayIncome = (PaymentAccountDetails)RunStaticMethod(t, "GetFirstIncomeGateway");
            var gatewayOutcome = (PaymentAccountDetails)RunStaticMethod(t, "GetFirstOutcomeGateway");

            return Income ? gatewayIncome : gatewayOutcome;
        }

        public static object RunStaticMethod(Type t, string methodName)
        {
            MethodInfo method = typeof(PaymentAccountDetails).GetMethod(methodName);
            MethodInfo generic = method.MakeGenericMethod(t);
            return generic.Invoke(null, null);
        }

        public static bool HasCashoutGateway<T>() where T : PaymentAccountDetails
        {
            var gatewayList = TableHelper.SelectAllRows<T>();

            foreach (var gateway in gatewayList)
            {
                if (gateway.IsActive && (gateway.Cashflow == GatewayCashflowDirection.FromGate ||
                    gateway.Cashflow == GatewayCashflowDirection.Both))
                {
                    return true;
                }
            }
            return false;
        }

        public static T GetFirstIncomeGateway<T>() where T : PaymentAccountDetails
        {
            var gatewayList = TableHelper.SelectAllRows<T>();
            T thegateway = null;

            foreach (var gateway in gatewayList)
            {
                if (gateway.IsActive && (gateway.Cashflow == GatewayCashflowDirection.ToGate ||
                    gateway.Cashflow == GatewayCashflowDirection.Both))
                {
                    thegateway = (T)gateway;
                    break;
                }
            }

            return thegateway;
        }

        public static T GetFirstOutcomeGateway<T>() where T : PaymentAccountDetails
        {
            var gatewayList = TableHelper.SelectAllRows<T>();
            T thegateway = null;

            foreach (var gateway in gatewayList)
            {
                if (gateway.IsActive && (gateway.Cashflow == GatewayCashflowDirection.FromGate ||
                    gateway.Cashflow == GatewayCashflowDirection.Both))
                {
                    thegateway = (T)gateway;
                    break;
                }
            }

            return thegateway;
        }

        public Money CalculateAmountWithFee(Money amount)
        {
            return amount + StaticFee + amount * (PercentFee / 100);
        }

        public Money CalculateAmountWithoutFee(Money amount)
        {
            return Money.Parse(((amount - StaticFee).ToDecimal() / (1 + (PercentFee / 100))).ToString());
        }

        public static Money GetAmountWithoutFee(string from, Money money)
        {
            return GetFirstGateway(from, true).CalculateAmountWithoutFee(money);
        }

        public static bool AreIncomingPaymentProcessorsAvailable()
        {
            var objectCache = new AreIncomingPaymentProcessorsAvailableCache();
            return (bool)objectCache.Get();
        }
        #endregion Misc
    }

    public class PaymentComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return y.GetType().ToString().CompareTo(x.GetType().ToString());
        }
    }
}