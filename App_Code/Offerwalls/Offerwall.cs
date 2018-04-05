using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using System.Data;
using Prem.PTC.Members;

namespace Titan
{
    [Serializable]
    public class Offerwall : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "Offerwalls"; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "Id";
            public const string OrderNumber = "OrderNumber";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int IntStatus { get { return status; } set { status = value; SetUpToDateAsFalse(); } }

        [Column("ClientHTML")]
        public string ClientHTML { get { return _ClientHTML; } set { _ClientHTML = value; SetUpToDateAsFalse(); } }

        [Column("DisplayName")]
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; SetUpToDateAsFalse(); } }

        [Column("IsIncludedInPTCContest")]
        public bool IsIncludedInPTCContest { get { return _IsIncludedInPTCContest; } set { _IsIncludedInPTCContest = value; SetUpToDateAsFalse(); } }

        [Column("Hash")]
        public string Hash { get { return _Hash; } set { _Hash = value; SetUpToDateAsFalse(); } }

        [Column("CreditAs")]
        protected int IntCreditAs { get { return _CreditAs; } set { _CreditAs = value; SetUpToDateAsFalse(); } }


        public bool RequiresConversion
        {
            get
            {
                if (CreditAs == Titan.CreditAs.MainBalance && WhatIsSent == Titan.WhatIsSent.Money)
                    return false;

                if (CreditAs == Titan.CreditAs.Points && WhatIsSent == Titan.WhatIsSent.Points)
                    return false;

                return true;
            }
        }

        [Column("WhatIsSent")]
        protected int IntWhatIsSent { get { return _IntWhatIsSent; } set { _IntWhatIsSent = value; SetUpToDateAsFalse(); } }

        [Column("HasRestrictedIPs")]
        public bool HasRestrictedIPs { get { return _HasRestrictedIPs; } set { _HasRestrictedIPs = value; SetUpToDateAsFalse(); } }

        [Column("RestrictedIPs")]
        public string RestrictedIPs { get { return _RestrictedIPs; } set { _RestrictedIPs = value; SetUpToDateAsFalse(); } }

        //Postback

        [Column("VarUsername")]
        public string VariableNameOfUsername { get { return s1; } set { s1 = value; SetUpToDateAsFalse(); } }

        [Column("VarBalance")]
        public string VariableNameOfBalance { get { return s2; } set { s2 = value; SetUpToDateAsFalse(); } }

        [Column("VarTrackingNo")]
        public string VariableNameOfTrackingInfo { get { return s3; } set { s3 = value; SetUpToDateAsFalse(); } }

        [Column("VarType")]
        public string VariableNameOfType { get { return s4; } set { s4 = value; SetUpToDateAsFalse(); } }

        [Column("VarTypeCredit")]
        public string VariableValueOfTypeCredited { get { return s5; } set { s5 = value; SetUpToDateAsFalse(); } }

        [Column("VarTypeReverse")]
        public string VariableValueOfTypeReversed { get { return s6; } set { s6 = value; SetUpToDateAsFalse(); } }

        [Column("VarSignature")]
        public string VariableNameOfSignature { get { return s7; } set { s7 = value; SetUpToDateAsFalse(); } }

        [Column("SignatureCondition")]
        public string SIgnatureCondition { get { return s8; } set { s8 = value; SetUpToDateAsFalse(); } }

        [Column("OKResponse")]
        public string ValueOfSuccessfulResponse { get { return s9; } set { s9 = value; SetUpToDateAsFalse(); } }

        [Column("CreditPercentage")]
        public int CreditPercentage { get { return _CreditPercentage; } set { _CreditPercentage = value; SetUpToDateAsFalse(); } }

        [Column("UseVirtualCurrencySetting")]
        public bool UseVirtualCurrencySetting { get { return _UseVirtualCurrencySetting; } set { _UseVirtualCurrencySetting = value; SetUpToDateAsFalse(); } }

        [Column("MoneyToPointsRate")]
        public Money MoneyToPointsRate { get { return _MoneyToPointsRate; } set { _MoneyToPointsRate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.OrderNumber)]
        protected int? OrderNumber { get { return _OrderNumber; } set { _OrderNumber = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// All offerwalls currency
        /// </summary>
        public string CurrencyCode { get { return "USD"; } }

        private int? _OrderNumber;
        private int _id, status, _CreditAs, _IntWhatIsSent, _CreditPercentage;
        private string _ClientHTML, _DisplayName, _Hash, _RestrictedIPs, s1, s2, s3, s4, s5, s6, s7, s8, s9;
        private bool _RequiresConversion, _HasRestrictedIPs, _IsIncludedInPTCContest, _UseVirtualCurrencySetting;
        private Money _MoneyToPointsRate;

        #endregion Columns

        #region Constructors

        public Offerwall() : base()
        {
            OrderNumber = GetDisplayingOrderNumber();
        }

        public Offerwall(int id) : base(id) { }

        public Offerwall(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

        public Offerwall(string offerwallName) : this(GetOfferwallId(offerwallName))
        {

        }
        
        #endregion Constructors

        #region Properties


        public WhatIsSent WhatIsSent
        {
            get
            {
                return (WhatIsSent)IntWhatIsSent;
            }

            set
            {
                IntWhatIsSent = (int)value;
            }
        }

        public OfferwallStatus Status
        {
            get
            {
                return (OfferwallStatus)IntStatus;
            }

            set
            {
                IntStatus = (int)value;
            }
        }

        public CreditAs CreditAs
        {
            get
            {
                return (CreditAs)IntCreditAs;
            }

            set
            {
                IntCreditAs = (int)value;
            }
        }

        #endregion Properties

        private static int GetDisplayingOrderNumber()
        {
            try
            {
                var query = string.Format("SELECT MAX(OrderNumber) FROM {0} ", TableName);

                return (int)TableHelper.SelectScalar(query) + 1;
            }
            catch(Exception e)
            {
                //No offerwalls
                return 1;
            }
        }

        private static int GetOfferwallId(string offerwallName)
        {
            var result = TableHelper.SelectRows<Offerwall>(TableHelper.MakeDictionary("DisplayName", offerwallName));

            if (result.Count == 0)
                throw new MsgException("Offerwall with a specified name has not been found");

            return result[0].Id;
        }

        public string ToClientHTML(Member User)
        {
            return OfferwallParser.ParseHTMLCode(ClientHTML, User);

        }

        public string GetHandlerURL()
        {
            return AppSettings.Site.Url + OfferwallFileManager.HANDLER_FOLDER + this.Hash + OfferwallFileManager.HANDLER_EXTENSION;
        }
    }
}