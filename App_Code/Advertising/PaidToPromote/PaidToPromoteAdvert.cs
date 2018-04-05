using Prem.PTC;
using Prem.PTC.Advertising;
using System;
using System.Data;

namespace Titan.PaidToPromote
{
    public class PaidToPromoteAdvert : BaseTableObject
    {
        public override Database Database { get { return Database.Client; } }
        public static new string TableName { get { return "PaidToPromoteAdverts"; } }
        protected override string dbTable { get { return TableName; } }

        #region Columns
        public static class Columns
        {
            public const string Id = "Id";
            public const string CreatorId = "CreatorId";
            public const string PackId = "PackId";
            public const string Status = "Status";
            public const string TargetUrl = "TargetUrl";
            public const string EndValue = "EndValue";
            public const string CreationDate = "CreationDate";
            public const string FinishDate = "FinishDate";
            public const string GeolocatedCC = "GeolocatedCC";
        }

        [Column(Columns.Id, IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CreatorId)]
        public int CreatorId { get { return _creatorId; } set { _creatorId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.PackId)]
        public int PackId { get { return _packId; } set { _packId = value; SetUpToDateAsFalse(); } }

        [Column(Columns.Status)]
        protected int _Status { get { return _status; } set { _status = value; SetUpToDateAsFalse(); } }

        public AdvertStatus Status
        {
            get { return (AdvertStatus)_Status; }
            set { _Status = (int)value; }
        }

        [Column(Columns.TargetUrl)]
        public string TargetUrl { get { return _targetUrl; } set { _targetUrl = value; SetUpToDateAsFalse(); } }

        [Column(Columns.EndValue)]
        public int EndValue { get { return _endValue; } set { _endValue = value; SetUpToDateAsFalse(); } }

        [Column(Columns.CreationDate)]
        public DateTime CreationDate { get { return _creationDate; } set { _creationDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.FinishDate)]
        public DateTime? FinishDate { get { return _finishDate; } set { _finishDate = value; SetUpToDateAsFalse(); } }

        [Column(Columns.GeolocatedCC)]
        public string GeolocatedCC { get { return _geoCC; } set { _geoCC = value; SetUpToDateAsFalse(); } }        

        private int _id, _creatorId, _packId, _status, _endValue;
        private string _targetUrl, _geoCC;
        private DateTime _creationDate;
        private DateTime? _finishDate;
        #endregion

        public PaidToPromoteAdvert() : base()
        {
            EndValue = 0;
            CreationDate = AppSettings.ServerTime;
            Status = AdvertStatus.Active;
        }

        public PaidToPromoteAdvert(int id) : base(id) { }

        public PaidToPromoteAdvert(DataRow row, bool isUpToDate = true) : base(row, isUpToDate) { }

        public bool IsGeo()
        {
            return string.IsNullOrEmpty(GeolocatedCC) ? false :true;
        }

        public bool CheckGeo(string countryCode)
        {
            if (!IsGeo() || string.IsNullOrEmpty(countryCode))
                return true;

            if (GeolocatedCC.Contains(countryCode))
                return false;

            return true;
        }
        
        public void CreditClicks()
        {
            EndValue += 1;

            if (EndValue >= new PaidToPromotePack(PackId).Ends.Value)
                Finish();
            else
                Save();
        }

        public void CheckDays()
        {
            if (CreationDate.AddDays(Convert.ToInt32(new PaidToPromotePack(PackId).Ends.Value)) <= AppSettings.ServerTime)
                Finish();
        }

        public void Activate()
        {
            Status = AdvertStatus.Active;
            Save();
        }

        public void Pause()
        {
            Status = AdvertStatus.Paused;
            Save();
        }

        public void Finish()
        {
            FinishDate = AppSettings.ServerTime;
            Status = AdvertStatus.Finished;
            Save();
        } 

        public override void Delete()
        {
            Status = AdvertStatus.Deleted;
            Save();
        }
    }
}