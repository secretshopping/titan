using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Resources;
using ExtensionMethods;
using Prem.PTC.Members;

public class VerificationCode : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "VerificationCodes"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _Id; } protected set { _Id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Code")]
    public int Code { get { return _Code; } set { _Code = value; SetUpToDateAsFalse(); } }

    [Column("CreationDate")]
    public DateTime CreationDate { get { return _CreationDate; } set { _CreationDate = value; SetUpToDateAsFalse(); } }

    int _Id, _UserId, _Code;
    DateTime _CreationDate;
    #endregion Columns

    public VerificationCode(int id) : base(id) { }

    public VerificationCode(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    private VerificationCode(int userId, int code)
    {
        UserId = userId;
        CreationDate = AppSettings.ServerTime;
        Code = code;
    }

    public static bool IsCodeValid(int userId, string codeEnteredByUser)
    {
        if (string.IsNullOrEmpty(codeEnteredByUser))
            return false;

        DeleteOldRecords();

        var query = string.Format("SELECT TOP 1 * FROM VerificationCodes WHERE UserId = {0} AND Code = {1}",
            userId, Convert.ToInt32(codeEnteredByUser.Trim()));

        var code = TableHelper.GetListFromRawQuery<VerificationCode>(query).FirstOrDefault();

        var isValid = false;

        if (code != null)
        {
            isValid = true;
            code.Delete();
        }

        return isValid;
    }

    public static void Create(int userId, int code)
    {
        var verificationCode = new VerificationCode(userId, code);
        verificationCode.Save();
    }

    public static bool ActiveCodeForUserExists(int userId)
    {
        var query = string.Format("SELECT TOP 1 * FROM VerificationCodes WHERE UserId = {0} AND CreationDate >= '{1}'",
           userId, AppSettings.ServerTime.AddMinutes(-AppSettings.Payments.WithdrawalVerificationCodeValidForMinutes).ToDBString());

        return TableHelper.GetListFromRawQuery<VerificationCode>(query).Any();
    }

    private static void DeleteOldRecords()
    {
        var query = string.Format("DELETE FROM VerificationCodes WHERE CreationDate < '{0}'",
            AppSettings.ServerTime.AddMinutes(-AppSettings.Payments.WithdrawalVerificationCodeValidForMinutes).ToDBString());

        TableHelper.ExecuteRawCommandNonQuery(query);
    }
}