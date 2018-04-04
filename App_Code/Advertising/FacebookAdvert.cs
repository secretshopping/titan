using System;
using System.Data;

namespace Prem.PTC.Advertising
{
    [Serializable]
    public class FacebookAdvert : Advert<IFacebookAdvertPack>
    {
        #region Columns

        protected override string dbTable { get { return TableName; } }
        public static new string TableName { get { return "FacebookAdverts"; } }

        public static new class Columns
        {
            public const string Id = "FbAdvertId";
            public const string AdvertPackId = "FbAdvertPackId";
            public const string MinFriends = "MinFriends";
            public const string HasProfilePicRestrictions = "HasProfilePicRestrictions";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Advert.Columns.TargetUrl)]
        public override string TargetUrl
        {
            get { return base.TargetUrl; }
            set
            {
                base.TargetUrl = value;
            }
        }

        [Column(Columns.AdvertPackId)]
        protected override int? AdvertPackId { get { return _advertPackId; } set { _advertPackId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.MinFriends)]
        public int MinFriends { get { return _minFriends; } set { _minFriends = value; SetUpToDateAsFalse(); } }

        [Column(Columns.HasProfilePicRestrictions)]
        public bool HasProfilePicRestrictions { get { return _hasProfilePicRestrictions; } set { _hasProfilePicRestrictions = value; SetUpToDateAsFalse(); } }

        private bool _hasProfilePicRestrictions;
        private int _id, _minFriends;
        private int? _advertPackId;

        #endregion Columns


        /// <summary>
        /// Sets all properties of pack except from Price
        /// </summary>
        public override IFacebookAdvertPack Pack
        {
            get { return (AdvertPackId != null) ? new FacebookAdvertPack(AdvertPackId.Value) : null; }
            set
            {
                AdvertPackId = value.Id;
                Ends = value.Ends;
            }
        }

        public override void Click()
        {
            base.Click();

            //Increase global statistics
            var stat = new Prem.PTC.Statistics.Statistics(Prem.PTC.Statistics.StatisticsType.FacebookClicks);
            stat.AddToData1(1);
            stat.Save();
        }

        public override void Unclick()
        {
            base.Unclick();
            var stat = new Prem.PTC.Statistics.Statistics(Prem.PTC.Statistics.StatisticsType.FacebookClicks);
            stat.AddToData1(-1);
            stat.Save();
        }

        #region Constructors

        /// <summary>
        /// Creates blank instance of FacebookAdvert class
        /// </summary>
        public FacebookAdvert()
            : base()
        {
            AdvertPackId = NotInDatabaseId;
        }

        public FacebookAdvert(int id) : base(id) { }

        public FacebookAdvert(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        #endregion Constructors

        public static bool IsFanpageInDatabase(string url)
        {
            int result = (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM FacebookAdverts WHERE TargetUrl = '{0}'", url));
            return result > 0;
        }
    }
}