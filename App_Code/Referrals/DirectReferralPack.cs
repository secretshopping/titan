using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Members;
using Resources;
using MarchewkaOne.Titan.Balances;
using System.Text;

namespace Prem.PTC.Referrals
{

    public class DirectReferralPack : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "DirectReferralPacks"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Price")]
        public Money Price { get { return _Price; } set { _Price = value; SetUpToDateAsFalse(); } }

        [Column("NumberOfRefs")]
        public int NumberOfRefs { get { return _NumberOfRefs; } set { _NumberOfRefs = value; SetUpToDateAsFalse(); } }

        [Column("Days")]
        public int Days { get { return _Days; } set { _Days = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        public int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

        [Column("MembershipId")]
        public int MembershipId { get { return _MembershipId; } set { _MembershipId = value; SetUpToDateAsFalse(); } }

        public UniversalStatus Status
        {
            get { return (UniversalStatus)StatusInt; }
            set { StatusInt = (int)value; }
        }

        int _id, _NumberOfRefs, _Days, _Status, _MembershipId;
        Money _Price;

        #endregion Columns

        public DirectReferralPack()
            : base() { }

        public DirectReferralPack(int id) : base(id) { }

        public DirectReferralPack(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    }

    public static class DirectReferralPackManager
    {
        /// <summary>
        /// Returns active packs with available NumberOfRefs
        /// </summary>
        /// <returns></returns>
        public static List<DirectReferralPack> GetAvailablePacks()
        {
            if (AppSettings.DirectReferrals.DirectReferralMembershipPacksEnabled)
            {
                var cache = new DirectReferralPackCache();
                return (List<DirectReferralPack>)cache.Get();
            }
            else
            {
                // Cached for better performance
                var count = (int)new DirectReferralPackMembersCache().Get();
                return TableHelper.GetListFromRawQuery<DirectReferralPack>(string.Format(@"SELECT * FROM DirectReferralPacks WHERE Status = {0} AND NumberOfRefs <= {1}", (int)UniversalStatus.Active, count));
            }        
        }

        public static List<Member> GetUsersWithoutReferer(int numberOfRefs, int requestingUserId, int membershipId)
        {
            var sb = new StringBuilder();

            if (membershipId == 0)                          
                sb.Append(string.Format(@"SELECT TOP {0} * FROM Users WHERE ReferrerId = -1 AND AccountStatusInt = {1} AND UserId != 1005 AND UserId != {2}",
                    numberOfRefs, (int)MemberStatus.Active, requestingUserId));            
            else
                sb.Append(string.Format(@"SELECT TOP {0} * FROM Users WHERE ReferrerId = -1 AND AccountStatusInt = {1} AND UserId != 1005 AND UserId != {2} AND UpgradeId = {3};",
                    numberOfRefs, (int)MemberStatus.Active, requestingUserId, membershipId));

            if (AppSettings.DirectReferrals.DefaultReferrerEnabled)
                sb.Append(string.Format(" AND UserId != {0}", AppSettings.DirectReferrals.DefaultReferrerId));

            return TableHelper.GetListFromRawQuery<Member>(sb.ToString());
        }

        public static void BuyPack(DirectReferralPack pack, Member user, PurchaseBalances targetBalance)
        {
            if (user.DirectReferralLimit < user.GetDirectReferralsCount() + pack.NumberOfRefs)
                throw new MsgException(L1.ER_RENT_LIMIT);

            var membersWithoutRefs = GetUsersWithoutReferer(pack.NumberOfRefs, user.Id, pack.MembershipId);

            if (membersWithoutRefs.Count < pack.NumberOfRefs)
                throw new MsgException(U5007.NOTENOUGHREFSAVAILABLE);

            PurchaseOption.ChargeBalance(user, pack.Price, PurchaseOption.Features.DirectReferral.ToString(), 
                targetBalance, "Direct ref purchase", BalanceLogType.DirectRefPurchase);

            foreach (Member member in membersWithoutRefs)
            {
                member.TryAddReferer(user, true, AppSettings.ServerTime.AddDays(pack.Days));
                member.CameFromUrl = "Purchase";
                member.Save();
            }
        }

        public static void ResetMembershipForAllPacks()
        {
            var query = "SELECT * FROM DirectReferralPacks";
            var packsList = TableHelper.GetListFromRawQuery<DirectReferralPack>(query);

            foreach(var pack in packsList)
            {
                pack.MembershipId = 0;
                pack.Save();
            }
        }

        public static void RemoveDeletedMembershipFromPacks(int removedId)
        {
            var query = "SELECT * FROM DirectReferralPacks";
            var packsList = TableHelper.GetListFromRawQuery<DirectReferralPack>(query);

            foreach (var pack in packsList.FindAll(x => x.MembershipId == removedId))
            {
                pack.MembershipId = 0;
                pack.Save();
            }
        }
    }
}