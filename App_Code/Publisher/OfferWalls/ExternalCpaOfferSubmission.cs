using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Resources;
using Prem.PTC.Advertising;
using Prem.PTC.Offers;
using Titan;
using System.Text;
using Prem.PTC.Members;
using Titan.Publisher;

public class ExternalCpaOfferSubmission : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "ExternalCpaOfferSubmissions"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("PublisherId")]
    public int PublisherId { get { return _PublisherId; } set { _PublisherId = value; SetUpToDateAsFalse(); } }

    [Column("OfferId")]
    public int OfferId { get { return _OfferId; } set { _OfferId = value; SetUpToDateAsFalse(); } }

    [Column("PublishersWebsiteId")]
    public int PublishersWebsiteId { get { return _PublishersWebsiteId; } set { _PublishersWebsiteId = value; SetUpToDateAsFalse(); } }

    [Column("ExternalUsername")]
    public string ExternalUsername { get { return _ExternalUsername; } set { _ExternalUsername = value; SetUpToDateAsFalse(); } }

    [Column("DateAdded")]
    public DateTime DateAdded { get { return _DateAdded; } set { _DateAdded = value; SetUpToDateAsFalse(); } }

    [Column("DateAccepted")]
    public DateTime? DateAccepted { get { return _DateAccepted; } set { _DateAccepted = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int StatusInt { get { return _StatusInt; } set { _StatusInt = value; SetUpToDateAsFalse(); } }

    [Column("LoginId")]
    public string LoginId { get { return _LoginId; } set { _LoginId = value; SetUpToDateAsFalse(); } }

    [Column("EmailId")]
    public string EmailId { get { return _EmailId; } set { _EmailId = value; SetUpToDateAsFalse(); } }

    [Column("SubId2")]
    public string SubId2 { get { return _SubId2; } set { _SubId2 = value; SetUpToDateAsFalse(); } }

    [Column("SubId3")]
    public string SubId3 { get { return _SubId3; } set { _SubId3 = value; SetUpToDateAsFalse(); } }

    [Column("Ip")]
    public string Ip { get { return _Ip; } set { _Ip = value; SetUpToDateAsFalse(); } }

    [Column("CountryCode")]
    public string CountryCode { get { return _CountryCode; } set { _CountryCode = value; SetUpToDateAsFalse(); } }

    [Column("Payout")]
    public Money Payout { get { return _Payout; } set { _Payout = value; SetUpToDateAsFalse(); } }

    [Column("Age")]
    public int Age { get { return _Age; } set { _Age = value; SetUpToDateAsFalse(); } }

    [Column("Gender")]
    protected int GenderInt { get { return _GenderInt; } set { _GenderInt = value; SetUpToDateAsFalse(); } }


    public OfferStatus Status
    {
        get
        {
            return (OfferStatus)StatusInt;
        }
        set
        {
            StatusInt = (int)value;
        }
    }

    public Gender Gender
    {
        get
        {
            return (Gender)GenderInt;
        }
        set
        {
            GenderInt = (int)value;
        }
    }

    int _Id, _PublisherId, _OfferId, _StatusInt, _PublishersWebsiteId, _Age, _GenderInt;
    DateTime _DateAdded;
    DateTime? _DateAccepted;
    string _LoginId, _EmailId, _ExternalUsername, _SubId2, _SubId3, _Ip, _CountryCode;
    Money _Payout;
    #endregion Columns

    private CPAOffer offer;

    public CPAOffer Offer
    {
        get
        {
            if (offer == null)
                offer = new CPAOffer(_OfferId);
            return offer;
        }
    }

    #region Constructors
    public ExternalCpaOfferSubmission(int id) : base(id) { }

    public ExternalCpaOfferSubmission(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    private ExternalCpaOfferSubmission(int publisherId, int offerId, int publishersWebsiteId, string externalUsername, string loginId, string emailId, string subId2,
        string subId3, string ip, string countryCode, Money payout, Gender gender = Prem.PTC.Members.Gender.Null, int age = -1)
    {
        PublisherId = publisherId;
        OfferId = offerId;
        PublishersWebsiteId = publishersWebsiteId;
        ExternalUsername = externalUsername;
        DateAdded = AppSettings.ServerTime;
        DateAccepted = null;
        Status = OfferStatus.Pending;
        LoginId = loginId;
        EmailId = emailId;
        SubId2 = subId2;
        SubId3 = subId3;
        Ip = ip;
        CountryCode = countryCode;
        Payout = payout;
        Gender = gender;
        Age = age;
    }
    #endregion

    public static void Create(PublishersWebsite website, int offerId, string externalUsername, string loginId, string emailId, string subId2,
        string subId3, string ip, string countryCode, Money payout)
    {
        var ExternalCpaOfferSubmission = new ExternalCpaOfferSubmission(website.UserId, offerId, website.Id, externalUsername,
            loginId, emailId, subId2, subId3, ip, countryCode, payout);
        ExternalCpaOfferSubmission.Save();
    }

    public void Accept()
    {
        if (Status == OfferStatus.Pending || Status == OfferStatus.UnderReview)
        {
            
            var moneyLeftForPools = new ExternalCpaOfferCrediter(this).Credit(HandleSuccessfulCredit);

           
        }
        else
            ErrorLogger.Log("Tried to Accept ExternalCpaOffer which had status " + Status.ToString());
    }

    private void HandleSuccessfulCredit(Money payout)
    {
        try
        {
            GlobalPostback.Create(GlobalPostbackType.Cpa, OfferId, Offer.Title, PublishersWebsiteId, ExternalUsername,
                SubId2, SubId3, Ip, CountryCode, payout, Gender, Age).Send();

            Payout = payout;
            Status = OfferStatus.Completed;
            DateAccepted = AppSettings.ServerTime;
            Save();
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
        }
    }

    public void Deny()
    {
        if (Status == OfferStatus.Pending || Status == OfferStatus.UnderReview)
        {
            Status = OfferStatus.Denied;
            Save();
        }
        else
            ErrorLogger.Log("Tried to Deny ExternalCpaOffer which had status " + Status.ToString());
    }

    public void SetUnderReview()
    {
        if (Status == OfferStatus.Pending || Status == OfferStatus.UnderReview)
        {
            Status = OfferStatus.UnderReview;
            Save();
        }
        else
            ErrorLogger.Log("Tried to SetUnderReview ExternalCpaOffer which had status " + Status.ToString());
    }
}