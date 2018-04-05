using Prem.PTC;
using Prem.PTC.Members;
using Prem.PTC.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

/// <summary>
/// Summary description for UserBets
/// </summary>
public class UserCustomGroup : BaseTableObject
{

    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "UserCustomGroups"; } }
    protected override string dbTable { get { return TableName; } }
    public static class Columns
    {
        public const string Id = "Id";
        public const string Name = "Name";
        public const string CreatorUserId = "CreatorUserId";
        public const string CustomGroupId = "CustomGroupId";
        public const string AdPacksAdded = "AdPacksAdded";
        public const string Description = "Description";
        public const string PromoUrl = "PromoUrl";
        public const string Email = "Email";
        public const string Skype = "Skype";
        public const string PhoneNumber = "PhoneNumber";
        public const string FacebookUrl = "FacebookUrl";
        public const string ImagePath = "ImagePath";
        public const string Status = "Status";
    }


    [Column(Columns.Id, IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Name)]
    public string Name { get { return _Name; } set { _Name = value; SetUpToDateAsFalse(); } }

    [Column(Columns.CreatorUserId)]
    public int CreatorUserId { get { return _CreatorUserId; } set { _CreatorUserId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.CustomGroupId)]
    public int CustomGroupId { get { return _CustomGroupId; } set { _CustomGroupId = value; SetUpToDateAsFalse(); } }

    [Column(Columns.AdPacksAdded)]
    public int AdPacksAdded { get { return _AdPacksAdded; } set { _AdPacksAdded = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Description)]
    public string Description { get { return _Description; } set { _Description = value; SetUpToDateAsFalse(); } }

    [Column(Columns.PromoUrl)]
    public string PromoUrl { get { return _PromoUrl; } set { _PromoUrl = value; SetUpToDateAsFalse(); } }

    [Column(Columns.ImagePath)]
    public string ImagePath { get { return _ImagePath; } set { _ImagePath = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Email)]
    public string Email { get { return _Email; } set { _Email = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Skype)]
    public string Skype { get { return _Skype; } set { _Skype = value; SetUpToDateAsFalse(); } }

    [Column(Columns.PhoneNumber)]
    public string PhoneNumber { get { return _PhoneNumber; } set { _PhoneNumber = value; SetUpToDateAsFalse(); } }

    [Column(Columns.FacebookUrl)]
    public string FacebookUrl { get { return _FacebookUrl; } set { _FacebookUrl = value; SetUpToDateAsFalse(); } }

    [Column(Columns.Status)]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

    public CustomGroupStatus Status { get { return (CustomGroupStatus)StatusInt; } set { StatusInt = (int)value; } }

    [Column("GotBonus")]
    public bool GotBonus { get { return _GotBonus; } set { _GotBonus = value; SetUpToDateAsFalse(); } }

    [Column("BonusExtraInformation")]
    public string BonusExtraInformation { get { return _BonusExtraInformation; } set { _BonusExtraInformation = value; SetUpToDateAsFalse(); } }
    #endregion

    public Member User
    {
        get
        {
            if (_user == null)
                _user = new Member(CreatorUserId);
            return _user;
        }
        set
        {
            _user = value;
            CreatorUserId = value.Id;
        }
    }

    private Banner _bannerImage;
    public Banner BannerImage
    {
        get { return _bannerImage; }
        set
        {
            _bannerImage = value;
            SetUpToDateAsFalse();
        }
    }

    private int _id, _CreatorUserId, _CustomGroupId, _AdPacksAdded, _StatusInt;
    string _Name, _Description, _PromoUrl, _Email, _Skype, _PhoneNumber, _FacebookUrl, _ImagePath, _BonusExtraInformation;
    bool _GotBonus;
    Member _user;
    public UserCustomGroup()
            : base()
    { }

    public UserCustomGroup(int id) : base(id) { }

    public UserCustomGroup(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate)
    { }


    private void bannerImage_PreSave()
    {
        if (_bannerImage != null)
        {
            if (!_bannerImage.IsSaved)
                _bannerImage.Save(AppSettings.FolderPaths.BannerAdvertImages);

            ImagePath = _bannerImage.Path;
        }
    }

    public override void Save(bool forceSave = true)
    {
        bannerImage_PreSave();
        base.Save(forceSave);
    }
}

