using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Prem.PTC.Memberships;
using MarchewkaOne.Titan.Balances;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        [Column("S4DSPackages")]
        public int S4DSPackages { get { return _S4DSPackages; } set { _S4DSPackages = value; SetUpToDateAsFalse(); } }


        [Column("FirstActiveDayOfAdPacks")]
        public DateTime? FirstActiveDayOfAdPacks { get { return _FirstActiveDayOfAdPacks; } set { _FirstActiveDayOfAdPacks = value; SetUpToDateAsFalse(); } }

        [Column("AdPackViewedCounter")]
        public int AdPackViewedCounter { get { return _AdPackViewedCounter; } set { _AdPackViewedCounter = value; SetUpToDateAsFalse(); } }


        private int _S4DSPackages, _AdPackViewedCounter;
        private DateTime? _FirstActiveDayOfAdPacks;

        private PropertyInfo[] buildCustomFeatures()
        {
            PropertyBuilder<Member> paymentsValues = new PropertyBuilder<Member>();
            paymentsValues
                .Append(x => x.S4DSPackages)
                .Append(x => x.FirstActiveDayOfAdPacks)
                .Append(x => x.AdPackViewedCounter);

            return paymentsValues.Build();
        }

        //TradeOwnSystem A variable
        public bool CommissionToMainBalanceCondition
        {
            get
            {
                int DaysOfActiveAdPacks = 1;
                if (FirstActiveDayOfAdPacks != null)
                    DaysOfActiveAdPacks = (int)(DateTime.Now.Subtract(Convert.ToDateTime(FirstActiveDayOfAdPacks))).TotalDays + 1;
                
                return (AdPackViewedCounter >= Membership.AdPackDailyRequiredClicks * DaysOfActiveAdPacks);
            }
        }

        //TradeOwnSystem Able to Transfer
        public bool CommissionToMainBalanceAllowForTransfer
        {
            get
            {
                if (AdPackManager.GetUsersActiveAdPacks(Id).Count > 0 && CommissionToMainBalanceCondition)
                    return true;
                return false;
            }
        }

        //TradeOwnSystem C variable
        public int CommissionToMainBalanceRequiredViewsMessageInt
        {
            get
            {
                int DaysOfActiveAdPacks = 1;
                if (FirstActiveDayOfAdPacks != null)
                    DaysOfActiveAdPacks = (int)(DateTime.Now.Subtract(Convert.ToDateTime(FirstActiveDayOfAdPacks))).TotalDays + 1;
                var temp = AdPackManager.GetUsersActiveAdPacks(Id).Count;

                if (CommissionToMainBalanceAllowForTransfer)
                    return -1;
                else if (AdPackManager.GetUsersActiveAdPacks(Id).Count == 0 && CommissionToMainBalanceCondition)
                    return 0;
                else if (!CommissionToMainBalanceCondition)
                    return Membership.AdPackDailyRequiredClicks * DaysOfActiveAdPacks - AdPackViewedCounter;
                else
                    return Membership.AdPackDailyRequiredClicks;
            }
        }

        public bool CheckAccessCustomizeTradeOwnSystem
        {
            get
            {
                if (TitanFeatures.IsTradeOwnSystem && CommissionToMainBalanceAllowForTransfer
                   || !TitanFeatures.IsTradeOwnSystem)
                    return true;
                return false;
            }
        }

        public void SaveCustomFeatures()
        {
            SavePartially(IsUpToDate, buildCustomFeatures());
        }
    }
}