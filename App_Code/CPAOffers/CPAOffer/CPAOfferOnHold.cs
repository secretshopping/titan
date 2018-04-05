using System;
using System.Data;

namespace Prem.PTC.Offers
{

    public class CPAOfferOnHold : CPAOfferBase
    {
        public static new string TableName { get { return "CPAOffersOnHold"; } }
        protected override string dbTable { get { return TableName; } }

        public CPAOfferOnHold()
            : base()
        { }

        public CPAOfferOnHold(int id)
            : base(id)
        { }

        public CPAOfferOnHold(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        { }

        public void Save(bool forceSave = false)
        {
            base.Save(forceSave);
        }

        public static void Delete(int id)
        {
            TableHelper.DeleteRows<CPAOfferOnHold>("id", id);
        }

        #region Actions
        public static bool ExistsAffiliateNetworkOffer(string networkOfferId, string networkName)
        {
            int Count = (int)TableHelper.SelectScalar(String.Format(@"SELECT COUNT(*) FROM {0} WHERE NetworkOfferIdInt='{1}' AND NetworkName='{2}'", TableName, networkOfferId, networkName));
            return Count >= 1;
        }

        #endregion
    }
}