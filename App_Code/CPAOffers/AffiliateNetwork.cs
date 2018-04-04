using System;
using System.Data;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using Titan;

namespace Prem.PTC.Offers
{
    public class AffiliateNetwork : BaseTableObject
    {
        public AffiliateNetwork() : base() { }
        public AffiliateNetwork(int id) : base(id) { }
        public AffiliateNetwork(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #region Columns
        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "NewAffiliateNetworks"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("DisplayName")]
        public string Name { get { return a1; } set { a1 = value; SetUpToDateAsFalse(); } }

        [Column("Hash")]
        public string Hash { get { return _Hash; } set { _Hash = value; SetUpToDateAsFalse(); } }

        [Column("CreditAs")]
        protected int IntCreditAs { get { return _CreditAs; } set { _CreditAs = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int IntStatus { get { return status; } set { status = value; SetUpToDateAsFalse(); } }

        [Column("WhatIsSent")]
        protected int IntWhatIsSent { get { return _IntWhatIsSent; } set { _IntWhatIsSent = value; SetUpToDateAsFalse(); } }

        [Column("RestrictedIPs")]
        public string RestrictedIPs { get { return a3; } set { a3 = value; SetUpToDateAsFalse(); } }

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

        /// <summary>
        /// Indicates wheather the Network uses postback
        /// </summary>
        [Column("IsPostbackMode")]
        public bool IsPostbackMode { get { return q3; } set { q3 = value; SetUpToDateAsFalse(); } }

        [Column("HasRestrictedIPs")]
        public bool HasRestrictedIPs { get { return q4; } set { q4 = value; SetUpToDateAsFalse(); } }

        //ONLY FOR PERFORMA

        /// <summary>
        /// For Performa-based networks
        /// </summary>
        [Column("PublisherUsername")]
        public string PublisherUsername { get { return u1; } set { u1 = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// For Performa-based networks
        /// </summary>
        [Column("PublisherPassword")]
        public string PublisherPassword { get { return u2; } set { u2 = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// For Performa-based networks
        /// </summary>
        [Column("WebsiteID")]
        public string WebsiteID { get { return u3; } set { u3 = value; SetUpToDateAsFalse(); } }

        /// <summary>
        /// For Performa-based networks
        /// </summary>
        [Column("PercentForMembers")]
        public int DefaultPercentForMembers { get { return sss; } set { sss = value; SetUpToDateAsFalse(); } }

        [Column("AffiliateNetworkSoftwareTypeInt")]
        private int AffiliateNetworkSoftwareTypeInt { get { return _AffiliateNetworkSoftwareTypeInt; } set { _AffiliateNetworkSoftwareTypeInt = value; SetUpToDateAsFalse(); } }

        [Column("LastSyncDate")]
        public DateTime? LastSyncDate { get { return dt; } set { dt = value; SetUpToDateAsFalse(); } }

        [Column("VarUsername")]
        public string VariableNameOfUsername { get { return s1; } set { s1 = value; SetUpToDateAsFalse(); } }

        [Column("VarBalance")]
        public string VariableNameOfBalance { get { return s2; } set { s2 = value; SetUpToDateAsFalse(); } }

        [Column("VarTrackingNo")]
        public string VariableNameOfOfferName { get { return s3; } set { s3 = value; SetUpToDateAsFalse(); } }

        [Column("VarOfferId")]
        public string VariableNameOfOfferId { get { return s33; } set { s33 = value; SetUpToDateAsFalse(); } }

        [Column("VarWebsiteId")]
        public string VariableNameOfWebsiteId { get { return s34; } set { s34 = value; SetUpToDateAsFalse(); } }

        [Column("VarMemberIP")]
        public string VariableNameOfMemberIP { get { return s35; } set { s35 = value; SetUpToDateAsFalse(); } }

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

        private bool q1, q2, q3, q4, q5, q6, _RequiresConversion;
        private int _id, sss, _CreditAs, status, _AffiliateNetworkSoftwareTypeInt, _IntWhatIsSent;
        private string a1, a2, a3, u1, u2, u3, _Hash, s1, s2, s3, s4, s5, s6, s7, s8, s9, s33, s34, s35;
        private DateTime? dt;

        public string PerformaPassword
        {
            get
            {
                return PublisherPassword;
            }
            set
            {
                PublisherPassword = value;
            }
        }

        /// <summary>
        /// ==CPALead ID for CPALead software types
        /// </summary>
        public string PerformaUsername
        {
            get
            {
                return PublisherUsername;
            }
            set
            {
                PublisherUsername = value;
            }
        }

        public NetworkStatus Status
        {
            get
            {
                return (NetworkStatus)IntStatus;
            }

            set
            {
                IntStatus = (int)value;
            }
        }

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

        public string PerformaWebsiteId
        {
            get
            {
                return WebsiteID;
            }
            set
            {
                WebsiteID = value;
            }
        }

        public AffiliateNetworkSoftwareType AffiliateNetworkSoftwareType
        {
            get
            {
                return (AffiliateNetworkSoftwareType)AffiliateNetworkSoftwareTypeInt;
            }

            set
            {
                AffiliateNetworkSoftwareTypeInt = (int)value;
            }
        }
        #endregion Columns

        public string GetHandlerURL()
        {
            string normalHandler = AppSettings.Site.Url + CPAFileManager.HANDLER_FOLDER + this.Hash + CPAFileManager.HANDLER_EXTENSION;

            if (this.AffiliateNetworkSoftwareType == AffiliateNetworkSoftwareType.Performa)
                return normalHandler + "?cid=%campaignid%&yti=%yti%&credit=%credit%&wid=%websiteid%&rate=%rate%&ip=%ip%&cn=%campaignname%";

            if (this.AffiliateNetworkSoftwareType == AffiliateNetworkSoftwareType.AdGateMedia)
                return normalHandler + "?s={status}&p={payout}&ip={session_ip}&on={offer_name}&so={aff_sub}&id={offer_id}";

            if (this.AffiliateNetworkSoftwareType == AffiliateNetworkSoftwareType.CPALead)
                return normalHandler + "?survid={campaign_id}&survey={campaign_name}&subid={subid}&earn={payout}&userip={ip_address}";

            if (this.AffiliateNetworkSoftwareType == AffiliateNetworkSoftwareType.HasOffers)
                return normalHandler + "?offer_id={offer_id}&offer_name={offer_name}&aff_sub={aff_sub}&earn={payout}&session_ip={session_ip}";

            return normalHandler;
        }

        public static ListItem[] ListItems
        {
            get
            {
                var list = TableHelper.SelectRows<AffiliateNetwork>(TableHelper.MakeDictionary("Status", (int)NetworkStatus.Active));

                var query = from AffiliateNetwork cat in list
                            orderby cat.Name
                            select new ListItem(cat.Name, cat.Name);

                return query.ToArray();
            }
        }

        public static ListItem[] AllListItems
        {
            get
            {
                var list = TableHelper.GetListFromRawQuery<AffiliateNetwork>("SELECT * FROM NewAffiliateNetworks WHERE Status IN (1, 2)");

                var query = from AffiliateNetwork cat in list
                            orderby cat.Name
                            select new ListItem(cat.Name, cat.Name);

                return query.ToArray();
            }
        }

        public static ListItem[] AllActiveListItems
        {
            get
            {
                var query = from string networkName in AllActiveNetworkNames
                            orderby networkName
                            select new ListItem(networkName, networkName);

                return query.ToArray();
            }
        }

        public static List<string> AllActiveNetworkNames
        {
            get
            {
                return TableHelper.GetStringListFromRawQuery("SELECT DisplayName FROM NewAffiliateNetworks WHERE Status = 1");
            }
        }
    }

    public enum AffiliateNetworkSoftwareType
    {
        Null = 0,
        Universal = 1,
        Performa = 2,
        CPALead = 3,
        AdGateMedia = 4,
        PointClickTrack = 5,
        ProLeadsMedia = 6,
        HasOffers = 7,
        RedFireNetwork = 8
    }
}