using System;
using System.Data;

namespace Prem.PTC.Advertising
{
    [Serializable]
    public class AdPacksAdvert : BaseTableObject
    {
        #region Columns

        public override Database Database { get { return Database.Client; } }

        public static new string TableName { get { return "AdPacksAdverts"; } }
        protected override string dbTable { get { return TableName; } }

        [Column("Id", IsPrimaryKey = true)]
        public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

        [Column("Status")]
        protected int StatusInt { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

        [Column("TargetUrl")]
        public string TargetUrl { get { return UrlCreator.ParseUrl(_TargetUrl); } set { _TargetUrl = value; SetUpToDateAsFalse(); } }

        [Column("ConstantImagePath")]
        public string ConstantImagePath { get { return _imagePath; } set { _imagePath = value; SetUpToDateAsFalse(); } }

        [Column("NormalImagePath")]
        public string NormalImagePath { get { return _imagePath2; } set { _imagePath2 = value; SetUpToDateAsFalse(); } }

        [Column("CreatorUserId")]
        public int CreatorUserId { get { return _CreatorUserId; } set { _CreatorUserId = value; SetUpToDateAsFalse(); } }

        [Column("Title")]
        public string Title { get { return _Title; } set { _Title = value; SetUpToDateAsFalse(); } }

        [Column("Description")]
        public string Description { get { return _Description; } set { _Description = value; SetUpToDateAsFalse(); } }

        [Column("StartPageDate")]
        public DateTime? StartPageDate { get { return _StartPageDate; } set { _StartPageDate = value; SetUpToDateAsFalse(); } }

        [Column("Priority")]
        public int Priority { get { return _priority; } set { _priority = value; SetUpToDateAsFalse(); } }

        [Column("AddedAsType")]
        private int AddedAsTypeInt { get { return _AddedAsType; } set { _AddedAsType = value; SetUpToDateAsFalse(); } }

        private int _id, _Status, _CreatorUserId, _priority, _AddedAsType;
        private string _imagePath, _imagePath2, _TargetUrl, _Title, _Description;
        DateTime? _StartPageDate;

        private Banner _bannerImage1;
        public Banner ConstantBannerImage
        {
            get { return _bannerImage1; }
            set
            {
                _bannerImage1 = value;
                SetUpToDateAsFalse();
            }
        }

        private Banner _bannerImage2;
        public Banner NormalBannerImage
        {
            get { return _bannerImage2; }
            set
            {
                _bannerImage2 = value;
                SetUpToDateAsFalse();
            }
        }

        public AdvertStatus Status
        {
            get { return (AdvertStatus)StatusInt; }
            set { StatusInt = (int)value; }
        }

        public PurchaseOption.Features AddedAsType
        {
            get { return (PurchaseOption.Features)AddedAsTypeInt; }
            set { AddedAsTypeInt = (int)value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates blank instance of BannerAdvert class
        /// </summary>
        public AdPacksAdvert()
            : base()
        { }

        public AdPacksAdvert(int id)
            : base(id)
        {
        }

        public AdPacksAdvert(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
        {
        }

        #endregion Constructors

        public void Save(bool forceSave = false, bool IsFromAdminPanel = false)
        {
            bannerImage_PreSave(IsFromAdminPanel);
            base.Save(forceSave);
        }


        private void bannerImage_PreSave(bool isFromAdminPanel = false)
        {
            if (_bannerImage1 != null)
            {
                if (!_bannerImage1.IsSaved)
                {
                    if (isFromAdminPanel)
                        _bannerImage1.SaveOnClient(AppSettings.FolderPaths.BannerAdvertImages);
                    else
                        _bannerImage1.Save(AppSettings.FolderPaths.BannerAdvertImages);
                }

                ConstantImagePath = _bannerImage1.Path;
            }

            if (_bannerImage2 != null)
            {
                if (!_bannerImage2.IsSaved)
                {
                    if (isFromAdminPanel)
                        _bannerImage2.SaveOnClient(AppSettings.FolderPaths.BannerAdvertImages);
                    else
                        _bannerImage2.Save(AppSettings.FolderPaths.BannerAdvertImages);
                }

                NormalImagePath = _bannerImage2.Path;
            }
        }

        /// <exception cref="DbException"/>
        public static void Delete(int id)
        {
            TableHelper.DeleteRows<AdPacksAdvert>("Id", id);
        }


        public bool IsEmpty
        {
            get
            {
                return this.ConstantImagePath == "#";
            }
            set
            {
                this.ConstantImagePath = value ? "#" : String.Empty;
            }
        }
    }
}