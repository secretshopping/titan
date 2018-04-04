using ExtensionMethods;
using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class ApiAccessToken : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "ApiAccessTokens"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("Token")]
    public string Token { get { return _Token; } set { _Token = value; SetUpToDateAsFalse(); } }

    [Column("ExpiresDate")]
    public DateTime ExpiresDate { get { return _ExpiresDate; } set { _ExpiresDate = value; SetUpToDateAsFalse(); } }

    private int _id, _UserId;
    private string _Token;
    private DateTime _ExpiresDate;

    public ApiAccessToken()
     : base()
    { }

    public ApiAccessToken(int id) : base(id) { }

    public ApiAccessToken(System.Data.DataRow row, bool isUpToDate = true)
        : base(row, isUpToDate)
    { }

    #endregion Columns

    public static ApiAccessToken GetOrCreate(int userId)
    {
        var tokens = TableHelper.GetListFromRawQuery<ApiAccessToken>(String.Format(
            "SELECT * FROM ApiAccessTokens WHERE UserId = {0} AND ExpiresDate >= '{1}'", userId, AppSettings.ServerTime.ToDBString()));

        ApiAccessToken result = null;

        if (tokens.Count > 0)
            result = tokens[0];
        else
        {
            result = new ApiAccessToken();
            result.UserId = userId;
            result.ExpiresDate = DateTime.Now.AddDays(7);
            result.Token = HashingManager.SHA512HMAC(HashingManager.GenerateMD5(userId + DateTime.Now.ToString()),
                HashingManager.SHA256(AppSettings.Offerwalls.UniversalHandlerPassword + userId + DateTime.Now));
            result.Save();
        }

        return result;
    }

    public static int ValidateAndGetUserId(string token)
    {
        var tokens = TableHelper.GetListFromRawQuery<ApiAccessToken>(String.Format(
          "SELECT * FROM ApiAccessTokens WHERE Token = '{0}' AND ExpiresDate >= '{1}'", token.Trim(), AppSettings.ServerTime.ToDBString()));

        if (tokens.Count == 0)
            throw new MsgException("Invalid Access Token.");

        ApiAccessToken targetToken = tokens[0];

        return targetToken.UserId;
    }

    public static void CRON()
    {
        TableHelper.ExecuteRawCommandNonQuery(String.Format(
            "DELETE FROM ApiAccessTokens WHERE ExpiresDate < '{0}'", AppSettings.ServerTime.ToDBString()));
    }
}
