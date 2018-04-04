using System;
using System.Collections.Generic;
using System.Data;
using ExtensionMethods;
using Prem.PTC;
using Prem.PTC.Members;
using SocialNetwork;

[Serializable]
public class Representative : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "Representatives"; } }

    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int IntStatus { get { return _Status; } set { _Status = value; SetUpToDateAsFalse(); } }

    [Column("Name")]
    public string Name { get { return _Name; } set { _Name = value; SetUpToDateAsFalse(); } }

    [Column("Email")]
    public string Email { get { return _Email; } set { _Email = value; SetUpToDateAsFalse(); } }

    [Column("Why")]
    public string Why { get { return _Why; } set { _Why = value; SetUpToDateAsFalse(); } }

    [Column("City")]
    public string City { get { return _City; } set { _City = value; SetUpToDateAsFalse(); } }

    [Column("Country")]
    public string Country { get { return _Country; } set { _Country = value; SetUpToDateAsFalse(); } }

    [Column("PhoneNumber")]
    public string PhoneNumber { get { return _PhoneNumber; } set { _PhoneNumber = value; SetUpToDateAsFalse(); } }

    [Column("Languages")]
    public string Languages { get { return _Languages; } set { _Languages = value; SetUpToDateAsFalse(); } }

    [Column("RejectReason")]
    public string RejectReason { get { return _RejectReason; } set { _RejectReason = value; SetUpToDateAsFalse(); } }

    [Column("Skype")]
    public string Skype { get { return _Skype; } set { _Skype = value; SetUpToDateAsFalse(); } }

    [Column("Facebook")]
    public string Facebook { get { return _Facebook; } set { _Facebook = value; SetUpToDateAsFalse(); } }

    [Column("DepositInstructions")]
    public string DepositInstructions { get { return _DepositInstructions; } set { _DepositInstructions = value; SetUpToDateAsFalse(); } }

    [Column("WithdrawalInstructions")]
    public string WithdrawalInstructions { get { return _WithdrawalInstructions; } set { _WithdrawalInstructions = value; SetUpToDateAsFalse(); } }


    private int _id, _UserId, _Status;
    private string _Name, _Email, _City, _Country, _PhoneNumber, _Languages, _Why, _RejectReason, _Skype, _Facebook, _DepositInstructions,
        _WithdrawalInstructions;

    #endregion Columns

    #region Constructors

    public Representative()
            : base() { }

    public Representative(int id) : base(id) { }

    public Representative(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }
     
    #endregion

    public UniversalStatus Status
    {
        get
        {
            return (UniversalStatus)IntStatus;
        }

        set
        {
            IntStatus = (int)value;
        }
    }

    public static List<Representative> GetAllActive()
    {
        return TableHelper.GetListFromRawQuery<Representative>(string.Format("SELECT * FROM Representatives WHERE Status = {0}", (int)UniversalStatus.Active));
    }

    public static List<Representative> GetAllActiveFromCountry(string country)
    {
        return TableHelper.GetListFromRawQuery<Representative>(string.Format("SELECT TOP {0} * FROM Representatives WHERE Status = {1} AND Country = '{2}'", AppSettings.Representatives.NoOfRepresentatives, (int)UniversalStatus.Active, country));
    }

    public static bool IsActiveRepresentative(int memberId)
    {
        int results = (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM Representatives WHERE Status = {0} AND UserId = {1}", (int)UniversalStatus.Active, memberId));
        return results > 0;
    }

    public static bool DidUserSendRequest(int memberId)
    {
        int results = (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM Representatives WHERE Status != {0} AND UserId = {1}", (int)UniversalStatus.Deleted, memberId));
        return results > 0;
    }

    public static bool IsEmailInDatabase(string email)
    {
        int results = (int)TableHelper.SelectScalar(string.Format("SELECT COUNT(*) FROM Representatives WHERE Email = '{0}'", email));
        return results > 0;
    }

    public void Reject()
    {
        Status = UniversalStatus.Deleted;
        this.Save();
    }

    public void Reject(string reason)
    {
        RejectReason = reason;
        Status = UniversalStatus.Deleted;
        this.Save();
    }

    public void Accept()
    {
        Status = UniversalStatus.Active;
        this.Save();
    }

    public static void SetAllStatuses(UniversalStatus fromStatus, UniversalStatus toStatus)
    {
        TableHelper.ExecuteRawCommandNonQuery(string.Format("UPDATE Representatives SET Status = {0} WHERE Status = {1}", (int)toStatus, (int)fromStatus));
    }

    public static Money IncreaseAmountForRepresentative(Money input, Member user, int refererID, int level)
    {
        Money amount = Money.Zero;

        if (AppSettings.TitanFeatures.IsRepresentativesEnabled && level == 1)
        {
            if (AppSettings.Representatives.Policy == AppSettings.Representatives.RepresentativesPolicy.Users && Representative.IsActiveRepresentative(refererID))
                 return amount = Money.MultiplyPercent(input, AppSettings.Representatives.ProfitForRepresentantFromReferral);

            if (AppSettings.Representatives.Policy == AppSettings.Representatives.RepresentativesPolicy.Automatic && user.RepresentativeId != null)
            {
                Member representative = new Member(user.RepresentativeId.Value);

                if (Representative.IsActiveRepresentative(representative.Id) && representative.Country == user.Country)
                    return amount = Money.MultiplyPercent(input, AppSettings.Representatives.ProfitForRepresentantFromReferral);
            }
        }
        return amount;
    }

    public static void ChangeRepresentatives(int fromRepresentativeID, int toRepresentativeID)
    {
        var members = TableHelper.GetListFromRawQuery<Member>(string.Format("SELECT * FROM Users WHERE RepresentativeId = {0}", fromRepresentativeID));

        foreach (var member in members)
        {
            member.RepresentativeId = toRepresentativeID;

            if(member.ReferrerId == fromRepresentativeID)
            {
                member.ReferrerId = toRepresentativeID;
                member.PointsEarnedToReferer = 0;
                member.TotalPTCClicksToDReferer = 0;
                member.TotalEarnedToDReferer = new Money(0);
                member.TotalPointsEarnedToDReferer = 0;
                member.TotalAdPacksToDReferer = new Money(0);
                member.TotalCashLinksToDReferer = new Money(0);
                member.LastPointableActivity = null;
                member.LastDRActivity = DateTime.Now.Zero();
            }
            member.Save();
        }
    }

    public static int GetRepresentativeTotalRequests(int userId)
    {
        return (int)TableHelper.SelectScalar(GetQueryStringForRepresentativeTotalRequests(userId, false));
    }

    public static int GetRepresentativeTotalPositiveRequests(int userId)
    {
        return (int)TableHelper.SelectScalar(GetQueryStringForRepresentativeTotalRequests(userId, true));
    }

    private static String GetQueryStringForRepresentativeTotalRequests(int userId, bool onlyPositive = false)
    {
        String Filter = String.Empty;
        if (onlyPositive)
            Filter = ((int)RepresentativeRequestStatus.Completed).ToString();
        else
            Filter = String.Format("{0},{1}", (int)RepresentativeRequestStatus.Completed, (int)RepresentativeRequestStatus.Rejected);

        return String.Format(@"SELECT COUNT(*) FROM ConversationMessages WHERE 
                                MessageType IN ({0},{1}) AND
                                RepresentativeRequestStatus IN ({2}) AND
                                UserId <> {3} AND
                                ConversationId IN 
                                    (SELECT Id FROM Conversations WHERE
                                        UserIdOne={3} OR UserIdTwo={3})",
                                (int)MessageType.RepresentativeDepositRequest,
                                (int)MessageType.RepresentativeWithdrawalRequest,
                                Filter,
                                userId);
    }

    public static RatingMemberInfo LoadNewUserData(int userId)
    {
        int TotalActions = GetRepresentativeTotalRequests(userId);
        int TotalPositiveActions = GetRepresentativeTotalPositiveRequests(userId);

        return new RatingMemberInfo(TotalActions, TotalPositiveActions);
    }
}
