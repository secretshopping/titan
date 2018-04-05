using Prem.PTC;
using Prem.PTC.Advertising;
using Prem.PTC.Members;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Titan.Publisher.InTextAds
{
    [Serializable]
    public class InTextAdvertPack : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "InTextAdvertPacks"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

        [Column("Clicks")]
        public int Clicks
        {
            get { return _Clicks; }
            protected set
            {
                if (value <= 0)
                    throw new MsgException("Number of Clicks must be greater than 0.");
                _Clicks = value; SetUpToDateAsFalse();
            }
        }

        [Column("MaxNumberOfTags")]
        public int MaxNumberOfTags
        {
            get { return _MaxNumberOfTags; }
            protected set
            {
                if (value <= 0)
                    throw new MsgException("Max number of tags must be greater than 0.");
                _MaxNumberOfTags = value; SetUpToDateAsFalse();
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

        [Column("Status")]
        protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }


        #endregion Columns
        public UniversalStatus Status
        {
            get { return (UniversalStatus)StatusInt; }
            protected set { StatusInt = (int)value; }
        }

        int _Id, _StatusInt, _Clicks, _MaxNumberOfTags;
        Money _Price;

        public InTextAdvertPack(int id) : base(id) { }

        public InTextAdvertPack(DataRow row, bool isUpToDate = true)
                    : base(row, isUpToDate) { }

        private InTextAdvertPack(int clicks, Money price, int maxNumberOfTags)
        {
            Clicks = clicks;
            Price = price;
            MaxNumberOfTags = maxNumberOfTags;
            Status = UniversalStatus.Paused;
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

        public static void Create(int clicks, Money price, int maxNumberOfTags)
        {
            var pack = new InTextAdvertPack(clicks, price, maxNumberOfTags);
            pack.Save();
        }

        public static List<InTextAdvertPack> GetActive()
        {
            return TableHelper.SelectRows<InTextAdvertPack>(TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
        }

        public static bool AreAnyActive()
        {
            return TableHelper.RowExists("InTextAdvertPacks", TableHelper.MakeDictionary("Status", (int)UniversalStatus.Active));
        }

        public void Update(int clicks, Money price, int maxNumberOfTags)
        {
            Clicks = clicks;
            Price = price;
            MaxNumberOfTags = maxNumberOfTags;
            this.Save();
        }
    }
}