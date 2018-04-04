using System.Data;
using Prem.PTC;
using Prem.PTC.Members;

namespace Titan.PaidToPromote
{
    public class PaidToPromoteUser : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "PaidToPromoteUsers"; } }
        protected override string dbTable { get { return TableName; } }

        #region Columns
        public static class Columns
        {
            public const string Id = "Id";
            public const string UserId = "UserId";
            public const string ClicksDelivered  = "ClicksDelivered";
            public const string CreditedCPM = "CreditedCPM";
            public const string ReferralsDelivered  = "ReferralsDelivered";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }        

        [Column(Columns.UserId)]
        public int UserId { get { return _userId; } set { _userId = value; SetUpToDateAsFalse(); } }
        
        [Column(Columns.ClicksDelivered)]
        public int ClicksDelivered { get { return _clicksDelivered; } set { _clicksDelivered = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CreditedCPM)]
        public int CreditedCPM { get { return _creditedCPM; } set { _creditedCPM = value; SetUpToDateAsFalse(); } }

        [Column(Columns.ReferralsDelivered)]
        public int ReferralsDelivered { get { return _referralsDelivered; } set { _referralsDelivered = value; SetUpToDateAsFalse(); } }

        private int _id, _userId, _clicksDelivered, _creditedCPM, _referralsDelivered;
        #endregion

        public PaidToPromoteUser() : base() { }

        public PaidToPromoteUser(int id) : base(id) { }

        public PaidToPromoteUser(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public void InceaseClicks()
        {
            ClicksDelivered++;

            if(ClicksDelivered / 1000 > CreditedCPM)
            {
                CreditedCPM++;
                var user = new Member(UserId);

                user.AddToMainBalance(AppSettings.PaidToPromote.CostPerMillePrice, "PaidToPromote CPM");
                user.SaveBalances();
            }

            Save();
        }
    }
}