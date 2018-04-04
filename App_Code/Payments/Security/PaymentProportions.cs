using Prem.PTC;
using Prem.PTC.Payments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PayoutSecurityManager
/// </summary>
public class PaymentProportions : BaseTableObject
{
    #region Columns
    public override Database Database { get { return Database.Client; } }
    public static new string TableName { get { return "PaymentProportions"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey =true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("UserId")]
    public int UserId { get { return _UserId; } set { _UserId = value; SetUpToDateAsFalse(); } }

    [Column("TotalIn")]
    public Money TotalIn { get { return _TotalIn; } set { _TotalIn = value; SetUpToDateAsFalse(); } }

    [Column("TotalOut")]
    public Money TotalOut { get { return _TotalOut; } set { _TotalOut = value; SetUpToDateAsFalse(); } }

    [Column("Processor")]
    protected int IntProcessor { get { return _IntProcessor; } set { _IntProcessor = value; SetUpToDateAsFalse(); } }
    #endregion
    public PaymentProcessor Processor
    {
        get
        {
            return (PaymentProcessor)IntProcessor;
        }

        set
        {
            IntProcessor = (int)value;
        }
    }

    int _id, _UserId, _IntProcessor;
    Money _TotalIn, _TotalOut;

    #region Constructors

    public PaymentProportions()
                : base()
    { }

    public PaymentProportions(int id) : base(id) { }

    public PaymentProportions(DataRow row, bool isUpToDate = true)
                : base(row, isUpToDate)
    { }

    #endregion

    public static void Add(int userId, Money nowIn, Money nowOut, PaymentProcessor processor)
    {

    }
}