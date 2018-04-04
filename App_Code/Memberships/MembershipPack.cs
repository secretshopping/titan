using Prem.PTC.Members;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Prem.PTC.Memberships
{
    public class MembershipPack : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return AppSettings.TableNames.MembershipPacks; } }
        protected override string dbTable { get { return TableName; } }

        public static class Columns
        {
            public const string Id = "MembershipPackId";
            public const string MembershipId = "MembershipId";
            public const string TimeDays = "TimeDays";
            public const string Price = "Price";
            public const string CopiesBought = "CopiesBought";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MembershipId)]
        public int MembershipId { get { return _membershipId; } set { _membershipId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.TimeDays)]
        public int TimeDays { get { return _timeDays; } set { _timeDays = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CopiesBought)]
        public int CopiesBought { get { return _CopiesBought; } set { _CopiesBought = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Price)]
        public Money Price { get { return _price; } set { _price = value; SetUpToDateAsFalse(); } }

        private int _id, _membershipId, _timeDays, _CopiesBought;
        private Money _price;

        #endregion


        #region Constructors

        public MembershipPack() : base() { }
        public MembershipPack(int id) : base(id) { }
        public MembershipPack(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion

        public Money GetPrice(Member user)
        {
            if (!IsProgressiveUpdate(user))
                return this.Price;

            int RemainingPackDays = Convert.ToInt32(user.MembershipExpires.Value.Subtract(AppSettings.ServerTime).TotalDays) - 1;

            if (RemainingPackDays < 0)
                RemainingPackDays = 0;

            Money Discount = RemainingPackDays * Prem.PTC.Memberships.Membership.GetPricePerDay(user.MembershipId);
            Money Result = this.Price - Discount;

            return Result >= Money.Zero ? Result : Money.Zero;
        }

        public bool IsProgressiveUpdate(Member user)
        {
            if (AppSettings.Addons.IsProgressiveUpgradeEnabled && user.MembershipId != Prem.PTC.Memberships.Membership.Standard.Id &&
                user.Membership.DisplayOrder < this.Membership.DisplayOrder)
            {

                return true;
            }
            return false;
        }

        public void SavePrice()
        {
            var builder = new PropertyBuilder<MembershipPack>();
            SavePartially(IsUpToDate, builder.Append(x => x.Price).Build());
        }

        public IMembership Membership
        {
            get
            {
                var cache = (List<Membership>)(new MembershipsAllStatusCache()).Get();
                return cache.Where(membership => membership.Id == MembershipId).First();
            }
            set { MembershipId = value.Id; }
        }

        public Money RenewalPrice
        {
            get { return new Money(Price.ToDecimal() * ((decimal)(100 - Membership.RenewalDiscount) / 100)); }
        }


        /// <exception cref="DbException" />
        public static List<MembershipPack> SelectPacksAssignedToMembership(int MembershipId)
        {
            var whereMembershipId = TableHelper.MakeDictionary(MembershipPack.Columns.MembershipId, MembershipId);

            return TableHelper.SelectRows<MembershipPack>(whereMembershipId);
        }

        /// <exception cref="DbException" />
        public static List<MembershipPack> SelectPacksWithTheSameDuration(int TimeDays)
        {
            var whereTimeDays = TableHelper.MakeDictionary(MembershipPack.Columns.TimeDays, TimeDays);

            return TableHelper.SelectRows<MembershipPack>(whereTimeDays);
        }

        /// <summary>
        /// TimeSpan is the argument however only days are taken into consideration
        /// </summary>
        /// <exception cref="DbException" />
        public static List<MembershipPack> SelectPacksWithTheSameDuration(TimeSpan TimeDays)
        {
            return SelectPacksWithTheSameDuration((int)TimeDays.TotalDays);
        }

        /// <summary>
        /// Returns sorted list of all durations
        /// </summary>
        /// <exception cref="DbException" />
        public static List<int> SelectAllDurations
        {
            get
            {
                DataTable durations;
                using (var bridge = ParserPool.Acquire(Database.Client))
                {
                    durations = bridge.Instance.Select(MembershipPack.TableName, Parser.Columns(MembershipPack.Columns.TimeDays), null);
                }

                var timeDaysUniqueList = (from DataRow duration in durations.Rows
                                          select duration.Field<int>(MembershipPack.Columns.TimeDays))
                                            .GroupBy(timeDays => timeDays)
                                            .Select(group => group.First())
                                            .OrderBy(timeDays => timeDays);

                return timeDaysUniqueList.ToList();
            }
        }

        /// <exception cref="DbException" />
        public static void DeletePacksWithDuration(int timeDays)
        {
            var whereTimeDays = TableHelper.MakeDictionary(MembershipPack.Columns.TimeDays, timeDays);

            TableHelper.DeleteRows(MembershipPack.TableName, whereTimeDays);
        }

        public static List<MembershipPack> AllPacks
        {
            get
            {
                var query = @"SELECT m.* FROM MembershipPacks AS m 
                              JOIN Memberships AS p on p.MembershipId = m.MembershipId
                              ORDER BY p.DisplayOrder, m.TimeDays";
                return TableHelper.GetListFromRawQuery<MembershipPack>(query);
            }
        }        

        public static List<MembershipPack> AllPurchaseablePacks
        {
            get
            {
                return MembershipPack.AllPacks
                    .Where(pack => pack.MembershipId != Memberships.Membership.Standard.Id && pack.Membership.Status == MembershipStatus.Active).ToList();             
            }
        }
    }
}