using Prem.PTC;
using System;
using System.Data;
using Titan.WelcomeTour;

public class WelcomeTourStep : BaseTableObject
{
    #region Columns

    public override Database Database { get { return Database.Client; } }

    public static new string TableName { get { return "WelcomeTourSteps"; } }
    protected override string dbTable { get { return TableName; } }

    [Column("Id", IsPrimaryKey = true)]
    public override int Id { get { return _id; } protected set { _id = value; SetUpToDateAsFalse(); } }

    [Column("ContentIdentifier")]
    protected int IntContentIdentifier { get { return _IntContentIdentifier; } set { _IntContentIdentifier = value; SetUpToDateAsFalse(); } }

    [Column("Title")]
    public String Title { get { return _Title; } set { _Title = value; SetUpToDateAsFalse(); } }

    [Column("Content")]
    public String Content { get { return _Content; } set { _Content = value; SetUpToDateAsFalse(); } }

    [Column("Status")]
    protected int IntStatus { get { return _IntStatus; } set { _IntStatus = value; SetUpToDateAsFalse(); } }

    [Column("StepOrder")]
    public int Order { get { return _Order; } set { _Order = value; SetUpToDateAsFalse(); } }

    public WelcomeTourStepStatus Status { get { return (WelcomeTourStepStatus)IntStatus; } set { IntStatus = (int)value;  } }

    public ContentIdentifierForMainSite ContentIdentifier { get { return (ContentIdentifierForMainSite)IntContentIdentifier; } set { IntContentIdentifier = (int)value; } }

    private int _id, _IntContentIdentifier, _IntStatus, _Order;
    private String _Title, _Content;

    #endregion Columns

    #region Constructors
    public WelcomeTourStep()
            : base() { }

    public WelcomeTourStep(int id) : base(id) { }

    public WelcomeTourStep(DataRow row, bool isUpToDate = true)
            : base(row, isUpToDate) { }

    #endregion

    #region Helpers
    public void Enable()
    {
        Status = WelcomeTourStepStatus.Enabled;
        Save();
    }

    public void Disable()
    {
        Status = WelcomeTourStepStatus.Disabled;
        Save();
    }
    #endregion
}
