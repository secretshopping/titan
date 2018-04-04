using Prem.PTC.Utils;
using System;
using Prem.PTC;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using Prem.PTC.Members;

public class ResetPasswordRequest : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "ResetPasswordRequests"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return quantity; } set { quantity = value; SetUpToDateAsFalse(); } }

    [Column("SecretCode")]
    public string SecretCode { get { return name; } set { name = value; SetUpToDateAsFalse(); } }

    [Column("ExpiresOn")]
    public DateTime ExpiresOn { get { return _ExpiresOn; } set { _ExpiresOn = value; SetUpToDateAsFalse(); } }


    private int _id, quantity, type;
    private string name;
    private DateTime _ExpiresOn;

    #endregion Columns

    public ResetPasswordRequest()
        : base() { }

    public ResetPasswordRequest(int id) : base(id) { }

    public ResetPasswordRequest(DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate) { }


    /// <summary>
    /// Returns generated Secret Code
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public static string Add(int userId, string email)
    {
        string code = MemberAuthenticationService.ComputeHash(DateTime.Now.ToString() + userId + email + AppSettings.Offerwalls.UniversalHandlerPassword);
        code = code.Replace("+", "").Replace("=", "");

        ResetPasswordRequest Created = new ResetPasswordRequest();
        Created.ExpiresOn = DateTime.Now.AddDays(1);
        Created.SecretCode = code;
        Created.UserId = userId;
        Created.Save();

        return code;        
    }

    /// <summary>
    /// Returns NULL if not exists
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static ResetPasswordRequest Get(int userId, string code)
    {
        var where = TableHelper.MakeDictionary("SecretCode", code);
        where.Add("UserId", userId);

        var list = TableHelper.SelectRows<ResetPasswordRequest>(where);

        if (list.Count == 0)
            return null;

        return list[0];
    }

}
