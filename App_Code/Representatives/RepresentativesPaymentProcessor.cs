using Prem.PTC;
using System;
using System.Collections.Generic;
using System.Data;

[Serializable]
public class RepresentativesPaymentProcessor : BaseTableObject
{
    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "RepresentativesPaymentProcessors"; } }

    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _userId; } set { _userId = value; SetUpToDateAsFalse(); } }

    [Column("Name")]
    public string Name { get { return _name; } set { _name = value; SetUpToDateAsFalse(); } }

    [Column("LogoPath")]
    public string LogoPath { get { return _logoPath; } set { _logoPath = value; SetUpToDateAsFalse(); } }

    [Column("WithdrawalInfo")]
    public string WithdrawalInfo { get { return _withdrawalInfo; } set { _withdrawalInfo = value; SetUpToDateAsFalse(); } }

    [Column("DepositInfo")]
    public string DepositInfo { get { return _depositInfo; } set { _depositInfo = value; SetUpToDateAsFalse(); } }

    private int _id, _userId;
    private string _name, _logoPath, _withdrawalInfo, _depositInfo;

    public RepresentativesPaymentProcessor()
            : base() { }

    public RepresentativesPaymentProcessor(int id) : base(id) { }

    public RepresentativesPaymentProcessor(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    public static List<RepresentativesPaymentProcessor> GetAllPaymentOptions(int userId)
    {
        var query = string.Format("SELECT * FROM {0} WHERE UserId = {1}", TableName, userId);

        return TableHelper.GetListFromRawQuery<RepresentativesPaymentProcessor>(query);
    }
}