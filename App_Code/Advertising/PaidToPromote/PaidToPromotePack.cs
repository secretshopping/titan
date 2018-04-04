using System;
using System.Data;
using Prem.PTC;
using Prem.PTC.Utils;

namespace Titan.PaidToPromote
{
    public class PaidToPromotePack : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "PaidToPromotePacks"; } }
        protected override string dbTable { get { return TableName; } }

        #region Columns
        public static class Columns
        {
            public const string Id = "Id";
            public const string Price = "Price";
            public const string EndMode = "EndMode";
            public const string EndValue = "EndValue";
            public const string Status = "Status";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Price)]
        public Money Price
        {
            get { return _price; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _price = value; SetUpToDateAsFalse();
            }
        }

        public End Ends
        {
            get
            {
                return End.FromModeValue(_endMode, EndValue);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                int endValue = value.EndMode == End.Mode.Endless ? 0 : (int)value.Value;

                EndMode = (int)value.EndMode;
                EndValue = endValue;
            }
        }

        [Column(Columns.EndMode)]
        protected int EndMode { get { return (int)_endMode; } set { _endMode = (End.Mode)value; SetUpToDateAsFalse(); } }

        [Column(Columns.EndValue)]
        protected int EndValue { get { return _endValue; } set { _endValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Status)]
        protected int _Status { get { return _status; } set { _status = value; SetUpToDateAsFalse(); } }

        public UniversalStatus Status
        {
            get { return (UniversalStatus)_Status; }
            set { _Status = (int)value; }
        }

        private int _id, _status, _endValue;
        private Money _price;
        private End.Mode _endMode;
        #endregion

        public PaidToPromotePack() : base() { }

        public PaidToPromotePack(int id) : base(id) { }

        public PaidToPromotePack(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public void Activate()
        {
            Status = UniversalStatus.Active;
            Save();
        }

        public void Pause()
        {
            Status = UniversalStatus.Paused;
            Save();
        }

        public override void Delete()
        {
            Status = UniversalStatus.Deleted;
            Save();
        }
    }
}