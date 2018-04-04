using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.Publish.PTCOfferWalls
{

    [Serializable]
    public class PTCOfferWallPack : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "PTCOfferWallPacks"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("CompletionTimes")]
        public int CompletionTimes
        {
            get { return _CompletionTimes; }
            protected set
            {
                if (value <= 0)
                    throw new MsgException("Number of CompletionTimes must be greater than 0.");
                _CompletionTimes = value; SetUpToDateAsFalse();
            }
        }

        [Column("Price")]
        public Money Price
        {
            get { return _Price; }
            protected set
            {
                if (value <= Money.Zero)
                    throw new MsgException("Price must be greater than 0.");
                _Price = value; SetUpToDateAsFalse();
            }
        }

        [Column("Adverts")]
        public int Adverts
        {
            get { return _Adverts; }
            protected set
            {
                if (value <= 0)
                    throw new MsgException("Number of Adverts must be greater than 0.");
                _Adverts = value; SetUpToDateAsFalse();
            }
        }

        [Column("DisplayTime")]
        public int DisplayTime
        {
            get { return _DisplayTime; }
            protected set
            {
                if (value <= 0)
                    throw new MsgException("Display Time must be greater than 0.");
                _DisplayTime = value; SetUpToDateAsFalse();
            }
        }

        [Column("Status")]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }


        #endregion Columns
        public UniversalStatus Status
        {
            get { return (UniversalStatus)StatusInt; }
            protected set { StatusInt = (int)value; }
        }

        int _Id, _StatusInt, _Adverts, _CompletionTimes, _DisplayTime;
        Money _Price;

        public PTCOfferWallPack(int id) : base(id) { }

        public PTCOfferWallPack(DataRow row, bool isUpToDate = true)
                    : base(row, isUpToDate) { }

        private PTCOfferWallPack(int completionTimes, Money price, int adverts, int displayTime)
        {
            CompletionTimes = completionTimes;
            Price = price;
            Status = UniversalStatus.Paused;
            Adverts = adverts;
            DisplayTime = displayTime;
        }

        public void Activate()
        {
            Status = UniversalStatus.Active;
            this.Save();
        }

        public void Pause()
        {
            Status = UniversalStatus.Paused;
            this.Save();
        }

        public override void Delete()
        {
            Status = UniversalStatus.Deleted;
            this.Save();
        }

        public static void Create(int completionTimes, Money price, int adverts, int displayTime)
        {
            var pack = new PTCOfferWallPack(completionTimes, price, adverts, displayTime);
            pack.Save();
        }

        public static List<PTCOfferWallPack> GetActive()
        {
            return TableHelper.SelectRows<PTCOfferWallPack>(TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
        }

        public static bool AreAnyActive()
        {
            return TableHelper.RowExists("PTCOfferWallPacks", TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
        }

        public void Update(int completionTimes, int adverts, int displayTime, Money price)
        {
            CompletionTimes = completionTimes;
            Price = price;
            Adverts = adverts;
            DisplayTime = displayTime;
            this.Save();
        }
    }
}