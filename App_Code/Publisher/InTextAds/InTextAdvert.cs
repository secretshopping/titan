using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using Resources;
using System.Collections.Generic;
using System.Data;

namespace Titan.Publisher.InTextAds
{
    public class InTextAdvert : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "InTextAdverts"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("UserId")]
        public int UserId { get { return _UserId; } protected set { _UserId = value; SetUpToDateAsFalse(); } }

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

        [Column("Url")]
        public string Url
        {
            get { return _Url; }
            protected set { _Url = value; SetUpToDateAsFalse(); }
        }

        [Column("ClicksReceived")]
        public int ClicksReceived
        {
            get { return _ClicksReceived; }
            protected set { _ClicksReceived = value; SetUpToDateAsFalse(); }
        }

        [Column("ClicksBought")]
        public int ClicksBought
        {
            get { return _ClicksBought; }
            protected set { _ClicksBought = value; SetUpToDateAsFalse(); }
        }

        [Column("PricePaid")]
        public Money PricePaid
        {
            get { return _PricePaid; }
            protected set { _PricePaid = value; SetUpToDateAsFalse(); }
        }

        [Column("Status")]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

        [Column("TargetBalance")]
        protected int TargetBalanceInt { get { return _TargetBalance; } set { _TargetBalance = value; SetUpToDateAsFalse(); } }

        public PurchaseBalances TargetBalance { get { return (PurchaseBalances)TargetBalanceInt; } set { TargetBalanceInt = (int)value; } }

        #endregion Columns

        public AdvertStatus Status
        {
            get { return (AdvertStatus)StatusInt; }
            protected set { StatusInt = (int)value; }
        }

        int _Id, _StatusInt, _ClicksReceived, _ClicksBought, _UserId, _TargetBalance;
        Money _PricePaid;
        string _Title, _Description, _Url;

        public InTextAdvert(int id) : base(id) { }

        public InTextAdvert(DataRow row, bool isUpToDate = true)
                    : base(row, isUpToDate) { }

        private InTextAdvert(int userId, InTextAdvertPack pack, string title, string description, string url, PurchaseBalances targetBalance)
        {
            UserId = userId;
            ClicksBought = pack.Clicks;
            ClicksReceived = 0;
            PricePaid = pack.Price;
            Title = title;
            Description = description;
            Url = url;
            Status = AdvertStatus.Active;
            TargetBalance = targetBalance;
        }

        public void Activate()
        {
            Status = AdvertStatus.Active;
            this.Save();
        }

        public void Pause()
        {
            Status = AdvertStatus.Paused;
            this.Save();
        }

        public override void Delete()
        {
            Status = AdvertStatus.Deleted;
            this.Save();
        }

        public static void Buy(Member user, InTextAdvertPack pack, string title, string description, string url, PurchaseBalances targetBalance, List<string> tags)
        {
            if (tags.Count > pack.MaxNumberOfTags)
                throw new MsgException(string.Format(U6002.TOOMANYTAGS, pack.MaxNumberOfTags));

            if (tags.Count == 0)
                throw new MsgException(U6002.MUSTADDTAGS);

            PurchaseOption.ChargeBalance(user, pack.Price, PurchaseOption.Features.InTextAds.ToString(), targetBalance, "InText Ad");
            var ad = new InTextAdvert(user.Id, pack, title, description, url, targetBalance);
            ad.Save();

            MapTags(ad.Id, tags);

            MatrixBase.TryAddMemberAndCredit(user, pack.Price, AdvertType.InText);
        }

        private static void MapTags(int inTextAdvertId, List<string> tags)
        {
            foreach (var tag in tags)
            {
                InTextAdvertTag.CreateMapping(inTextAdvertId, tag);
            }
        }

        public static List<InTextAdvert> GetActive()
        {
            return TableHelper.SelectRows<InTextAdvert>(TableHelper.MakeDictionary("Status", (int)AdvertStatus.Active));
        }

        public void AddClick()
        {
            ClicksReceived++;

            if (ClicksReceived >= ClicksBought)
            {
                this.Status = AdvertStatus.Finished;
            }

            this.Save();
        }
    }
}








