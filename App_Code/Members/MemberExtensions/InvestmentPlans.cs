using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Prem.PTC.Utils;

namespace Prem.PTC.Members
{
    public partial class Member : BaseTableObject
    {
        [Column(Columns.InvestedIntoPlans)]
        public Money InvestedIntoPlans { get { return _InvestedIntoPlans; } set { _InvestedIntoPlans = value; SetUpToDateAsFalse(); } }

        [Column(Columns.InvestedIntoPlansStructure)]
        public Money InvestedIntoPlansStructure { get { return _InvestedIntoPlansStructure; } set { _InvestedIntoPlansStructure = value; SetUpToDateAsFalse(); } }

        private Money _InvestedIntoPlans, _InvestedIntoPlansStructure;


        #region Helpers

        public void SaveInvestmentPlans()
        {
            PropertyInfo[] propertiesToSave = BuildInvestmentPlans();

            SavePartially(IsUpToDate, propertiesToSave);
        }

        public void ReloadInvestmentPlans()
        {
            PropertyInfo[] propertiesToReload = BuildInvestmentPlans();

            ReloadPartially(IsUpToDate, propertiesToReload);
        }

        private PropertyInfo[] BuildInvestmentPlans()
        {
            PropertyBuilder<Member> builder = new PropertyBuilder<Member>();

            builder.Append(x => x.InvestedIntoPlans)
                   .Append(x => x.InvestedIntoPlansStructure);

            return builder.Build();
        }

        #endregion
    }
}