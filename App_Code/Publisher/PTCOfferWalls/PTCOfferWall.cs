using MarchewkaOne.Titan.Balances;
using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Prem.PTC.Offers;
using Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Titan.Matrix;

namespace Titan.Publish.PTCOfferWalls
{
    public class PTCOfferWall : GeolocationBase
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "PTCOfferWalls"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return _UserId; } protected set { _UserId = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

        [Column("CompletionTimes")]
        public int CompletionTimes { get { return _CompletionTimes; } protected set { _CompletionTimes = value; SetUpToDateAsFalse(); } }

        [Column("CompletionTimesBought")]
        public int CompletionTimesBought { get { return _CompletionTimesBought; } protected set { _CompletionTimesBought = value; SetUpToDateAsFalse(); } }

        [Column("PricePaid")]
        public Money PricePaid { get { return _PricePaid; } protected set { _PricePaid = value; SetUpToDateAsFalse(); } }

        [Column("DisplayTime")]
        public int DisplayTime
        {
            get { return _DisplayTime; }
            protected set { _DisplayTime = value; SetUpToDateAsFalse(); }
        }

        [Column("Adverts")]
        public int Adverts
        {
            get { return _Adverts; }
            protected set { _Adverts = value; SetUpToDateAsFalse(); }
        }

        [Column("Title")]
        public string Title
        {
            get { return _Title; }
            protected set { _Title = value; SetUpToDateAsFalse(); }
        }

        [Column("Description")]
        public string Description
        {
            get { return _Description; }
            protected set { _Description = value; SetUpToDateAsFalse(); }
        }

        [Column("PCAllowed")]
        public bool PCAllowed
        {
            get { return _PCAllowed; }
            protected set { _PCAllowed = value; SetUpToDateAsFalse(); }
        }

        [Column("MobileAllowed")]
        public bool MobileAllowed
        {
            get { return _MobileAllowed; }
            protected set { _MobileAllowed = value; SetUpToDateAsFalse(); }
        }

        [Column("AutosurfEnabled")]
        public bool AutosurfEnabled
        {
            get { return _AutosurfEnabled; }
            protected set { _AutosurfEnabled = value; SetUpToDateAsFalse(); }
        }

        [Column("SingleUserViewsPerDay")]
        public int SingleUserViewsPerDay
        {
            get { return _SingleUserViewsPerDay; }
            protected set { _SingleUserViewsPerDay = value; SetUpToDateAsFalse(); }
        }

        #endregion Columns
        public AdvertStatus Status
        {
            get { return (AdvertStatus)StatusInt; }
            protected set { StatusInt = (int)value; }
        }


        int _Id, _UserId, _StatusInt, _CompletionTimesBought, _CompletionTimes, _DisplayTime,
            _Adverts, _SingleUserViewsPerDay;
        string _Title, _Description;
        Money _PricePaid;
        bool _PCAllowed, _MobileAllowed, _AutosurfEnabled;

        public PTCOfferWall(int id) : base(id) { }

        public PTCOfferWall(DataRow row, bool isUpToDate = true)
                    : base(row, isUpToDate) { }

        private PTCOfferWall(int userId, PTCOfferWallPack pack, string title, string description,
            bool pcAllowed, bool mobileAllowed, bool autosurfEnabled, int maxSingleUserDailyViews)
        {
            UserId = userId;
            Status = AdvertStatus.Active;
            CompletionTimes = 0;
            CompletionTimesBought = pack.CompletionTimes;
            PricePaid = pack.Price;
            DisplayTime = pack.DisplayTime;
            Adverts = pack.Adverts;
            Title = title;
            Description = description;
            PCAllowed = pcAllowed;
            MobileAllowed = mobileAllowed;
            AutosurfEnabled = autosurfEnabled;
            SingleUserViewsPerDay = maxSingleUserDailyViews;
        }
        public void Pause()
        {
            Status = AdvertStatus.Paused;
            this.Save();
        }

        public static void Buy(Member user, PTCOfferWallPack pack, PurchaseBalances targetBalance, List<UserUrl> userUrls,
            string title, string description, GeolocationUnit geoUnit, bool pcAllowed, bool mobileAllowed,
            bool autosurfEnabled, int maxSingleUserDailyViews)
        {
            if (pack.Adverts != userUrls.Count)
                throw new MsgException(string.Format(U6002.NUMBEROFURLSERROR, pack.Adverts));

            if (userUrls.Any(u => u.Status != AdvertStatus.Active))
                throw new MsgException("Fraud! Only active urls are permitted.");

            PurchaseOption.ChargeBalance(user, pack.Price, PurchaseOption.Features.PtcOfferWall.ToString(), targetBalance, "PTC OfferWall");

            var offerWall = new PTCOfferWall(user.Id, pack, title, description, pcAllowed, mobileAllowed, autosurfEnabled, maxSingleUserDailyViews);

            if (geoUnit != null)
                offerWall.AddGeolocation(geoUnit);

            offerWall.Save();
            offerWall.MapWithUrls(userUrls);

            MatrixBase.TryAddMemberAndCredit(user, pack.Price, AdvertType.PTCOfferWall);
        }

        private void MapWithUrls(List<UserUrl> userUrlIds)
        {
            foreach (var userUrl in userUrlIds)
            {
                PTCOfferWallUserUrl.CreateMapping(this.Id, userUrl.Id);
            }
        }

        public static bool ArePacksConfigured()
        {
            return PTCOfferWallPack.AreAnyActive();
        }

        public static List<PTCOfferWall> GetActive()
        {
            string query = string.Format("SELECT * FROM PTCOfferWalls WHERE Status = {0}", (int)AdvertStatus.Active);
            return TableHelper.GetListFromRawQuery<PTCOfferWall>(query);
        }

        public static List<PTCOfferWall> GetActive(string countryCode, int? age, Gender gender, bool ismobile)
        {
            var active = GetActive();

            return active.Where(o => o.IsGeolocationMet(countryCode, age, gender) && (o.MobileAllowed == ismobile || o.PCAllowed == !ismobile)).ToList();
        }

        public void Watched()
        {
            CompletionTimes++;

            if (CompletionTimes >= CompletionTimesBought)
                this.Status = AdvertStatus.Finished;

            this.Save();
        }
    }
}